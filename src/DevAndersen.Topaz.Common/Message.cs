using DevAndersen.Topaz.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DevAndersen.Topaz.Common
{
    public class Message
    {
        public string Type => Enum.GetName(typeof(MessageType), MessageType);

        [JsonIgnore]
        public MessageType MessageType { get; init; }

        public string Data { get; init; }

        public string DefaultValue { get; init; }

        public string Metadata { get; init; }

        public static implicit operator Message(MessageType messageType) => new Message(messageType);

        /// <summary>
        /// Used for JSON serialization.
        /// </summary>
        public Message()
        {
        }

        public Message(MessageType messageType) : this(messageType, string.Empty)
        {
        }

        public Message(MessageType messageType, string data) : this(messageType, data, string.Empty)
        {
        }

        public Message(MessageType messageType, string data, string defaultValue) : this(messageType, data, defaultValue, string.Empty)
        {
        }

        public Message(MessageType messageType, string data, string defaultValue, string metadata)
        {
            MessageType = messageType;
            Data = data;
            DefaultValue = defaultValue;
            Metadata = metadata;
        }
    }
}
