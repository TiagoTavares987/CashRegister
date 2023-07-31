namespace IFood.Entities
{
    public class IFoodAddress
    {
        //Endereço formatado
        public string FormattedAddress { get; set; }
        //Pais
        public string Country { get; set; }
        //Estado
        public string State { get; set; }
        //Cidade
        public string City { get; set; }
        //Bairro
        public string Neighborhood { get; set; }
        //Endereço (Tipo logradouro + Logradouro)
        public string StreetName { get; set; }
        //Numero
        public string StreetNumber { get; set; }
        //CEP
        public string PostalCode { get; set; }
    }
}
