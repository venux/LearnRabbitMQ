using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Server
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
                    channel.QueueDeclare(queue: "rpcQueue", durable: false, exclusive: false, autoDelete: false, arguments: null);
                    //公平分发
                    channel.BasicQos(0, 1, false);

                    //1.作为消费者
                    var consumer = new EventingBasicConsumer(channel);
                    Console.WriteLine("服务端等待客户端发送请求...");

                    channel.BasicConsume(queue: "rpcQueue", autoAck: false, consumer: consumer);
                    consumer.Received += (model, ea) =>
                    {
                        string response = null;

                        var body = ea.Body;//接收到的请求内容
                        var props = ea.BasicProperties;//请求附带的属性参数
                        var replyProps = channel.CreateBasicProperties();
                        replyProps.CorrelationId = props.CorrelationId;//响应时附带请求带过来的唯一ID

                        try
                        {
                            var message = Encoding.UTF8.GetString(body);
                            int n = int.Parse(message);
                            Console.WriteLine("计算{0}的斐波那契数。", n);
                            response = CalculateFib(n).ToString();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("计算出错，错误信息：" + ex.Message);
                            response = "";
                        }
                        finally
                        {
                            //2.处理完后，身份变为生产者
                            var responseBytes = Encoding.UTF8.GetBytes(response);
                            channel.BasicPublish(exchange: "", routingKey: props.ReplyTo, basicProperties: replyProps, body: responseBytes);
                            channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                        }
                    };

                    Console.WriteLine("输入回车后退出！");
                    Console.ReadLine();
                }
            }
        }

        private static int CalculateFib(int n)
        {
            if (n < 0)
            {
                throw new ArgumentException("请输入大于或等于零的整数！");
            }

            if (n == 0 || n == 1)
            {
                return n;
            }

            return CalculateFib(n - 1) + CalculateFib(n - 2);
        }
    }
}
