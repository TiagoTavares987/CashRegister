using Newtonsoft.Json;
using System.Collections.Generic;

namespace IFood.Entities
{
    public class IFoodItemCategory : IFoodCategory
    {
        [JsonProperty(PropertyName = "items")]
        public List<IFoodItem> Items { get; set; }
    }
}
