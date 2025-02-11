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
        public AuthPage()
        {
            InitializeComponent();
        }

        private void LogingGuest_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginTB.Text;
            string password = PassTB.Text;
            User user = Alekseev41Entities.GetContext().User.ToList().Find(p => p.UserLogin == login && p.UserPassword == password);
            Manager.MainFrame.Navigate(new ProductPage(user));
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginTB.Text;
            string password = PassTB.Text;
            if (login == "" || password == "")
            {
                MessageBox.Show("Есть пустые поля");
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
                LoginButton.IsEnabled = false;
                Thread.Sleep(10000); // 10 секунд таймер
                LoginButton.IsEnabled = true;
            }
        }
    }
}
