using System.Runtime.Serialization;

namespace OrbitechWeb.Models
{
    [DataContract]
    public class Feedback
    {
        [DataMember]
        public int FeedbackID { get; set; }

        [DataMember]
        public int OrderID { get; set; }

        [DataMember]
        public int DeliveryRating { get; set; }

        [DataMember]
        public int SatisfactionRating { get; set; }

        [DataMember]
        public string Comments { get; set; }

        [DataMember]
        public bool IsComplaint { get; set; }

        [DataMember]
        public string ComplaintCategory { get; set; }

        [DataMember]
        public string FeedbackDate { get; set; }

        [DataMember]
        public decimal AverageDeliveryRating { get; set; }

        [DataMember]
        public decimal AvergeSatisfactionRating { get; set; }

        [DataMember]
        public int TotalFeedbackCount { get; set; }

        [DataMember]
        public int TotalComplaints { get; set; }

    }
}
