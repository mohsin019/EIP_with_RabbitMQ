﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReceiveLogsDirectInfo
{
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;

    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare("direct_logs", "direct");
                    var queueName = channel.QueueDeclare().QueueName;

                    //if (args.Length < 1)
                    //{
                    //    Console.Error.WriteLine("Usage: {0} [info] [warning] [error]",
                    //                            Environment.GetCommandLineArgs()[0]);
                    //    Environment.ExitCode = 1;
                    //    return;
                    //}

                    foreach (var severity in new string[] { "info", "warning" })
                    {
                        channel.QueueBind(queueName, "direct_logs", severity);
                    }

                    Console.WriteLine(" [*] Waiting for messages. " +
                                      "To exit press CTRL+C");

                    var consumer = new QueueingBasicConsumer(channel);
                    channel.BasicConsume(queueName, true, consumer);

                    while (true)
                    {
                        var ea = (BasicDeliverEventArgs)consumer.Queue.Dequeue();

                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body);
                        var routingKey = ea.RoutingKey;
                        Console.WriteLine(" [x] Received '{0}':'{1}'",
                                          routingKey, message);
                    }
                }
            }
        }
    }
}
