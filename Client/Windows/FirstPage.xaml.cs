using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
    /// Логика взаимодействия для FirstPage.xaml
    /// </summary>  
    /// 
    public partial class FirstPage : Window
    {
        public FirstPage()
        {
            InitializeComponent();
            Chart chart = this.FindName("MyWinformChart") as Chart;

            chart.ChartAreas.Add(new ChartArea("Default"));

            // Добавим линию, и назначим ее в ранее созданную область "Default"
            chart.Series.Add(new Series("Series1"));
            chart.Series.Add(new Series("Series2"));

            chart.Series[1].BorderWidth = 5;


            chart.Series["Series1"].ChartArea = "Default";
            chart.Series["Series2"].ChartArea = "Default";

            chart.Series["Series2"].ChartType = SeriesChartType.Spline;
            chart.Series["Series1"].ChartType = SeriesChartType.Area;
            chart.Height = 500;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            double res = 0;
            if (double.TryParse(Left.Text, out res) || double.TryParse(Step.Text, out res) || double.TryParse(Right.Text, out res))
            { }
            else
            {
                string expression = Expression.Text;
                string[] methods = { "L", "T", "M" };
                bool?[] method = { LeftIntegral.IsChecked, TrapIntegral.IsChecked, MidIntegral.IsChecked };
                int nomer = 0;
                for (int i = 0; i < 3; i++)
                    if (method[i] == true)
                    {
                        nomer = i;
                        break;
                    }
                var values = new Dictionary<string, string>{
                                                        { "Action", "CalculateIntegral" },
                                                        { "Func", expression },
                                                        { "Left", Left.Text },
                                                        { "Right", Right.Text },
                                                        { "Step", Step.Text },
                                                        { "Method",methods[nomer]} };


                var response = await Sender.Sender.Send(values);

                var responseString = await response.Content.ReadAsStringAsync();

                Regex argument = new Regex(@"([^\&]*)=([^\&]*)&", RegexOptions.Compiled);
                MatchCollection matches = argument.Matches(responseString);

                Dictionary<string, double> arguments = new Dictionary<string, double>();
                foreach (Match item in matches)
                {
                    arguments.Add(item.Groups[1].Value, double.Parse(item.Groups[2].Value));
                }

                Parser.Parser parser = new Parser.Parser(expression);

                Chart chart = this.FindName("MyWinformChart") as Chart;
                chart.Series["Series1"].Points.Clear();
                chart.Series["Series2"].Points.Clear();

                double left = Convert.ToDouble(Left.Text);
                double right = Convert.ToDouble(Right.Text);
                double step = Convert.ToDouble(Step.Text);

                for (double i = 0; i < arguments.Count / 2 - 1; i++)
                {
                    chart.Series["Series1"].Points.AddXY(arguments[$"X{i}"], arguments[$"Y{i}"]);
                }

                for (double x = left; x < right + step / 2; x += step)
                    chart.Series["Series2"].Points.AddXY(x, parser.Calculate(x));
                Result.Text = arguments["Value"].ToString();
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
