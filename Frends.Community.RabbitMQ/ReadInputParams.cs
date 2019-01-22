using System.ComponentModel;

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
        public string QueueName { get; set; }
        /// <summary>
        /// RabbitMQ host name
        /// </summary>
        [DefaultValue("localhost")]
        public string HostName { get; set; }
        /// <summary>
        /// Maximum number of messages to read
        /// </summary>
        [DefaultValue(1)]
        public int ReadMessageCount { get; set; }
        /// <summary>
        /// Acknowledge read messages. False to just peek last message
        /// </summary>
        [DefaultValue(true)]
        public bool AutoAck { get; set; }
    }
}
