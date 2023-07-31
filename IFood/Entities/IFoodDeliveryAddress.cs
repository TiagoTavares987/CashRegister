using Newtonsoft.Json;

namespace IFood.Entities
{
    public class IFoodDeliveryAddress
    {
        //Endereço completo do cliente
        [JsonProperty(PropertyName = "formattedAddress")]
        public string FormattedAddress { get; set; }
        //Pais
        [JsonProperty(PropertyName = "country")]
        public string Country { get; set; }
        //Estado
        [JsonProperty(PropertyName = "state")]
        public string State { get; set; }
        //Cidade
        [JsonProperty(PropertyName = "city")]
        public string City { get; set; }
        //Coordenadas
        public IFoodCoordinates Coordinates { get; set; }
        //Bairro
        [JsonProperty(PropertyName = "neighborhood")]
        public string Neighborhood { get; set; }
        //Endereço(Tipo logradouro + Logradouro)
        [JsonProperty(PropertyName = "streetName")]
        public string StreetName { get; set; }
        //Numero
        [JsonProperty(PropertyName = "streetNumber")]
        public string StreetNumber { get; set; }
        //CEP
        [JsonProperty(PropertyName = "postalCode")]
        public string PostalCode { get; set; }
        //Referencia
        public string Reference { get; set; }
        //Complemento do endereço
        public string Complement { get; set; }

        [JsonProperty(PropertyName = "coordinates")]
        public IFoodApiCoordinates ApiCoordinates { get; set; }

        public class IFoodApiCoordinates
        {
            public string latitude { get; set; }
            public string longitude { get; set; }
        }
    }
}
