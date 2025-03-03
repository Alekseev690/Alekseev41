using System;
using System.Collections.Generic;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace Alekseev41
{
    /// <summary>
    /// Логика взаимодействия для AuthPage.xaml
    /// </summary>
    public partial class AuthPage : Page
    {
        private string _captchaAnswer = "";
        private string _ValidLitters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwyz1234567890";
        private bool _isCaptched = false;

        public AuthPage()
        {
            InitializeComponent();
        }

        private void CaptchaEnable()
        {
            _isCaptched = false;
            TBCaptcha.Visibility = Visibility.Visible;

            Random random = new Random();

            captchaOneWord.Text = Convert.ToString(_ValidLitters[random.Next(_ValidLitters.Length)]);
            captchaTwoWord.Text = Convert.ToString(_ValidLitters[random.Next(_ValidLitters.Length)]);
            captchaThreeWord.Text = Convert.ToString(_ValidLitters[random.Next(_ValidLitters.Length)]);
            captchaFourWord.Text = Convert.ToString(_ValidLitters[random.Next(_ValidLitters.Length)]);

            _captchaAnswer = captchaOneWord.Text + captchaTwoWord.Text + captchaThreeWord.Text + captchaFourWord.Text;
        }

        private void LogingGuest_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginTB.Text;
            string password = PassTB.Text;
            User user = Alekseev41Entities.GetContext().User.ToList().Find(p => p.UserLogin == login && p.UserPassword == password);
            Manager.MainFrame.Navigate(new ProductPage(user));
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginTB.Text;
            string password = PassTB.Text;
            if (login == "" || password == "")
            {
                MessageBox.Show("Есть пустые поля");
                return;
            }

            if (TBCaptcha.Text == _captchaAnswer)
                _isCaptched = true;

            if (_isCaptched)
            {
                if (TBCaptcha.Text != _captchaAnswer)
                {
                    MessageBox.Show("Каптча введена неверно!");
                    CaptchaEnable();
                    LoginButton.IsEnabled = false;
                    await Task.Delay(10000);
                    LoginButton.IsEnabled = true;
                    return;
                }
                _isCaptched = false;
                CaptchaPanel.Visibility = Visibility;
            }
            if (login == "" || password == "")
            {
                MessageBox.Show("Есть пустые поля");
                CaptchaEnable();
                return;
            }

            User user = Alekseev41Entities.GetContext().User.ToList().Find(p => p.UserLogin == login && p.UserPassword == password);
            if (user != null)
            {
                Manager.MainFrame.Navigate(new ProductPage(user));
                LoginTB.Text = "";
                PassTB.Text = "";
            }
            else
            {
                MessageBox.Show("Введены неверные данные");
                if (TBCaptcha.IsVisible)
                {
                    LoginButton.IsEnabled = false;
                    await Task.Delay(10000);
                    LoginButton.IsEnabled = true;
                }
                CaptchaEnable();
            }
        }
        
    }
}

