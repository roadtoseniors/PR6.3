using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace pr14V2.Pages
{
    public partial class ForgotPass : Page
    {
        private string _captcha;

        public ForgotPass()
        {
            InitializeComponent();
            GenerateCaptcha();
        }

        private void GenerateCaptcha()
        {
            _captcha = Guid.NewGuid().ToString().Substring(0, 5);
            CaptchaText.Text = _captcha;
        }

        private void CheckButton_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginTextBox.Text.Trim();
            string captchaInput = CaptchaInput.Text.Trim();

            using (var db = new _14prLoobchkinGusenkovContext()) 
            {
                bool userExists = db.Users.Any(u => u.Login == login);

                if (!userExists)
                {
                    MessageBox.Show("Проверьте логин. Пользователь не найден.");
                    GenerateCaptcha();
                    return;
                }
            }

            if (captchaInput != _captcha)
            {
                MessageBox.Show("Капча введена неверно!");
                GenerateCaptcha();
                return;
            }

            MessageBox.Show("Красавчик! Теперь вспоминай");
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
