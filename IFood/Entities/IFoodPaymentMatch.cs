using System.Collections.Generic;
using System.Linq;

namespace XDPeople.Entities.Brazil.IFood
{
    //To make matches 'any' exlusive
    //remove uncommented lines
    //and uncomment lines
    //marked with "# make matches 'any' exlusive"

    public class IFoodPaymentMatch
    {
        public int PaymentId { get; set; }
        public string IFoodPaymentType { get; set; }
        public string IFoodPaymentMethod { get; set; }
        public string IFoodPaymentBrand { get; set; }
    }

    public static class IFoodPaymentMatchTranslation
    {
        public const string Na = "<Não aplicável>";
        public const string Any = "<Qualquer>";

        public static Dictionary<string, string> GetTypes() => new Dictionary<string, string>()
        {
            { "ONLINE", "Plataforma" },
            { "OFFLINE", "Loja" },
        };

        public static Dictionary<string, string> GetMethods() => new Dictionary<string, string>()
        {
            { "CASH", "Numerário" },
            { "CREDIT", "Crédito" },
            { "DEBIT", "Débito" },
            { "MEAL_VOUCHER", "Voucher/cupom (MEAL_VOUCHER)" },
            { "FOOD_VOUCHER", "Voucher/cupom (FOOD_VOUCHER)" },
            { "GIFT_CARD", "Cartão oferta" },
            { "DIGITAL_WALLET", "APPLE_PAY/GOOGLE_PAY/SAMSUNG_PAY" },
            { "PIX", "PIX" },
            { "BANK_DRAFT", "Cheque" },
            { "OTHER", "Outros" },
        };

        public static Dictionary<string, HashSet<string>> GetMatches() => new Dictionary<string, HashSet<string>>()
        {
            { "ONLINE", new HashSet<string>()
                {
                    //# make matches 'any' exlusive
                    string.Empty,
                    "CREDIT",
                    "DEBIT",
                    "MEAL_VOUCHER",
                    "FOOD_VOUCHER",
                    "GIFT_CARD",
                    "DIGITAL_WALLET",
                    "PIX",
                    "BANK_DRAFT",
                    "OTHER",
                }
            },
            { "OFFLINE", new HashSet<string>()
                {
                    //# make matches 'any' exlusive
                    string.Empty,
                    "CASH",
                    "CREDIT",
                    "DEBIT",
                    "MEAL_VOUCHER",
                    "FOOD_VOUCHER",
                    "GIFT_CARD",
                    "DIGITAL_WALLET",
                    "PIX",
                    "BANK_DRAFT",
                    "OTHER",
                }
            },
        };
    }

    public class IFoodPaymentMatchConfig
    {
        private string _na => IFoodPaymentMatchTranslation.Na;
        private string _any = IFoodPaymentMatchTranslation.Any;

        private Dictionary<string, string> _types = IFoodPaymentMatchTranslation.GetTypes();
        private Dictionary<string, string> _methods = IFoodPaymentMatchTranslation.GetMethods();
        private Dictionary<string, Dictionary<string, HashSet<string>>> _matches = IFoodPaymentMatchTranslation.GetMatches().ToDictionary(t => t.Key, x => x.Value.ToDictionary(m => m, y => new HashSet<string>()));

        public IFoodPaymentMatchConfig(List<IFoodPaymentMatch> paymentsMatch)
        {
            foreach (var payment in paymentsMatch)
            {
                if (_matches.ContainsKey(payment.IFoodPaymentType))
                {
                    //# make matches 'any' exlusive
                    //if (string.Empty.Equals(payment.IFoodPaymentMethod))
                    //{
                    //    foreach (var brands in _matches[payment.IFoodPaymentType].Values)
                    //        brands.Add(string.Empty);
                    //}
                    //else 
                    if (_matches[payment.IFoodPaymentType].ContainsKey(payment.IFoodPaymentMethod)
                        && !_matches[payment.IFoodPaymentType][payment.IFoodPaymentMethod].Contains(payment.IFoodPaymentBrand)
                    )
                        _matches[payment.IFoodPaymentType][payment.IFoodPaymentMethod].Add(payment.IFoodPaymentBrand);
                }
            }
        }

        public string[] GetTypes(out int first)
        {
            first = 0;
            return _types.Values.ToArray();
        }
        public string[] GetMethods(string typeValue, out int first)
        {
            if (TypeExists(typeValue, out string typeKey))
            {
                var list = _matches[typeKey].Keys.Where(x => !string.IsNullOrEmpty(x)).Select(y => _methods[y]).ToList();
                list.Insert(0, _any);
                first = list.Count > 1 ? 1 : 0;
                return list.ToArray();
            }
            else
            {
                first = -1;
                return new string[] { _na };
            }
        }
        public string[] GetBrands(string typeValue, string methodValue, out int first)
        {
            if (!TypeExists(typeValue, out string typeKey) || !MethodExists(methodValue, out string methodKey) || "CASH".Equals(methodKey))
            {
                first = -1;
                return new string[] { _na };
            }
            else
            {
                var list = _matches[typeKey][methodKey].Where(x => !string.IsNullOrEmpty(x)).ToList();
                list.Insert(0, _any);
                first = list.Count > 1 ? 1 : 0;
                return list.ToArray();
            }
        }

        public bool ExistMatch(string typeValue, string methodValue, string brandValue, out string typeKey, out string methodKey, out string brandKey)
        {
            if (TypeExists(typeValue, out typeKey))
            {
                if (_any.Equals(methodValue))
                {
                    methodKey = string.Empty;
                    brandKey = null;
                    //# make matches 'any' exlusive
                    return _matches[typeKey][methodKey].Any();
                    //return _matches[typeKey].Any(x => x.Value.Count > 0);
                }
                else if (MethodExists(methodValue, out methodKey))
                {
                    brandKey = _na.Equals(brandValue) ? null : (_any.Equals(brandValue) ? string.Empty : brandValue.ToUpper());
                    //# make matches 'any' exlusive
                    return _matches[typeKey][methodKey].Contains(brandKey);
                    //return _matches[typeKey][methodKey].Contains(string.Empty) || _matches[typeKey][methodKey].Contains(brandKey) || (string.Empty.Equals(brandKey) && _matches[typeKey][methodKey].Count > 0);
                }
            }

            // not suposed to get here
            methodKey = null;
            brandKey = null;
            return true;
        }

        private bool TypeExists(string typeValue, out string typeKey)
        {
            var matchType = _types.FirstOrDefault(x => x.Value.Equals(typeValue));
            if (default(KeyValuePair<string, string>).Equals(matchType))
            {
                typeKey = null;
                return false;
            }
            else
            {
                typeKey = matchType.Key;
                return true;
            }
        }
        private bool MethodExists(string methodValue, out string methodKey)
        {
            var matchMethod = _methods.FirstOrDefault(x => x.Value.Equals(methodValue));
            if (default(KeyValuePair<string, string>).Equals(matchMethod))
            {
                methodKey = null;
                return false;
            }
            else
            {
                methodKey = matchMethod.Key;
                return true;
            }
        }
    }
}
