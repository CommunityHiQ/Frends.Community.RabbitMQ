using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RabbitMQ.Client;

namespace Frends.Community.RabbitMQ.Tests
{
    [TestClass]
    public class UnitTests
    {
        //public const string TestURI = "amqp://user:password@hostname:port/vhost";
        public const string TestURI = "localhost";

        [TestInitialize]
        public void TestInit()
        {

            //var retVal = Frends.Community.RabbitMQ.RabbitMQTask.ReadMessage(new ReadInputParams { HostName = "localhost", QueueName = "queue", AutoAck = ReadAckType.AutoAck, ReadMessageCount = 1000 });
            //var retVal2 = Frends.Community.RabbitMQ.RabbitMQTask.ReadMessage(new ReadInputParams { HostName = TestURI, QueueName = "queue", AutoAck = ReadAckType.AutoAck, ReadMessageCount = 1000, ConnectWithURI = true });

        }

        /// <summary>
        /// Deletes test exchange and queue if it exists
        /// </summary>
        private void DeleteExchangeAndQueue()
        {
            var factory = new ConnectionFactory();
            factory.HostName = "localhost";

            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDelete("queue", false, false);
                    channel.ExchangeDelete("exchange", ifUnused: false);
                }
            }
        }

        /// <summary>
        /// Creates test exchange and queue
        /// </summary>
        private void CreateExchangeAndQueue()
        {
            var factory = new ConnectionFactory();
            factory.HostName = "localhost";

            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare("exchange", type: "fanout", durable: false, autoDelete: false);
                    channel.QueueDeclare("queue", durable: false, exclusive: false, autoDelete: false);
                    channel.QueueBind("queue", "exchange", routingKey: "");
                }
            }
        }

        [TestMethod]
        [Ignore("RabbitMQ is not installed on build server.")]
        public void TestWriteRead()
        {
            DeleteExchangeAndQueue();
            CreateExchangeAndQueue();
            Frends.Community.RabbitMQ.RabbitMQTask.WriteMessage(new WriteInputParams { Data = new byte[] { 0, 1, 2 }, HostName = "localhost", RoutingKey = "queue", QueueName = "queue" });
            var retVal = Frends.Community.RabbitMQ.RabbitMQTask.ReadMessage(new ReadInputParams { HostName = "localhost", QueueName = "queue", AutoAck = ReadAckType.AutoAck, ReadMessageCount = 1 });
            Assert.IsTrue(retVal != null && retVal.Messages.Count() == 1);
        }

        [TestMethod]
        [Ignore("Rabbit/*M*/Q is not installed on build server.")]
        public void TestReadWithAck10()
        {
            DeleteExchangeAndQueue();
            CreateExchangeAndQueue();
            for (int i = 0; i < 10; i++)
                Frends.Community.RabbitMQ.RabbitMQTask.WriteMessage(new WriteInputParams { Data = new byte[] { 0, (byte)(i * i), (byte)i }, HostName = "localhost", RoutingKey = "queue", QueueName = "queue" });
            var retVal = Frends.Community.RabbitMQ.RabbitMQTask.ReadMessage(new ReadInputParams { HostName = "localhost", QueueName = "queue", AutoAck = ReadAckType.AutoAck, ReadMessageCount = 1000 });
            Assert.IsTrue(retVal != null && retVal.Messages.Count() == 10);
        }

        [TestMethod]
        [Ignore("RabbitMQ is not installed on build server.")]
        public void TestReadNoAck10()
        {
            DeleteExchangeAndQueue();
            CreateExchangeAndQueue();
            for (int i = 0; i < 10; i++)
                Frends.Community.RabbitMQ.RabbitMQTask.WriteMessage(new WriteInputParams { Data = new byte[] { 0, (byte)(i * i), (byte)i }, HostName = "localhost", RoutingKey = "queue", QueueName = "queue" });

          var retVal = Frends.Community.RabbitMQ.RabbitMQTask.ReadMessage(new ReadInputParams { HostName = "localhost", QueueName = "queue", AutoAck = ReadAckType.AutoNackAndRequeue, ReadMessageCount = 10 });

            Assert.IsTrue(retVal != null && retVal.Messages.Count() == 10);
        }
        [TestMethod]
        [Ignore("RabbitMQ is not installed on build server.")]
        public void TestWriteReadWithURI()
        {
            DeleteExchangeAndQueue();
            CreateExchangeAndQueue();

            Frends.Community.RabbitMQ.RabbitMQTask.WriteMessage(new WriteInputParams { Data = new byte[] { 0, 1, 2 }, HostName = TestURI, RoutingKey = "queue", QueueName = "queue", ConnectWithURI = true });
            var retVal = Frends.Community.RabbitMQ.RabbitMQTask.ReadMessage(new ReadInputParams { HostName = TestURI, QueueName = "queue", AutoAck = ReadAckType.AutoAck, ReadMessageCount = 1, ConnectWithURI = true });

            Assert.IsTrue(retVal != null && retVal.Messages.Count() == 1);
        }

        [TestMethod]
        [Ignore("RabbitMQ is not installed on build server.")]
        public void TestReadWithAck10WithURI()
        {
            DeleteExchangeAndQueue();
            CreateExchangeAndQueue();
            for (int i = 0; i < 10; i++)
            {
                Frends.Community.RabbitMQ.RabbitMQTask.WriteMessage(new WriteInputParams { Data = new byte[] { 0, (byte)(i * i), (byte)i }, HostName = TestURI, RoutingKey = "queue", QueueName = "queue", ConnectWithURI = false });
            }


            var retVal = Frends.Community.RabbitMQ.RabbitMQTask.ReadMessage(new ReadInputParams { HostName = TestURI, QueueName = "queue", AutoAck = ReadAckType.AutoAck, ReadMessageCount = 10, ConnectWithURI = true });

            Assert.IsTrue(retVal != null && retVal.Messages.Count() == 10);
        }

        [TestMethod]
        [Ignore("RabbitMQ is not installed on build server.")]
        public void TestReadNoAck10WithURI()
        {
            DeleteExchangeAndQueue();
            CreateExchangeAndQueue();
            for (int i = 0; i < 10; i++)
            {
                Frends.Community.RabbitMQ.RabbitMQTask.WriteMessage(new WriteInputParams { Data = new byte[] { 0, (byte)(i * i), (byte)i }, HostName = TestURI, RoutingKey = "queue", QueueName = "queue", ConnectWithURI = false });
            }

          var retVal = Frends.Community.RabbitMQ.RabbitMQTask.ReadMessage(new ReadInputParams { HostName = TestURI, QueueName = "queue", AutoAck = ReadAckType.AutoNackAndRequeue, ReadMessageCount = 10, ConnectWithURI = true });

            Assert.IsTrue(retVal != null && retVal.Messages.Count() == 10);
        }


        [TestMethod]
        [Ignore("RabbitMQ is not installed on build server.")]
        public void TestWriteToNonExistingQueue()
        {
            DeleteExchangeAndQueue();
            Exception xx = null;
            try
            {
                Frends.Community.RabbitMQ.RabbitMQTask.WriteMessage(new WriteInputParams { Data = new byte[] { 0 }, HostName = TestURI, RoutingKey = "queue", QueueName = "queue", ConnectWithURI = false, Create = false });

                var retVal = Frends.Community.RabbitMQ.RabbitMQTask.ReadMessage(new ReadInputParams { HostName = TestURI, QueueName = "queue", AutoAck = ReadAckType.AutoAck, ReadMessageCount = 1000, ConnectWithURI = false });

            }
            catch (Exception x)
            {
                xx = x;
            }
            Assert.IsTrue(xx != null);
        }

        [TestMethod]
        [Ignore("RabbitMQ is not installed on build server.")]
        public void TestWriteToExistingQueue()
        {
            DeleteExchangeAndQueue();
            CreateExchangeAndQueue();
            Frends.Community.RabbitMQ.RabbitMQTask.WriteMessage(new WriteInputParams { Data = new byte[] { 0 }, HostName = TestURI, RoutingKey = "queue", QueueName = "queue", ConnectWithURI = false, Create = false, Durable = false });
            var retVal = Frends.Community.RabbitMQ.RabbitMQTask.ReadMessage(new ReadInputParams { HostName = TestURI, QueueName = "queue", AutoAck = ReadAckType.AutoAck, ReadMessageCount = 1000, ConnectWithURI = false });
            Assert.IsTrue(retVal != null && retVal.Messages.Count() == 1);
        }

        [TestMethod]
        [Ignore("RabbitMQ is not installed on build server.")]
        public void TestWriteToExistingExchange()
        {
            DeleteExchangeAndQueue();
            CreateExchangeAndQueue();
            Frends.Community.RabbitMQ.RabbitMQTask.WriteMessage(new WriteInputParams { Data = new byte[] { 0 }, HostName = TestURI, ExchangeName = "exchange", ConnectWithURI = false, Create = false, Durable = false });
            var retVal = Frends.Community.RabbitMQ.RabbitMQTask.ReadMessage(new ReadInputParams { HostName = TestURI, QueueName = "queue", AutoAck = ReadAckType.AutoAck, ReadMessageCount = 1000, ConnectWithURI = false });
            Assert.IsTrue(retVal != null && retVal.Messages.Count() == 1);
        }

        [TestMethod]
        [Ignore("RabbitMQ is not installed on build server.")]
        public void TestWriteReadStringToQueue()
        {
            DeleteExchangeAndQueue();
            CreateExchangeAndQueue();
            Frends.Community.RabbitMQ.RabbitMQTask.WriteMessageString(new WriteInputParamsString { Data = "test message", HostName = TestURI, RoutingKey = "queue", QueueName = "queue", ConnectWithURI = false, Create = false, Durable = false });
            var retVal = Frends.Community.RabbitMQ.RabbitMQTask.ReadMessageString(new ReadInputParams { HostName = TestURI, QueueName = "queue", AutoAck = ReadAckType.AutoAck, ReadMessageCount = 1000, ConnectWithURI = false });
            Assert.IsTrue(retVal != null && retVal.Messages.Count() == 1 && retVal.Messages[0].Data == "test message");
        }

        [TestMethod]
        [Ignore("RabbitMQ is not installed on build server.")]
        public void TestWriteReadStringToExchange()
        {
            DeleteExchangeAndQueue();
            CreateExchangeAndQueue();
            Frends.Community.RabbitMQ.RabbitMQTask.WriteMessageString(new WriteInputParamsString { Data = "test message", HostName = TestURI, ExchangeName = "exchange", RoutingKey = "queue", ConnectWithURI = false, Create = false, Durable = false });

            var retVal = Frends.Community.RabbitMQ.RabbitMQTask.ReadMessageString(new ReadInputParams { HostName = TestURI, QueueName = "queue", AutoAck = ReadAckType.AutoAck, ReadMessageCount = 1000, ConnectWithURI = false });
            Assert.IsTrue(retVal != null && retVal.Messages.Count() == 1 && retVal.Messages[0].Data == "test message");
        }


        /// <summary>
        /// Used for debugging, if connection is closed and opened for new hostname
        /// </summary>
        [TestMethod]
        [Ignore("RabbitMQ is not installed on build server.")]
        public void TestChangingHostName()
        {
            DeleteExchangeAndQueue();
            CreateExchangeAndQueue();
            Frends.Community.RabbitMQ.RabbitMQTask.WriteMessageString(new WriteInputParamsString { Data = "test message", HostName = "amqp://localhost",  ExchangeName = "exchange", RoutingKey = "queue", ConnectWithURI = true, Create = false, Durable = false });
            Frends.Community.RabbitMQ.RabbitMQTask.WriteMessageString(new WriteInputParamsString { Data = "test message", HostName = "localhost2", ExchangeName = "exchange", RoutingKey = "queue", ConnectWithURI = false, Create = false, Durable = false });

            Assert.IsTrue(true);
        }


    }
}
