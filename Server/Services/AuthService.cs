using Google.Cloud.Firestore;
using Google.Cloud.Storage.V1;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Server.Services
{

    public interface IAuthService
    {
        Task<LoginResponse> Login(LoginRequest model);
        Task<RegisterResponse> Register(string username, IFormFile avatar, string email, string password);
        Task<List<User>> GetAllUsersAsync();
    }



    public class AuthService : IAuthService
    {

        private readonly IConfiguration _configuration;
        private readonly FirestoreDb _firestoreDb;
        private StorageClient _storageClient;

        public AuthService(IConfiguration configuration)
        {
            _configuration = configuration;

            string filepath = Path.Combine(Directory.GetCurrentDirectory(), "Properties", "Quinns Creds.json");
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", filepath);

            _firestoreDb = FirestoreDb.Create("real-estate-test-eb656");
            _storageClient = StorageClient.Create();
        }



        public async Task<RegisterResponse> Register(string username, IFormFile avatar, string email, string password)
        {
            try
            {
                IEnumerable<User> allUsers = await GetAllUsersAsync();
                if (username.Length > 12)
                    return new RegisterResponse { IsSuccess = false, ErrorMessage = "Username must be less than or equal to 12 characters long" };

                if (allUsers.Any(x => x.Username == username))
                    return new RegisterResponse { IsSuccess = false, ErrorMessage = $"Username '{username}' is already taken" };

                if (allUsers.Any(x => x.Email == email))
                    return new RegisterResponse { IsSuccess = false, ErrorMessage = $"Email '{email}' is already taken" };

                if (avatar == null || avatar.Length == 0)
                    return new RegisterResponse { IsSuccess = false, ErrorMessage = "Invalid file" };

                var user = new User
                {
                    Username = username,
                    Email = email,
                    Password = BCrypt.Net.BCrypt.HashPassword(password),
                    Avatar = $"{username}_{Guid.NewGuid()}{Path.GetExtension(avatar.FileName)}"
                };

                string bucketName = "real-estate-test-eb656.appspot.com";
                //string objectName = $"Profile Pictures/{username}_{Guid.NewGuid()}{Path.GetExtension(avatar.FileName)}";
                string objectName = $"Profile_Pictures/{user.Avatar}";

                using (var stream = avatar.OpenReadStream())
                {
                    await _storageClient.UploadObjectAsync(
                        bucketName,
                        objectName,
                        "image/jpeg",
                        stream
                    );
                }

                DocumentReference docRef = _firestoreDb.Collection("UserData").Document(user.Username);
                await docRef.SetAsync(user);

                return new RegisterResponse { IsSuccess = true, ErrorMessage = "Registration successful" };
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Exception during registration: {ex}");
                return new RegisterResponse { IsSuccess = false, ErrorMessage = "Internal Server Error" };
            }
        }




        public async Task<LoginResponse> Login(LoginRequest model)
        {
            try
            {
                IEnumerable<User> allUsers = await GetAllUsersAsync();

                var account = allUsers.SingleOrDefault(x => x.Email.Equals(model.Email, StringComparison.OrdinalIgnoreCase));

                if (account == null || !BCrypt.Net.BCrypt.Verify(model.Password, account.Password))
                {
                    return new LoginResponse { ErrorMessage = "Invalid email or password" };
                }

                return new LoginResponse
                {
                    user = account,
                    Token = generateJwtToken(account.Id)
                };
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Login error: {ex}");
                return new LoginResponse { ErrorMessage = "An error occurred during login" };
            }
        }


        public async Task<List<User>> GetAllUsersAsync()
        {
            CollectionReference usersCollection = _firestoreDb.Collection("UserData");
            QuerySnapshot querySnapshot = await usersCollection.GetSnapshotAsync();

            List<User> users = new List<User>();

            foreach (DocumentSnapshot documentSnapshot in querySnapshot.Documents)
            {
                if (documentSnapshot.Exists)
                {
                    User user = documentSnapshot.ConvertTo<User>();
                    users.Add(user);
                }
            }

            return users;
        }



        private string generateJwtToken(string id)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Secret"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", id) }),
                Expires = DateTime.UtcNow.AddMinutes(15),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }




    }
}
