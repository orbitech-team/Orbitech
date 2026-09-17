using System.Runtime.Serialization;

namespace OrbitechWeb.Models
{
    [DataContract]
    public class Product
    {
        [DataMember]
        public int ProductID { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public decimal Price { get; set; }

        [DataMember]
        public int Quantity { get; set; }

        [DataMember]
        public string ImageURL { get; set; }

        [DataMember]
        public int CategoryID { get; set; }

        [DataMember]
        public string CategoryName { get; set; }

        [DataMember]
        public string Brand { get; set; }

        [DataMember]
        public string Colour { get; set; }

        [DataMember]
        public string Condition { get; set; }

        [DataMember]
        public string Grade { get; set; }
    }
}
