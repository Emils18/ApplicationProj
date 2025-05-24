using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BookingSystem
{
    public class BookingViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Booking> _bookings = new ObservableCollection<Booking>();
        public ObservableCollection<Booking> Bookings
        {
            get => _bookings;
            set
            {
                _bookings = value;
                OnPropertyChanged();
            }
        }

        public void AddBooking(Booking booking)
        {
            Bookings.Add(booking);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}