using Newtonsoft.Json;

namespace IFood.Entities
{
    public abstract class IFoodCategory
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        [JsonProperty(PropertyName = "externalCode")]
        public string ExternalCode { get; set; }
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "status")]
        public string Availability => "AVAILABLE";
        //public int sequence { get; set; }
        //[JsonProperty(PropertyName = "index")]
        //public int Index { get; set; }
        //[JsonProperty(PropertyName = "description")]
        //public string Description { get; set; }
        //[JsonProperty(PropertyName = "order")]
        //public int Order { get; set; }
        [JsonProperty(PropertyName = "template")]
        public virtual string Template => "DEFAULT";
    }
}
