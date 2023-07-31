using Newtonsoft.Json;

namespace IFood.Entities
{
    public class IFoodOptionGroup
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "max")]
        public int MaxQuantity { get; set; }
        [JsonProperty(PropertyName = "min")]
        public int MinQuantity => 0;
        [JsonProperty(PropertyName = "index")]
        public int Index => 0;
    }
}
