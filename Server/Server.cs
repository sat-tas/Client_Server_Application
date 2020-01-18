using FSharpVSPowerTools ;
using HTTPClient;
using Server;
using Entity;
using System;
using DataBase;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
using System.Runtime.InteropServices;


namespace HttpServer
{
    public class Server
    {


        // Объект, принимающий TCP-клиентов
        private HttpListener Listener;
        //Объект DataBase
        private static ServerDbContext db;
        

        //Список активных пользователей
        public static Dictionary<string,User> activeUsers=new Dictionary<string, User>();

        //Обработка подключения
        static void ClientThread(Object StateInfo)
        {
            new Client((HttpListenerContext)StateInfo, db);

        }
         
        // Запуск сервера
        public Server()
        {
               
            //Подключаем базу данныъ
            using (db = new ServerDbContext())
            {
                // Создаем "слушателя" для указанного порта
         
                Listener = new HttpListener();
#if DEBUG
                
                Listener.Prefixes.Add($"http://+:80/");
#else
                Listener.Prefixes.Add("http://127.0.0.1/");


#endif
                // Запускаем его
                Listener.Start();
                Console.WriteLine($"Server is run");

                // В бесконечном цикле
                while (true)
                {
                    // Принимаем новых клиентов
                    Console.WriteLine($"Waiting connect");
                    HttpListenerContext context = Listener.GetContext();

                    //Создаем поток
                    Thread Thread = new Thread(new ParameterizedThreadStart(ClientThread));

                    // И запускаем этот поток, передавая ему принятого клиента
                    Thread.Start(context);
                    

                }
            }
        }

        // Остановка сервера(деконструктор)
        ~Server()
        {
            // Если "слушатель" был создан
            if (Listener != null)
            {
                // Остановим его
                Listener.Stop();
            }
        }

        static void Main(string[] args)
        {
            // Создадим новый сервер на порту 80
            new Server();
        }
    }
}
