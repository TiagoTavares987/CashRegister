using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace IFood.Entities
{
    public class IFoodPrice
    {
        [JsonProperty(PropertyName = "value")]
        public double Value { get; set; }
    }

    public class IFoodPricePromotion : IFoodPrice
    {
        [JsonProperty(PropertyName = "originalValue")]
        public double OriginalValue { get; set; }
    }

    public class IFoodPizzaPrice : ISerializable
    {
        private string _property;
        private IFoodPrice _value;

        public IFoodPizzaPrice(string property, IFoodPrice value)
        {
            _property = property;
            _value = value;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(_property, _value);
        }
    }
}
