﻿using System;
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
            var retVal = Frends.Community.RabbitMQ.RabbitMQTask.ReadMessage(new ReadInputParams { HostName = "localhost", QueueName = "queue", AutoAck = true, ReadMessageCount = 1000 });
            //var retVal2 = Frends.Community.RabbitMQ.RabbitMQTask.ReadMessage(new ReadInputParams { HostName = TestURI, QueueName = "queue", AutoAck = true, ReadMessageCount = 1000, ConnectWithURI = true });
        }

        /// <summary>
        /// Deletes test queue if it exists
        /// </summary>
        private void DeleteQueue()
        {
            var factory = new ConnectionFactory();
            factory.HostName = "localhost";

            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDelete("queue", true, true);
                }
            }
        }

        /// <summary>
        /// Creates test queue
        /// </summary>
        private void CreateQueue()
        {
            var factory = new ConnectionFactory();
            factory.HostName = "localhost";

            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare("queue", false, false, false, null);
                }
            }
        }

        [TestMethod]
        [Ignore("RabbitMQ is not installed on build server.")]
        public void TestWriteRead()
        {
            Frends.Community.RabbitMQ.RabbitMQTask.WriteMessage(new WriteInputParams { Data = new byte[] { 0, 1, 2 }, HostName = "localhost", RoutingKey = "queue", QueueName = "queue" });
            var retVal = Frends.Community.RabbitMQ.RabbitMQTask.ReadMessage(new ReadInputParams { HostName = "localhost", QueueName = "queue", AutoAck = true, ReadMessageCount = 1 });
            Assert.IsTrue(retVal != null && retVal.Messages.Count() == 1);
        }

        [TestMethod]
        [Ignore("RabbitMQ is not installed on build server.")]
        public void TestReadWithAck10()
        {
            for (int i = 0; i < 10; i++)
                Frends.Community.RabbitMQ.RabbitMQTask.WriteMessage(new WriteInputParams { Data = new byte[] { 0, (byte)(i * i), (byte)i }, HostName = "localhost", RoutingKey = "queue", QueueName = "queue" });
            var retVal = Frends.Community.RabbitMQ.RabbitMQTask.ReadMessage(new ReadInputParams { HostName = "localhost", QueueName = "queue", AutoAck = true, ReadMessageCount = 1000 });
            Assert.IsTrue(retVal != null && retVal.Messages.Count() == 10);
        }

        [TestMethod]
        [Ignore("RabbitMQ is not installed on build server.")]
        public void TestReadNoAck10()
        {
            for (int i = 0; i < 10; i++)
                Frends.Community.RabbitMQ.RabbitMQTask.WriteMessage(new WriteInputParams { Data = new byte[] { 0, (byte)(i * i), (byte)i }, HostName = "localhost", RoutingKey = "queue", QueueName = "queue" });
            var retVal = Frends.Community.RabbitMQ.RabbitMQTask.ReadMessage(new ReadInputParams { HostName = "localhost", QueueName = "queue", AutoAck = false, ReadMessageCount = 1000 });
            Assert.IsTrue(retVal != null && retVal.Messages.Count() == 1000);
        }
        [TestMethod]
        [Ignore("RabbitMQ is not installed on build server.")]
        public void TestWriteReadWithURI()
        {
            Frends.Community.RabbitMQ.RabbitMQTask.WriteMessage(new WriteInputParams { Data = new byte[] { 0, 1, 2 }, HostName = TestURI, RoutingKey = "queue", QueueName = "queue", ConnectWithURI = true });
            var retVal = Frends.Community.RabbitMQ.RabbitMQTask.ReadMessage(new ReadInputParams { HostName = TestURI, QueueName = "queue", AutoAck = true, ReadMessageCount = 1, ConnectWithURI = true });
            Assert.IsTrue(retVal != null && retVal.Messages.Count() == 1);
        }

        [TestMethod]
        [Ignore("RabbitMQ is not installed on build server.")]
        public void TestReadWithAck10WithURI()
        {
            for (int i = 0; i < 10; i++)
            {
                Frends.Community.RabbitMQ.RabbitMQTask.WriteMessage(new WriteInputParams { Data = new byte[] { 0, (byte)(i * i), (byte)i }, HostName = TestURI, RoutingKey = "queue", QueueName = "queue", ConnectWithURI = true });
            }

            var retVal = Frends.Community.RabbitMQ.RabbitMQTask.ReadMessage(new ReadInputParams { HostName = TestURI, QueueName = "queue", AutoAck = true, ReadMessageCount = 1000, ConnectWithURI = true });
            Assert.IsTrue(retVal != null && retVal.Messages.Count() == 10);
        }

        [TestMethod]
        [Ignore("RabbitMQ is not installed on build server.")]
        public void TestReadNoAck10WithURI()
        {
            for (int i = 0; i < 10; i++)
            {
                Frends.Community.RabbitMQ.RabbitMQTask.WriteMessage(new WriteInputParams { Data = new byte[] { 0, (byte)(i * i), (byte)i }, HostName = TestURI, RoutingKey = "queue", QueueName = "queue", ConnectWithURI = true });
            }
            var retVal = Frends.Community.RabbitMQ.RabbitMQTask.ReadMessage(new ReadInputParams { HostName = TestURI, QueueName = "queue", AutoAck = false, ReadMessageCount = 1000, ConnectWithURI = true });
            Assert.IsTrue(retVal != null && retVal.Messages.Count() == 1000);
        }


        [TestMethod]
        [Ignore("RabbitMQ is not installed on build server.")]
        public void TestWriteToNonExistingQueue()
        {
            DeleteQueue();

            Frends.Community.RabbitMQ.RabbitMQTask.WriteMessage(new WriteInputParams { Data = new byte[] { 0 }, HostName = TestURI, RoutingKey = "queue", QueueName = "queue", ConnectWithURI = false, Create = false });
            var retVal = Frends.Community.RabbitMQ.RabbitMQTask.ReadMessage(new ReadInputParams { HostName = TestURI, QueueName = "queue", AutoAck = true, ReadMessageCount = 1000, ConnectWithURI = false });
            Assert.IsTrue(retVal != null && retVal.Messages.Count() == 0);
        }

        [TestMethod]
        [Ignore("RabbitMQ is not installed on build server.")]
        public void TestWriteToExistingQueue()
        {
            DeleteQueue();
            CreateQueue();
            Frends.Community.RabbitMQ.RabbitMQTask.WriteMessage(new WriteInputParams { Data = new byte[] { 0 }, HostName = TestURI, RoutingKey = "queue", QueueName = "queue", ConnectWithURI = false, Create = false, Durable = false });
            var retVal = Frends.Community.RabbitMQ.RabbitMQTask.ReadMessage(new ReadInputParams { HostName = TestURI, QueueName = "queue", AutoAck = true, ReadMessageCount = 1000, ConnectWithURI = false });
            Assert.IsTrue(retVal != null && retVal.Messages.Count() == 1);
        }

        [TestMethod]
        [Ignore("RabbitMQ is not installed on build server.")]
        public void TestWriteReadStringToQueue()
        {
            DeleteQueue();
            CreateQueue();
            Frends.Community.RabbitMQ.RabbitMQTask.WriteMessageString(new WriteInputParamsString { Data = "test message", HostName = TestURI, RoutingKey = "queue", QueueName = "queue", ConnectWithURI = false, Create = false, Durable = false });
            var retVal = Frends.Community.RabbitMQ.RabbitMQTask.ReadMessageString(new ReadInputParams { HostName = TestURI, QueueName = "queue", AutoAck = true, ReadMessageCount = 1000, ConnectWithURI = false });
            Assert.IsTrue(retVal != null && retVal.Messages.Count() == 1 && retVal.Messages[0].Data == "test message");
        }
    }
}
