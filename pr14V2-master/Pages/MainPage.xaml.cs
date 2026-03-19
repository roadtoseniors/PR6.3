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

namespace pr14V2.Pages
{
    /// <summary>
    /// Логика взаимодействия для MainPage.xaml
    /// </summary>
    
    public partial class MainPage : Page
    {
        public List<Movie> movies = Core.Context.Movies.ToList();
        public MainPage()
        {
            InitializeComponent();
            prod.ItemsSource = movies;
            UpdateLoginButton();
        }
        private void UpdateLoginButton()
        {
            if (Core.CurrentUser != null)
            {
                MainLoginButton.Content = Core.CurrentUser.Login;
            }
            else
            {
                MainLoginButton.Content = "Войти";
            }
        }

        private void SortFilm(object sender, SelectionChangedEventArgs e)
        {
            if (SortComboBox.SelectedIndex == 0)
            {
                List<Movie> moviesRate = movies.OrderByDescending(u => u.Rating).ToList();
                prod.ItemsSource = moviesRate;
            }
            else
            {
                List<Movie> moviesName = movies.OrderByDescending(u => u.MovieName).ToList();
                prod.ItemsSource = moviesName;
            }
        }

        private void SearchFilm(object sender, TextChangedEventArgs e)
        {
            List<Movie> moviesSearch = movies.Where(p => p.MovieName.ToLower().Contains(Search.Text.ToLower())).ToList();
            prod.ItemsSource = moviesSearch;
        }

        private void MainLoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (Core.CurrentUser == null)
            {
                NavigationService.Navigate(new LoginPage());
            }
            else
            {
                NavigationService.Navigate(new ProfilePage(Core.CurrentUser));
            }
        }
        private void MovieCard_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                Border border = sender as Border;
                if (border != null && border.DataContext is Movie movie)
                {
                    NavigationService.Navigate(new MovieDetailPage(movie));
                }
            }
        }
    }
}
