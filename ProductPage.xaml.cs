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

namespace Alekseev41
{
    /// <summary>
    /// Логика взаимодействия для ProductPage.xaml
    /// </summary>
    public partial class ProductPage : Page
    {
        List<Product> selectedProdList = new List<Product>();
        List<OrderProduct> selectedOrderProducts = new List<OrderProduct>();
        private bool guestMode = false;
        private int _newOrderID = Alekseev41Entities.GetContext().OrderProduct.Count() + 1;
        private int _clientID;
        public ProductPage(User user)
        {
            InitializeComponent();

            OrderButton.Visibility = Visibility.Hidden;

            if (user == null)
            {
                FIOTB.Text = "";
                RoleTB.Text = "";
                guestMode = true;
            }
            else
            {
                FIOTB.Text = user.UserSurname + " " + user.UserName + " " + user.UserPatronymic;
                switch (user.UserRole)
                {
                    case 1:
                        RoleTB.Text = "Клиент"; break;
                    case 2:
                        RoleTB.Text = "Менеджер"; break;
                    case 3:
                        RoleTB.Text = "Администратор"; break;
                }
            }

            var currentProduct = Alekseev41Entities.GetContext().Product.ToList();

            ProductListView.ItemsSource = currentProduct;

            ComboType.SelectedIndex = 0;

            UpdateProduct();
        }

        public void UpdateProduct()
        {
            var currentProducts = Alekseev41Entities.GetContext().Product.ToList();

            if (ComboType.SelectedIndex == 0)
            {
                currentProducts = currentProducts.Where(p => (Convert.ToInt32(p.ProductDiscountAmount) >= 0 && Convert.ToInt32(p.ProductDiscountAmount) <= 100)).ToList();
            }
            if (ComboType.SelectedIndex == 1)
            {
                currentProducts = currentProducts.Where(p => (Convert.ToInt32(p.ProductDiscountAmount) >= 0 && Convert.ToInt32(p.ProductDiscountAmount) < 10)).ToList();
            }
            if (ComboType.SelectedIndex == 2)
            {
                currentProducts = currentProducts.Where(p => (Convert.ToInt32(p.ProductDiscountAmount) >= 10 && Convert.ToInt32(p.ProductDiscountAmount) < 15)).ToList();
            }
            if (ComboType.SelectedIndex == 3)
            {
                currentProducts = currentProducts.Where(p => (Convert.ToInt32(p.ProductDiscountAmount) >= 15 && Convert.ToInt32(p.ProductDiscountAmount) < 100)).ToList();
            }

            if (RButtonDown.IsChecked.Value)
            {
                currentProducts = currentProducts.OrderByDescending(p => p.ProductCostInt).ToList();
            }
            if (RButtonUp.IsChecked.Value)
            {
               currentProducts = currentProducts.OrderBy(p => p.ProductCostInt).ToList();
            }

            currentProducts = currentProducts.Where(p => p.ProductName.ToLower().Contains(TBoxSearch.Text.ToLower())).ToList();

            TBlockCount.Text = Convert.ToString(currentProducts.Count());
            TBlockMaxCount.Text = Convert.ToString(Alekseev41Entities.GetContext().Product.Count());

            ProductListView.ItemsSource = currentProducts.ToList();
        }

        private void TBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateProduct();
        }

        private void ComboType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateProduct();
        }

        private void RButtonUp_Checked(object sender, RoutedEventArgs e)
        {
            UpdateProduct();
        }

        private void RButtonDown_Checked(object sender, RoutedEventArgs e)
        {
            UpdateProduct();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (guestMode)
            {
                MessageBox.Show("Это функция доступна только авторизированным пользователям!");
                return;
            }
            if (ProductListView.SelectedIndex >= 0)
            {
                var selectedProduct = ProductListView.SelectedItem as Product;
                selectedProdList.Add(selectedProduct);

                var newOrderProd = new OrderProduct();
                newOrderProd.OrderID = _newOrderID;

                newOrderProd.ProductArticleNumber = selectedProduct.ProductArticleNumber;
                newOrderProd.ProductCount = 1;

                var selOP = selectedOrderProducts.Where(p => Equals(p.ProductArticleNumber, selectedProduct.ProductArticleNumber));

                if (selOP.Count() == 0)
                {
                    selectedOrderProducts.Add(newOrderProd);
                }
                else
                {
                    foreach (OrderProduct p in selectedOrderProducts)
                    {
                        if (p.ProductArticleNumber == selectedProduct.ProductArticleNumber)
                            p.ProductCount++;
                    }
                }
                OrderButton.Visibility = Visibility.Visible;
                ProductListView.SelectedIndex = -1;
            }
        }

        private void OrderButton_Click(object sender, RoutedEventArgs e)
        {
            selectedProdList = selectedProdList.Distinct().ToList();

            OrderWindow orderWindow = new OrderWindow(selectedOrderProducts, selectedProdList, FIOTB.Text, _clientID);
            orderWindow.ShowDialog();
        }
    }
}
