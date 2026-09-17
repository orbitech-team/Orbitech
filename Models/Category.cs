using System.Runtime.Serialization;

namespace OrbitechWeb.Models
{
    [DataContract]
    public class Category
    {
        [DataMember]
        public int CategoryID { get; set; }

        [DataMember]
        public string CategoryName { get; set; }
    }
}
