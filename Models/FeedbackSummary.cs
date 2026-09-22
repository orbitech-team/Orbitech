using System.Collections.Generic;
using System.Runtime.Serialization;


namespace OrbitechWeb.Models
{
    [DataContract]
    public class FeedbackSummary
    {
        [DataMember] 
        public int TotalResponses { get; set; }

        [DataMember] 
        public double AvgDeliveryDays { get; set; }

        [DataMember] 
        public double AvgDeliveryRating { get; set; }

        [DataMember] 
        public double AvgSatisfactionRating { get; set; }

        [DataMember] 
        public double PercentSatisfied { get; set; }

        [DataMember] 
        public List<SatisfactionBucket> SatisfactionDistribution { get; set; }

        public FeedbackSummary()
        {
            SatisfactionDistribution = new List<SatisfactionBucket>();
        }
    }

    [DataContract]
    public class SatisfactionBucket
    {
        [DataMember] 
        public int Stars { get; set; }

        [DataMember] 
        public int Count { get; set; }

        [DataMember] 
        public double Percent { get; set; }
    }
}