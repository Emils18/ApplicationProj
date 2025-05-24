// PaymentPanelEventArgs.cs
using System;

namespace BookingSystem
{
    public class PaymentPanelEventArgs : EventArgs
    {
        public int BookingId { get; }
        public decimal TotalPrice { get; }
        public string RoomDetailsText { get; } // e.g., "Room: 101 - Standard"
        public string BookingDateText { get; } // e.g., "Date: May 23, 2025"
        public string DurationText { get; }    // e.g., "Duration: 5 nights"

        public PaymentPanelEventArgs(int bookingId, decimal totalPrice, string roomDetailsText, string bookingDateText, string durationText)
        {
            BookingId = bookingId;
            TotalPrice = totalPrice;
            RoomDetailsText = roomDetailsText;
            BookingDateText = bookingDateText;
            DurationText = durationText;
        }
    }
}