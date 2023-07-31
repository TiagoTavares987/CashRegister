using Newtonsoft.Json;
using System.Collections.Generic;

namespace IFood.Entities
{
    public class IFoodItem
    {
        //Id do item
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        //Id do item
        [JsonProperty(PropertyName = "productId")]
        public string ProductId { get; set; }
        //Nome do item
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        //Descrição do item
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }
        //Código do e-PDV
        [JsonProperty(PropertyName = "externalCode")]
        public string ExternalCode { get; set; }
        //Disponibilidade
        [JsonProperty(PropertyName = "status")]
        public string Status => "AVAILABLE";
        //código de barras
        [JsonProperty(PropertyName = "ean")]
        public string Barcode { get; set; }
        //preço
        [JsonProperty(PropertyName = "price")]
        public IFoodPrice Price { get; set; }
        //grupos de opções
        [JsonProperty(PropertyName = "optionGroups")]
        public List<IFoodOptionGroup> OptionGroups { get; set; }
    }
}
