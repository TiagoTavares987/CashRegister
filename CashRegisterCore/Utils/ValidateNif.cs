using CashRegisterCore.Enumerators;
using System;

namespace CashRegisterCore.Utils
{
    internal static class Validate
    {
        public static bool Nif(Country country, string nif) 
        { 
            switch (country)
            {
                case Country.PT:
                    return ValidatePT(nif);

                default: 
                    return true;
            }
        }

        private static bool ValidatePT(string nif)
        {
            int tempNif = 0;
            int checkDigit = 0;
            decimal tempDiv = 0m;
            int tempDivInt = 0;

            try
            {
                if (string.IsNullOrWhiteSpace(nif))
                    return false;

                nif = nif.Trim();

                if (nif.Length > 0)
                {
                    char c = Convert.ToChar(nif.Substring(0, 1));
                    if (!char.IsNumber(c))
                    {
                        if (nif.ToUpper().StartsWith("PT"))
                        {
                            nif = nif.ToUpper();
                            nif.Replace("PT", "");
                            nif.Replace(" ", "");
                            nif.Replace("-", "");
                            nif.Replace("_", "");
                        }
                        else
                            return false;
                    }
                }
                else
                    return false;


                if (nif.Length != 9)
                    return false;

                if (!int.TryParse(nif, out tempNif))
                    return false;

                if (tempNif < 10000000 || tempNif > 999999999)
                    return false;

                if (!nif.StartsWith("1") && !nif.StartsWith("2") && !nif.StartsWith("3") && !nif.StartsWith("5") &&
                    !nif.StartsWith("6") && !nif.StartsWith("7") && !nif.StartsWith("8") && !nif.StartsWith("9"))
                    return false;

                for (int i = 1; i < 9; i++)
                {
                    checkDigit += Convert.ToInt32(nif.Substring(i - 1, 1)) * (10 - i);
                }

                tempDiv = checkDigit / 11;
                tempDivInt = Convert.ToInt32(tempDiv) * 11;
                checkDigit -= tempDivInt;
                if (checkDigit == 0 || checkDigit == 1)
                    checkDigit = 0;
                else
                    checkDigit = 11 - checkDigit;

                if (checkDigit.ToString() == nif.Substring(8, 1))
                    return true;
            }
            catch (Exception)
            {
            }

            return false;
        }
    }
}
