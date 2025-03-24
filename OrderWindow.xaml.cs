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
using System.Windows.Shapes;

namespace Alekseev41
{
    /// <summary>
    /// Логика взаимодействия для OrderWindow.xaml
    /// </summary>
    public partial class OrderWindow : Window
    {
        List<OrderProduct> selectedOrderProducts = new List<OrderProduct>();
        List<Product> selectedProducts = new List<Product>();
        private bool guest_Mode;
        private int IDclient, CodeOrder, IDorder, OorderPickUpPoint, SumProduct = 0, SumProductWithDiscount = 0;
        private DateTime OrderDateTime = DateTime.Now, OrderDeliveryDateTime;

        public OrderWindow(List<OrderProduct> selectedOrderProducts, List<Product> selectedProducts, string FIO, int clientId, bool guestMode)
        {
            InitializeComponent();

            var currentPickups = Alekseev41Entities.GetContext().PickUpPoint.Select(p => p.PickUpPointIndex + p.PickUpPointCity + p.PickUpPointStreet + " " + p.PickUpPointHome).ToList();
            PickUpComboBox.ItemsSource = currentPickups;

            IDclient = clientId;
            CodeOrder = Alekseev41Entities.GetContext().Order.ToList().Last().OrderCode + 1;
            IDorder = selectedOrderProducts.Last().OrderID + 1;
            ClientTB.Text = FIO;
            TBOrderID.Text = IDorder.ToString();
            guest_Mode = guestMode;

            ShoeListView.ItemsSource = selectedProducts;
            this.selectedOrderProducts = selectedOrderProducts;
            this.selectedProducts = selectedProducts;
            Refresh();
            SetDeliveryDate();
            foreach (Product product in selectedProducts)
            {
                SumProduct += product.ProductCostInt;
                SumProductWithDiscount += product.ProductPriceWithDiscount;
            }
            TBSumProduct.Text = SumProduct.ToString();
            TBSumProductDiscount.Text = SumProductWithDiscount.ToString();
        }
        public void Refresh()
        {
            ShoeListView.ItemsSource = selectedProducts;
        }

        private void SetDeliveryDate()
        {
            OrderDeliveryDateTime = OrderDateTime;
            bool isFastDelivery = true;
            foreach (Product prod in selectedProducts)
            {
                if (prod.ProductQuantityInStock <= 3)
                {
                    isFastDelivery = false;
                }
            }
            if (!isFastDelivery)
                OrderDeliveryDateTime = OrderDeliveryDateTime.AddDays(6);
            else
                OrderDeliveryDateTime = OrderDeliveryDateTime.AddDays(3);
            OrderDP.Text = OrderDateTime.ToString();
            OrderDeliveryDP.Text = OrderDeliveryDateTime.ToString();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            List<OrderProduct> newOrderProducts = new List<OrderProduct>();
            Order newOrder = new Order();

            OrderDateTime = (DateTime)OrderDP.SelectedDate;
            OrderDeliveryDateTime = (DateTime)OrderDeliveryDP.SelectedDate;
            OorderPickUpPoint = PickUpComboBox.SelectedIndex + 1;
            SetDeliveryDate();

            if (selectedProducts.Count == 0)
            {
                MessageBox.Show("Нет ни одного продукта нет в корзине!");
                return;
            }
            if (OorderPickUpPoint == 0)
            {
                MessageBox.Show("Не выбрана точка для вывоза!");
                return;
            }

            newOrder.OrderID = IDorder;
            newOrder.OrderDate = OrderDateTime;
            newOrder.OrderDeliveryDate = OrderDeliveryDateTime;
            newOrder.OrderPickupPoint = OorderPickUpPoint;
            newOrder.OrderCode = CodeOrder;
            newOrder.OrderStatus = "Новый";
            if (guest_Mode)
                newOrder.OrderClient = null;
            else
                newOrder.OrderClient = IDclient;

            foreach (Product selectprod in selectedProducts)
            {
                OrderProduct newOrderProd = new OrderProduct();
                newOrderProd.OrderID = IDorder;
                newOrderProd.ProductArticleNumber = selectprod.ProductArticleNumber;
                newOrderProd.ProductCount = selectprod.GetOrderProductCount;

                newOrderProducts.Add(newOrderProd);
            }
            try
            {
                ShoeListView.ItemsSource = null;
                MessageBox.Show("Заказ успешно оформлен!");
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
            CodeOrder++;
            IDorder++;
        }

        private void MinusButton_Click(object sender, RoutedEventArgs e)
        {
            var prod = (sender as Button).DataContext as Product;
            prod.OrderProductCount--;
            var selectedOP = selectedOrderProducts.FirstOrDefault(p => p.ProductArticleNumber == prod.ProductArticleNumber);
            int index = selectedOrderProducts.IndexOf(selectedOP);
            selectedOrderProducts[index].ProductCount = prod.OrderProductCount;

            if (prod.OrderProductCount <= 0)
                selectedProducts.Remove(prod);
            SumProduct -= prod.ProductCostInt;
            SumProductWithDiscount -= prod.ProductPriceWithDiscount;
            TBSumProduct.Text = SumProduct.ToString();
            TBSumProductDiscount.Text = SumProductWithDiscount.ToString();

            Refresh();
            SetDeliveryDate();
            ShoeListView.Items.Refresh();
        }

        private void PlusButton_Click(object sender, RoutedEventArgs e)
        {
            var prod = (sender as Button).DataContext as Product;
            foreach (Product origProd in Alekseev41Entities.GetContext().Product)
            {
                if (prod.ProductArticleNumber == origProd.ProductArticleNumber)
                {
                    if (prod.OrderProductCount >= origProd.ProductQuantityInStock)
                    {
                        MessageBox.Show("Выбрано максимальное кол-во товаров!");
                        return;
                    }
                }
            }

            prod.OrderProductCount++;
            var selectedOP = selectedOrderProducts.FirstOrDefault(p => p.ProductArticleNumber == prod.ProductArticleNumber);
            int index = selectedOrderProducts.IndexOf(selectedOP);
            selectedOrderProducts[index].ProductCount = prod.OrderProductCount;

            SumProduct += prod.ProductCostInt;
            SumProductWithDiscount += prod.ProductPriceWithDiscount;
            TBSumProduct.Text = SumProduct.ToString();
            TBSumProductDiscount.Text = SumProductWithDiscount.ToString();

            SetDeliveryDate();
            Refresh();
            ShoeListView.Items.Refresh();
        }
    }
}
