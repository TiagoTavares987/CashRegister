using Newtonsoft.Json;
using System;
using System.Linq;
using System.Collections.Generic;
using IFood.Enumerators;

namespace IFood.Entities
{
    public class IFoodOrderDetail
    {
        //Id de referencia interno
        public string Id { get; set; }
        //Id de referencia do pedido
        [JsonProperty(PropertyName = "displayId")]
        public string Reference { get; set; }
        //Extranet Id
        public string ShortReference { get; set; }
        //Timestamp do pedido
        public DateTime CreatdAt { get; set; }
        //Tipo do pedido('DELIVERY' ou 'TOGO')
        [JsonProperty(PropertyName = "orderType")]
        public string Type { get; set; }
        public IFoodOrderType OrderType
        {
            get
            {
                //TODO IFOOD remove debug and try/catch after properly tested
                if (System.Diagnostics.Debugger.IsAttached)
                    return (IFoodOrderType)Enum.Parse(typeof(IFoodOrderType), Type.Replace(" ", "_"));
                else
                {
                    try { return (IFoodOrderType)Enum.Parse(typeof(IFoodOrderType), Type.Replace(" ", "_")); }
                    catch { return IFoodOrderType.INVALID; }
                }
            }
            set { Type = value.ToString(); }
        }
        //Tipo do pedido('IMMEDIATE' ou '...')
        [JsonProperty(PropertyName = "orderTiming")]
        public string Timing { get; set; }
        //Restaurante
        [JsonProperty(PropertyName = "merchant")]
        public IFoodMerchant Merchant { get; set; }
        //Pagamentos
        [JsonIgnore]
        public List<IFoodPayment> Payments
        {
            get { return ApiPayments != null ? ApiPayments.methods : null; }
            set 
            {
                if (ApiPayments == null)
                    ApiPayments = new IFoodApiPayments();
                ApiPayments.methods = value;
            }
        }

        //Cliente
        public IFoodCustomer Customer { get; set; }

        //Items
        [JsonProperty(PropertyName = "items")]
        public List<IFoodOrderItem> Items { get; set; }

        [JsonProperty(PropertyName = "salesChannel")]
        public string SalesChannel { get; set; }

        //Total do pedido(Sem taxa de entrega)
        public decimal SubTotal 
        {
            get { return ApiTotal != null ? ApiTotal.subTotal : 0; }
            set 
            {
                if (ApiTotal == null)
                    ApiTotal = new IFoodApiTotal();
                ApiTotal.subTotal = value;
            }
        }
        //Total do pedido(Com taxa de entrega)
        public decimal TotalPrice
        {
            get { return ApiTotal != null ? ApiTotal.orderAmount : 0; }
            set
            {
                if (ApiTotal == null)
                    ApiTotal = new IFoodApiTotal();
                ApiTotal.orderAmount = value;
            }
        }
        //Taxa de entrega
        public decimal DeliveryFee
        {
            get { return ApiTotal != null ? ApiTotal.deliveryFee : 0; }
            set
            {
                if (ApiTotal == null)
                    ApiTotal = new IFoodApiTotal();
                ApiTotal.deliveryFee = value;
            }
        }
        //Morada de entrega
        public IFoodDeliveryAddress DeliveryAddress
        {
            get { return ApiDelivery != null ? ApiDelivery.deliveryAddress : null; }
            set
            {
                if (ApiDelivery == null)
                    ApiDelivery = new IFoodApiDelivery();
                ApiDelivery.deliveryAddress = value;
            }
        }
        //Timestamp do pedido
        public DateTime DeliveryDateTime
        {
            get
            {
                var date = DateTime.MinValue;
                switch (OrderType)
                {
                    case IFoodOrderType.DELIVERY:
                        if (ApiDelivery != null)
                            date = ApiDelivery.deliveryDateTime;
                        break;
                    case IFoodOrderType.TAKEOUT:
                        if (ApiTakeout != null)
                            date = ApiTakeout.takeoutDateTime;
                        break;
                    case IFoodOrderType.INDOOR:
                        if (ApiIndoor != null)
                            date = ApiIndoor.indoorDateTime;
                        break;
                }
                return date;
            }
            set
            {
                switch(OrderType)
                {
                    default:
                    case IFoodOrderType.DELIVERY:
                        if (ApiDelivery == null)
                            ApiDelivery = new IFoodApiDelivery();
                        ApiDelivery.deliveryDateTime = value;
                        break;
                    case IFoodOrderType.TAKEOUT:
                        if (ApiTakeout == null)
                            ApiTakeout = new IFoodApiTakeout();
                        ApiTakeout.takeoutDateTime = value;
                        break;
                    case IFoodOrderType.INDOOR:
                        if (ApiIndoor == null)
                            ApiIndoor = new IFoodApiIndoor();
                        ApiIndoor.indoorDateTime = value;
                        break;
                }
            }
        }

