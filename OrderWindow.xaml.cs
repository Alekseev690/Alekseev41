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
        private int IDclient, CodeOrder, IDorder, OorderPickUpPoint;
        private DateTime OrderDateTime = DateTime.Now, OrderDeliveryDateTime;

        public OrderWindow(List<OrderProduct> selectedOrderProducts, List<Product> selectedProducts, string FIO, int clientId)
        {
            InitializeComponent();

            var currentPickups = Alekseev41Entities.GetContext().PickUpPoint.Select(p => p.PickUpPointIndex + p.PickUpPointCity + p.PickUpPointStreet + " " + p.PickUpPointHome).ToList();
            PickUpComboBox.ItemsSource = currentPickups;

            IDclient = clientId;
            CodeOrder = Alekseev41Entities.GetContext().Order.ToList().Last().OrderCode + 1;
            IDorder = selectedOrderProducts.Last().OrderID + 1;
            ClientTB.Text = FIO;
            TBOrderID.Text = IDorder.ToString();

            foreach (Product p in selectedProducts)
            {
                p.ProductQuantityInStock = 1;
                foreach (OrderProduct q in selectedOrderProducts)
                {
                    if (p.ProductArticleNumber == q.ProductArticleNumber)
                        p.ProductQuantityInStock = q.ProductCount;
                }
            }
            this.selectedOrderProducts = selectedOrderProducts;
            this.selectedProducts = selectedProducts;
            Refresh();
            SetDeliveryDate();
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
                MessageBox.Show("Ни одного продукта нет в корзине!");
                return;
            }
            if (OorderPickUpPoint == 0)
            {
                MessageBox.Show("Не выбрана точка вывоза!");
                return;
            }

            newOrder.OrderID = IDorder;
            newOrder.OrderDate = OrderDateTime;
            newOrder.OrderDeliveryDate = OrderDeliveryDateTime;
            newOrder.OrderPickupPoint = OorderPickUpPoint;
            newOrder.OrderClient = IDclient;
            newOrder.OrderCode = CodeOrder;
            newOrder.OrderStatus = "Новый";

            foreach (Product selectprod in selectedProducts)
            {
                OrderProduct newOrderProd = new OrderProduct();
                newOrderProd.OrderID = IDorder;
                newOrderProd.ProductArticleNumber = selectprod.ProductArticleNumber;
                newOrderProd.ProductCount = selectprod.ProductQuantityInStock;

                newOrderProducts.Add(newOrderProd);
            }

            foreach (OrderProduct ordprod in newOrderProducts)
                Alekseev41Entities.GetContext().OrderProduct.Add(ordprod);
            Alekseev41Entities.GetContext().Order.Add(newOrder);
            try
            {
                Alekseev41Entities.GetContext().SaveChanges();

                MessageBox.Show("Заказ успешно оформлен");
                Manager.MainFrame.GoBack();
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
            prod.ProductQuantityInStock--;
            var selectedOP = selectedOrderProducts.FirstOrDefault(p => p.ProductArticleNumber == prod.ProductArticleNumber);
            int index = selectedOrderProducts.IndexOf(selectedOP);
            selectedOrderProducts[index].ProductCount--;

            if (prod.ProductQuantityInStock <= 0)
                selectedProducts.Remove(prod);
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
                    if (prod.ProductQuantityInStock >= origProd.ProductQuantityInStock)
                    {
                        MessageBox.Show("Выбрано максимальное кол-во товаров!");
                        return;
                    }
                }
            }
            prod.ProductQuantityInStock++;
            var selectedOP = selectedOrderProducts.FirstOrDefault(p => p.ProductArticleNumber == prod.ProductArticleNumber);
            int index = selectedOrderProducts.IndexOf(selectedOP);
            selectedOrderProducts[index].ProductCount++;
            SetDeliveryDate();
            ShoeListView.Items.Refresh();
        }
    }
}
