using RabbitMQ.Client;
using System;
using System.ComponentModel;
using System.Text;
using System.Linq;
using System.Runtime.InteropServices;

namespace Frends.Community.RabbitMQ
{
    /// <summary>
    /// Collection of tasks for interfacing with RabbitMQ
    /// </summary>
    public class RabbitMQTask
    {
        private static readonly ConnectionFactory Factory = new ConnectionFactory();
        
        private static IConnection OpenConnectionIfClosed(string hostName, bool connectWithURI)
        {
            lock (Factory)
            {
                IConnection _connection = null;
                {
                    if (connectWithURI)
                    {
                        Factory.Uri = new Uri(hostName);
                    }
                    else
                    {
                        Factory.HostName = hostName;
                    }
                    _connection = Factory.CreateConnection();
                }
                
                return _connection; 
            }
        }

        /// <summary>
        /// Closes connection and channel to RabbitMQ
        /// </summary>
        public static void CloseConnection(IModel _channel, IConnection _connection)
        {
            if (_channel != null)
            {
                _channel.Close();
                _channel.Dispose();
            }
            if (_connection != null)
            {
                _connection.Close();
            }
        }

        /// <summary>
        /// Writes messages into a queue with batch publish. All messages are under transaction. If one the message failes all messages will be rolled back.  
        /// </summary>
        /// <param name="inputParams"></param>
        public static bool WriteMessage([PropertyTab] WriteInputParams inputParams)
        {
            IConnection _connection = OpenConnectionIfClosed(inputParams.HostName, inputParams.ConnectWithURI);
            IModel _channel = _connection.CreateModel();
            try
            {
                if (inputParams.Create)
                {
                    _channel.QueueDeclare(queue: inputParams.QueueName,
                        durable: inputParams.Durable,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null);
                    _channel.ConfirmSelect();
                }

                IBasicProperties basicProperties = null;

                if (inputParams.Durable == true)
                {
                    basicProperties = _channel.CreateBasicProperties();
                    basicProperties.Persistent = true;
                }

                
                if (inputParams.WriteMessageCount == null || 
                    inputParams.WriteMessageCount != null && int.Parse(inputParams.WriteMessageCount) == 1)
                {
                    _channel.BasicPublish(exchange:
                        inputParams.ExchangeName,
                        routingKey: inputParams.RoutingKey,
                        basicProperties: basicProperties,
                        body: inputParams.Data);
                    return true;
                }
                else
                {
                    // Add message into a memory based on producer write capability.
                    if (inputParams.WriteMessageCount != null &&
                        _channel.MessageCount(inputParams.QueueName) <= int.Parse(inputParams.WriteMessageCount))
                    {
                        _channel.CreateBasicPublishBatch().Add(
                            exchange: inputParams.ExchangeName,
                            routingKey: inputParams.RoutingKey,
                            mandatory: true,
                            properties: basicProperties,
                            body: inputParams.Data);
                        return false;
                    }
                
                    // Commit under transaction when all of the messages have been received for the producer.
                    if (inputParams.WriteMessageCount != null &&
                        _channel.MessageCount(inputParams.QueueName) == int.Parse(inputParams.WriteMessageCount))
                    {
                        try
                        {
                            _channel.TxSelect();
                            _channel.CreateBasicPublishBatch().Publish();
                            _channel.TxCommit();
                            if (_channel.MessageCount(inputParams.QueueName) > 0)
                            {
                                _channel.TxRollback();
                                return false;
                            }
                            return true;
                        }
                        catch (Exception exception)
                        {
                            _channel.TxRollback();
                            throw exception;
                        }
                    }
                
                    else
                    {
                        return false;
                    }
                }
            }
            finally
            {
               CloseConnection(_channel, _connection); 
            }

            
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
            IConnection _connection = OpenConnectionIfClosed(inputParams.HostName, inputParams.ConnectWithURI);
            IModel _channel = _connection.CreateModel();
            try
            {
                Output output = new Output();
                
                while (inputParams.ReadMessageCount-- > 0)
                {
                    var rcvMessage = _channel.BasicGet(queue: inputParams.QueueName,
                        autoAck: inputParams.AutoAck == ReadAckType.AutoAck);
                    if (rcvMessage != null)
                    {
                        output.Messages.Add(new Message
                        {
                            Data = Convert.ToBase64String(rcvMessage.Body), MessagesCount = rcvMessage.MessageCount,
                            DeliveryTag = rcvMessage.DeliveryTag
                        });
                    }
                    //break the loop if no more messagages are present
                    else
                    {
                        break;
                    }
                }

                // Auto acking
                if (inputParams.AutoAck != ReadAckType.AutoAck && inputParams.AutoAck != ReadAckType.ManualAck)
                {
                    ManualAckType ackType = ManualAckType.NackAndRequeue;

                    switch (inputParams.AutoAck)
                    {
                        case ReadAckType.AutoNack:
                            ackType = ManualAckType.Nack;
                            break;

                        case ReadAckType.AutoNackAndRequeue:
                            ackType = ManualAckType.NackAndRequeue;
                            break;


                        case ReadAckType.AutoReject:
                            ackType = ManualAckType.Reject;
                            break;

                        case ReadAckType.AutoRejectAndRequeue:
                            ackType = ManualAckType.RejectAndRequeue;
                            break;
                    }

                    foreach (var message in output.Messages)
                    {
                        AcknowledgeMessage(_channel, ackType, message.DeliveryTag);
                    }
                }
                
                return output;
            }
            finally
            {
                CloseConnection(_channel, _connection); 
            }
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

        /// <summary>
        /// Acknowledges received message. Throws exception on error.
        /// </summary>
        /// <param name="ackType"></param>
        /// <param name="deliveryTag"></param>
        public static void AcknowledgeMessage(IModel _channel, ManualAckType ackType, ulong deliveryTag)
        {
            if (_channel == null)
            {
                // do not try to re-connect, because messages already nacked automatically
                throw new Exception("No connection to RabbitMQ");
            }

            switch (ackType)
            {
                case ManualAckType.Ack:
                    _channel.BasicAck(deliveryTag, multiple: false);
                    break;

                case ManualAckType.Nack:
                    _channel.BasicNack(deliveryTag, multiple: false, requeue: false);
                    break;

                case ManualAckType.NackAndRequeue:
                    _channel.BasicNack(deliveryTag, multiple: false, requeue: true);
                    break;

                case ManualAckType.Reject:
                    _channel.BasicReject(deliveryTag, requeue: false);
                    break;

                case ManualAckType.RejectAndRequeue:
                    _channel.BasicReject(deliveryTag, requeue: true);
                    break;
            }
        }
    }
}
