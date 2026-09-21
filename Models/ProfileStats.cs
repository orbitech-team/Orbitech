using System;
using System.Runtime.Serialization;

namespace OrbitechWeb.Models
{
    // PHASE 3: Customer-side activity stats for the profile page.
    // Sized to exactly what the customer profile tiles display.
    [DataContract]
    public class ProfileStats
    {
        [DataMember]
        public string Username { get; set; }

        [DataMember]
        public DateTime MemberSince { get; set; }

        [DataMember]
        public int TotalOrders { get; set; }

        [DataMember]
        public decimal TotalSpent { get; set; }

        [DataMember]
        public int FavouriteCount { get; set; }
    }
}
