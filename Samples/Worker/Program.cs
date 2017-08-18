using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Worker
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

                    var consumer = new QueueingBasicConsumer(channel);

                    channel.BasicConsume("workQueue", false, consumer);
                    /**
                     * 公平分发
                     * 在一个工作者还在处理消息，并且没有响应消息之前，不要给他分发新的消息。
                     * 相反，将这条新的消息发送给下一个不那么忙碌的工作者。
                     */
                    channel.BasicQos(0, 1, false);

                    Console.WriteLine("等待接收消息：");
                    while (true)
                    {
                        var ea = (BasicDeliverEventArgs)consumer.Queue.Dequeue();

                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body);

                        int dots = message.Split('.').Length - 1;
                        Thread.Sleep(dots * 1000);

                        Console.WriteLine("接收消息成功,消息内容：\n{0}", message);

                        channel.BasicAck(ea.DeliveryTag, false);
                    }
                }
            }

            Console.ReadLine();
        }
    }
}
