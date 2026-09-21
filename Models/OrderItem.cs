using System.Runtime.Serialization;

namespace OrbitechWeb.Models
{
    // PART A: One line of a placed order (one row of ORDER_ITEM).
    [DataContract]
    public class OrderItem
    {
        [DataMember]
        public int OrderItemID { get; set; }

        [DataMember]
        public int ProductID { get; set; }

        [DataMember]
        public string ProductName { get; set; }

        [DataMember]
        public string ProductImage { get; set; }

        [DataMember]
        public int Quantity { get; set; }

        [DataMember]
        public decimal UnitPrice { get; set; }

        [DataMember]
        public decimal LineTotal { get; set; }
    }
}
