using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Client.Windows
{
    /// <summary>
    /// Логика взаимодействия для SecondPage.xaml
    /// </summary>  
    /// 
    public partial class SecondPage : Window
    {
        public SecondPage()
        {
            InitializeComponent();
            Chart chart = this.FindName("MyWinformChart") as Chart;

            chart.ChartAreas.Add(new ChartArea("Default"));

            // Добавим линию, и назначим ее в ранее созданную область "Default"
            chart.Height = 500;
        }

        [DllImport("MemDll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern double MidFind(double[] ar, ref int size, FPtr ptr, double left, double right, double eps);

        public delegate double FPtr(double x);

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            double tr = 0;
            if (double.TryParse(Left.Text, out tr) || double.TryParse(Eps.Text, out tr) || double.TryParse(Right.Text, out tr))
            { }
            else
            {

                string expression = Expression.Text;
                Chart chart = this.FindName("MyWinformChart") as Chart;
                chart.Series.Clear();
                chart.Series.Add(new Series("Series1"));

                chart.Series[0].BorderWidth = 5;

                chart.Series["Series1"].ChartArea = "Default";
                chart.Series["Series1"].ChartType = SeriesChartType.Spline;

                double left = Convert.ToDouble(Left.Text);
                double right = Convert.ToDouble(Right.Text);
                double step = (right - left) / 20;
                Parser.Parser parser = new Parser.Parser(expression);



                if (MainWindow.isConnected)
                {
                    var values = new Dictionary<string, string>{
                                                        { "Action", "CalculateSecond" },
                                                        { "Func", expression },
                                                        { "Left", Left.Text },
                                                        { "Right", Right.Text },
                                                        { "Eps", Eps.Text } };


                    var response = await Sender.Sender.Send(values);

                    var responseString = await response.Content.ReadAsStringAsync();

                    Regex argument = new Regex(@"([^\&]*)=([^\&]*)&", RegexOptions.Compiled);
                    MatchCollection matches = argument.Matches(responseString);

                    Dictionary<string, double> arguments = new Dictionary<string, double>();
                    foreach (Match item in matches)
                    {
                        arguments.Add(item.Groups[1].Value, double.Parse(item.Groups[2].Value));
                    }

                    for (double i = 0; i < arguments.Count / 2 - 1; i += 2)
                    {

                        chart.Series.Add(new Series($"Series{2 + i}"));
                        chart.Series[$"Series{2 + i}"].ChartArea = "Default";
                        chart.Series[$"Series{2 + i}"].ChartType = SeriesChartType.Spline;
                        chart.Series[$"Series{2 + i}"].BorderWidth = 3;

                        chart.Series[$"Series{2 + i}"].Points.AddXY(arguments[$"X{i}"], arguments[$"Y{i}"]);
                        chart.Series[$"Series{2 + i}"].Points.AddXY(arguments[$"X{i + 1}"], arguments[$"Y{i + 1}"]);
                        Result.Text = arguments["Value"].ToString();

                    }
                }
                else
                {
                    FPtr f = new FPtr(parser.Calculate);
                    int n = 0;
                    double[] res = new double[1000];

                    double integralValue = 0;
                    integralValue = MidFind(res, ref n, f, left, right, Convert.ToDouble(Eps.Text));
                    Result.Text = integralValue.ToString();

                }
                for (double x = left; x < right + step / 2; x += step)
                    chart.Series["Series1"].Points.AddXY(x, parser.Calculate(x));
            }
        }

    private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
    {

    }

    private async void Window1_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        Choose.window.Show();
    }

}
}
