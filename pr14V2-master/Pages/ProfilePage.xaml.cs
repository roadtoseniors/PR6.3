using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;

namespace pr14V2.Pages
{
    public partial class ProfilePage : Page
    {
        private User _user;

        public ProfilePage(User user)
        {
            InitializeComponent();
            _user = user;
            LoadData();
        }

        private void LoadData()
        {
            LoginText.Text = _user.Login;
            DateText.Text = _user.CreatedAt.ToString("dd.MM.yyyy") ?? "Не указана";

            using (var db = new _14prLoobchkinGusenkovContext())
            {
                var tickets = db.Tickets
                    .Include(t => t.Seat)
                        .ThenInclude(s => s.SessionSeats)
                            .ThenInclude(ss => ss.Session)
                                .ThenInclude(se => se.Movie)
                    .Include(t => t.Seat)
                        .ThenInclude(s => s.SessionSeats)
                            .ThenInclude(ss => ss.Session)
                                .ThenInclude(se => se.Hall)
                    .Where(t => t.UserId == _user.Id)
                    .Select(t => new
                    {
                        Movie = t.Seat.SessionSeats.First().Session.Movie.MovieName,
                        Hall = t.Seat.SessionSeats.First().Session.Hall.Name,
                        Date = t.Seat.SessionSeats.First().Session.StartDateTime.HasValue
                            ? t.Seat.SessionSeats.First().Session.StartDateTime.Value.ToString("dd.MM.yyyy HH:mm")
                            : "Время не указано",
                        Seat = $"Ряд {t.Seat.RowNumber}, Место {t.Seat.SeatNumber}",
                        Price = t.FinalPrice.HasValue ? $"{t.FinalPrice.Value:N0} ₽" : "—",
                        Status = t.Status ?? "Оплачен"
                    })
                    .ToList();

                TicketsList.ItemsSource = tickets;

                
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
                _user = null;
                NavigationService.Navigate(new LoginPage());
                while (NavigationService.CanGoBack)
                {
                    NavigationService.RemoveBackEntry();
                }
                MessageBox.Show("Вы успешно вышли из аккаунта");
            
        }
    }
}