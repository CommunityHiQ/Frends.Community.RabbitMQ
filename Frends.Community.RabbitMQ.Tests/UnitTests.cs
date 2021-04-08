using NUnit.Framework;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
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
        public static string TestUri = Environment.GetEnvironmentVariable("HIQ_RABBITMQ_CONNECTIONSTRING");
        public static string TestHost = "localhost";

        private WriteInputParams _inputParameters = new WriteInputParams();
        private WriteInputParamsString _inputParametersString = new WriteInputParamsString();

        private ReadInputParams _outputReadParams;


        /// <summary>
        /// Deletes test exchange and queue if it exists
        /// </summary>
        [TearDown]
        public void DeleteExchangeAndQueue()
        {
            var factory = new ConnectionFactory
            {
                Uri = new Uri(TestUri)
            };

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
            var factory = new ConnectionFactory
            {
                Uri = new Uri(TestUri)
            };

            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare("exchange", type: "fanout", durable: false, autoDelete: false);
                    channel.QueueDeclare("queue", durable: false, exclusive: false, autoDelete: false);
                    channel.QueueBind("queue", "exchange", routingKey: "");
                }
            }

            _inputParameters = new WriteInputParams
            {
                Data = new byte[] { 0, 1, 2 },
                HostName = TestHost,
                RoutingKey = "queue",
                QueueName = "queue",
                ConnectWithURI = false,
                Create = false,
                Durable = false,
                Headers = null
            };



            _inputParametersString = new WriteInputParamsString
            {
                Data = "test message",
                HostName = TestHost,
                RoutingKey = "queue",
                QueueName = "queue",
                ConnectWithURI = false,
                Create = false,
                Durable = false,
                Headers = new Header[]
                {
                    new Header { Name = "X-AppId", Value = "application id" },
                    new Header { Name = "X-ClusterId", Value = "cluster id" },
                    new Header { Name = "Content-Type", Value = "content type" },
                    new Header { Name = "Content-Encoding", Value = "content encoding" },
                    new Header { Name = "X-CorrelationId", Value = "correlation id" },
                    new Header { Name = "X-Expiration", Value = "100" },
                    new Header { Name = "X-MessageId", Value = "message id" },
                    new Header { Name = "Custom-Header", Value = "custom header" }
                }
            };

            _outputReadParams = new ReadInputParams
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
            RabbitMQTask.WriteMessage(_inputParameters);
            var retVal = RabbitMQTask.ReadMessage(_outputReadParams);
            Assert.IsTrue(retVal != null && retVal.Messages.Count() == 1);
        }

        [Test]
        public void TestReadWithAck10()
        {
            _outputReadParams.ReadMessageCount = 1000;

            for (int i = 0; i < 10; i++)
            {
                _inputParameters.Data = new byte[] { 0, (byte)(i * i) };
                RabbitMQTask.WriteMessage(_inputParameters);
            }
            var retVal = RabbitMQTask.ReadMessage(_outputReadParams);
            Assert.IsTrue(retVal != null && retVal.Messages.Count() == 10);
        }

        [Test]
        public void TestReadNoAck10()
        {
            _inputParameters.ConnectWithURI = true;
            _inputParameters.HostName = TestUri;

            _outputReadParams.ReadMessageCount = 10;
            _outputReadParams.AutoAck = ReadAckType.AutoNackAndRequeue;


            for (int i = 0; i < 10; i++)
            {
                _inputParameters.Data = new byte[] { 0, (byte)(i * i), (byte)i };

                RabbitMQTask.WriteMessage(_inputParameters);
            }
            var retVal = RabbitMQTask.ReadMessage(_outputReadParams);

            Assert.IsTrue(retVal != null && retVal.Messages.Count() == 10);
        }

        [Test]
        public void TestWriteReadWithUri()
        {
            _inputParameters.HostName = TestUri;
            _inputParameters.ConnectWithURI = true;

            _outputReadParams.ConnectWithURI = true;
            _outputReadParams.HostName = TestUri;


            RabbitMQTask.WriteMessage(_inputParameters);
            var retVal = RabbitMQTask.ReadMessage(_outputReadParams);

            Assert.IsTrue(retVal != null && retVal.Messages.Count() == 1);
        }

        [Test]
        public void TestReadWithAck10WithUri()
        {
            _inputParameters.HostName = TestUri;
            _inputParameters.ConnectWithURI = true;


            _outputReadParams.ConnectWithURI = true;
            _outputReadParams.HostName = TestUri;
            _outputReadParams.ReadMessageCount = 10;


            for (int i = 0; i < 10; i++)
            {
                _inputParameters.Data = new byte[] { 0, (byte)(i * i), (byte)i };

                RabbitMQTask.WriteMessage(_inputParameters);
            }

            var retVal = RabbitMQTask.ReadMessage(_outputReadParams);

            Assert.IsTrue(retVal != null && retVal.Messages.Count() == 10);
        }

        [Test]
        public void TestReadNoAck10WithUri()
        {
            _inputParameters.HostName = TestUri;
            _inputParameters.ConnectWithURI = true;


            _outputReadParams.ConnectWithURI = true;
            _outputReadParams.AutoAck = ReadAckType.AutoNackAndRequeue;
            _outputReadParams.HostName = TestUri;
            _outputReadParams.ReadMessageCount = 10;

            for (int i = 0; i < 10; i++)
            {
                _inputParameters.Data = new byte[] { 0, (byte)(i * i), (byte)i };

                RabbitMQTask.WriteMessage(_inputParameters);
            }

            var retVal = RabbitMQTask.ReadMessage(_outputReadParams);

            Assert.IsTrue(retVal != null && retVal.Messages.Count() == 10);
        }


        [Test]
        public void TestWriteToNonExistingQueue()
        {
            Exception xx = null;

            _inputParameters.QueueName = "queue2"; // Queue won't exist, but don't create it

            _outputReadParams.QueueName = "queue2";

            try
            {
                RabbitMQTask.WriteMessage(_inputParameters);

                var _ = RabbitMQTask.ReadMessage(_outputReadParams);

            }
            catch (Exception x)
            {
                xx = x;
            }
            Assert.IsTrue(xx != null);
        }

        [Test]
        public void TestWriteToExistingExchange()
        {

            _inputParameters.QueueName = null;
            _inputParameters.ExchangeName = "exchange";

            RabbitMQTask.WriteMessage(_inputParameters);
            var retVal = RabbitMQTask.ReadMessage(_outputReadParams);
            Assert.IsTrue(retVal != null && retVal.Messages.Count() == 1 && retVal.Messages[0].Headers.Count == 0);
        }

        [Test]
        public void TestWriteReadStringToQueue()
        {

            RabbitMQTask.WriteMessageString(_inputParametersString);
            var retVal = RabbitMQTask.ReadMessageString(_outputReadParams);

            Assert.IsTrue(retVal != null && retVal.Messages.Count() == 1);
            Assert.AreEqual("test message", retVal.Messages[0].Data);
            Assert.AreEqual("application id", retVal.Messages[0].Headers["X-AppId"]);
            Assert.AreEqual("cluster id", retVal.Messages[0].Headers["X-ClusterId"]);
            Assert.AreEqual("content type", retVal.Messages[0].Headers["Content-Type"]);
            Assert.AreEqual("content encoding", retVal.Messages[0].Headers["Content-Encoding"]);
            Assert.AreEqual("correlation id", retVal.Messages[0].Headers["X-CorrelationId"]);
            Assert.AreEqual("100", retVal.Messages[0].Headers["X-Expiration"]);
            Assert.AreEqual("message id", retVal.Messages[0].Headers["X-MessageId"]);
            Assert.AreEqual("custom header", retVal.Messages[0].Headers["Custom-Header"]);
        }

        [Test]
        public void TestWriteReadStringToExchange()
        {

            _inputParametersString.QueueName = null;
            _inputParametersString.ExchangeName = "exchange";

            RabbitMQTask.WriteMessageString(_inputParametersString);

            var retVal = RabbitMQTask.ReadMessageString(new ReadInputParams { HostName = TestUri, QueueName = "queue", AutoAck = ReadAckType.AutoAck, ReadMessageCount = 1000, ConnectWithURI = true });
            Assert.IsTrue(retVal != null && retVal.Messages.Count() == 1);
            Assert.AreEqual("test message", retVal.Messages[0].Data);
            Assert.AreEqual("application id", retVal.Messages[0].Headers["X-AppId"]);
            Assert.AreEqual("cluster id", retVal.Messages[0].Headers["X-ClusterId"]);
            Assert.AreEqual("content type", retVal.Messages[0].Headers["Content-Type"]);
            Assert.AreEqual("content encoding", retVal.Messages[0].Headers["Content-Encoding"]);
            Assert.AreEqual("correlation id", retVal.Messages[0].Headers["X-CorrelationId"]);
            Assert.AreEqual("100", retVal.Messages[0].Headers["X-Expiration"]);
            Assert.AreEqual("message id", retVal.Messages[0].Headers["X-MessageId"]);
            Assert.AreEqual("custom header", retVal.Messages[0].Headers["Custom-Header"]);
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
