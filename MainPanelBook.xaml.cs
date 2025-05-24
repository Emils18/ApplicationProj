using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BookingSystem
{
    public partial class MainPanelBook : Window
    {
        private BookingPanel _bookingPanel;
        private MyBookingPanel _myBookingPanel;
        private PaymentPanel _paymentPanel;
        private ProfilePanel _profilePanel;

        public MainPanelBook()
        {
            InitializeComponent();
            this.Loaded += MainPanelBook_Loaded;
            ShowBookingPanel();
        }

        private void MainPanelBook_Loaded(object sender, RoutedEventArgs e)
        {
            if (_myBookingPanel == null)
            {
                _myBookingPanel = new MyBookingPanel();
                _myBookingPanel.OnShowPaymentPanelRequested += MyBookingPanel_OnShowPaymentPanelRequested;
            }
        }

        private void Profile_Click(object sender, MouseButtonEventArgs e)
        {
            ShowProfilePanel();
        }

        private void BookingIcon_Click(object sender, MouseButtonEventArgs e)
        {
            ShowBookingPanel();
        }

        private void MyBookingIcon_Click(object sender, MouseButtonEventArgs e)
        {
            ShowMyBookingPanel();
        }

        private void PaymentIcon_Click(object sender, MouseButtonEventArgs e)
        {
            ShowMyBookingPanel();
        }

        private void LogoutIcon_Click(object sender, MouseButtonEventArgs e)
        {
            PerformLogout();
        }

        private void ShowBookingPanel()
        {
            if (_bookingPanel == null)
            {
                _bookingPanel = new BookingPanel();
                _bookingPanel.BookingCompleted += BookingPanel_BookingCompleted;
            }
            MainContentArea.Content = _bookingPanel;
        }

        private void ShowMyBookingPanel()
        {
            if (_myBookingPanel == null)
            {
                _myBookingPanel = new MyBookingPanel();
                _myBookingPanel.OnShowPaymentPanelRequested += MyBookingPanel_OnShowPaymentPanelRequested;
            }
            MainContentArea.Content = _myBookingPanel;
            _myBookingPanel.LoadUserBookings();
        }

        private void ShowProfilePanel()
        {
            if (_profilePanel == null)
            {
                _profilePanel = new ProfilePanel();
            }
            MainContentArea.Content = _profilePanel;
            _profilePanel.LoadProfileData();
        }

        private void ShowPaymentPanel(int bookingId, decimal totalPrice, string roomDetails, string bookingDate, string duration)
        {
            _paymentPanel = new PaymentPanel
            {
                BookingId = bookingId,
                TotalPrice = totalPrice,
                RoomDetailsText = roomDetails,
                BookingDateText = bookingDate,
                DurationText = duration
            };
            _paymentPanel.PaymentCompleted += PaymentPanel_PaymentCompleted;
            MainContentArea.Content = _paymentPanel;
        }

        private void MyBookingPanel_OnShowPaymentPanelRequested(object sender, PaymentPanelEventArgs e)
        {
            ShowPaymentPanel(e.BookingId, e.TotalPrice, e.RoomDetailsText, e.BookingDateText, e.DurationText);
        }

        private void PaymentPanel_PaymentCompleted(object sender, EventArgs e)
        {
            ShowMyBookingPanel();
        }

        private void BookingPanel_BookingCompleted(object sender, EventArgs e)
        {
            ShowMyBookingPanel();
        }

        private void PerformLogout()
        {
            UserSession.ClearSession();
            MainWindow loginWindow = new MainWindow();
            loginWindow.Show();
            this.Close();
        }
    }
}
