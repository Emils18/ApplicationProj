using System;
using System.Windows;
using System.Windows.Controls;
using MySql.Data.MySqlClient;
using System.Globalization;

namespace BookingSystem
{
    public partial class PaymentPanel : UserControl
    {
        // Properties to receive booking details from MyBookingPanel
        public int BookingId { get; set; }
        public decimal TotalPrice { get; set; }
        public string RoomDetailsText { get; set; }
        public string BookingDateText { get; set; }
        public string DurationText { get; set; }

        // Event to notify MainPanelBook when payment is completed or cancelled
        public event EventHandler PaymentCompleted;

        public PaymentPanel()
        {
            InitializeComponent();
            this.Loaded += PaymentPanel_Loaded;
            PaymentMethodComboBox.SelectionChanged += PaymentMethodComboBox_SelectionChanged;
        }

        private void PaymentPanel_Loaded(object sender, RoutedEventArgs e)
        {
            // Populate the TextBlocks with the received booking details
            RoomDetailsTextBlock.Text = RoomDetailsText;
            BookingDateTextBlock.Text = BookingDateText;
            DurationTextBlock.Text = DurationText;
            TotalPriceTextBlock.Text = TotalPrice.ToString("C", CultureInfo.CurrentCulture); // Format as currency

            // Select the first item by default if nothing is selected
            if (PaymentMethodComboBox.SelectedItem == null && PaymentMethodComboBox.Items.Count > 0)
            {
                PaymentMethodComboBox.SelectedIndex = 0;
            }
        }

        private void PaymentMethodComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Hide all detail panels first
            CardDetailsPanel.Visibility = Visibility.Collapsed;
            GCashDetailsPanel.Visibility = Visibility.Collapsed;
            PayPalDetailsPanel.Visibility = Visibility.Collapsed;

            // Show the relevant panel based on selection
            if (PaymentMethodComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                switch (selectedItem.Content.ToString())
                {
                    case "Credit Card":
                        CardDetailsPanel.Visibility = Visibility.Visible;
                        break;
                    case "GCash":
                        GCashDetailsPanel.Visibility = Visibility.Visible;
                        break;
                    case "PayPal":
                        PayPalDetailsPanel.Visibility = Visibility.Visible;
                        break;
                }
            }
        }

        private void ConfirmPayment_Click(object sender, RoutedEventArgs e)
        {
            string selectedPaymentMethod = (PaymentMethodComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

            if (string.IsNullOrEmpty(selectedPaymentMethod))
            {
                MessageBox.Show("Please select a payment method.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Basic validation based on selected method (can be expanded)
            bool isValid = false;
            switch (selectedPaymentMethod)
            {
                case "Credit Card":
                    isValid = !string.IsNullOrWhiteSpace(CardNumberTextBox.Text) &&
                              !string.IsNullOrWhiteSpace(CardholderNameTextBox.Text) &&
                              !string.IsNullOrWhiteSpace(ExpirationDateTextBox.Text) &&
                              !string.IsNullOrWhiteSpace(CvvTextBox.Text);
                    break;
                case "GCash":
                    isValid = !string.IsNullOrWhiteSpace(GCashNumberTextBox.Text) &&
                              !string.IsNullOrWhiteSpace(GCashRefTextBox.Text);
                    break;
                case "PayPal":
                    isValid = !string.IsNullOrWhiteSpace(PayPalEmailTextBox.Text) &&
                              !string.IsNullOrWhiteSpace(PayPalTransactionIdTextBox.Text);
                    break;
            }

            if (!isValid)
            {
                MessageBox.Show("Please fill in all required payment details.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Simulate payment processing and update database
            ProcessPayment(selectedPaymentMethod);
        }

        private void ProcessPayment(string paymentMethod)
        {
            string connectionString = "server=localhost;user id=root;password=;database=bookingsystem";
            // Update both status and payment_method
            string query = "UPDATE bookings SET status = 'Paid', payment_method = @paymentMethod WHERE id = @bookingId";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@paymentMethod", paymentMethod);
                        cmd.Parameters.AddWithValue("@bookingId", BookingId);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Payment confirmed successfully! Booking status updated to 'Paid'.", "Payment Success", MessageBoxButton.OK, MessageBoxImage.Information);
                            PaymentCompleted?.Invoke(this, EventArgs.Empty); // Raise event to notify MainPanelBook
                        }
                        else
                        {
                            MessageBox.Show("Could not confirm payment. Booking might not exist or status is already 'Paid'.", "Payment Failed", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"Database error during payment: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unexpected error occurred during payment: {ex.Message}", "Application Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelPayment_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to cancel the payment process?", "Confirm Cancel", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                PaymentCompleted?.Invoke(this, EventArgs.Empty); // Treat as payment not completed, go back
            }
        }
    }
}