        [JsonProperty(PropertyName = "payments")]
        public IFoodApiPayments ApiPayments { get; set; }

        [JsonProperty(PropertyName = "delivery")]
        public IFoodApiDelivery ApiDelivery { get; set; }

        [JsonProperty(PropertyName = "takeout")]
        public IFoodApiTakeout ApiTakeout { get; set; }

        [JsonProperty(PropertyName = "indoor")]
        public IFoodApiIndoor ApiIndoor { get; set; }

        [JsonProperty(PropertyName = "benefits")]
        public List<IFoodApiBenefit> ApiBenefits { get; set; }

        [JsonProperty(PropertyName = "total")]
        public IFoodApiTotal ApiTotal { get; set; }

        public class IFoodApiPayments
        {
            public decimal prepaid { get; set; }
            public decimal pending { get; set; }
            public List<IFoodPayment> methods { get; set; }
        }

        public class IFoodApiDelivery
        {
            public string mode { get; set; }
            public string deliveredBy { get; set; }
            public DateTime deliveryDateTime { get; set; }
            public IFoodDeliveryAddress deliveryAddress { get; set; }
        }
        public class IFoodApiTakeout
        {
            public string mode { get; set; }
            public DateTime takeoutDateTime { get; set; }
        }
        public class IFoodApiIndoor
        {
            public DateTime indoorDateTime { get; set; }
        }

        public class IFoodApiBenefit
        {
            private string _targetId;

            public decimal value { get; set; }
            public string target { get; set; }
            public IFoodApiBenefitType TargetType
            {
                get
                {
                    //TODO IFOOD remove debug and try/catch after properly tested
                    if (System.Diagnostics.Debugger.IsAttached)
                        return (IFoodApiBenefitType)Enum.Parse(typeof(IFoodApiBenefitType), target.Replace(" ", "_"));
                    else
                    {
                        try { return (IFoodApiBenefitType)Enum.Parse(typeof(IFoodApiBenefitType), target.Replace(" ", "_")); }
                        catch { return IFoodApiBenefitType.UNKNOWN; }
                    }
                }
                set { target = value.ToString(); }
            }
            public string targetId
            {
                get { return _targetId; }
                set
                {
                    _targetId = value;
                    if (_targetId != null && int.TryParse(value.ToString(), out int targetItem) && TargetType == IFoodApiBenefitType.UNKNOWN)
                        TargetType = IFoodApiBenefitType.ITEM;
                }
            }
            public List<IFoodApiBenefitValue> sponsorshipValues { get; set; }

            public int targetItem { get; set; }
            public decimal IFoodValue => sponsorshipValues == null ? 0 : sponsorshipValues.Where(x => "IFOOD".Equals(x.name)).Sum(y => y.value);
            public decimal MerchantValue => sponsorshipValues == null ? 0 : sponsorshipValues.Where(x => "MERCHANT".Equals(x.name)).Sum(y => y.value);
        }
        public class IFoodApiBenefitValue
        {
            public string name { get; set; }
            public decimal value { get; set; }
        }

        public class IFoodApiTotal
        {
            public decimal subTotal { get; set; }
            public decimal deliveryFee { get; set; }
            //public decimal benefits { get; set; }
            public decimal orderAmount { get; set; }
            //public decimal additionalFees { get; set; }
        }
    }
}
