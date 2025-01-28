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
        public ProductPage()
        {
            InitializeComponent();
            
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
                currentProducts = currentProducts.Where(p => (Convert.ToInt32(p.ProductMaxDiscount) >= 0 && Convert.ToInt32(p.ProductMaxDiscount) <= 100)).ToList();
            }
            if (ComboType.SelectedIndex == 1)
            {
                currentProducts = currentProducts.Where(p => (Convert.ToInt32(p.ProductMaxDiscount) >= 0 && Convert.ToInt32(p.ProductMaxDiscount) < 5)).ToList();
            }
            if (ComboType.SelectedIndex == 2)
            {
                currentProducts = currentProducts.Where(p => (Convert.ToInt32(p.ProductMaxDiscount) >= 5 && Convert.ToInt32(p.ProductMaxDiscount) < 15)).ToList();
            }
            if (ComboType.SelectedIndex == 3)
            {
                currentProducts = currentProducts.Where(p => (Convert.ToInt32(p.ProductMaxDiscount) >= 15 && Convert.ToInt32(p.ProductMaxDiscount) < 30)).ToList();
            }
            if (ComboType.SelectedIndex == 4)
            {
                currentProducts = currentProducts.Where(p => (Convert.ToInt32(p.ProductMaxDiscount) >= 30 && Convert.ToInt32(p.ProductMaxDiscount) < 70)).ToList();
            }
            if (ComboType.SelectedIndex == 5)
            {
                currentProducts = currentProducts.Where(p => (Convert.ToInt32(p.ProductMaxDiscount) >= 70 && Convert.ToInt32(p.ProductMaxDiscount) < 100)).ToList();
            }

            currentProducts = currentProducts.Where(p => p.ProductName.ToLower().Contains(TBoxSearch.Text.ToLower())).ToList();

            ProductListView.ItemsSource = currentProducts.ToList();

            if (RButtonDown.IsChecked.Value)
            {
                ProductListView.ItemsSource = currentProducts.OrderByDescending(p => p.ProductCost).ToList();
            }
            if (RButtonUp.IsChecked.Value)
            {
                ProductListView.ItemsSource = currentProducts.OrderBy(p => p.ProductCost).ToList();
            }
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
    }
}
