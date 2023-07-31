using System;

namespace ThermalPrinter.Utils
{
    public static class Barcode
    {
        public static bool VerifyForPrint(string toVerify, out string toPrint)
        {
            toPrint = toVerify;
            return true;
        }

        public static bool Verify(BarcodeType barcodeType, string code)
        {
            if (string.IsNullOrEmpty(code))
                return false;

            switch (barcodeType)
            {
                case BarcodeType.EAN_13:
                    if (code.Length != 13)
                        return false;
                    break;
                case BarcodeType.EAN_8:
                    if (code.Length != 8)
                        return false;
                    break;
            }

            return code.Substring(code.Length - 1, 1) == CalculateCheckDigit(code.Substring(0, code.Length - 1));
        }

        public static string CalculateCheckDigit(string partialCode)
        {
            string sTemp = partialCode;
            int iSum = 0;
            int iDigit = 0;

            int oddFactor = 1;
            int evenFactor = 1;

            if (partialCode.Length % 2 == 0)
            {
                oddFactor = 3;
            }
            else
            {
                evenFactor = 3;
            }

            // Calculate the checksum digit here.
            for (int i = 0; i < partialCode.Length; i++)
            {
                if (char.IsDigit(sTemp[i]))
                {
                    iDigit = Convert.ToInt32(sTemp.Substring(i, 1));

                    if (i % 2 == 0)
                    { // even  
                        iSum += iDigit * evenFactor;
                    }
                    else
                    { // odd
                        iSum += iDigit * oddFactor;
                    }
                }
                else
                {
                    return string.Empty;
                }
            }

            int iCheckSum = 0;

            iCheckSum = 10 - (iSum % 10);
            if (iCheckSum == 10)
                iCheckSum = 0;

            return iCheckSum.ToString();
        }
    }
}
