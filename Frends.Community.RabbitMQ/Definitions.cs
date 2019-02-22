using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frends.Community.RabbitMQ
{
    /// <summary>
    /// Collection of read message parameters
    /// </summary>
    public class ReadInputParams
    {
        /// <summary>
        /// Name of the queue
        /// </summary>
        [DefaultValue("sampleQueue")]
        [DisplayName(@"Queue name")]
        [DisplayFormat(DataFormatString = "Text")]
        public string QueueName { get; set; }
        /// <summary>
        /// RabbitMQ host name
        /// </summary>
        [DefaultValue("localhost")]
        [DisplayName(@"Host name")]
        [DisplayFormat(DataFormatString = "Text")]
        public string HostName { get; set; }
        /// <summary>
        /// Maximum number of messages to read
        /// </summary>
        [DefaultValue(1)]
        [DisplayName(@"Read message count")]
        public int ReadMessageCount { get; set; }
        /// <summary>
        /// Acknowledge read messages. False to just peek last message
        /// </summary>
        [DefaultValue(true)]
        [DisplayName(@"Auto ack")]
        [DisplayFormat(DataFormatString = "Text")]
        public bool AutoAck { get; set; }
		/// <summary>
		/// Use URI instead of a hostname
		/// </summary>
		[DefaultValue(false)]
		[DisplayName(@"Use URI for connection")]
		public bool ConnectWithURI { get; set; }
	}

    /// <summary>
    /// Collection of write message parameters
    /// </summary>
    public class WriteInputParams
    {
        /// <summary>
        /// Data payload
        /// </summary>
        [DisplayName(@"Data")]
        [DisplayFormat(DataFormatString = "Expression")]
        public byte[] Data { get; set; }
        /// <summary>
        /// Name of the queue
        /// </summary>
        [DefaultValue("sampleQueue")]
        [DisplayName(@"Queue name")]
        [DisplayFormat(DataFormatString = "Text")]
        public string QueueName { get; set; }
        /// <summary>
        /// Routing key name
        /// </summary>
        [DefaultValue("sampleQueue")]
        [DisplayName(@"Routing key")]
        [DisplayFormat(DataFormatString = "Text")]
        public string RoutingKey { get; set; }
        /// <summary>
        /// RabbitMQ host name
        /// </summary>
        [DisplayName(@"Host name")]
        [DisplayFormat(DataFormatString = "Text")]
        public string HostName { get; set; }
		/// <summary>
		/// Use URI instead of a hostname
		/// </summary>
		[DefaultValue(false)]
		[DisplayName(@"Use URI for connection")]
		public bool ConnectWithURI { get; set; }
	}

    public class Output
    {
        public List<Message> Messages { get; set; } = new List<Message>();

    }

    public class Message
    {
        public string Data { get; set; }
        public uint MessagesCount { get; set; }
        public ulong DeliveryTag { get; set; }
    }

}
