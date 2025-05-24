// BookingData.cs
namespace BookingSystem
{
    public class BookingData
    {
        public int Id { get; set; }
        public string RoomNumber { get; set; }
        public string RoomType { get; set; }
        public string GuestName { get; set; }
        public string ContactInfo { get; set; }
        public System.DateTime BookingDate { get; set; }
        public int DurationDays { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; }
        public string PaymentMethod { get; set; } // Added this property
    }
}