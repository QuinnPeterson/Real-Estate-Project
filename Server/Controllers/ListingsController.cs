using FireSharp.Extensions;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Server.Services;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Controllers
{

    [EnableCors("MyAllowSpecificOrigins")]
    [ApiController]
    [Route("api/[controller]")]
    public class ListingsController : ControllerBase
    {

        private readonly IConfiguration _configuration;
        //private readonly IListingService _listingService;
            

        private readonly FirestoreDb _firestoreDb;
        private StorageClient _storageClient;


        public ListingsController(IConfiguration configuration) //, IListingService listingService)
        {
            _configuration = configuration;
            //_listingService = listingService;

            string filepath = Path.Combine(Directory.GetCurrentDirectory(), "Properties", "Quinns Creds.json");
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", filepath);

            _firestoreDb = FirestoreDb.Create("real-estate-test-eb656");
            _storageClient = StorageClient.Create();
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateListing([FromBody] CreateListingRequest request)
        {
            try
            {
                var listing = new Listing
                {
                    Name = request.Name,
                    Description = request.Description,
                    Address = request.Address,
                    RegularPrice = request.RegularPrice,
                    DiscountPrice = request.DiscountPrice,
                    Bathrooms = request.Bathrooms,
                    Bedrooms = request.Bedrooms,
                    Furnished = request.Furnished,
                    Parking = request.Parking,
                    Type = request.Type,
                    Offer = request.Offer,
                    UserRef = request.UserRef,
                    ImageUrls = request.ImageUrls,
                };

                var listingCollection = _firestoreDb.Collection("Listings");
                var documentReference = await listingCollection.AddAsync(listing);
                var createdListing = await documentReference.GetSnapshotAsync();
                var createdListingData = createdListing.ConvertTo<Listing>();
                return CreatedAtAction(nameof(CreateListing), new { id = documentReference.Id }, createdListingData);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing request: {ex.Message}");
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }


        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteListing(string id)
        {
            try
            {
                var listingRef = _firestoreDb.Collection("Listings").Document(id);
                var snapshot = await listingRef.GetSnapshotAsync();

                if (!snapshot.Exists)
                {
                    return NotFound("Listing not found!");
                }

                var listing = snapshot.ConvertTo<Listing>();
                await listingRef.DeleteAsync();

                return Ok("Listing has been deleted!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting listing: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }


        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateListing(string id, [FromBody] UpdateListingRequest request)
        {
            try
            {
                var listingRef = _firestoreDb.Collection("Listings").Document(id);
                var snapshot = await listingRef.GetSnapshotAsync();

                if (!snapshot.Exists)
                {
                    return NotFound("Listing not found!");
                }

                var listing = snapshot.ConvertTo<Listing>();

                listing.Name = request.Name ?? listing.Name;
                listing.Description = request.Description ?? listing.Description;
                listing.Address = request.Address ?? listing.Address;
                listing.RegularPrice = request.RegularPrice != 0 ? request.RegularPrice : listing.RegularPrice;
                listing.DiscountPrice = request.DiscountPrice != 0 ? request.DiscountPrice : listing.DiscountPrice;
                listing.Bathrooms = request.Bathrooms != 0 ? request.Bathrooms : listing.Bathrooms;
                listing.Bedrooms = request.Bedrooms != 0 ? request.Bedrooms : listing.Bedrooms;
                listing.Furnished = request.Furnished;
                listing.Parking = request.Parking;
                listing.Type = request.Type ?? listing.Type;
                listing.Offer = request.Offer;
                listing.ImageUrls = request.ImageUrls ?? listing.ImageUrls;

                await listingRef.SetAsync(listing);

                return Ok(listing);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating listing: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }




        [HttpGet("getListing/{id}")]
        public async Task<ActionResult<Listing>> GetListingByIdAsync(string id)
        {
            try
            {
                DocumentReference listingRef = _firestoreDb.Collection("Listings").Document(id);
                DocumentSnapshot snapshot = await listingRef.GetSnapshotAsync();

                if (snapshot.Exists)
                {
                    Listing listing = snapshot.ConvertTo<Listing>();
                    return Ok(listing);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving listing: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }


        [HttpGet("get")]
        public async Task<ActionResult<List<Listing>>> GetListingsByType([FromQuery] string type, [FromQuery] int limit = 4)
        {
            try
            {
                if (string.IsNullOrEmpty(type) || (type != "offer" && type != "rent" && type != "sale"))
                {
                    return BadRequest("Invalid listing type.");
                }

                var listingsCollection = _firestoreDb.Collection("Listings");
                var query = listingsCollection.WhereEqualTo("Type", type).Limit(limit);
                var querySnapshot = await query.GetSnapshotAsync();
                var listings = querySnapshot.Documents.Select(doc => doc.ConvertTo<Listing>()).ToList();

                return Ok(listings);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving listings: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("getUserListings/{userId}")]
        public async Task<IActionResult> GetUserListings(string userId)
        {
            try
            {

                var listingsRef = _firestoreDb.Collection("Listings").WhereEqualTo("UserRef", userId );
                var snapshot = await listingsRef.GetSnapshotAsync();

                if (snapshot.IsNullOrEmpty())
                {
                    return NotFound("No listings found for the user.");
                }

                var listings = snapshot.Documents.Select(doc => doc.ConvertTo<Listing>()).ToList();
                return Ok(listings);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving user listings: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("/SearchListings")]
        public async Task<IActionResult> SearchListings(
            [FromQuery] int limit = 9,
            [FromQuery] int startIndex = 0,
            [FromQuery] bool offer = false,
            [FromQuery] bool furnished = false,
            [FromQuery] bool parking = false,
            [FromQuery] string type = "all",
            [FromQuery] string searchTerm = "")
        {
            try
            {
                var listingsRef = _firestoreDb.Collection("Listings")
                    .WhereGreaterThanOrEqualTo("Name", searchTerm)
                    .WhereLessThanOrEqualTo("Name", searchTerm + "\uf8ff");
                if (offer)
                    listingsRef = listingsRef.WhereEqualTo("Offer", true);

                if (furnished)
                    listingsRef = listingsRef.WhereEqualTo("Furnished", true);

                if (parking)
                    listingsRef = listingsRef.WhereEqualTo("Parking", true);

                if (type != "all")
                    listingsRef = listingsRef.WhereEqualTo("Type", type);

                listingsRef = listingsRef.Offset(startIndex).Limit(limit);
                var snapshot = await listingsRef.GetSnapshotAsync();

                if (snapshot.IsNullOrEmpty())
                {
                    return Ok(new List<Listing>());
                }

                var listings = snapshot.Documents.Select(doc => doc.ConvertTo<Listing>()).ToList();
                return Ok(listings);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving listings: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }






















    }








}

