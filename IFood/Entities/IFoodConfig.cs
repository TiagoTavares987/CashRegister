using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XDPeople.Entities.Brazil.IFood;
using XDPeople.Utils;

namespace XDPeople.Entities
{
    public class IFoodConfig : IConfig
    {

        #region Members
        #endregion

        #region Properties
        public bool Active { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string MerchantId { get; set; }
        public List<IFoodPaymentMatch> PaymentsMatch { get; set; }
        public int PriceLine { get; set; }
        public bool BeepOnNewOrder { get; set; }
        public MediaProcessInfo CustomSoundInfo { get; set; }
        #endregion

        #region IConfig

        public IFoodConfig()
        {
            PriceLine = 1;
            PaymentsMatch = new List<IFoodPaymentMatch>();
        }

        public string Id
        {
            get { return "IFoodConfig"; }
        }

        public string SecondaryId
        {
            get;
            set;
        }

        public string Serialize()
        {
            if (this != null)
                return JsonConvert.SerializeObject(this);
            else
                return "";
        }

        public bool Deserialize(string objectString)
        {
            IFoodConfig temp;

            try
            {
                temp = JsonConvert.DeserializeObject<IFoodConfig>(objectString);
            }
            catch (Exception)
            {
                return false;
            }

            if (temp != null)
            {
                this.Active = temp.Active;
                this.Username = temp.Username;
                this.Password = temp.Password;
                this.MerchantId = temp.MerchantId;
                this.PaymentsMatch = temp.PaymentsMatch;
                this.PriceLine = temp.PriceLine;
                this.BeepOnNewOrder = temp.BeepOnNewOrder;
                this.CustomSoundInfo = temp.CustomSoundInfo;

                if (this.PaymentsMatch == null)
                    this.PaymentsMatch = new List<IFoodPaymentMatch>();

                if (this.PaymentsMatch.Count == 0)
                    this.Active = false;

                return true;
            }
            else
                return false;
        }
        #endregion
    }
}
