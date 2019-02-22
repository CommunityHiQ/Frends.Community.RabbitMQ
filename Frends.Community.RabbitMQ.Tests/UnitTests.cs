using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Frends.Community.RabbitMQ.Tests
{
    [TestClass]
    public class UnitTests
    {
		public const string TestURI = "amqp://user:password@hostname:port/vhost";

		[TestInitialize]
        public void TestInit()
        {
            var retVal = Frends.Community.RabbitMQ.RabbitMQTask.ReadMessage(new ReadInputParams { HostName = "localhost", QueueName = "queue", AutoAck = true, ReadMessageCount = 1000 });
			var retVal2 = Frends.Community.RabbitMQ.RabbitMQTask.ReadMessage(new ReadInputParams { HostName = TestURI, QueueName = "queue", AutoAck = true, ReadMessageCount = 1000, ConnectWithURI = true });
		}

		[TestMethod]
        [Ignore("RabbitMQ is not installed on build server.")]
        public void TestWriteRead()
        {
            Frends.Community.RabbitMQ.RabbitMQTask.WriteMessage(new WriteInputParams { Data = new byte[] { 0, 1, 2 }, HostName = "localhost", RoutingKey = "queue", QueueName = "queue" });
            var retVal = Frends.Community.RabbitMQ.RabbitMQTask.ReadMessage(new ReadInputParams { HostName = "localhost", QueueName = "queue", AutoAck = true, ReadMessageCount = 1 });
            Assert.IsTrue(retVal!=null && retVal.Messages.Count()==1);
        }

        [TestMethod]
        [Ignore("RabbitMQ is not installed on build server.")]
        public void TestReadWithAck10()
        {
            for(int i  = 0; i < 10;i++)
                Frends.Community.RabbitMQ.RabbitMQTask.WriteMessage(new WriteInputParams { Data = new byte[] { 0, (byte)(i * i), (byte)i }, HostName = "localhost", RoutingKey = "queue", QueueName = "queue" });
            var retVal = Frends.Community.RabbitMQ.RabbitMQTask.ReadMessage(new ReadInputParams { HostName = "localhost", QueueName = "queue", AutoAck = true, ReadMessageCount = 1000 });
            Assert.IsTrue(retVal != null && retVal.Messages.Count() == 10);
        }

        [TestMethod]
        [Ignore("RabbitMQ is not installed on build server.")]
        public void TestReadNoAck10()
        {
            for (int i = 0; i < 10; i++)
                Frends.Community.RabbitMQ.RabbitMQTask.WriteMessage(new WriteInputParams { Data = new byte[] { 0, (byte)(i*i), (byte)i }, HostName = "localhost", RoutingKey = "queue", QueueName = "queue" });
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
	}
}
