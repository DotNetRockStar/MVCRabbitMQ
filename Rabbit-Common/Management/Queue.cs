using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jerrod.RabbitCommon.Management
{
    /// <summary>
    /// Queue POCO used in the ManagementApiClient for administrative purposes.
    /// </summary>
    public class Queue
    {
        public int Memory { get; set; }
        public int Messages { get; set; }
        [JsonProperty(PropertyName = "messages_details")]
        public MessagesDetails MessagesDetails { get; set; }
        [JsonProperty(PropertyName = "messages_ready")]
        public int MessagesReady { get; set; }
        [JsonProperty(PropertyName = "messages_ready_details")]
        public MessagesReadyDetails MessagesReadyDetails { get; set; }
        [JsonProperty(PropertyName = "messages_unacknowledged")]
        public int MessagesUnacknowledged { get; set; }
        [JsonProperty(PropertyName = "messages_unacknowledged_details")]
        public MessagesUnacknowledgedDetails MessagesUnacknowledgedDetails { get; set; }
        [JsonProperty(PropertyName = "idle_since")]
        public string IdleSince { get; set; }
        [JsonProperty(PropertyName = "consumer_utilisation")]
        public string ConsumerUtilisation { get; set; }
        public string Policy { get; set; }
        [JsonProperty(PropertyName = "exclusive_consumer_tag")]
        public string ExclusiveConsumerTag { get; set; }
        public int Consumers { get; set; }
        [JsonProperty(PropertyName = "slave_nodes")]
        public List<string> SlaveNodes { get; set; }
        [JsonProperty(PropertyName = "synchronised_slave_nodes")]
        public List<string> SynchronisedSlaveNodes { get; set; }
        [JsonProperty(PropertyName = "recoverable_slaves")]
        public List<object> RecoverableSlaves { get; set; }
        public string State { get; set; }
        [JsonProperty(PropertyName = "messages_ram")]
        public int MessagesRam { get; set; }
        [JsonProperty(PropertyName = "messages_ready_ram")]
        public int MessagesReadyRam { get; set; }
        [JsonProperty(PropertyName = "messages_unacknowledged_ram")]
        public int MessagesUnacknowledgedRam { get; set; }
        [JsonProperty(PropertyName = "messages_persistent")]
        public int MessagesPersistent { get; set; }
        [JsonProperty(PropertyName = "message_bytes")]
        public int MessageBytes { get; set; }
        [JsonProperty(PropertyName = "message_bytes_ready")]
        public int MessageBytesReady { get; set; }
        [JsonProperty(PropertyName = "message_bytes_unacknowledged")]
        public int MessageBytesUnacknowledged { get; set; }
        [JsonProperty(PropertyName = "message_bytes_ram")]
        public int MessageBytesRam { get; set; }
        [JsonProperty(PropertyName = "message_bytes_persistent")]
        public int MessageBytesPersistent { get; set; }
        [JsonProperty(PropertyName = "disk_reads")]
        public int DiskReads { get; set; }
        [JsonProperty(PropertyName = "disk_writes")]
        public int DiskWrites { get; set; }
        [JsonProperty(PropertyName = "backing_queue_status")]
        public BackingQueueStatus BackingQueueStatus { get; set; }
        public string Name { get; set; }
        public string VHost { get; set; }
        public bool Durable { get; set; }
        [JsonProperty(PropertyName = "auto_delete")]
        public bool AutoDelete { get; set; }
        public Dictionary<string, string> Arguments { get; set; }
        public string Node { get; set; }
    }

    public class MessagesDetails
    {
        public decimal Rate { get; set; }
    }

    public class MessagesReadyDetails
    {
        public decimal Rate { get; set; }
    }

    public class MessagesUnacknowledgedDetails
    {
        public decimal Rate { get; set; }
    }

    public class BackingQueueStatus
    {
        public int Q1 { get; set; }
        public int Q2 { get; set; }
        public List<object> Delta { get; set; }
        public int Q3 { get; set; }
        public int Q4 { get; set; }
        public int Len { get; set; }
        [JsonProperty(PropertyName = "target_ram_count")]
        public string TargetRamCount { get; set; }
        [JsonProperty(PropertyName = "next_seq_id")]
        public int NextSeqDd { get; set; }
        [JsonProperty(PropertyName = "avg_ingress_rate")]
        public decimal AvgIngressRate { get; set; }
        [JsonProperty(PropertyName = "avg_egress_rate")]
        public decimal AvgEgressRate { get; set; }
        [JsonProperty(PropertyName = "avg_ack_ingress_rate")]
        public decimal AvgAckIngressRate { get; set; }
        [JsonProperty(PropertyName = "avg_ack_egress_rate")]
        public decimal AvgAckEgressRate { get; set; }
        [JsonProperty(PropertyName = "mirror_seen")]
        public int MirrorSeen { get; set; }
        [JsonProperty(PropertyName = "mirror_senders")]
        public int MirrorSenders { get; set; }
    }
}
