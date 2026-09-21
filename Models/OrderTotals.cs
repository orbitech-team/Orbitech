using System.Runtime.Serialization;

namespace OrbitechWeb.Models
{
    // PART A: Carries the full price breakdown of an order.
    // One method (CalculateOrderTotals) fills this object so the
    // Cart and Checkout pages always show the same numbers.
    [DataContract]
    public class OrderTotals
    {
        [DataMember]
        public decimal Subtotal { get; set; }

        [DataMember]
        public decimal Discount { get; set; }

        [DataMember]
        public decimal Shipping { get; set; }

        [DataMember]
        public decimal Tax { get; set; }

        [DataMember]
        public decimal Total { get; set; }

        [DataMember]
        public string PromoCode { get; set; }

        [DataMember]
        public bool PromoApplied { get; set; }
    }
}
    