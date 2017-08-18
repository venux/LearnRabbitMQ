# LearnRabbitMQ
## [官网地址](https://www.rabbitmq.com)

## 安装&配置
- 见参考文章1

## 概念
### 1. 生产者（producer）
![producer](https://www.rabbitmq.com/img/tutorials/producer.png)
### 2. 队列（queue）
![queue](https://www.rabbitmq.com/img/tutorials/queue.png)
### 3. 消费者（consumer）
![consumer](https://www.rabbitmq.com/img/tutorials/consumer.png)
### 4. 交换机（exchange）
![exchange](https://www.rabbitmq.com/img/tutorials/exchanges.png)
### 4. 工作/任务队列（work/task queue）
### 5. 工作者（worker）
### 6. 任务（task）
### 7. 轮询分发（round-robin）
默认，RabbitMQ会将每个消息按照顺序依次分发给下一个消费者。所以每个消费者接收到的消息个数大致是平均的。
### 8.消息响应（acknowledgments）
为了防止消息丢失，RabbitMQ提供了消息响应机制。消费者会通过一个ack（响应），告诉RabbitMQ已经收到并处理了某条消息，然后RabbitMQ才会释放并删除这条消息。
```C#
bool no_ack=false;
channel.BasicConsume("hello", no_ack, consumer);

while (true)
{
    var ea = (BasicDeliverEventArgs)consumer.Queue.Dequeue();

    var body = ea.Body;
    var message = Encoding.UTF8.GetString(body);

    int dots = message.Split('.').Length - 1;
    Thread.Sleep(dots * 1000);

    Console.WriteLine("Received {0}", message);
    Console.WriteLine("Done");

    /**
     * It's a common mistake to miss the BasicAck. It's an easy error, but the consequences are serious. 
     * Messages will be redelivered when your client quits (which may look like random redelivery), 
     * but RabbitMQ will eat more and more memory as it won't be able to release any unacked messages.
     */
    channel.BasicAck(ea.DeliveryTag, false);
}
```
### 9.消息持久化
当RabbitMQ Server 关闭或者崩溃，那么里面存储的队列和消息默认是不会保存下来的。如果要让RabbitMQ保存住消息，需要在两个地方同时设置：需要保证队列和消息都是持久化的。
```C#
bool durable = true;
channel.queueDeclare("task_queue", durable, false, false, null);

var properties = channel.CreateBasicProperties();
properties.SetPersistent(true);
```
### 10.公平分发
消息的分发可能并没有如我们想要的那样公平分配。比如，对于两个工作者。当奇数个消息的任务比较重，但是偶数个消息任务比较轻时，奇数个工作者始终处理忙碌状态，而偶数个工作者始终处理空闲状态。但是RabbitMQ并不知道这些，他仍然会平均依次的分发消息。
```c#
channel.BasicQos(0, 1, false); 
```

## [插件](https://www.rabbitmq.com/management.html)
`rabbitmq-plugins enable rabbitmq_management`
### 管理界面
[地址](http://localhost:15672/)

## 章节
### 1.HelloWorld
[见参考文章1](#doc1)
### 2.Work Queue
见参考文章1
### 3.Publish/Subscribe 

- exchange

    An exchange is a very simple thing. On one side it receives messages from producers and the other side it pushes them to queues. The exchange must know exactly what to do with a message it receives. 
  
    类型：
    - direct
    
        ![direct](https://www.rabbitmq.com/img/tutorials/direct-exchange.png)
    - topic

        `* `(star) can substitute for exactly one word.

        `#` (hash) can substitute for zero or more words.
        
        ![topic](https://www.rabbitmq.com/img/tutorials/python-five.png)
    - headers
    - fanout

        ![exchange](https://www.rabbitmq.com/img/tutorials/exchanges.png)
        
- 临时队列（Temporary queues）
    
    In the .NET client, when we supply no parameters to queueDeclare() we create a non-durable, exclusive, autodelete queue with a generated name. For example it may look like amq.gen-JzTY20BRgKO-HjmUJj0wLg.

    随机队列名称、非持久、独占、自动删除
    ```C#
    var queueName = channel.QueueDeclare();
    ```

## 参考文章
<span id="doc1">1. [.NET 环境中使用RabbitMQ](http://www.cnblogs.com/yangecnu/p/Introduce-RabbitMQ.html) </span>
