
using System.ComponentModel.DataAnnotations;


public class UpdateListingRequest
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string Address { get; set; }
    public double RegularPrice { get; set; }
    public double DiscountPrice { get; set; }
    public int Bathrooms { get; set; }
    public int Bedrooms { get; set; }
    public bool Furnished { get; set; }
    public bool Parking { get; set; }
    public string Type { get; set; }
    public bool Offer { get; set; }
    public List<string> ImageUrls { get; set; }
}

public class CreateListingRequest
{
    public List<string> ImageUrls { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Address { get; set; }
    public double RegularPrice { get; set; }
    public double DiscountPrice { get; set; }
    public int Bathrooms { get; set; }
    public int Bedrooms { get; set; }
    public bool Furnished { get; set; }
    public bool Parking { get; set; }
    public string Type { get; set; }
    public bool Offer { get; set; }
    public string UserRef { get; set; }
}


#region Login

public class LoginRequest
{
    [Required]
    public string Email { get; set; }
    [Required]
    public string Password { get; set; }
}


public class LoginResponse
{
    public User user { get; set; }

    public string Token { get; set; }


    public string ErrorMessage { get; set; }

}


#endregion



#region Register

public class RegisterRequest
{
    [Required]
    public string Username { get; set; }

    [Required]
    public string Email { get; set; }


    [Required]
    public IFormFile Avatar { get; set; }
    //public string Avatar { get; set; }

    [Required]
    public string Password { get; set; }
}

public class RegisterResponse
{
    public bool IsSuccess { get; set; }
    public string ErrorMessage { get; set; }
}


#endregion

