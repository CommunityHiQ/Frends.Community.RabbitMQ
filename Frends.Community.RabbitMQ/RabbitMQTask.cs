using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Linq;

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

            var factory = new ConnectionFactory();

			if (inputParams.ConnectWithURI)
			{
				factory.Uri = new Uri(inputParams.HostName);
			}
			else
			{
				factory.HostName = inputParams.HostName;
			}

			using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    if (inputParams.Create)
                    {
                        channel.QueueDeclare(queue: inputParams.QueueName,
                                     durable: inputParams.Durable,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                    }

                    channel.BasicPublish(exchange: inputParams.ExchangeName,
                                         routingKey: inputParams.RoutingKey,
                                         basicProperties: null,
                                         body: inputParams.Data);
                }
            }

            return true;
        }


        /// <summary>
        /// Writes message to queue. Message is a string and there is internal conversion from string to byte[] using UTF8 encoding
        /// </summary>
        /// <param name="inputParams"></param>
        /// <returns></returns>
        public static bool WriteMessageString([PropertyTab]WriteInputParamsString inputParams)
        {
            WriteInputParams wip = new WriteInputParams
            {
                ConnectWithURI = inputParams.ConnectWithURI,
                Create = inputParams.Create,
                Data = Encoding.UTF8.GetBytes(inputParams.Data),
                Durable = inputParams.Durable,
                HostName = inputParams.HostName,
				ExchangeName = inputParams.ExchangeName,
                QueueName = inputParams.QueueName,
                RoutingKey = inputParams.RoutingKey
            };

            return WriteMessage(wip);

        }

        /// <summary>
        /// Reads message(s) from a queue. Returns JSON structure with message contents. Message data is byte[] encoded to base64 string
        /// </summary>
        /// <param name="inputParams"></param>
        /// <returns>JSON structure with message contents</returns>
        public static Output ReadMessage([PropertyTab]ReadInputParams inputParams)
        {
            Output output = new Output();

			var factory = new ConnectionFactory();

			if (inputParams.ConnectWithURI)
			{
				factory.Uri = new Uri(inputParams.HostName);
			}
			else
			{
				factory.HostName = inputParams.HostName;
			}

			using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {

                    //channel.QueueDeclare(queue: inputParams.QueueName,
                    //             durable: false,
                    //             exclusive: false,
                    //             autoDelete: false,
                    //             arguments: null);

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

        /// <summary>
        /// Reads message(s) from a queue. Returns JSON structure with message contents. Message data is string converted from byte[] using UTF8 encoding
        /// </summary>
        /// <param name="inputParams"></param>
        /// <returns>JSON structure with message contents</returns>
        public static OutputString ReadMessageString([PropertyTab]ReadInputParams inputParams)
        {
            var messages = ReadMessage(inputParams);
            OutputString outString = new OutputString();
            outString.Messages = messages.Messages.Select(m =>
              new MessageString
              {
                  DeliveryTag = m.DeliveryTag,
                  MessagesCount = m.MessagesCount,
                  Data = Encoding.UTF8.GetString(Convert.FromBase64String(m.Data))
              }).ToList();

            return outString;
        }
    }
}
