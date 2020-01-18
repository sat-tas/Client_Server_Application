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
    public partial class Third : Window
    {
        private static int n;
        private static int m;
        List<int> PDouble = new List<int>();
        List<int> VDouble = new List<int>();
        List<List<double>> VVDouble = new List<List<double>>();
        List<List<double>> VPDouble = new List<List<double>>();

        public Third()
        {
            InitializeComponent();
            But2.IsEnabled = false;

        }

        public static DataTable ToDataTable<T>(T[,] matrix)
        {
            var res = new DataTable();

            res.Columns.Add("Работники|Вакансии", typeof(string));



            for (int i = 0; i < n; i++)
            {
                res.Columns.Add($"Вакансия - {i + 1}", typeof(T));
            }

            for (int i = 0; i < m; i++)
            {
                var row = res.NewRow();
                row[0] = "Работник - " + (i + 1);
                for (int j = 1; j < n + 1; j++)
                {
                    row[j] = matrix[j-1,i];
                }

                res.Rows.Add(row);
            }


            return res;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            n = Convert.ToInt32(N.Text);
            m = Convert.ToInt32(M.Text);
            List<string> t0 = new List<string>();
            int[,] arr = { { 1, 2, 3, 4 }, { 5, 6, 7, 8 }, { 9, 10, 11, 12 } };
            dg.ItemsSource = ToDataTable(arr).DefaultView;
            But2.IsEnabled = true;
        }

        private void Calculate(object sender, RoutedEventArgs e)
        {
            n = Convert.ToInt32(M.Text);
            m = Convert.ToInt32(N.Text);
            List<List<double>> result = new List<List<double>>();
            for (int i = 0; i < n; i++)
            {
                result.Add(new List<double>());
                DataRowView dataTable = dg.Items[i] as DataRowView;
                for (int j = 0; j < m; j++)
                {
                    result[i].Add(Convert.ToDouble(dataTable[j + 1]));
                }
            }

            List<List<Double>> VVDoubleCopy = new List<List<double>>(result);
            List<List<int>> rez = hungarian(result);

            Paragraph paragraph = new Paragraph();
            paragraph.Inlines.Add("Исполнитель\tРабота\r\n");

            double sum = 0;
            for (int i = 0; i < m; i++)
            {
                foreach (int j in rez[i])
                    paragraph.Inlines.Add((j + 1).ToString() + " \t\t\t ");
                sum += VVDoubleCopy[rez[i][0]][rez[i][1]];
                paragraph.Inlines.Add("\r\n");
            }
            paragraph.Inlines.Add("F = " + sum + "\r\n");
            richTextBox1.Document.Blocks.Add(paragraph);
        }
               
        List<List<int>> hungarian(List<List<double>> matrix)
        {
            try
            {
                // Размеры матрицы
                int height = matrix.Count, width = matrix.Sum(x => x.Count) / height;
                if (height > width)
                {
                    for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j < height - width; j++)
                            matrix[i].Add(0);
                    }
                }
                width = height;
                // Значения, вычитаемые из строк (u) и столбцов (v)
                // VDouble u(height, 0), v(width, 0);
                List<double> u = new List<double>(height);
                List<double> v = new List<double>(width);

                for (int a = 0; a < height; a++)
                    u.Add(0);
                for (int a = 0; a < width; a++)
                    v.Add(0);


                // Индекс помеченной клетки в каждом столбце
                List<int> markIndices = new List<int>(width);
                for (int a = 0; a < width; a++)
                    markIndices.Add(-1);

                // Будем добавлять строки матрицы одну за другой
                int count = 0;
                for (int i = 0; i < height; i++)
                {
                    List<int> links = new List<int>(width);
                    List<double> mins = new List<double>(width);
                    List<int> visited = new List<int>(width);

                    for (int a = 0; a < width; a++)
                    {
                        links.Add(-1);
                        mins.Add(int.MaxValue);
                        visited.Add(0);
                    }

                    // Разрешение коллизий (создание "чередующейся цепочки" из нулевых элементов)
                    int markedI = i, markedJ = -1, j = 0;
                    while (markedI != -1)
                    {
                        // Обновим информацию о минимумах в посещенных строках непосещенных столбцов
                        // Заодно поместим в j индекс непосещенного столбца с самым маленьким из них
                        j = -1;
                        for (int j1 = 0; j1 < width; j1++)
                            if (visited[j1] != 1)
                            {
                                if (matrix[markedI][j1] - u[markedI] - v[j1] < mins[j1])
                                {
                                    mins[j1] = matrix[markedI][j1] - u[markedI] - v[j1];
                                    links[j1] = markedJ;
                                }
                                if (j == -1 || mins[j1] < mins[j])
                                    j = j1;
                            }

                        // Теперь нас интересует элемент с индексами (markIndices[links[j]], j)
                        // Произведем манипуляции со строками и столбцами так, чтобы он обнулился
                        double delta = mins[j];
                        for (int j1 = 0; j1 < width; j1++)
                            if (visited[j1] == 1)
                            {
                                u[markIndices[j1]] += delta;
                                v[j1] -= delta;
                            }
                            else
                            {
                                mins[j1] -= delta;
                            }
                        u[i] += delta;

                        // Если коллизия не разрешена - перейдем к следующей итерации
                        visited[j] = 1;
                        markedJ = j;
                        markedI = markIndices[j];
                        count++;
                    }

                    // Пройдем по найденной чередующейся цепочке клеток, снимем отметки с
                    // отмеченных клеток и поставим отметки на неотмеченные
                    for (; links[j] != -1; j = links[j])
                        markIndices[j] = markIndices[links[j]];
                    markIndices[j] = i;
                }

                // Вернем результат в естественной форме
                List<List<int>> result = new List<List<int>>();
                for (int j = 0; j < width; j++)
                    if (markIndices[j] != -1)
                        result.Add(new List<int>() { markIndices[j], j });
                return result;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return new List<List<int>>();
            }

        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Choose.window.Show();
        }


    }
}
