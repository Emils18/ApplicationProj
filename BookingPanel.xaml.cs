using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MySql.Data.MySqlClient;
using System.Text.RegularExpressions; // <--- This line is REQUIRED for the Regex functionality

namespace BookingSystem
{
    public partial class BookingPanel : UserControl
    {
        public event EventHandler BookingCompleted;

        private string _currentSelectedRoomNumber;
        private string _currentSelectedRoomType;
        private decimal _currentSelectedRoomPrice;
        private int _currentSelectedRoomId; // This variable will store the 'id' from the 'rooms' table in your database

        public BookingPanel()
        {
            InitializeComponent();
            BookingDatePicker.SelectedDate = DateTime.Today; // Sets the default booking date to today
        }

        private void RoomCard_Click(object sender, MouseButtonEventArgs e)
        {
            var border = sender as Border;
            var stack = border?.Child as StackPanel;
            if (stack == null) return;

            // Get the full text from the TextBlock that starts with "ROOM".
            // For example, if your UI displays "ROOM 1 Standard", this will get that string.
            string roomInfoTextBlockText = stack.Children.OfType<TextBlock>()
                                         .FirstOrDefault(tb => tb.Text.StartsWith("ROOM"))?.Text;

            // --- START: Robust Extraction of Room Number using Regular Expressions ---
            // This part extracts the numeric room number from the 'roomInfoTextBlockText'
            if (!string.IsNullOrEmpty(roomInfoTextBlockText))
            {
                // Regex pattern: "ROOM" followed by optional spaces, then one or more digits.
                // The digits are captured in group 1.
                Match match = Regex.Match(roomInfoTextBlockText, @"ROOM\s*(\d+)");
                if (match.Success)
                {
                    _currentSelectedRoomNumber = match.Groups[1].Value; // Extracts just the numerical part, e.g., "1", "2"
                }
                else
                {
                    _currentSelectedRoomNumber = null; // If no number is found, set to null
                }
            }
            else
            {
                _currentSelectedRoomNumber = null; // If the TextBlock containing "ROOM" itself isn't found
            }
            // --- END: Room Number Extraction ---

            // Extract room type. This assumes the room type (Standard, Deluxe, Suite) is in its own TextBlock
            // within the StackPanel of the room card.
            // IMPORTANT: These strings ("Standard", "Deluxe", "Suite") MUST precisely match the
            // values in the 'room_type' column of your 'rooms' database table (case-sensitive!).
            _currentSelectedRoomType = stack.Children.OfType<TextBlock>()
                                           .FirstOrDefault(tb => tb.Text == "Standard" || tb.Text == "Deluxe" || tb.Text == "Suite")?.Text;

            // --- Validation before database lookup ---
            // If either the room number or type couldn't be extracted from the UI, show an error
            if (string.IsNullOrEmpty(_currentSelectedRoomNumber) || string.IsNullOrEmpty(_currentSelectedRoomType))
            {
                MessageBox.Show("Failed to extract complete room details (number or type) from the selected room card. Please ensure your UI text blocks are formatted correctly.", "Data Extraction Error", MessageBoxButton.OK, MessageBoxImage.Error);
                BookingPopup.Visibility = Visibility.Collapsed; // Hide the popup if details are missing
                return;
            }

            // Get the actual room_id from the database using the extracted number and type
            _currentSelectedRoomId = GetRoomIdFromDb(_currentSelectedRoomNumber, _currentSelectedRoomType);

            // --- Error Handling for Room ID Lookup ---
            // If _currentSelectedRoomId is 0, it means no matching room was found in the database.
            if (_currentSelectedRoomId == 0)
            {
                MessageBox.Show("Error: Could not find room ID for the selected room in the database. This likely means the room number '" + _currentSelectedRoomNumber + "' or room type '" + _currentSelectedRoomType + "' from the UI do not precisely match an entry in your 'rooms' table. Please verify your database data (e.g., in phpMyAdmin).", "Room Not Found Error", MessageBoxButton.OK, MessageBoxImage.Error);
                BookingPopup.Visibility = Visibility.Collapsed; // Hide popup if ID not found
                return;
            }

            // Determine room availability based on UI text (assuming a TextBlock with "✅" or "❌")
            bool available = stack.Children.OfType<TextBlock>().Any(tb => tb.Text == "✅");

            // Get the price based on room type
            _currentSelectedRoomPrice = GetPrice(_currentSelectedRoomType);

            // Update the UI elements in the booking popup with the selected room's details
            PopupRoomNumber.Text = "ROOM " + (_currentSelectedRoomNumber ?? "N/A"); // Display "ROOM 1", "ROOM 2", etc.
            PopupRoomType.Text = "Type: " + (_currentSelectedRoomType ?? "N/A");
            PopupRoomPrice.Text = $"Price: {_currentSelectedRoomPrice:C}";

            PopupAvailability.Text = "Availability: " + (available ? "Available" : "Unavailable");
            PopupAvailability.Foreground = available ? Brushes.Green : Brushes.Red;

            // Show or hide the booking form elements based on room availability
            if (available)
            {
                BookingForm.Visibility = Visibility.Visible;
                NotAvailableMessage.Visibility = Visibility.Collapsed;
            }
            else
            {
                BookingForm.Visibility = Visibility.Collapsed;
                NotAvailableMessage.Visibility = Visibility.Visible;
            }

            BookingPopup.Visibility = Visibility.Visible; // Finally, show the main booking popup
        }

