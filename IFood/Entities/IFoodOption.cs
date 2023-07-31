using Newtonsoft.Json;

namespace IFood.Entities
{
    public class IFoodOption
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        [JsonProperty(PropertyName = "externalCode")]
        public string ExternalCode { get; set; }
        [JsonProperty(PropertyName = "status")]
        public string Status => "AVAILABLE";
        [JsonProperty(PropertyName = "price")]
        public IFoodPrice Price { get; set; }
        //[JsonProperty(PropertyName = "min")]
        //public int MinQuantity => 0;
        //[JsonProperty(PropertyName = "max")]
        //public int MaxQuantity => 1;
        [JsonProperty(PropertyName = "index")]
        public int Index => 0;
    }
}
