using System.Runtime.Serialization;

namespace OrbitechWeb.Models
{
    // PHASE 3: The four KPI tiles on the admin profile dashboard.
    // Deliberately ONLY simple aggregates - the full Reports page
    // (charts, categories, top products) belongs to Member C.
    [DataContract]
    public class AdminDashboardStats
    {
        [DataMember]
        public int TotalProducts { get; set; }

        [DataMember]
        public int TotalOrders { get; set; }

        [DataMember]
        public int TotalUsers { get; set; }

        [DataMember]
        public decimal TotalRevenue { get; set; }
    }
}
