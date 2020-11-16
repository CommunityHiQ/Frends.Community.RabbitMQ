using NUnit.Framework;
using RabbitMQ.Client;
using System;
using System.Linq;

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
        public static string TestURI = Environment.GetEnvironmentVariable("HIQ_RABBITMQ_CONNECTIONSTRING");
        public static string TestHost = "localhost";

        private WriteInputParams InputParameters = new WriteInputParams();
        private WriteInputParamsString inputParametersString = new WriteInputParamsString();
        private ReadInputParams OutputReadParams;


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

            InputParameters = new WriteInputParams
            {
                Data = new byte[] { 0, 1, 2 },
                HostName = TestHost,
                RoutingKey = "queue",
                QueueName = "queue",
                ProcessExecutionId = Guid.NewGuid().ToString(),
                ConnectWithURI = false,
                Create = false,
                Durable = false
            };



            inputParametersString = new WriteInputParamsString
            {
                Data = "test message",
                HostName = TestHost,
                RoutingKey = "queue",
                QueueName = "queue",
                ProcessExecutionId = Guid.NewGuid().ToString(),
                ConnectWithURI = false,
                Create = false,
                Durable = false
            };

            OutputReadParams = new ReadInputParams
            {
                HostName = TestHost,
                QueueName = "queue",
                AutoAck = ReadAckType.AutoAck,
                ReadMessageCount = 1,
                ConnectWithURI = false
            };
        }

        [Test]
        public void TestWriteRead()
        {
            RabbitMQTask.WriteMessage(InputParameters);
            var retVal = RabbitMQTask.ReadMessage(OutputReadParams);
            Assert.IsTrue(retVal != null && retVal.Messages.Count() == 1);
        }

        [Test]
        public void TestReadWithAck10()
        {
            OutputReadParams.ReadMessageCount = 1000;

            for (int i = 0; i < 10; i++)
            {
                InputParameters.Data = new byte[] { 0, (byte)(i * i) };
                RabbitMQTask.WriteMessage(InputParameters);
            }
            var retVal = RabbitMQTask.ReadMessage(OutputReadParams);
            Assert.IsTrue(retVal != null && retVal.Messages.Count() == 10);
        }

        [Test]
        public void TestReadNoAck10()
        {
            InputParameters.ConnectWithURI = true;
            InputParameters.HostName = TestURI;

            OutputReadParams.ReadMessageCount = 10;
            OutputReadParams.AutoAck = ReadAckType.AutoNackAndRequeue;


            for (int i = 0; i < 10; i++)
            {
                InputParameters.Data = new byte[] { 0, (byte)(i * i), (byte)i };

                RabbitMQTask.WriteMessage(InputParameters);
            }
            var retVal = RabbitMQTask.ReadMessage(OutputReadParams);

            Assert.IsTrue(retVal != null && retVal.Messages.Count() == 10);
        }

        [Test]
        public void TestWriteReadWithURI()
        {
            InputParameters.HostName = TestURI;
            InputParameters.ConnectWithURI = true;

            OutputReadParams.ConnectWithURI = true;
            OutputReadParams.HostName = TestURI;


            RabbitMQTask.WriteMessage(InputParameters);
            var retVal = RabbitMQTask.ReadMessage(OutputReadParams);

            Assert.IsTrue(retVal != null && retVal.Messages.Count() == 1);
        }

        [Test]
        public void TestReadWithAck10WithURI()
        {
            InputParameters.HostName = TestURI;
            InputParameters.ConnectWithURI = true;


            OutputReadParams.ConnectWithURI = true;
            OutputReadParams.HostName = TestURI;
            OutputReadParams.ReadMessageCount = 10;


            for (int i = 0; i < 10; i++)
            {
                InputParameters.Data = new byte[] { 0, (byte)(i * i), (byte)i };

                RabbitMQTask.WriteMessage(InputParameters);
            }

            var retVal = RabbitMQTask.ReadMessage(OutputReadParams);

            Assert.IsTrue(retVal != null && retVal.Messages.Count() == 10);
        }

        [Test]
        public void TestReadNoAck10WithURI()
        {
            InputParameters.HostName = TestURI;
            InputParameters.ConnectWithURI = true;


            OutputReadParams.ConnectWithURI = true;
            OutputReadParams.AutoAck = ReadAckType.AutoNackAndRequeue;
            OutputReadParams.HostName = TestURI;
            OutputReadParams.ReadMessageCount = 10;

            for (int i = 0; i < 10; i++)
            {
                InputParameters.Data = new byte[] { 0, (byte)(i * i), (byte)i };

                RabbitMQTask.WriteMessage(InputParameters);
            }

            var retVal = RabbitMQTask.ReadMessage(OutputReadParams);

            Assert.IsTrue(retVal != null && retVal.Messages.Count() == 10);
        }


        [Test]
        public void TestWriteToNonExistingQueue()
        {
            Exception xx = null;

            InputParameters.QueueName = "queue2"; // Queue won't exist, but don't create it

            OutputReadParams.QueueName = "queue2";

            try
            {
                RabbitMQTask.WriteMessage(InputParameters);

                var _ = RabbitMQTask.ReadMessage(OutputReadParams);

            }
            catch (Exception x)
            {
                xx = x;
            }
            Assert.IsTrue(xx != null);
        }

        [Test]
        public void TestWriteToNewQueue() // pois TODO
        {

            InputParameters.QueueName = "queue2";
            InputParameters.Create = true;

            OutputReadParams.QueueName = "queue2";

            RabbitMQTask.WriteMessage(new WriteInputParams { Data = new byte[] { 0 }, HostName = TestURI, RoutingKey = "queue", QueueName = "queue", ConnectWithURI = true, Create = false, Durable = false, ProcessExecutionId = Guid.NewGuid().ToString() });
            var retVal = RabbitMQTask.ReadMessage(new ReadInputParams { HostName = TestURI, QueueName = "queue", AutoAck = ReadAckType.AutoAck, ReadMessageCount = 1000, ConnectWithURI = true });
            Assert.IsTrue(retVal != null && retVal.Messages.Count() == 1);
        }

        [Test]
        public void TestWriteToExistingExchange()
        {

            InputParameters.QueueName = null;
            InputParameters.ExchangeName = "exchange";

            RabbitMQTask.WriteMessage(InputParameters);
            var retVal = RabbitMQTask.ReadMessage(OutputReadParams);
            Assert.IsTrue(retVal != null && retVal.Messages.Count() == 1);
        }

        [Test]
        public void TestWriteReadStringToQueue()
        {

            RabbitMQTask.WriteMessageString(inputParametersString);
            var retVal = RabbitMQTask.ReadMessageString(OutputReadParams);

            Assert.IsTrue(retVal != null && retVal.Messages.Count() == 1 && retVal.Messages[0].Data == "test message");
        }

        [Test]
        public void TestWriteReadStringToExchange()
        {

            inputParametersString.QueueName = null;
            inputParametersString.ExchangeName = "exchange";

            RabbitMQTask.WriteMessageString(inputParametersString);

            var retVal = RabbitMQTask.ReadMessageString(new ReadInputParams { HostName = TestURI, QueueName = "queue", AutoAck = ReadAckType.AutoAck, ReadMessageCount = 1000, ConnectWithURI = true });
            Assert.IsTrue(retVal != null && retVal.Messages.Count() == 1 && retVal.Messages[0].Data == "test message");
        }


        /// <summary>
        /// Used for debugging, if connection is closed and opened for new hostname
        /// </summary>
        [Test]
        [Ignore("This test is actually used for debugging while developing task.")]
        public void TestChangingHostName()
        {
            RabbitMQTask.WriteMessageString(new WriteInputParamsString { Data = "test message", HostName = "amqp://localhost", ExchangeName = "exchange", RoutingKey = "queue", ConnectWithURI = true, Create = false, Durable = false });
            RabbitMQTask.WriteMessageString(new WriteInputParamsString { Data = "test message", HostName = "localhost2", ExchangeName = "exchange", RoutingKey = "queue", ConnectWithURI = false, Create = false, Durable = false });

            Assert.IsTrue(true);
        }
    }
}
