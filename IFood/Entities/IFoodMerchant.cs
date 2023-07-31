using Newtonsoft.Json;
using System.Collections.Generic;

namespace IFood.Entities
{
    public class IFoodMerchant
    {
        //Identificador unico do restaurante⁎
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        //Nome do restaurante
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        //Telefone do restaurante
        public List<string> Phones { get; set; }
        //Endereço
        public IFoodAddress Address { get; set; }

    }
}
