using System.Runtime.Serialization;

namespace OrbitechWeb.Models
{
    [DataContract]
    public class CartItem
    {
        [DataMember]
        public int CartItemID { get; set; }

        [DataMember]
        public int ProductID { get; set; }

        [DataMember]
        public string ProductName { get; set; }

        [DataMember]
        public string ProductImage { get; set; }

        [DataMember]
        public decimal ProductPrice { get; set; }

        [DataMember]
        public int Quantity { get; set; }

        [DataMember]
        public decimal LineTotal { get; set; }

        [DataMember]
        public string Brand { get; set; }

        [DataMember]
        public string Condition { get; set; }
    }
}
