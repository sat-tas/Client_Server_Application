using HttpServer;
using Entity;
using DataBase;

using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Text.RegularExpressions;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Server;
using System.Net.Http;
using System.Runtime.InteropServices;

namespace HTTPClient
{
    public class Client
    {
        // Отправка страницы с ошибкой

        [DllImport("MemDll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern double Left(double[] ar, int size, FPtr ptr, double left, double right, double step);

        [DllImport("MemDll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern double Trap(double[] ar, int size, FPtr ptr, double left, double right, double step);

        [DllImport("MemDll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern double Mid(double[] ar, int size, FPtr ptr, double left, double right, double step);

        [DllImport("MemDll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern double MidFind(double[] ar,ref int size, FPtr ptr, double left, double right, double eps);


        public delegate double FPtr(double x);

        private void SendResult(HttpListenerContext Client, int Code)
        {
            HttpListenerResponse response = Client.Response;

            response.ContentType = "text/html";
            response.StatusCode = Code;
            string CodeStr = Code + " " + ((HttpStatusCode)Code).ToString();
            // Код простой HTML-странички
            string Html = "<html><body><h1>" + CodeStr + "</h1></body></html>";
            // Необходимые заголовки: ответ сервера, тип и длина содержимого. После двух пустых строк - само содержимое
            // Приведем строку к виду массива байт
            byte[] Buffer = Encoding.ASCII.GetBytes(Html);
            response.ContentLength64 = Buffer.Length;

            Stream output = response.OutputStream;
            output.Write(Buffer, 0, Buffer.Length);
            output.Close();
            Console.WriteLine("Обработка подключений завершена");
        }

        private void SendResult(HttpListenerContext Client, int Code, string content)
        {
            Console.WriteLine(content + " " + Code);
            HttpListenerResponse response = Client.Response;

            response.ContentType = "text/html";
            response.StatusCode = Code;
            string CodeStr = Code + " " + ((HttpStatusCode)Code).ToString();
            // Необходимые заголовки: ответ сервера, тип и длина содержимого. После двух пустых строк - само содержимое
            // Приведем строку к виду массива байт
            byte[] Buffer = Encoding.ASCII.GetBytes(content);
            response.ContentLength64 = Buffer.Length;

            Stream output = response.OutputStream;

            output.Write(Buffer, 0, Buffer.Length);
            output.Close();
            Console.WriteLine("Обработка подключений завершена");
        }

        public Client(HttpListenerContext Client, ServerDbContext db)
        {

            HttpListenerRequest request = Client.Request;

            // Объявим строку, в которой будет хранится запрос клиента
            string Request = "";
            // Буфер для хранения принятых от клиента данных
            List<byte> Buffer = new List<byte>();
            // Переменная для хранения количества байт, принятых от клиента
            int nextbyte = 0; ;
            // Читаем из потока клиента до тех пор, пока от него поступают данные
            while ((nextbyte = request.InputStream.ReadByte()) != -1)
            {
                Buffer.Add((byte)nextbyte);
                Request += Encoding.ASCII.GetString(new byte[] { (byte)nextbyte }, 0, 1);
                if (Request.IndexOf("\r\n\r\n") >= 0 || Request.Length > 4096)
                {
                    break;
                }
            }
            Request += '&';
            Console.WriteLine(Request);

            #region Get
            // Парсим строку запроса с использованием регулярных выражений
            if (request.HttpMethod == "GET")
            {
                string Response = "";
                if (request.Url.AbsolutePath.EndsWith("/"))
                {
                    Response += "index.html";
                }
                string FilePath = "www/" + Response;

                // Если в папке www не существует данного файла, посылаем ошибку 404
                if (!File.Exists(FilePath))
                {
                    SendResult(Client, 404);
                    return;
                }

                // Открываем файл, страхуясь на случай ошибки
                FileStream FS;
                try
                {
                    FS = new FileStream(FilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                }
                catch (Exception)
                {
                    // Если случилась ошибка, посылаем клиенту ошибку 500
                    SendResult(Client, 500);
                    return;
                }

                HttpListenerResponse response = Client.Response;

                byte[] buffer = new byte[(int)FS.Length];
                // Читаем данные из файла
                FS.Read(buffer, 0, (int)FS.Length);
                response.ContentLength64 = buffer.Length;
                response.ContentType = "text/html";
                Stream output = response.OutputStream;
                output.Write(buffer, 0, buffer.Length);
                // закрываем поток
                output.Close();
                // Закроем файл и соединение
                FS.Close();
                Console.WriteLine("Обработка подключений завершена");
            }
            #endregion Get

            #region Post
            if (request.HttpMethod == "POST")
            {
                Regex argument = new Regex(@"(\w*)=([^\&]*)&", RegexOptions.Compiled);
                MatchCollection matches = argument.Matches(Request);

                Dictionary<string, string> arguments = new Dictionary<string, string>();
                foreach (Match item in matches)
                {
                    arguments.Add(item.Groups[1].Value, item.Groups[2].Value);
                }

                switch (arguments.GetValueOrDefault("Action"))
                {
                    case "Authentication":
                        {
                            var user = db.Users.Select(a => a).Where(a => a.Name == arguments.GetValueOrDefault("Login")).ToList<User>();

                            if (user.Count != 0 && user[0].Authorization(arguments.GetValueOrDefault("Password")))
                            {
                                HttpServer.Server.activeUsers.Add(Client.Request.RemoteEndPoint.ToString(), user[0]);
                                SendResult(Client, 200);
                            }
                            else
                            {
                                SendResult(Client, 400);
                            }
                            break;
                        }
                    case "Closing":
                        {
                            HttpServer.Server.activeUsers.Remove(Client.Request.RemoteEndPoint.ToString());
                            break;
                        }
                    case "Registration":
                        {
                            var user = db.Users.Select(a => a).Where(a => a.Name == arguments.GetValueOrDefault("Login")).ToList<User>();
                            if (user.Count != 0)
                            {
                                SendResult(Client, 401);
                            }
                            else if (Verefications.isCorrectLogin(arguments.GetValueOrDefault("Login")))
                            {
                                User u = new User(arguments.GetValueOrDefault("Login"), arguments.GetValueOrDefault("Password"), arguments.GetValueOrDefault("Email"));
                                db.SaveUser(u);
                                SendResult(Client, 200);
                            }
                            else
                                SendResult(Client, 402);

                            break;
                        }
                    case "CalculateIntegral":
                        {
                            string op = "()+-*/^,";
                            string function = arguments.GetValueOrDefault("Func");
                            double left = Convert.ToDouble(arguments.GetValueOrDefault("Left").Replace($"%{Convert.ToString((int)',', 16)}", ",", ignoreCase: true, null));
                            double right = Convert.ToDouble(arguments.GetValueOrDefault("Right").Replace($"%{Convert.ToString((int)',', 16)}", ",", ignoreCase: true, null));
                            double step = Convert.ToDouble(arguments.GetValueOrDefault("Step").Replace($"%{Convert.ToString((int)',', 16)}", ",", ignoreCase: true, null));
                            string method = arguments.GetValueOrDefault("Method");

                            for (int i = 0; i < op.Length; i++)
                            {
                                function = function.Replace($"%{Convert.ToString((int)op[i], 16)}", op[i].ToString(), ignoreCase: true, null);

                            }
                            function.Replace(" ", "");

                            Parser.Parser parser = new Parser.Parser(function);
                            FPtr f = new FPtr(parser.Calculate);
                            int n = (int)((right - left) / step);
                            double[] res = new double[1000];
                            double integralValue = 0;
                            switch (method)
                            {
                                case "L":
                                    {
                                        integralValue = Left(res, n, f, left, right, step);
                                        break;
                                    }
                                case "T":
                                    {
                                        integralValue = Trap(res, n, f, left, right, step);
                                        break;
                                    }
                                case "M":
                                    {
                                        integralValue = Mid(res, n, f, left, right, step);
                                        break;
                                    }
                                default:
                                    break;
                            }

                            string result = "";
                            int nom = 0;
                            for (int i = 0; i < n * 4; i += 2, nom++)
                            {
                                result += $"X{nom}={String.Format("{0:f3}", res[i])}&Y{nom}={String.Format("{0:f3}", res[i + 1])}&";
                            }
                            result += $"Value={String.Format("{0:f4}", integralValue)}&";
                            SendResult(Client, 200, result);
                            break;
                        }
                    case "CalculateSecond":
                        {
                            string op = "()+-*/^,";
                            string function = arguments.GetValueOrDefault("Func");
                            double left = Convert.ToDouble(arguments.GetValueOrDefault("Left").Replace($"%{Convert.ToString((int)',', 16)}", ",", ignoreCase: true, null));
                            double right = Convert.ToDouble(arguments.GetValueOrDefault("Right").Replace($"%{Convert.ToString((int)',', 16)}", ",", ignoreCase: true, null));
                            double eps = Convert.ToDouble(arguments.GetValueOrDefault("Eps").Replace($"%{Convert.ToString((int)',', 16)}", ",", ignoreCase: true, null));

                            for (int i = 0; i < op.Length; i++)
                            {
                                function = function.Replace($"%{Convert.ToString((int)op[i], 16)}", op[i].ToString(), ignoreCase: true, null);

                            }
                            function.Replace(" ", "");

                            Parser.Parser parser = new Parser.Parser(function);
                            FPtr f = new FPtr(parser.Calculate);
                            int n = 0;
                            double[] res = new double[1000];
                            
                            double integralValue = 0;
                            integralValue = MidFind(res,ref n, f, left, right, eps);

                            string result = "";
                            int nom = 0;
                            for (int i = 0; i < n; i += 2, nom++)
                            {
                                result += $"X{nom}={String.Format("{0:f3}", res[i])}&Y{nom}={String.Format("{0:f3}", res[i + 1])}&";
                            }
                            result += $"Value={String.Format("{0:f4}", integralValue)}&";
                            SendResult(Client, 200, result);
                            break;
                        }

                }
            }
            #endregion Post
            Client.Response.Close();
        }
    }
}