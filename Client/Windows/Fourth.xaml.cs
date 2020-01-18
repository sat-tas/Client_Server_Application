using System;
using System.Collections.Generic;
using System.Data;
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

namespace Client.Windows
{
    /// <summary>
    /// Логика взаимодействия для Third.xaml
    /// </summary>
    public partial class Fourth : Window
    {
        private static int n;
        private static int m;
        private List<List<int>> Data;
        private List<double> P;

        public Fourth()
        {
            InitializeComponent();
            But2.IsEnabled = false;

        }

        public static DataTable ToDataTable<T>(T[,] matrix)
        {
            var res = new DataTable();

            res.Columns.Add("Предложение|Спрос", typeof(string));
                       
            for (int i = 0; i < n; i++)
            {
                res.Columns.Add($"{i + 1}", typeof(T));
            }

            for (int i = 0; i < m; i++)
            {
                var row = res.NewRow();
                row[0] = (i + 1);
                for (int j = 1; j < n + 1; j++)
                {
                    row[j] = matrix[j-1,i];
                }

                res.Rows.Add(row);
            }

            DataRow row1 = res.NewRow();
            row1[0] = "Вероятность";
            res.Rows.Add(row1);
            return res;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            n = Convert.ToInt32(N.Text);
            m = Convert.ToInt32(M.Text);
            List<string> t0 = new List<string>();
            int[,] arr = new int[n,m];
            dg.ItemsSource = ToDataTable(arr).DefaultView;
            But2.IsEnabled = true;
        }

        private void Calculate(object sender, RoutedEventArgs e)
        {
            n = Convert.ToInt32(M.Text);
            m = Convert.ToInt32(N.Text);
            Data = new List<List<int>>();
            for (int i = 0; i < n; i++)
            {
                Data.Add(new List<int>());
                DataRowView dataTable = dg.Items[i] as DataRowView;
                for (int j = 0; j < m; j++)
                {
                    Data[i].Add(Convert.ToInt32(dataTable[j + 1]));
                }
            }

            currentMethod[] currents = { Maxmin, Gurvich, Bernul, Savidg };
            string[] str = { "Метод Вальди:", "Метод Гурвицца:", "Метод Бернули", "Метод Сэвиджа:" };

            bool[] vib = { (bool)Valdi.IsChecked, (bool)Gyrvich.IsChecked, (bool)Bernuli.IsChecked, (bool)Savidgc.IsChecked};

            Paragraph paragraph = new Paragraph();
            paragraph.Inlines.Add("Результат\r\n");


            int[] result = new int[n];
            for (int i = 0; i < 4; i++)
            {
                if (vib[i] == true)
                {
                    int[] res = currents[i]();
                    paragraph.Inlines.Add(str[i]+" ");
                    for (int j = 0; j < res.Length; j++)
                    {
                        paragraph.Inlines.Add((res[j] + 1).ToString());
                        result[res[j]]++;
                    }
                    paragraph.Inlines.Add("\n");
                }
            }
            int max = result.Max();
            int nom = 0;
            for (int i = 0; i < n; i++)
                if (result[i] == max)
                {
                    nom = i + 1;
                    break;
                }
            paragraph.Inlines.Add($"Среди выбраных решений наиболее частым является {nom}, следовательно опимальным решением будет выбрать стратегию A{nom}\n");
            richTextBox1.Document.Blocks.Add(paragraph);
        }

        delegate int[] currentMethod();



        private int[] Maxmin()
        {
            List<int> minArray = new List<int>();
            for (int i = 0; i < Data.Count; i++)
            {
                minArray.Add(Data[i].Min());
            }
            int max = minArray.Max();

            List<int> res = new List<int>();
            for (int i = 0; i < minArray.Count; i++)
            {
                if (minArray[i] == max)
                    res.Add(i);
            }
            return res.ToArray();
        }

        private int[] Savidg()
        {
            List<List<int>> copyData = new List<List<int>>(Data);

            for (int i = 0; i < copyData[0].Count; i++)
            {
                List<int> Array = new List<int>();
                for (int j = 0; j < copyData.Count; j++)
                {
                    Array.Add(copyData[j][i]);
                }
                (int max, _) = findMax(Array);
                for (int j = 0; j < copyData[i].Count; j++)
                {
                    copyData[j][i] -= max;
                    copyData[j][i] *= -1;

                }
            }

            List<int> maxArray = new List<int>();
            for (int i = 0; i < copyData.Count; i++)
            {
                maxArray.Add(copyData[i].Max());
            }

            int minimum = maxArray.Min();

            List<int> res = new List<int>();
            for (int i = 0; i < maxArray.Count; i++)
            {
                if (maxArray[i] == minimum)
                    res.Add(i);
            }
            return res.ToArray();

        }

        private int[] Gurvich()
        {
            double a = 0.5;
            List<double> aArray = new List<double>();

            //if (textBox3.Text != "")
            //    a = Convert.ToDouble(textBox3.Text);


            for (int i = 0; i < Data.Count; i++)
            {
                (int max, _) = findMax(Data[i]);
                (int min, _) = findMin(Data[i]);
                aArray.Add(a * min + (1 - a) * max);
            }

            double minimum = aArray.Min();

            List<int> res = new List<int>();
            for (int i = 0; i < aArray.Count; i++)
            {
                if (aArray[i] == minimum)
                    res.Add(i);
            }
            return res.ToArray();

        }

        private int[] Bernul()
        {
            DataRowView data = dg.Items[m] as DataRowView;
            P = new List<double>();
            for (int i = 1; i < m + 1; i++)
                P.Add(Convert.ToDouble(data[i]));

            List<double> pArray = new List<double>();

            for (int i = 0; i < Data.Count; i++)
            {
                double sum = 0;
                for (int j = 0; j < Data[i].Count; j++)
                {
                    sum += P[j] * Data[i][j];
                }
                pArray.Add(sum);
            }

            double max = pArray.Max();

            List<int> res = new List<int>();
            for (int i = 0; i < pArray.Count; i++)
            {
                if (pArray[i] == max)
                    res.Add(i);
            }
            return res.ToArray();

        }


        private (int, int[]) findMax(List<int> elements)
        {
            int max = elements[0];
            for (int i = 0; i < elements.Count; i++)
                max = max < elements[i] ? elements[i] : max;

            List<int> res = new List<int>();
            for (int i = 0; i < elements.Count; i++)
            {
                if (max == elements[i])
                    res.Add(i);
            }
            return (max, res.ToArray());
        }

        private (int, int[]) findMin(List<int> elements)
        {
            int min = elements[0];
            for (int i = 0; i < elements.Count; i++)
                min = min > elements[i] ? elements[i] : min;

            List<int> res = new List<int>();
            for (int i = 0; i < elements.Count; i++)
            {
                if (min == elements[i])
                    res.Add(i);
            }
            return (min, res.ToArray());
        }


        private void Window_Closed(object sender, EventArgs e)
        {
            Choose.window.Show();
        }


    }
}
