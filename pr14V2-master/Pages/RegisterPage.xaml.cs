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
    /// Логика взаимодействия для RegisterPage.xaml
    /// </summary>
    public partial class RegisterPage : Page
    {
        // ВАЖНО: Убрали инициализацию Context в поле класса!
        // public List<User> users = Core.Context.Users.ToList(); // <- ЭТА СТРОКА ДОЛЖНА БЫТЬ УДАЛЕНА!

        public RegisterPage()
        {
            InitializeComponent();
        }

        // Метод регистрации, вынесенный из обработчика события
        public bool Register(string login, string password, string confirmPassword)
        {
            try
            {
                // Валидация: проверка на пустой логин
                if (string.IsNullOrWhiteSpace(login))
                {
                    MessageBox.Show("Введите логин");
                    return false;
                }

                // Валидация: проверка на существование пользователя
                if (Core.Context.Users.Any(u => u.Login == login))
                {
                    MessageBox.Show("Пользователь с таким логином уже существует");
                    return false;
                }

                // Валидация: проверка на пустой пароль
                if (string.IsNullOrWhiteSpace(password))
                {
                    MessageBox.Show("Введите пароль");
                    return false;
                }

                // Валидация: проверка на совпадение паролей
                if (password != confirmPassword)
                {
                    MessageBox.Show("Пароли не совпадают");
                    return false;
                }

                // Создание нового пользователя
                var newUser = new User
                {
                    Login = login,
                    Password = password,
                    CreatedAt = DateTime.Now
                };

                Core.Context.Users.Add(newUser);
                Core.Context.SaveChanges();

                MessageBox.Show("Регистрация прошла успешно!");

                // Навигация может вызвать ошибку в тестах, оборачиваем в try-catch
                try
                {
                    NavigationService?.Navigate(new LoginPage());
                }
                catch
                {
                    // Игнорируем ошибки навигации в тестах
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при регистрации: {ex.Message}");
                return false;
            }
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void InputButton_Click(object sender, RoutedEventArgs e)
        {
            // Вызов метода регистрации с параметрами из полей формы
            Register(RegLoginTextBox.Text, FirstRegPassTextBox.Password, SecondRegPassTextBox.Password);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}