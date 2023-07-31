using Newtonsoft.Json;

namespace IFood.Entities
{
    //Forma de pagamento⁎⁎
    public class IFoodPayment
    {
        //Nome da forma de pagamento
        public string Name { get; set; }
        //Tipo de forma de pagamento⁎⁎⁎
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }
        //Valor pago na forma
        [JsonProperty(PropertyName = "value")]
        public decimal Value { get; set; }
        //Pedido pago('true' ou 'false')
        [JsonProperty(PropertyName = "prepaid")]
        public bool Prepaid { get; set; }
        //Codigo da forma de pagamento⁎⁎⁎⁎
        [JsonProperty(PropertyName = "method")]
        public string ExternalCode { get; set; }
        //Bandeira
        public string Issuer { get { return Card != null ? Card.brand : string.Empty; } }
        //Troco
        public decimal ChangeFor 
        {
            get { return Cash != null ? Cash.changeFor : 0; }
            set
            {
                if (Cash == null)
                    Cash = new IFoodApiCashPayment();
                Cash.changeFor = value;
            }
        }


        [JsonProperty(PropertyName = "card")]
        public IFoodApiCardPayment Card { get; set; }
        [JsonProperty(PropertyName = "cash")]
        public IFoodApiCashPayment Cash { get; set; }

        public class IFoodApiCardPayment
        {
            public string brand { get; set; }
        }
        public class IFoodApiCashPayment
        {
            public decimal changeFor { get; set; }
        }
    }
}
