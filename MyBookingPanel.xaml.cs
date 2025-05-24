// MyBookingPanel.xaml.cs
using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Controls.Primitives; // For Popup
using MySql.Data.MySqlClient;
using System.Windows.Input; // For Cursors

namespace BookingSystem
{
    public partial class MyBookingPanel : UserControl
    {
        private int _currentEditingBookingId;

        // Event to notify MainPanelBook that the PaymentPanel should be shown
        public event EventHandler<PaymentPanelEventArgs> OnShowPaymentPanelRequested;

        public MyBookingPanel()
        {
            InitializeComponent();
            this.Loaded += MyBookingPanel_Loaded;
        }

        private void MyBookingPanel_Loaded(object sender, RoutedEventArgs e)
        {
            LoadUserBookings();
        }

        public void LoadUserBookings()
        {
            if (BookingsDisplayPanel == null)
            {
                MessageBox.Show("BookingsDisplayPanel is not initialized. Please ensure it's defined in XAML with x:Name='BookingsDisplayPanel'.", "Configuration Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            BookingsDisplayPanel.Children.Clear(); // Clear existing booking cards

            int userId = UserSession.UserId; // Get the logged-in user's ID
            if (userId <= 0)
            {
                BookingsDisplayPanel.Children.Add(new TextBlock { Text = "Please log in to view your bookings.", FontSize = 16, HorizontalAlignment = HorizontalAlignment.Center, Margin = new Thickness(0, 20, 0, 0) });
                return;
            }

            string connectionString = "server=localhost;user id=root;password=;database=bookingsystem";
            // IMPORTANT: Ensure 'b.payment_method' is included in your SELECT query
            string query = "SELECT b.id, r.room_number, r.room_type, b.guest_name, b.contact_info, b.booking_date, b.duration_days, b.total_price, b.status, b.user_id, b.payment_method " +
                           "FROM bookings b " +
                           "JOIN rooms r ON b.room_id = r.id " +
                           "WHERE b.user_id = @userId ORDER BY b.booking_date DESC";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@userId", userId);
                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);

                            if (dt.Rows.Count == 0)
                            {
                                // Show "You have no bookings yet." message
                                BookingsDisplayPanel.Children.Add(new TextBlock { Text = "You have no bookings yet.", FontSize = 16, HorizontalAlignment = HorizontalAlignment.Center, Margin = new Thickness(0, 20, 0, 0) });
                                return;
                            }

                            foreach (DataRow row in dt.Rows)
                            {
                                // Create a BookingData object from the DataRow
                                BookingData bookingData = new BookingData
                                {
                                    Id = Convert.ToInt32(row["id"]),
                                    RoomNumber = row["room_number"].ToString(),
                                    RoomType = row["room_type"].ToString(),
                                    GuestName = row["guest_name"].ToString(),
                                    ContactInfo = row["contact_info"].ToString(),
                                    BookingDate = Convert.ToDateTime(row["booking_date"]),
                                    DurationDays = Convert.ToInt32(row["duration_days"]),
                                    TotalPrice = Convert.ToDecimal(row["total_price"]),
                                    Status = row["status"].ToString(),
                                    // Handle DBNull for payment_method
                                    PaymentMethod = row.IsNull("payment_method") ? "N/A" : row["payment_method"].ToString()
                                };


                                Border bookingCard = new Border
                                {
                                    Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F0F0F0")),
                                    BorderBrush = Brushes.LightGray,
                                    BorderThickness = new Thickness(1),
                                    CornerRadius = new CornerRadius(5),
                                    Padding = new Thickness(10),
                                    Margin = new Thickness(5),
                                    Tag = bookingData // Set the entire BookingData object as the Tag
                                };

                                StackPanel cardContent = new StackPanel();
                                cardContent.Children.Add(new TextBlock { Text = $"Booking ID: {bookingData.Id}", FontWeight = FontWeights.Bold });
                                cardContent.Children.Add(new TextBlock { Text = $"Room: {bookingData.RoomNumber} - {bookingData.RoomType}" });
                                cardContent.Children.Add(new TextBlock { Text = $"Guest: {bookingData.GuestName}" });
                                cardContent.Children.Add(new TextBlock { Text = $"Contact: {bookingData.ContactInfo}" });
                                cardContent.Children.Add(new TextBlock { Text = $"Date: {bookingData.BookingDate.ToShortDateString()}" });
                                cardContent.Children.Add(new TextBlock { Text = $"Duration: {bookingData.DurationDays} Day(s)" });
                                cardContent.Children.Add(new TextBlock { Text = $"Price: {bookingData.TotalPrice:C}" }); // Format as currency

                                TextBlock statusTextBlock = new TextBlock { Text = $"Status: {bookingData.Status}", FontWeight = FontWeights.Bold };
                                string status = bookingData.Status;
                                if (status == "Pending") statusTextBlock.Foreground = Brushes.OrangeRed;
                                else if (status == "Paid") statusTextBlock.Foreground = Brushes.Green;
                                else if (status == "Cancelled") statusTextBlock.Foreground = Brushes.DarkGray;
                                cardContent.Children.Add(statusTextBlock);

                                // Display Payment Method if available and not 'N/A'
                                if (!string.IsNullOrEmpty(bookingData.PaymentMethod) && bookingData.PaymentMethod != "N/A")
                                {
                                    cardContent.Children.Add(new TextBlock { Text = $"Payment Method: {bookingData.PaymentMethod}" });
                                }

                                StackPanel buttonPanel = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right, Margin = new Thickness(0, 10, 0, 0) };

                                Button editButton = new Button
                                {
                                    Content = "Edit",
                                    Tag = bookingData.Id, // Only ID needed for edit popup
                                    Margin = new Thickness(5, 0, 0, 0),
                                    Padding = new Thickness(10, 5, 10, 5),
                                    Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E0E0E0")),
                                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#333333")),
                                    BorderThickness = new Thickness(0),
                                    Cursor = System.Windows.Input.Cursors.Hand
                                };
                                editButton.Click += EditBooking_Click;
                                buttonPanel.Children.Add(editButton);

                                Button cancelButton = new Button
                                {
                                    Content = "Cancel",
                                    Tag = bookingData.Id, // Only ID needed for cancel
                                    Margin = new Thickness(5, 0, 0, 0),
                                    Padding = new Thickness(10, 5, 10, 5),
                                    Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF6347")),
                                    Foreground = Brushes.White,
                                    BorderThickness = new Thickness(0),
                                    Cursor = System.Windows.Input.Cursors.Hand
                                };
                                cancelButton.Click += CancelBooking_Click;
                                if (status == "Paid" || status == "Cancelled") cancelButton.IsEnabled = false;
                                buttonPanel.Children.Add(cancelButton);

                                Button payButton = new Button
                                {
                                    Content = "Pay",
                                    Tag = bookingData, // Pass the entire BookingData object for payment
                                    Margin = new Thickness(5, 0, 0, 0),
                                    Padding = new Thickness(10, 5, 10, 5),
                                    Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#007ACC")),
                                    Foreground = Brushes.White,
                                    BorderThickness = new Thickness(0),
                                    Cursor = System.Windows.Input.Cursors.Hand
                                };
                                payButton.Click += Pay_Click; // Link to the Pay_Click event handler
                                if (status == "Paid" || status == "Cancelled") payButton.IsEnabled = false;
                                buttonPanel.Children.Add(payButton);

                                cardContent.Children.Add(buttonPanel);
                                bookingCard.Child = cardContent;

                                BookingsDisplayPanel.Children.Add(bookingCard);
                            }
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"Database error loading bookings: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unexpected error occurred loading bookings: {ex.Message}", "Application Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // --- EDIT BOOKING LOGIC (remains largely unchanged) ---
        private void EditBooking_Click(object sender, RoutedEventArgs e)
        {
            Button editButton = sender as Button;
            if (editButton == null || editButton.Tag == null) return;

            _currentEditingBookingId = Convert.ToInt32(editButton.Tag);

            if (PopupEditBookingIdText != null)
            {
                PopupEditBookingIdText.Text = $"Editing Booking ID: {_currentEditingBookingId}";
            }

            string connectionString = "server=localhost;user id=root;password=;database=bookingsystem";
            string query = "SELECT guest_name, contact_info, booking_date FROM bookings WHERE id = @bookingId LIMIT 1";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@bookingId", _currentEditingBookingId);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                if (EditNameInput != null) EditNameInput.Text = reader["guest_name"].ToString();
                                if (EditContactInput != null) EditContactInput.Text = reader["contact_info"].ToString();
                                if (EditBookingDatePicker != null) EditBookingDatePicker.SelectedDate = Convert.ToDateTime(reader["booking_date"]);

                                if (EditBookingPopup != null)
                                {
                                    EditBookingPopup.IsOpen = true;
                                }
                            }
                            else
                            {
                                MessageBox.Show("Booking not found for editing.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"Database error fetching booking for edit: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unexpected error occurred fetching booking for edit: {ex.Message}", "Application Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveBookingChanges_Click(object sender, RoutedEventArgs e)
        {
            string newName = EditNameInput?.Text.Trim();
            string newContact = EditContactInput?.Text.Trim();
            DateTime? newBookingDate = EditBookingDatePicker?.SelectedDate;

            if (string.IsNullOrWhiteSpace(newName) || string.IsNullOrWhiteSpace(newContact) || !newBookingDate.HasValue)
            {
                MessageBox.Show("Please fill all fields for booking edit.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string connectionString = "server=localhost;user id=root;password=;database=bookingsystem";
            string query = "UPDATE bookings SET guest_name = @guestName, contact_info = @contact, booking_date = @bookingDate WHERE id = @bookingId";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@guestName", newName);
                        cmd.Parameters.AddWithValue("@contact", newContact);
                        cmd.Parameters.AddWithValue("@bookingDate", newBookingDate.Value.Date);
                        cmd.Parameters.AddWithValue("@bookingId", _currentEditingBookingId);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Booking updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                            if (EditBookingPopup != null)
                            {
                                EditBookingPopup.IsOpen = false;
                            }
                            LoadUserBookings(); // Refresh the list after saving changes
                        }
                        else
                        {
                            MessageBox.Show("No changes applied. Booking might not exist or no data was different.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"Database error updating booking: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unexpected error occurred updating booking: {ex.Message}", "Application Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CloseEditPopup_Click(object sender, RoutedEventArgs e)
        {
            if (EditBookingPopup != null)
            {
                EditBookingPopup.IsOpen = false;
            }
        }

        // --- CANCEL BOOKING LOGIC (remains unchanged) ---
        private void CancelBooking_Click(object sender, RoutedEventArgs e)
        {
            Button cancelButton = sender as Button;
            if (cancelButton == null || cancelButton.Tag == null) return;

            int bookingIdToCancel = Convert.ToInt32(cancelButton.Tag);

            MessageBoxResult result = MessageBox.Show($"Are you sure you want to cancel booking ID: {bookingIdToCancel}?", "Confirm Cancellation", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                string connectionString = "server=localhost;user id=root;password=;database=bookingsystem";
                string query = "UPDATE bookings SET status = 'Cancelled' WHERE id = @bookingId";

                try
                {
                    using (MySqlConnection conn = new MySqlConnection(connectionString))
                    {
                        conn.Open();
                        using (MySqlCommand cmd = new MySqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@bookingId", bookingIdToCancel);

                            int rowsAffected = cmd.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                MessageBox.Show($"Booking ID {bookingIdToCancel} has been cancelled.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                                LoadUserBookings(); // Refresh the list after cancellation
                            }
                            else
                            {
                                MessageBox.Show("Could not cancel booking. It might already be cancelled or not exist.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                        }
                    }
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show($"Database error cancelling booking: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An unexpected error occurred cancelling booking: {ex.Message}", "Application Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        // --- PAY BUTTON LOGIC (triggers the event) ---
        private void Pay_Click(object sender, RoutedEventArgs e)
        {
            Button payButton = sender as Button;
            if (payButton != null)
            {
                BookingData bookingData = payButton.Tag as BookingData;

                if (bookingData != null)
                {
                    if (bookingData.Status == "Paid")
                    {
                        MessageBox.Show($"This booking (ID: {bookingData.Id}) is already marked as 'Paid'.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }
                    else if (bookingData.Status == "Cancelled")
                    {
                        MessageBox.Show($"This booking (ID: {bookingData.Id}) is 'Cancelled' and cannot be paid.", "Information", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    // This line is crucial for showing the payment panel
                    OnShowPaymentPanelRequested?.Invoke(this, new PaymentPanelEventArgs(
                        bookingData.Id,
                        bookingData.TotalPrice,
                        $"Room: {bookingData.RoomNumber} - {bookingData.RoomType}",
                        $"Date: {bookingData.BookingDate.ToShortDateString()}",
                        $"Duration: {bookingData.DurationDays} Day(s)"
                    ));
                }
                else
                {
                    MessageBox.Show("Error: Booking data not found for payment.", "Data Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}