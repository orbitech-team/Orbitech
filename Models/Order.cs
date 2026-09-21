using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace OrbitechWeb.Models
{
    // PART A: Represents one placed order (one row of ORBI_ORDER).
    [DataContract]
    public class Order
    {
        [DataMember]
        public int OrderID { get; set; }

        [DataMember]
        public int UserID { get; set; }

        [DataMember]
        public string Username { get; set; }

        [DataMember]
        public DateTime OrderDate { get; set; }

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
        public string Status { get; set; }

        [DataMember]
        public string InvoiceNumber { get; set; }

        [DataMember]
        public List<OrderItem> Items { get; set; }
    }
}
