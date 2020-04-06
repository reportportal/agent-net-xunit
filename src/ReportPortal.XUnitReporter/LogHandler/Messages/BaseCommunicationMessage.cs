using System.Runtime.Serialization;

namespace ReportPortal.XUnitReporter.LogHandler.Messages
{
    [DataContract]
    class BaseCommunicationMessage
    {
        [DataMember]
        public virtual CommunicationAction Action { get; set; }
    }
}
