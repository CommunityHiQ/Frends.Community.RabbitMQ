using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Frends.Community.RabbitMQ
{
    /// <summary>
    /// Collection of tasks for interfacing with RabbitMQ
    /// </summary>
    public class RabbitMQTask
    {
        /// <summary>
        /// Writes message to a queue
        /// </summary>
        /// <param name="inputParams"></param>
        public static bool WriteMessage([PropertyTab]WriteInputParams inputParams)
        {

            var factory = new ConnectionFactory() { HostName = inputParams.HostName };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: inputParams.QueueName ,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);


                    channel.BasicPublish(exchange: "",
                                         routingKey: inputParams.RoutingKey,
                                         basicProperties: null,
                                         body: inputParams.Data);
                }
            }

            return true;
        }

        /// <summary>
        /// Reads message(s) from a queue. Returns JSON structure with message contents
        /// </summary>
        /// <param name="inputParams"></param>
        /// <returns>JSON structure with message contents</returns>
        public static Output ReadMessage([PropertyTab]ReadInputParams inputParams)
        {
            Output output = new Output();
            
            var factory = new ConnectionFactory() { HostName = inputParams.HostName };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: inputParams.QueueName,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

                    while (inputParams.ReadMessageCount-- > 0)
                    {
                        var rcvMessage = channel.BasicGet(queue: inputParams.QueueName, autoAck: inputParams.AutoAck);
                        if (rcvMessage != null)
                        {
                            output.Messages.Add(new Message { Data = Convert.ToBase64String(rcvMessage.Body), MessagesCount = rcvMessage.MessageCount, DeliveryTag = rcvMessage.DeliveryTag });
                            if (!inputParams.AutoAck)
                                channel.BasicNack(rcvMessage.DeliveryTag, false, true);

                        }
                        //break the loop if no more messagages are present
                        else
                        {
                            break;
                        }
                    }
                }
            }
            return output;
        }

    }
}
