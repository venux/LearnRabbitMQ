using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Send
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
                    channel.QueueDeclare("helloTest", false, false, false, null);

                    string message = "Hello World!!!";
                    var body = Encoding.UTF8.GetBytes(message);
                    channel.BasicPublish("", "helloTest", null, body);

                    Console.WriteLine("发送消息\"{0}\"成功!", message);
                }
            }

            Console.ReadLine();
        }
    }
}
