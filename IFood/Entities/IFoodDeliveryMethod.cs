using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IFood.Entities
{
    public class IFoodDeliveryMethod
    {
        public string Id { get; set; }
        public decimal Value { get; set; }
        public int MinTime { get; set; }
        public int MaxTime { get; set; }
        public string Mode { get; set; }
        public string DeliveredBy { get; set; }
    }
}
