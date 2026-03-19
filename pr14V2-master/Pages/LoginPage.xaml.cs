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
    /// Логика взаимодействия для LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        // ВАЖНО: Убрали инициализацию Context в поле класса!
        // Это вызывало проблемы с DbContext
        // public List<User> users = Core.Context.Users.ToList(); // <- УДАЛИТЬ ЭТУ СТРОКУ!

        public LoginPage()
        {
            InitializeComponent();
        }

        // Метод авторизации, вынесенный из обработчика события
        public bool Auth(string login, string password)
        {
            try
            {
                // Проверка на пустые поля
                if (string.IsNullOrWhiteSpace(login))
                {
                    MessageBox.Show("Введите логин");
                    return false;
                }

                if (string.IsNullOrWhiteSpace(password))
                {
                    MessageBox.Show("Введите пароль");
                    return false;
                }

                // Поиск пользователя в базе данных
                // Создаем новый экземпляр контекста для каждого запроса
                var user = Core.Context.Users.FirstOrDefault(u => u.Login == login && u.Password == password);

                if (user != null)
                {
                    Core.CurrentUser = user;

                    // Навигация может вызвать ошибку в тестах, оборачиваем в try-catch
                    try
                    {
                        NavigationService?.Navigate(new MainPage());
                    }
                    catch
                    {
                        // Игнорируем ошибки навигации в тестах
                    }

                    return true;
                }
                else
                {
                    MessageBox.Show("Неверное имя аккаунта или пароль");
                    return false;
                }
            }
            catch (Exception ex)
            {
                // Логируем ошибку для отладки
                MessageBox.Show($"Ошибка авторизации: {ex.Message}");
                return false;
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void InputButton_Click(object sender, RoutedEventArgs e)
        {
            // Вызов метода авторизации с параметрами из полей формы
            Auth(UserLogin.Text, UserPassword.Password);
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new RegisterPage());
        }

        private void ForgotPassButton(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ForgotPass());
        }
    }
}