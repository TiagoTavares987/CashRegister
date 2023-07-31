using Newtonsoft.Json;
using System.Collections.Generic;

namespace IFood.Entities
{
    public class IFoodOrderItem
    {
        //Id do item
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        //Nome do item
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        //Código do e-PDV
        [JsonProperty(PropertyName = "externalCode")]
        public string ExternalCode { get; set; }

        //Quantidade
        [JsonProperty(PropertyName = "quantity")]
        public decimal Quantity { get; set; }

        //Observação do item
        [JsonProperty(PropertyName = "observations")]
        public string Observations { get; set; }

        //Subitens
        [JsonProperty(PropertyName = "options")]
        public List<IFoodOrderItem> SubItems { get; set; }

        //Preço por unidade
        [JsonProperty(PropertyName = "unitPrice")]
        public decimal UnitPrice { get; set; }

        //Adição
        [JsonProperty(PropertyName = "addition")]
        public decimal Addition { get; set; }

        //Preço (unitPrice * quantity) + addition(se for subitem)
        [JsonProperty(PropertyName = "price")]
        public decimal Price { get; set; }
    
        //Preço dos subitens
        [JsonProperty(PropertyName = "optionsPrice")]
        public decimal SubItemsPrice { get; set; }

        //Preço total (unitPrice * quantity) + optionsPrice(subitens)
        [JsonProperty(PropertyName = "totalPrice")]
        public decimal TotalPrice { get; set; }

        //Desconto
        public decimal Discount { get; set; }

        //Preço total por unidade do item (desconsidera preço de subitens);
        //É diferente do unitPrice pois considera addition(ocorre em pizzas para ser cobrado o preço da maior);        
        public decimal RetailPrice => Quantity > 0 ? Price / Quantity : 0;
    }    
}
