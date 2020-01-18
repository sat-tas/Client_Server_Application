using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net.Sockets;
using System.Net.Http;
using Client.Windows;

namespace Client
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow window = null;
        public static bool isConnected=true;
        Dictionary<int, double> value;
        int count = 0;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            window = this;
        }

        private async void Login_Clicked(object sender, RoutedEventArgs e)
        {
            if (count++ < 3)
            {
                var values = new Dictionary<string, string>{
                                                        { "Action", "Authentication" },
                                                        { "Login", Login.Text },
                                                        { "Password", Password.Text }
                                                       };


                var responseMessage =await Sender.Sender.Send(values);
                if (responseMessage.StatusCode==System.Net.HttpStatusCode.NotFound)
                {
                    ErrorLabel.Visibility = Visibility.Hidden;
                    count = 0;
                    isConnected = false;
                    Choose choose = new Choose();
                    choose.B1.IsEnabled = false;
                    choose.Title = "Подключение не было установлено";
                    this.Hide();
                    choose.Show();
                }
                else
                if (responseMessage.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    ErrorLabel.Visibility = Visibility.Hidden;
                    count = 0;
                    isConnected = true;
                    Choose choose = new Choose();
                    this.Hide();
                    choose.Show();
                }
                else
                {
                    ErrorLabel.Visibility = Visibility.Visible;
                }

            }
            else
            {
                MessageBox.Show("Количество попыток израсходавано, попробуйте позже");
                this.Close();
            }

        }

        private async void Register_Clicked(object sender, RoutedEventArgs e)
        {
            if (count++ < 3)
            {
                var values = new Dictionary<string, string>{
                                                        { "Action", "Registration" },
                                                        { "Login", Login1.Text },
                                                        { "Password", Password1.Text },
                                                        { "Email",Email.Text }
                                                       };


                var responseMessage = await Sender.Sender.Send(values);
                if (responseMessage.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    RegError.Visibility = Visibility.Hidden;
                    count = 0;
                    Choose choose = new Choose();
                    this.Hide();
                    choose.Show();
                }
                else
                {
                    switch ((int)responseMessage.StatusCode)
                    {
                        case 401:
                            {
                                RegError.Content = "Данный пользователь уже зарегистрирован";
                                break;
                            }
                        case 402:
                            {
                                RegError.Content = "Неверный логин или пароль";
                                break;
                            }
                        default:
                            break;
                    }
                    RegError.Visibility = Visibility.Visible;
                }

            }
            else
            {
                MessageBox.Show("Количество попыток израсходавано, попробуйте позже");
                this.Close();
            }
        }

        private async void Window_Closed(object sender, EventArgs e)
        {
        }
    }
}
