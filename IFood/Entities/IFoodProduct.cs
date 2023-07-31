using Newtonsoft.Json;

namespace IFood.Entities
{
    public class IFoodProduct
    {
        //Id do produto
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        //Nome do produto
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        //Descrição do produto
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }
        //Código do e-PDV
        [JsonProperty(PropertyName = "externalCode")]
        public string ExternalCode { get; set; }
        //nome da imagem
        [JsonProperty(PropertyName = "image")]
        public string Image { get; set; }
        //???
        [JsonProperty(PropertyName = "serving")]
        public string Serving => "NOT_APPLICABLE";
        //código de barras
        [JsonProperty(PropertyName = "ean")]
        public string Barcode { get; set; }
    }
}
