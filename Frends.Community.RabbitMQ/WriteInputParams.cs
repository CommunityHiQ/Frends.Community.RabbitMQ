using System.ComponentModel;

namespace Frends.Community.RabbitMQ
{
    /// <summary>
    /// Collection of write message parameters
    /// </summary>
    public class WriteInputParams
    {
        /// <summary>
        /// Data payload
        /// </summary>
        public byte[] Data { get; set; }
        /// <summary>
        /// Name of the queue
        /// </summary>
         [DefaultValue("sampleQueue")]
        public string QueueName { get; set; }
        /// <summary>
        /// Routing key name
        /// </summary>
        [DefaultValue("sampleQueue")]
        public string RoutingKey { get; set; }
        /// <summary>
        /// RabbitMQ host name
        /// </summary>
        [DefaultValue("localhost")]
        public string HostName { get; set; }
    }

    
}
