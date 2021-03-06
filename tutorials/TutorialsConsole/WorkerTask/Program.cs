﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace WorkerTask
{
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;
    
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare("task_queue", true, false, false, null);

                channel.BasicQos(0, 1, false);
                var consumer = new QueueingBasicConsumer(channel);
                channel.BasicConsume("task_queue", false, consumer);

                Console.WriteLine(" [*] Waiting for messages. " +
                                  "To exit press CTRL+C");
                while (true)
                {
                    var ea =
                        (BasicDeliverEventArgs)consumer.Queue.Dequeue();

                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine(" [x] Received {0}", message);

                    int dots = message.Split('.').Length - 1;
                    Thread.Sleep(dots * 1000);

                    Console.WriteLine(" [x] Done " + ea.DeliveryTag);

                    channel.BasicAck(ea.DeliveryTag, false);
                }
            }

        }
    }
}
