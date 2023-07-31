using Newtonsoft.Json;
using System.Linq;
using System.Collections.Generic;

namespace IFood.Entities
{
    public class IFoodPizza
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        [JsonProperty(PropertyName = "catalogId")]
        public string CatalogId { get; set; }
        [JsonProperty(PropertyName = "sizes")]
        public List<IFoodPizzaSize> Sizes { get; set; }
        [JsonProperty(PropertyName = "crusts")]
        public List<IFoodCrust> Crusts { get; set; }
        [JsonProperty(PropertyName = "edges")]
        public List<IFoodEdge> Edges { get; set; }
        [JsonProperty(PropertyName = "toppings")]
        public List<IFoodTopping> Toppings { get; set; }
        [JsonProperty(PropertyName = "shifts")]
        public object[] Shifts => new object[0];
    }

    public class IFoodTopping : IFoodProduct
    {
        public string status => "AVAILABLE";
        public object prices { get; set; }
    }

    public class IFoodPizzaSize
    {
        public string id { get; set; }
        public string name => acceptedFractions.Max() + " " + (acceptedFractions.Max() == 1 ? "Sabor" : "Sabores");
        //public int index { get; set; }
        public string status => "AVAILABLE";
        public string externalCode { get; set; }
        public int slices => 0;
        public List<int> acceptedFractions { get; set; }

        public IFoodPizzaSize Clone()
        {
            var tmp = (IFoodPizzaSize)MemberwiseClone();
            tmp.acceptedFractions = acceptedFractions.ToList();
            return tmp;
        }
    }

    public class IFoodCrust
    {
        public string id { get; set; }
        public string name => "Tradicional";
        public string status => "AVAILABLE";
        public string externalCode => 0.ToString();
        //public int index { get; set; }
        public IFoodPrice price => new IFoodPrice();
    }

    public class IFoodEdge
    {
        private string _name;
        private string _externalCode;
        private IFoodPrice _price;

        public string id { get; set; }
        public string name
        {
            get => string.IsNullOrWhiteSpace(_name) ? "Tradicional" : _name;
            set => _name = value;
        }
        public string status => "AVAILABLE";
        public string externalCode
        {
            get => string.IsNullOrWhiteSpace(_externalCode) ? 0.ToString() : _externalCode;
            set => _externalCode = value;
        }
        //public int index { get; set; }
        public IFoodPrice price
        {
            get => _price == null ? new IFoodPrice() : _price;
            set => _price = value;
        }
    }
}
