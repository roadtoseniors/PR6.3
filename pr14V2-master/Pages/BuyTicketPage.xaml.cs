using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.EntityFrameworkCore;
namespace pr14V2.Pages
{
    /// <summary>
    /// Логика взаимодействия для BuyTicketPage.xaml
    /// </summary>
    public partial class BuyTicketPage : Page
    {
        private Session _session;
        private User _user;
        private _14prLoobchkinGusenkovContext _db = new _14prLoobchkinGusenkovContext();

        public BuyTicketPage(Session session, User user)
        {
            InitializeComponent();
            _session = session;
            _user = user;
            LoadSeats();
            UpdateInfo();
        }

        private void LoadSeats()
        {
            var seats = _db.Seats
                .Include(s => s.Tickets)
                .Include(s => s.SessionSeats)
                .Where(s => s.SessionSeats.Any(ss => ss.SessionId == _session.Id))
                .ToList();

            foreach (var seat in seats)
            {
                bool isPurchased = seat.Tickets.Any(t => t.Status == "Куплен");

                var button = new Button
                {
                    Content = $"{seat.RowNumber}-{seat.SeatNumber}",
                    Width = 45,
                    Height = 45,
                    Margin = new Thickness(3),
                    Tag = seat,
                    Background = isPurchased ? Brushes.Gray : Brushes.LightGreen,
                    IsEnabled = !isPurchased
                };

                button.Click += Seat_Click;
                SeatsControl.Items.Add(button);
            }
        }

        private void Seat_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var seat = btn.Tag as Seat;

            if (btn.Background == Brushes.LightGreen)
                btn.Background = Brushes.Orange;
            else if (btn.Background == Brushes.Orange)
                btn.Background = Brushes.LightGreen;

            UpdateInfo();
        }

        private void UpdateInfo()
        {
            var selected = SeatsControl.Items
                .Cast<Button>()
                .Where(b => b.Background == Brushes.Orange)
                .ToList();

            SelectedSeatsText.Text = $"Выбрано мест: {selected.Count}";
            TotalPriceText.Text = $"Итого: {selected.Count * _session.BaseTickerPrice:N0} ₽";
            BuyButton.IsEnabled = selected.Count > 0;
        }

        private void BuyButton_Click(object sender, RoutedEventArgs e)
        {
            var selected = SeatsControl.Items
                .Cast<Button>()
                .Where(b => b.Background == Brushes.Orange)
                .ToList();

            if (!selected.Any()) return;

            foreach (var btn in selected)
            {
                var seat = btn.Tag as Seat;

                var ticket = new Ticket
                {
                    UserId = _user.Id,
                    SeatId = seat.Id,
                    PurchaseDateTime = System.DateTime.Now,
                    FinalPrice = _session.BaseTickerPrice,
                    Status = "Куплен"
                };

                _db.Tickets.Add(ticket);
                btn.Background = Brushes.Gray;
                btn.IsEnabled = false;
            }

            _db.SaveChanges();
            MessageBox.Show("Билеты успешно куплены!");
            UpdateInfo();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
