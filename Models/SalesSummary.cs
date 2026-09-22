using System.Runtime.Serialization;
using System.Collections.Generic;


namespace OrbitechWeb.Models
{
    [DataContract]
    public class SalesSummary
    {
        [DataMember] 
        public decimal TotalRevenue { get; set; }

        [DataMember] 
        public int OrderCount { get; set; }

        [DataMember] 
        public decimal AvgOrderValue { get; set; }

        
        [DataMember] public List<MonthlySale> MonthlySales { get; set; }
        [DataMember] public List<TopProduct> TopProducts { get; set; }

        public SalesSummary()
        {
            MonthlySales = new List<MonthlySale>();
            TopProducts = new List<TopProduct>();
        }
    }

    [DataContract]
    public class MonthlySale
    {
        [DataMember] 
        public string MonthLabel { get; set; }

        [DataMember] 
        public int OrderCount { get; set; }

        [DataMember] 
        public decimal Revenue { get; set; }
    }

    [DataContract]
    public class TopProduct
    {
        [DataMember] 
        public string ProductName { get; set; }

        [DataMember] 
        public int QuantitySold { get; set; }

        [DataMember] 
        public decimal Revenue { get; set; }
    }
}
