using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace pr14V2.Pages
{
    public class SessionDisplay
    {
        public int SessionId { get; set; }
        public DateTime SessionTime { get; set; }
        public string HallName { get; set; }
        public decimal Price { get; set; }
    }

    public partial class MovieDetailPage : Page
    {
        private Movie movie;

        public MovieDetailPage(Movie selectedMovie)
        {
            InitializeComponent();
            movie = selectedMovie;
            ShowMovieInfo();
            ShowSessions();
        }

        private void ShowMovieInfo()
        {
            MovieTitle.Text = movie.MovieName;
            MovieRating.Text = $"★ {movie.Rating}";
            MovieAge.Text = $"{movie.AgeRating}+";
            MovieDescription.Text = movie.Description;
            if (!string.IsNullOrEmpty(movie.Image))
            {
                MoviePoster.Source = new BitmapImage(new Uri(movie.Image, UriKind.RelativeOrAbsolute));
            }
        }

        private void ShowSessions()
        {
            using (var db = new _14prLoobchkinGusenkovContext())
            {
                var sessions = db.Sessions
                    .Include(s => s.Hall)
                    .Where(s => s.MovieId == movie.Id)
                    .Where(s => s.StartDateTime >= DateTime.Now)
                    .OrderBy(s => s.StartDateTime)
                    .ToList();

                List<SessionDisplay> displayList = new List<SessionDisplay>();
                foreach (var s in sessions)
                {
                    displayList.Add(new SessionDisplay
                    {
                        SessionId = s.Id,
                        SessionTime = s.StartDateTime ?? DateTime.Now,
                        HallName = s.Hall?.Name ?? $"Зал {s.HallId}",
                        Price = s.BaseTickerPrice ?? 0
                    });
                }

                SessionsList.ItemsSource = displayList;

                if (displayList.Count == 0)
                {
                    MessageBox.Show("Для этого фильма пока нет доступных сеансов", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void SelectSession_Click(object sender, RoutedEventArgs e)
        {
            if (Core.CurrentUser == null)
            {
                MessageBox.Show("Для выбора сеанса необходимо авторизоваться");
                return;
            }

            var button = sender as Button;
            if (button == null) return;

            int sessionId = (int)button.Tag;

            using (var db = new _14prLoobchkinGusenkovContext())
            {
                var session = db.Sessions
                    .Include(s => s.Movie)
                    .Include(s => s.Hall)
                    .FirstOrDefault(s => s.Id == sessionId);

                if (session == null)
                {
                    MessageBox.Show("Сеанс не найден");
                    return;
                }

                NavigationService.Navigate(new BuyTicketPage(session, Core.CurrentUser));
            }
        }
    }
}