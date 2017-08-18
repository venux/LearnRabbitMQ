using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory();
            factory.HostName = "localhost";
            factory.UserName = "venux";
            factory.Password = "311005040117";

            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    //第二个参数durable表示队列是否持久化
                    channel.QueueDeclare("workQueue", false, false, false, null);

                    string message = GetMessage(args);
                    var properties = channel.CreateBasicProperties();
                    properties.DeliveryMode = 2;
                    //消息持久化
                    properties.Persistent=true;
                   
                    var body = Encoding.UTF8.GetBytes(message);
                    channel.BasicPublish("", "workQueue", properties, body);

                    Console.WriteLine("发送消息\"{0}\"成功!", message);
                }
            }

            Console.ReadLine();
        }

        private static string GetMessage(string[] args)
        {
            return ((args.Length > 0) ? string.Join(" ", args) : "Hello World!");
        }
    }
}
