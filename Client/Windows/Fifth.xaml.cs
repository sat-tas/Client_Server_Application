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
    public partial class Fifth : Window
    {
        public Fifth()
        {
            InitializeComponent();
            Chart chart = this.FindName("MyWinformChart") as Chart;

            chart.ChartAreas.Add(new ChartArea("Default"));

            // Добавим линию, и назначим ее в ранее созданную область "Default"
            chart.Series.Add(new Series("Series1"));

            chart.Series[0].BorderWidth = 5;


            chart.Series["Series1"].ChartArea = "Default";
 
            chart.Series["Series1"].ChartType = SeriesChartType.Spline;
            chart.Height = 500;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            string expression = Expression.Text;
       
            Parser.Parser parser = new Parser.Parser(expression);

            Chart chart = this.FindName("MyWinformChart") as Chart;
            chart.Series["Series1"].Points.Clear();
       
            double left = Convert.ToDouble(Left.Text);
            double right = Convert.ToDouble(Right.Text);
            double step = Convert.ToDouble(Step.Text);

            for (double x = left; x < right + step / 2; x += step)
                chart.Series["Series1"].Points.AddXY(x, parser.Calculate(x));
       
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
