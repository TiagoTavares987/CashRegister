using Newtonsoft.Json;
using System;

namespace IFood.Entities
{
    public class IFoodCustomer
    {
        //Id do cliente
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        [JsonProperty(PropertyName = "name")]
        //Nome do cliente
        public string Name { get; set; }
        //CPF/CNPJ do cliente
        [JsonProperty(PropertyName = "documentNumber")]
        public string TaxPayerIdentificationNumber { get; set; }
        //Telefone do cliente
        public string Phone 
        {
            get { return ApiPhone != null ? ApiPhone.number : ""; }
            set 
            {
                if (ApiPhone == null)
                    ApiPhone = new IFoodApiPhone();
                ApiPhone.number = value;
            }
        }
        //Email do cliente
        public string Email { get; set; }

        [JsonProperty(PropertyName = "phone")]
        public IFoodApiPhone ApiPhone { get; set; }

        public class IFoodApiPhone
        {
            public string number { get; set; }
            public string localizer { get; set; }
            public DateTime localizerExpiration { get; set; }
        }
    }
}
