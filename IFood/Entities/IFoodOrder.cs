using IFood.Enumerators;
using Newtonsoft.Json;
using System;

namespace IFood.Entities
{
    public class IFoodOrder
    {
        public string DeliveryExtenderId => "IFood";

        [JsonProperty(PropertyName = "code")]
        public string Code { get; set; }
        public IFoodOrderFullCode FullCode
        {
            get
            {
                try { return (IFoodOrderFullCode)Enum.Parse(typeof(IFoodOrderFullCode), FullCodeHolder.Replace(" ", "_")); }
                catch { return IFoodOrderFullCode.INVALID; }
            }
            set { FullCodeHolder = value.ToString(); }
        }
        [JsonProperty(PropertyName = "fullCode")]
        public string FullCodeHolder { get; set; }
        [JsonProperty(PropertyName = "orderId")]
        public string CorrelationId { get; set; }
        [JsonProperty(PropertyName = "createdAt")]
        public DateTime CreatedAt { get; set; }
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public IFoodOrderDetail Detail { get; set; }

        public DateTime OrderDate { get => CreatedAt; }
        public DateTime DeliveryDate { get => CreatedAt; }
    }
}
