using Google.Cloud.Firestore;

[FirestoreData]
public class Listing
{
    [FirestoreDocumentId]
    public string Id { get; set; }

    [FirestoreProperty]
    public string Name { get; set; }

    [FirestoreProperty]
    public string Description { get; set; }

    [FirestoreProperty]
    public string Address { get; set; }

    [FirestoreProperty]
    public double RegularPrice { get; set; }

    [FirestoreProperty]
    public double DiscountPrice { get; set; }

    [FirestoreProperty]
    public int Bathrooms { get; set; }

    [FirestoreProperty]
    public int Bedrooms { get; set; }

    [FirestoreProperty]
    public bool Furnished { get; set; }

    [FirestoreProperty]
    public bool Parking { get; set; }

    [FirestoreProperty]
    public string Type { get; set; }

    [FirestoreProperty]
    public bool Offer { get; set; }

    [FirestoreProperty]
    public List<string> ImageUrls { get; set; }

    [FirestoreProperty]
    public string UserRef { get; set; }

    [FirestoreProperty]
    public Timestamp CreatedAt { get; set; }

    [FirestoreProperty]
    public Timestamp UpdatedAt { get; set; }
}
