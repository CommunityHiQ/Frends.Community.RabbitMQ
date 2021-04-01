using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.Community.RabbitMQ
{
    /// <summary>
    /// Acknowledge type for manual operation
    /// </summary>
    public enum ManualAckType
    {
        Ack,
        Nack,
        NackAndRequeue,
        Reject,
        RejectAndRequeue
    }

    /// <summary>
    /// Acknowledge type while reading message
    /// </summary>
    public enum ReadAckType
    {
        ManualAck,
        AutoAck,
        AutoNack,
        AutoNackAndRequeue,
        AutoReject,
        AutoRejectAndRequeue
    }

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
        /// Set acknowledgement type. AutoAck,AutoNack, AutoNackAndRequeue,AutoReject,AutoRejectAndRequeue,ManualAck.
        /// </summary>
        [DefaultValue(ReadAckType.AutoAck)]
        [DisplayName(@"Auto ack")]
        public ReadAckType AutoAck { get; set; }
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

        public WriteInputParams()
        {
            ExchangeName = String.Empty;
            RoutingKey = String.Empty;
        }

        /// <summary>
        /// Data payload
        /// </summary>
        [DisplayName(@"Data")]
        [DisplayFormat(DataFormatString = "Expression")]
        public byte[] Data { get; set; }
        /// <summary>
        /// Name of the exchange e.g. sampleExchange
        /// </summary>
        [DefaultValue("")]
        [DisplayName(@"Exchange name")]
        [DisplayFormat(DataFormatString = "Text")]
        public string ExchangeName { get; set; }
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
        /// <summary>
        /// True to declare queue when writing
        /// </summary>
        [DefaultValue(false)]
        [DisplayName(@"True to declare queue before writing. False to not declare it")]
        public bool Create { get; set; }
        /// <summary>
		/// Durable option when creating queue
		/// </summary>
		[DefaultValue(true)]
        [DisplayName(@"Set durable option when creating queue")]
        public bool Durable { get; set; }
        
    }


    /// <summary>
    /// Collection of write message parameters
    /// </summary>
    public class WriteBatchInputParams
    {

        public WriteBatchInputParams()
        {
            ExchangeName = String.Empty;
            RoutingKey = String.Empty;
        }

        /// <summary>
        /// Data payload
        /// </summary>
        [DisplayName(@"Data")]
        [DisplayFormat(DataFormatString = "Expression")]
        public byte[] Data { get; set; }
        /// <summary>
        /// Name of the exchange e.g. sampleExchange
        /// </summary>
        [DefaultValue("")]
        [DisplayName(@"Exchange name")]
        [DisplayFormat(DataFormatString = "Text")]
        public string ExchangeName { get; set; }
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
        /// Amount of messages in the buffer under transaction which will be sent over the messaging channel as a chunk. 
        /// </summary>
        [DefaultValue(1)]
        [DisplayFormat(DataFormatString = "Text")]
        [DisplayName(@"Write message count")]
        public int WriteMessageCount { get; set; }
        /// <summary>
        /// Process execution id from the system 
        /// </summary>
        [DefaultValue("#process.executionid")]
        [DisplayFormat(DataFormatString = "Expression")]
        [DisplayName(@"Process execution id")]
        public string ProcessExecutionId { get; set; }
        /// <summary>
        /// Use URI instead of a hostname
        /// </summary>
        [DefaultValue(false)]
        [DisplayName(@"Use URI for connection")]
        public bool ConnectWithURI { get; set; }
        /// <summary>
        /// True to declare queue when writing
        /// </summary>
        [DefaultValue(false)]
        [DisplayName(@"True to declare queue before writing. False to not declare it")]
        public bool Create { get; set; }
        /// <summary>
		/// Durable option when creating queue
		/// </summary>
		[DefaultValue(true)]
        [DisplayName(@"Set durable option when creating queue")]
        public bool Durable { get; set; }
        
        /// <summary>
        /// Amount of seconds waiting for confirmation messages. 
        /// </summary>
        [DefaultValue(1)]
        [DisplayFormat(DataFormatString = "Text")]
        [DisplayName(@"Wait for acknowledgement in seconds")]
        public int WaitForAcknowledgement { get; set; }
        
    }

    public class WriteInputParamsString
    {
        public WriteInputParamsString()
        {
            ExchangeName = String.Empty;
            RoutingKey = String.Empty;
        }

        /// <summary>
        /// Data payload in string. Will be internally converted to byte array using UTF8.Convert method
        /// </summary>
        [DisplayName(@"Data")]
        [DisplayFormat(DataFormatString = "Text")]
        public string Data { get; set; }
        
        /// <summary>
        /// Name of the exchange
        /// </summary>
        [DefaultValue("sampleExchange")]
        [DisplayName(@"Exchange name")]
        [DisplayFormat(DataFormatString = "Text")]
        public string ExchangeName { get; set; }
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
        /// <summary>
        /// True to declare queue when writing
        /// </summary>
        [DefaultValue(false)]
        [DisplayName(@"True to declare queue before writing. False to not declare it")]
        public bool Create { get; set; }
        /// <summary>
		/// Durable option when creating queue
		/// </summary>
		[DefaultValue(true)]
        [DisplayName(@"Set durable option when creating queue")]
        public bool Durable { get; set; }
    }

    public class WriteBatchInputParamsString
    {
        public WriteBatchInputParamsString()
        {
            ExchangeName = String.Empty;
            RoutingKey = String.Empty;
        }

        /// <summary>
        /// Data payload in string. Will be internally converted to byte array using UTF8.Convert method
        /// </summary>
        [DisplayName(@"Data")]
        [DisplayFormat(DataFormatString = "Text")]
        public string Data { get; set; }

        /// <summary>
        /// Name of the exchange
        /// </summary>
        [DefaultValue("sampleExchange")]
        [DisplayName(@"Exchange name")]
        [DisplayFormat(DataFormatString = "Text")]
        public string ExchangeName { get; set; }
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
        /// Amount of messages in the buffer under acknoledgement which will be sent over the messaging channel as a chunk. 
        /// </summary>
        [DefaultValue(1)]
        [DisplayFormat(DataFormatString = "Text")]
        [DisplayName(@"Write message count")]
        public int WriteMessageCount { get; set; }

        /// <summary>
        /// Process execution id from the system 
        /// </summary>
        [DefaultValue("#process.executionid")]
        [DisplayFormat(DataFormatString = "Expression")]
        [DisplayName(@"Process execution id")]
        public string ProcessExecutionId { get; set; }
        /// <summary>
        /// Use URI instead of a hostname
        /// </summary>
        [DefaultValue(false)]
        [DisplayName(@"Use URI for connection")]
        public bool ConnectWithURI { get; set; }
        /// <summary>
        /// True to declare queue when writing
        /// </summary>
        [DefaultValue(false)]
        [DisplayName(@"True to declare queue before writing. False to not declare it")]
        public bool Create { get; set; }
        /// <summary>
		/// Durable option when creating queue
		/// </summary>
		[DefaultValue(true)]
        [DisplayName(@"Set durable option when creating queue")]
        public bool Durable { get; set; }
    }

    public class Output
    {
        public List<Message> Messages { get; set; } = new List<Message>();

    }

    public class Message
    {
        /// <summary>
        /// Data in base64 format
        /// </summary>
        public string Data { get; set; }
        public uint MessagesCount { get; set; }
        public ulong DeliveryTag { get; set; }
    }

    public class MessageString
    {
        /// <summary>
        /// Data in UTF8 string converted from byte[] array
        /// </summary>
        public string Data { get; set; }
        public uint MessagesCount { get; set; }
        public ulong DeliveryTag { get; set; }
    }



    public class OutputString
    {
        public List<MessageString> Messages { get; set; } = new List<MessageString>();
    }

}
