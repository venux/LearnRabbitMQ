using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
                UserName = "venux",
                Password = "311005040117"
            };

            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    var replyQueueName = channel.QueueDeclare().QueueName;

                    var consumer = new EventingBasicConsumer(channel);
                    Console.WriteLine("客户端发送请求...");

                    channel.BasicConsume(queue: replyQueueName, autoAck: true, consumer: consumer);
                    //1.作为生产者
                    var corrId = Guid.NewGuid().ToString();
                    var props = channel.CreateBasicProperties();
                    props.ReplyTo = replyQueueName;
                    props.CorrelationId = corrId;

                    int n = 30;
                    var messageBytes = Encoding.UTF8.GetBytes(n.ToString());
                    channel.BasicPublish(exchange: "", routingKey: "rpcQueue", basicProperties: props, body: messageBytes);

                    consumer.Received += (model, ea) =>
                    {
                        var responseProps = ea.BasicProperties;//附带的属性参数

                        if (responseProps.CorrelationId == corrId)
                        {
                            var body = Encoding.UTF8.GetString(ea.Body);//接收到的请求内容
                            Console.WriteLine("计算结果：" + body);
                        }
                    };

                    Console.WriteLine("输入回车后退出！");
                    Console.ReadLine();
                }
            }
        }
    }
}