        // Helper method to query the database for the 'id' (primary key) of a room
        // based on its room_number (e.g., "1") and room_type (e.g., "Standard").
        private int GetRoomIdFromDb(string roomNumber, string roomType)
        {
            int roomId = 0;
            // Ensure your connection string is correct for your MySQL setup
            string connectionString = "server=localhost;user id=root;password=;database=bookingsystem";
            // SQL query to select the 'id' from the 'rooms' table where both room_number and room_type match
            string query = "SELECT id FROM rooms WHERE room_number = @roomNumber AND room_type = @roomType LIMIT 1";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open(); // Open the database connection
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        // Add parameters for safe querying (prevents SQL injection)
                        cmd.Parameters.AddWithValue("@roomNumber", roomNumber); // Parameter for the room number
                        cmd.Parameters.AddWithValue("@roomType", roomType);     // Parameter for the room type

                        object result = cmd.ExecuteScalar(); // Executes the query and returns the first column of the first row
                        if (result != null)
                        {
                            roomId = Convert.ToInt32(result); // Convert the query result to an integer room ID
                        }
                    }
                }
                catch (MySqlException ex)
                {
                    // Catch and display specific MySQL database errors during the ID lookup
                    MessageBox.Show($"Database error during Room ID retrieval: {ex.Message}\nError Code: {ex.ErrorCode}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (Exception ex)
                {
                    // Catch and display any other unexpected errors during the ID lookup
                    MessageBox.Show($"An unexpected error occurred during Room ID retrieval: {ex.Message}", "Application Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            return roomId; // Returns the found room ID or 0 if no match or an error occurred
        }

        // Helper method to get the price of a room based on its type
        // Ensure these prices match your database if they are stored there, or keep this lookup consistent.
        private decimal GetPrice(string roomType)
        {
            switch (roomType)
            {
                case "Standard":
                    return 100.00m;
                case "Deluxe":
                    return 150.00m;
                case "Suite": // Price for Suite room, as seen in your database structure
                    return 250.00m;
                default:
                    return 0.00m; // Default or error price if room type is not recognized
            }
        }

        // Event handler for the "Close" button on the booking popup
        private void ClosePopup_Click(object sender, RoutedEventArgs e)
        {
            BookingPopup.Visibility = Visibility.Collapsed; // Hide the booking popup
            ClearBookingForm(); // Reset all input fields and temporary data
        }

        // Event handler for the "Book Now" button, to save the booking to the database












        // Event handler for the "Book Now" button, to save the booking to the database
        private void BookNow_Click(object sender, RoutedEventArgs e)
        {
            // Get user input from the booking form
            string guestName = InputName.Text.Trim();
            string guestContact = InputContact.Text.Trim();
            DateTime? selectedDate = BookingDatePicker.SelectedDate;

            int durationDays = 1; // Assuming a fixed booking duration of 1 day for now

            // Basic validation for required input fields
            if (string.IsNullOrWhiteSpace(guestName) || string.IsNullOrWhiteSpace(guestContact) || !selectedDate.HasValue)
            {
                MessageBox.Show("Please fill out all booking details (Guest Name, Contact Info, and Booking Date).", "Missing Information", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Critical validation: Ensure all necessary room details were successfully obtained before proceeding with booking
            if (_currentSelectedRoomId <= 0 || string.IsNullOrEmpty(_currentSelectedRoomNumber) || string.IsNullOrEmpty(_currentSelectedRoomType) || _currentSelectedRoomPrice <= 0)
            {
                MessageBox.Show("An internal error occurred: Room details are missing or invalid. Please re-select a room and try again.", "Booking Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Validate that a user is logged in (UserSession.UserId must be set upon successful login)
            int userId = UserSession.UserId;
            if (userId <= 0)
            {
                MessageBox.Show("You must be logged in to make a booking. Please log in first.", "Authentication Required", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string connectionString = "server=localhost;user id=root;password=;database=bookingsystem";

            // *** THIS IS THE CORRECT INSERT STATEMENT FOR THE 'bookings' TABLE ***
            string insertQuery = @"
        INSERT INTO bookings (
            user_id,
            room_id,
            guest_name,
            contact_info,
            booking_date,
            duration_days,
            total_price,
            status
        )
        VALUES (
            @userId,
            @roomId,
            @guestName,
            @guestContact,
            @bookingDate,
            @durationDays,
            @totalPrice,
            @status
        )";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open(); // Open the database connection
                    using (MySqlCommand cmd = new MySqlCommand(insertQuery, conn))
                    {
                        // Add parameters to the SQL command to safely insert data
                        cmd.Parameters.AddWithValue("@userId", userId);
                        cmd.Parameters.AddWithValue("@roomId", _currentSelectedRoomId);
                        cmd.Parameters.AddWithValue("@guestName", guestName);
                        cmd.Parameters.AddWithValue("@guestContact", guestContact);
                        cmd.Parameters.AddWithValue("@bookingDate", selectedDate.Value.Date);
                        cmd.Parameters.AddWithValue("@durationDays", durationDays);
                        cmd.Parameters.AddWithValue("@totalPrice", _currentSelectedRoomPrice);
                        cmd.Parameters.AddWithValue("@status", "Pending");

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show($"Room {(_currentSelectedRoomNumber)} booked successfully!", "Booking Confirmed", MessageBoxButton.OK, MessageBoxImage.Information);
                            BookingPopup.Visibility = Visibility.Collapsed;
                            ClearBookingForm();

                            // Raise an event to notify other parts of the application (e.g., MyBookingPanel) to refresh their data
                            BookingCompleted?.Invoke(this, EventArgs.Empty);
                        }
                        else
                        {
                            MessageBox.Show("Booking failed. No record was added to the database. Please try again.", "Booking Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show($"Database error during booking: {ex.Message}\nError Code: {ex.ErrorCode}", "Database Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An unexpected error occurred during booking: {ex.Message}", "Application Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }


































        // Resets all input fields and temporary variables related to the booking form
        private void ClearBookingForm()
        {
            InputName.Text = "";
            InputContact.Text = "";
            BookingDatePicker.SelectedDate = DateTime.Today; // Reset date to today
            _currentSelectedRoomNumber = null;
            _currentSelectedRoomType = null;
            _currentSelectedRoomPrice = 0.00m;
            _currentSelectedRoomId = 0; // Reset the room ID to indicate no room is selected
        }
    }
}