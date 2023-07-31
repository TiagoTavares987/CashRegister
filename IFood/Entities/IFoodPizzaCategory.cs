using Newtonsoft.Json;

namespace IFood.Entities
{
    public class IFoodPizzaCategory : IFoodCategory
    {
        public IFoodPizzaCategory() { }

        [JsonProperty(PropertyName = "template")]
        public override string Template => "PIZZA";

        [JsonProperty(PropertyName = "pizza")]
        public IFoodPizza Pizza { get; set; }
    }
}
