using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Frends.Community.RabbitMQ.Tests
{
    [TestClass]
    public class UnitTests
    {
        [TestInitialize]
        public void TestInit()
        {
            var retVal = Frends.Community.RabbitMQ.RabbitMQTask.ReadMessage(new ReadInputParams { HostName = "localhost", QueueName = "queue", AutoAck = true, ReadMessageCount = 1000 });
        }

        [TestMethod]
        public void TestWriteRead()
        {
            Frends.Community.RabbitMQ.RabbitMQTask.WriteMessage(new WriteInputParams { Data = new byte[] { 0, 1, 2 }, HostName = "localhost", RoutingKey = "queue", QueueName = "queue" });
            var retVal = Frends.Community.RabbitMQ.RabbitMQTask.ReadMessage(new ReadInputParams { HostName = "localhost", QueueName = "queue", AutoAck = true, ReadMessageCount = 1 });
            Assert.IsTrue(retVal!=null && retVal.Messages.Count()==1);
        }

        [TestMethod]
        public void TestReadWithAck10()
        {
            for(int i  = 0; i < 10;i++)
                Frends.Community.RabbitMQ.RabbitMQTask.WriteMessage(new WriteInputParams { Data = new byte[] { 0, (byte)(i * i), (byte)i }, HostName = "localhost", RoutingKey = "queue", QueueName = "queue" });
            var retVal = Frends.Community.RabbitMQ.RabbitMQTask.ReadMessage(new ReadInputParams { HostName = "localhost", QueueName = "queue", AutoAck = true, ReadMessageCount = 1000 });
            Assert.IsTrue(retVal != null && retVal.Messages.Count() == 10);
        }

        [TestMethod]
        public void TestReadNoAck10()
        {
            for (int i = 0; i < 10; i++)
                Frends.Community.RabbitMQ.RabbitMQTask.WriteMessage(new WriteInputParams { Data = new byte[] { 0, (byte)(i*i), (byte)i }, HostName = "localhost", RoutingKey = "queue", QueueName = "queue" });
            var retVal = Frends.Community.RabbitMQ.RabbitMQTask.ReadMessage(new ReadInputParams { HostName = "localhost", QueueName = "queue", AutoAck = false, ReadMessageCount = 1000 });
            Assert.IsTrue(retVal != null && retVal.Messages.Count() == 1000);
        }
    }
}
