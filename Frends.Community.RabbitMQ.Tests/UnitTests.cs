using System;
using System.Linq;
using RabbitMQ.Client;
using NUnit.Framework;

namespace Frends.Community.RabbitMQ.Tests
{

    /// <summary>
    /// You will need access to RabbitMQ queue, you can create it e.g. by running
    ///
    /// docker run -d --hostname my-rabbit -p 9080:15672 -p 5772:5672 -e RABBITMQ_DEFAULT_USER=agent -e RABBITMQ_DEFAULT_PASS=agent123  rabbitmq:3.7-management
    ///
    /// In that case URI would be amqp://agent:agent123@localhost:5772
    /// 
    /// </summary>

    [TestFixture]
    // [Ignore("RabbitMQ is not installed on build server.")]
    public class UnitTests
    {

        //public const string TestURI = "amqp://user:password@hostname:port/vhost";
        public string TestURI = Environment.GetEnvironmentVariable("HIQ_RABBITMQ_CONNECTIONSTRING");

        /// <summary>
        /// Deletes test exchange and queue if it exists
        /// </summary>
        [TearDown]
        public void DeleteExchangeAndQueue()
        {
            var factory = new ConnectionFactory();
            factory.Uri = new Uri(TestURI);

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
        [SetUp]
        public void CreateExchangeAndQueue()
        {
            var factory = new ConnectionFactory();
            //factory.HostName = "localhost";
            factory.Uri = new Uri(TestURI);

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

        [Test]
        public void TestWriteRead()
        {
            // DeleteExchangeAndQueue();
            // CreateExchangeAndQueue();
            Frends.Community.RabbitMQ.RabbitMQTask.WriteMessage(new WriteInputParams { Data = new byte[] { 0, 1, 2 }, HostName = TestURI, RoutingKey = "queue", QueueName = "queue", ProcessExecutionId = Guid.NewGuid().ToString(), ConnectWithURI = true});
            var retVal = Frends.Community.RabbitMQ.RabbitMQTask.ReadMessage(new ReadInputParams { HostName = TestURI, QueueName = "queue", AutoAck = ReadAckType.AutoAck, ReadMessageCount = 1, ConnectWithURI = true });
            Assert.IsTrue(retVal != null && retVal.Messages.Count() == 1);
        }

        [Test]
        public void TestReadWithAck10()
        {
            DeleteExchangeAndQueue();
            CreateExchangeAndQueue();
            for (int i = 0; i < 10; i++)
                Frends.Community.RabbitMQ.RabbitMQTask.WriteMessage(new WriteInputParams { Data = new byte[] { 0, (byte)(i * i), (byte)i }, HostName = TestURI, RoutingKey = "queue", QueueName = "queue", ProcessExecutionId = Guid.NewGuid().ToString(), ConnectWithURI = true });
            var retVal = Frends.Community.RabbitMQ.RabbitMQTask.ReadMessage(new ReadInputParams { HostName = TestURI, QueueName = "queue", AutoAck = ReadAckType.AutoAck, ReadMessageCount = 1000, ConnectWithURI = true });
            Assert.IsTrue(retVal != null && retVal.Messages.Count() == 10);
        }

        [Test]
        public void TestReadNoAck10()
        {
            DeleteExchangeAndQueue();
            CreateExchangeAndQueue();
            for (int i = 0; i < 10; i++)
                Frends.Community.RabbitMQ.RabbitMQTask.WriteMessage(new WriteInputParams { Data = new byte[] { 0, (byte)(i * i), (byte)i }, HostName = TestURI, RoutingKey = "queue", QueueName = "queue", ProcessExecutionId = Guid.NewGuid().ToString(), ConnectWithURI = true });

          var retVal = Frends.Community.RabbitMQ.RabbitMQTask.ReadMessage(new ReadInputParams { HostName = TestURI, QueueName = "queue", AutoAck = ReadAckType.AutoNackAndRequeue, ReadMessageCount = 10, ConnectWithURI = true });

            Assert.IsTrue(retVal != null && retVal.Messages.Count() == 10);
        }

        [Test]
        public void TestWriteReadWithURI()
        {
            DeleteExchangeAndQueue();
            CreateExchangeAndQueue();

            Frends.Community.RabbitMQ.RabbitMQTask.WriteMessage(new WriteInputParams { Data = new byte[] { 0, 1, 2 }, HostName = TestURI, RoutingKey = "queue", QueueName = "queue", ConnectWithURI = true, ProcessExecutionId = Guid.NewGuid().ToString() });
            var retVal = Frends.Community.RabbitMQ.RabbitMQTask.ReadMessage(new ReadInputParams { HostName = TestURI, QueueName = "queue", AutoAck = ReadAckType.AutoAck, ReadMessageCount = 1, ConnectWithURI = true });

            Assert.IsTrue(retVal != null && retVal.Messages.Count() == 1);
        }

        [Test]
        public void TestReadWithAck10WithURI()
        {
            DeleteExchangeAndQueue();
            CreateExchangeAndQueue();
            for (int i = 0; i < 10; i++)
            {
                Frends.Community.RabbitMQ.RabbitMQTask.WriteMessage(new WriteInputParams { Data = new byte[] { 0, (byte)(i * i), (byte)i }, HostName = TestURI, RoutingKey = "queue", QueueName = "queue", ConnectWithURI = true, ProcessExecutionId = Guid.NewGuid().ToString() });
            }

            var retVal = Frends.Community.RabbitMQ.RabbitMQTask.ReadMessage(new ReadInputParams { HostName = TestURI, QueueName = "queue", AutoAck = ReadAckType.AutoAck, ReadMessageCount = 10, ConnectWithURI = true });

            Assert.IsTrue(retVal != null && retVal.Messages.Count() == 10);
        }

        [Test]
        public void TestReadNoAck10WithURI()
        {
            DeleteExchangeAndQueue();
            CreateExchangeAndQueue();
            for (int i = 0; i < 10; i++)
            {
                Frends.Community.RabbitMQ.RabbitMQTask.WriteMessage(new WriteInputParams { Data = new byte[] { 0, (byte)(i * i), (byte)i }, HostName = TestURI, RoutingKey = "queue", QueueName = "queue", ConnectWithURI = true, ProcessExecutionId = Guid.NewGuid().ToString() });
            }
            
            var retVal = Frends.Community.RabbitMQ.RabbitMQTask.ReadMessage(new ReadInputParams { HostName = TestURI, QueueName = "queue", AutoAck = ReadAckType.AutoNackAndRequeue, ReadMessageCount = 10, ConnectWithURI = true });

            Assert.IsTrue(retVal != null && retVal.Messages.Count() == 10);
        }


        [Test]
        public void TestWriteToNonExistingQueue()
        {
            DeleteExchangeAndQueue();
            Exception xx = null;
            try
            {
                Frends.Community.RabbitMQ.RabbitMQTask.WriteMessage(new WriteInputParams { Data = new byte[] { 0 }, HostName = TestURI, RoutingKey = "queue", QueueName = "queue", ConnectWithURI = true, Create = false, ProcessExecutionId = Guid.NewGuid().ToString() });

                var retVal = Frends.Community.RabbitMQ.RabbitMQTask.ReadMessage(new ReadInputParams { HostName = TestURI, QueueName = "queue", AutoAck = ReadAckType.AutoAck, ReadMessageCount = 1000, ConnectWithURI = true });

            }
            catch (Exception x)
            {
                xx = x;
            }
            Assert.IsTrue(xx != null);
        }

        [Test]
        public void TestWriteToExistingQueue()
        {
            DeleteExchangeAndQueue();
            CreateExchangeAndQueue();
            Frends.Community.RabbitMQ.RabbitMQTask.WriteMessage(new WriteInputParams { Data = new byte[] { 0 }, HostName = TestURI, RoutingKey = "queue", QueueName = "queue", ConnectWithURI = true, Create = false, Durable = false, ProcessExecutionId = Guid.NewGuid().ToString() });
            var retVal = Frends.Community.RabbitMQ.RabbitMQTask.ReadMessage(new ReadInputParams { HostName = TestURI, QueueName = "queue", AutoAck = ReadAckType.AutoAck, ReadMessageCount = 1000, ConnectWithURI = true });
            Assert.IsTrue(retVal != null && retVal.Messages.Count() == 1);
        }

        [Test]
        public void TestWriteToExistingExchange()
        {
            DeleteExchangeAndQueue();
            CreateExchangeAndQueue();
            Frends.Community.RabbitMQ.RabbitMQTask.WriteMessage(new WriteInputParams { Data = new byte[] { 0 }, HostName = TestURI, ExchangeName = "exchange", ConnectWithURI = true, Create = false, Durable = false, ProcessExecutionId = Guid.NewGuid().ToString() });
            var retVal = Frends.Community.RabbitMQ.RabbitMQTask.ReadMessage(new ReadInputParams { HostName = TestURI, QueueName = "queue", AutoAck = ReadAckType.AutoAck, ReadMessageCount = 1000, ConnectWithURI = true });
            Assert.IsTrue(retVal != null && retVal.Messages.Count() == 1);
        }

        [Test]
        public void TestWriteReadStringToQueue()
        {
            DeleteExchangeAndQueue();
            CreateExchangeAndQueue();
            Frends.Community.RabbitMQ.RabbitMQTask.WriteMessageString(new WriteInputParamsString { Data = "test message", HostName = TestURI, RoutingKey = "queue", QueueName = "queue", ConnectWithURI = true, Create = false, Durable = false });
            var retVal = Frends.Community.RabbitMQ.RabbitMQTask.ReadMessageString(new ReadInputParams { HostName = TestURI, QueueName = "queue", AutoAck = ReadAckType.AutoAck, ReadMessageCount = 1000, ConnectWithURI = true });
            Assert.IsTrue(retVal != null && retVal.Messages.Count() == 1 && retVal.Messages[0].Data == "test message");
        }

        [Test]
        public void TestWriteReadStringToExchange()
        {
            DeleteExchangeAndQueue();
            CreateExchangeAndQueue();
            Frends.Community.RabbitMQ.RabbitMQTask.WriteMessageString(new WriteInputParamsString { Data = "test message", HostName = TestURI, ExchangeName = "exchange", RoutingKey = "queue", ConnectWithURI = true, Create = false, Durable = false });

            var retVal = Frends.Community.RabbitMQ.RabbitMQTask.ReadMessageString(new ReadInputParams { HostName = TestURI, QueueName = "queue", AutoAck = ReadAckType.AutoAck, ReadMessageCount = 1000, ConnectWithURI = true });
            Assert.IsTrue(retVal != null && retVal.Messages.Count() == 1 && retVal.Messages[0].Data == "test message");
        }


        /// <summary>
        /// Used for debugging, if connection is closed and opened for new hostname
        /// </summary>
        [Test]
        [Ignore("This test is actually used for debugging while developing task.")]
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
