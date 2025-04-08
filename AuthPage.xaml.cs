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
using System.Windows.Threading;

namespace Alekseev41
{
    /// <summary>
    /// Логика взаимодействия для AuthPage.xaml
    /// </summary>
    public partial class AuthPage : Page
    {
        private DispatcherTimer timer;
        private int count = 10;
        private string _captchaAnswer = "";
        private string _ValidLitters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwyz1234567890";
        private bool _isCaptched = false;

        public AuthPage()
        {
            InitializeComponent();
            TBCaptcha.Visibility = Visibility.Hidden;
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
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
            LoginTB.Text = "";
            PassTB.Text = "";
            string login = LoginTB.Text;
            string password = PassTB.Text;
            User user = Alekseev41Entities.GetContext().User.ToList().Find(p => p.UserLogin == login && p.UserPassword == password);
            Manager.MainFrame.Navigate(new ProductPage(user));
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            LoginTB.Text = "";
            PassTB.Text = "";
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
                CaptchaDisable();
            }
            else
            {
                MessageBox.Show("Введены неверные данные");
                if (TBCaptcha.IsVisible)
                {
                    LoginButton.IsEnabled = false;
                    for (int i = 10; i >= 0; i--)
                    {
                        Timer.Text = i.ToString();
                        await Task.Delay(1000); // Ждем 1 секунду
                        if (i == 0)
                        {
                            Timer.Visibility = Visibility.Hidden;
                        }
                    }
                    LoginButton.IsEnabled = true;
                }
                CaptchaEnable();
            }
        }
        private void CaptchaDisable()
        {
            _isCaptched = true;
            TBCaptcha.Visibility = Visibility.Hidden;
            captchaOneWord.Text = "";
            captchaTwoWord.Text = "";
            captchaThreeWord.Text = "";
            captchaFourWord.Text = "";
            TBCaptcha.Text = "";
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            count--;
            Timer.Text = count.ToString();

            if (count <= 0)
            {
                timer.Stop();
            }
        }
    }
}

