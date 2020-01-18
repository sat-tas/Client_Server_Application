using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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

namespace Client.Windows
{
    /// <summary>
    /// Логика взаимодействия для Choose.xaml
    /// </summary>
    public partial class Choose : Window
    {
        public static Choose window = null;
        public Choose()
        {
            window = this;
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            FirstPage firstPage = new FirstPage();
            this.Hide();
            firstPage.Show();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            SecondPage secondPage = new SecondPage();
            this.Hide();
            secondPage.Show();

        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Third third = new Third();
            this.Hide();
            third.Show();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            Fourth fourth = new Fourth();
            this.Hide();
            fourth.Show();
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            Fifth fifth = new Fifth();
            this.Hide();
            fifth.Show();
        }

        private async void Window_Closed(object sender, EventArgs e)
        {
            if (MainWindow.isConnected)
            {
                var values = new Dictionary<string, string>{
                                                        { "Action", "Closing" }
                                                       };
                await Sender.Sender.Send(values);
            }
            MainWindow.window.Show();
        }

    }
}
