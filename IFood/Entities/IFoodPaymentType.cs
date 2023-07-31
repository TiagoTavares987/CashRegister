using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XDPeople.Entities
{
    public class IFoodPaymentType
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public static List<IFoodPaymentType> IFoodPayments
        {
            get => new List<IFoodPaymentType>()
            {
                new IFoodPaymentType(){ Id = "IFOOD_ONLINE", Description = "iFood Online", Name = "IFOOD ONLINE" },
                new IFoodPaymentType(){ Id = "ALA", Description = "CARTÃO REFEIÇÃO ALELO", Name = "FOOD_VOUCHER ONLINE" },
                new IFoodPaymentType(){ Id = "ALR", Description = "VALE - ALELO REFEIÇÃO / VISA VALE (CARTÃO)", Name = "MEAL_VOUCHER ONLINE" },
                new IFoodPaymentType(){ Id = "AM", Description = "AMEX", Name = "CREDIT ONLINE" },
                new IFoodPaymentType(){ Id = "APL_MC", Description = "Apple Pay Master", Name = "DIGITAL_WALLET ONLINE" },
                new IFoodPaymentType(){ Id = "APL_VIS", Description = "Apple Pay Visa", Name = "DIGITAL_WALLET ONLINE" },
                new IFoodPaymentType(){ Id = "BANRC", Description = "CRÉDITO - BANRICOMPRAS (MÁQUINA)", Name = "CREDIT OFFLINE" },
                new IFoodPaymentType(){ Id = "BANRD", Description = "DÉBITO - BANRICOMPRAS (MÁQUINA)", Name = "DEBIT OFFLINE" },
                new IFoodPaymentType(){ Id = "BENVVR", Description = "BEN VISA REFEIÇÃO", Name = "MEAL_VOUCHER OFFLINE" },
                new IFoodPaymentType(){ Id = "BON", Description = "DESCONTO", Name = "CASH OFFLINE" },
                new IFoodPaymentType(){ Id = "CARNET", Description = "CARNET", Name = "CREDIT ONLINE" },
                new IFoodPaymentType(){ Id = "CHE", Description = "CHEQUE", Name = "BANK_DRAFT OFFLINE" },
                new IFoodPaymentType(){ Id = "CHF", Description = "CHEF CARD", Name = "CREDIT ONLINE" },
                new IFoodPaymentType(){ Id = "CPRCAR", Description = "VALE - COOPER CARD (CARTÃO)", Name = "MEAL_VOUCHER OFFLINE" },
                new IFoodPaymentType(){ Id = "CRE", Description = "CREDITO", Name = "CREDIT OFFLINE" },
                new IFoodPaymentType(){ Id = "DIN", Description = "DINHEIRO", Name = "CASH OFFLINE" },
                new IFoodPaymentType(){ Id = "DNR", Description = "DINERS", Name = "CREDIT ONLINE" },
                new IFoodPaymentType(){ Id = "DNREST", Description = "CRÉDITO - DINERS (MÁQUINA)", Name = "CREDIT OFFLINE" },
                new IFoodPaymentType(){ Id = "ELO", Description = "ELO", Name = "CREDIT ONLINE" },
                new IFoodPaymentType(){ Id = "ELOD", Description = "ELO DÉBITO", Name = "DEBIT ONLINE" },
                new IFoodPaymentType(){ Id = "GER_CC", Description = "GER_CC CRÉDITO (MÁQUINA)", Name = "CREDIT OFFLINE" },
                new IFoodPaymentType(){ Id = "GER_CT", Description = "TERMINAL BANCARIA - PAGO CONTRA ENTREGA", Name = "DEBIT OFFLINE" },
                new IFoodPaymentType(){ Id = "GER_DC", Description = "DÉBITO (MÁQUINA)", Name = "DEBIT OFFLINE" },
                new IFoodPaymentType(){ Id = "GOODC", Description = "CRÉDITO - GOODCARD (MÁQUINA)", Name = "CREDIT OFFLINE" },
                new IFoodPaymentType(){ Id = "GPY_ELO", Description = "Google Pay Elo", Name = "DIGITAL_WALLET ONLINE" },
                new IFoodPaymentType(){ Id = "GPY_MC", Description = "Google Pay Master", Name = "DIGITAL_WALLET ONLINE" },
                new IFoodPaymentType(){ Id = "GPY_MXMC", Description = "Google Pay Master", Name = "DIGITAL_WALLET ONLINE" },
                new IFoodPaymentType(){ Id = "GPY_MXVIS", Description = "Google Pay Visa", Name = "DIGITAL_WALLET ONLINE" },
                new IFoodPaymentType(){ Id = "GPY_VIS", Description = "Google Pay Visa", Name = "DIGITAL_WALLET ONLINE" },
                new IFoodPaymentType(){ Id = "GRNCAR", Description = "VALE - GREEN CARD (CARTÃO)", Name = "MEAL_VOUCHER OFFLINE" },
                new IFoodPaymentType(){ Id = "GRNCPL", Description = "VALE - GREEN CARD (PAPEL)", Name = "MEAL_VOUCHER OFFLINE" },
                new IFoodPaymentType(){ Id = "HIPER", Description = "HIPERCARD ONLINE", Name = "CREDIT ONLINE" },
                new IFoodPaymentType(){ Id = "IFE", Description = "IFOOD CORP", Name = "DIGITAL_WALLET ONLINE" },
                new IFoodPaymentType(){ Id = "LPCLUB", Description = "LOOP CLUB", Name = "DIGITAL_WALLET ONLINE" },
                new IFoodPaymentType(){ Id = "MC", Description = "MASTERCARD", Name = "CREDIT ONLINE" },
                new IFoodPaymentType(){ Id = "MCMA", Description = "MASTERCARD MAESTRO", Name = "DEBIT ONLINE" },
                new IFoodPaymentType(){ Id = "MEREST", Description = "DÉBITO - MASTERCARD (MÁQUINA)", Name = "DEBIT OFFLINE" },
                new IFoodPaymentType(){ Id = "MOVPAY_AM", Description = "Movile Pay Amex", Name = "DIGITAL_WALLET ONLINE" },
                new IFoodPaymentType(){ Id = "MOVPAY_DNR", Description = "Movile Pay Diners", Name = "DIGITAL_WALLET ONLINE" },
                new IFoodPaymentType(){ Id = "MOVPAY_ELO", Description = "Movile Pay Elo", Name = "DIGITAL_WALLET ONLINE" },
                new IFoodPaymentType(){ Id = "MOVPAY_HIPER", Description = "Movile Pay Hipercard", Name = "DIGITAL_WALLET ONLINE" },
                new IFoodPaymentType(){ Id = "MOVPAY_MC", Description = "Movile Pay Master", Name = "DIGITAL_WALLET ONLINE" },
                new IFoodPaymentType(){ Id = "MOVPAY_VIS", Description = "Movile Pay Visa", Name = "DIGITAL_WALLET ONLINE" },
                new IFoodPaymentType(){ Id = "MPAY", Description = "Movile Pay", Name = "DIGITAL_WALLET ONLINE" },
                new IFoodPaymentType(){ Id = "MXAM", Description = "American Express", Name = "CREDIT ONLINE" },
                new IFoodPaymentType(){ Id = "MXMC", Description = "MASTERCARD", Name = "CREDIT ONLINE" },
                new IFoodPaymentType(){ Id = "MXVIS", Description = "VISA", Name = "DEBIT ONLINE" },
                new IFoodPaymentType(){ Id = "NUGO", Description = "CRÉDITO - NUGO (MÁQUINA)", Name = "CREDIT OFFLINE" },
                new IFoodPaymentType(){ Id = "NUTCRD", Description = "NUTRICARD REFEICÃO E ALIMENTAÇÃO", Name = "MEAL_VOUCHER OFFLINE" },
                new IFoodPaymentType(){ Id = "PAY", Description = "Paypal", Name = "DIGITAL_WALLET ONLINE" },
                new IFoodPaymentType(){ Id = "PSE", Description = "PSE", Name = "DIGITAL_WALLET ONLINE" },
                new IFoodPaymentType(){ Id = "QRC", Description = "QR Code", Name = "QR_CODE OFFLINE" },
                new IFoodPaymentType(){ Id = "RAM", Description = "CRÉDITO - AMERICAN EXPRESS (MÁQUINA)", Name = "CREDIT OFFLINE" },
                new IFoodPaymentType(){ Id = "RDREST", Description = "CRÉDITO - MASTERCARD (MÁQUINA)", Name = "CREDIT OFFLINE" },
                new IFoodPaymentType(){ Id = "REC", Description = "CRÉDITO - ELO (MÁQUINA)", Name = "CREDIT OFFLINE" },
                new IFoodPaymentType(){ Id = "RED", Description = "DÉBITO - ELO (MÁQUINA)", Name = "DEBIT OFFLINE" },
                new IFoodPaymentType(){ Id = "RHIP", Description = "CRÉDITO - HIPERCARD (MÁQUINA)", Name = "CREDIT OFFLINE" },
                new IFoodPaymentType(){ Id = "RSELE", Description = "VALE - REFEISUL (CARTÃO)", Name = "MEAL_VOUCHER OFFLINE" },
                new IFoodPaymentType(){ Id = "RSODEX", Description = "VALE - SODEXO (CARTÃO)", Name = "MEAL_VOUCHER OFFLINE" },
                new IFoodPaymentType(){ Id = "SAP", Description = "Sodexo Alimentacao", Name = "FOOD_VOUCHER ONLINE" },
                new IFoodPaymentType(){ Id = "SRP", Description = "CARTÃO REFEIÇÃO SODEXO", Name = "MEAL_VOUCHER ONLINE" },
                new IFoodPaymentType(){ Id = "TAO", Description = "Ticket Alimentação ONLINE", Name = "FOOD_VOUCHER ONLINE" },
                new IFoodPaymentType(){ Id = "TOD", Description = "TODITO CASH", Name = "CREDIT ONLINE" },
                new IFoodPaymentType(){ Id = "TRE", Description = "VALE - TICKET RESTAURANTE (CARTÃO)", Name = "MEAL_VOUCHER OFFLINE" },
                new IFoodPaymentType(){ Id = "TRO", Description = "Ticket Restaurante ONLINE", Name = "MEAL_VOUCHER ONLINE" },
                new IFoodPaymentType(){ Id = "TVER", Description = "VALE - VEROCARD (CARTÃO)", Name = "MEAL_VOUCHER OFFLINE" },
                new IFoodPaymentType(){ Id = "VA_OFF", Description = "VA Offline", Name = "FOOD_VOUCHER OFFLINE" },
                new IFoodPaymentType(){ Id = "VA_ON", Description = "VA Online", Name = "FOOD_VOUCHER ONLINE" },
                new IFoodPaymentType(){ Id = "VALECA", Description = "VALE - VALE CARD", Name = "MEAL_VOUCHER OFFLINE" },
                new IFoodPaymentType(){ Id = "VERDEC", Description = "CRÉDITO - VERDECARD (MÁQUINA)", Name = "CREDIT OFFLINE" },
                new IFoodPaymentType(){ Id = "VIREST", Description = "DÉBITO - VISA", Name = "DEBIT OFFLINE" },
                new IFoodPaymentType(){ Id = "VIS", Description = "VISA", Name = "CREDIT ONLINE" },
                new IFoodPaymentType(){ Id = "VISAVR", Description = "VISA REFEIÇÃO", Name = "MEAL_VOUCHER OFFLINE" },
                new IFoodPaymentType(){ Id = "VISE", Description = "VISA ELECTRON", Name = "DEBIT ONLINE" },
                new IFoodPaymentType(){ Id = "VOUCHER", Description = "VOUCHER", Name = "VOUCHER" },
                new IFoodPaymentType(){ Id = "VR_SMA", Description = "VALE - VR SMART (CARTÃO)", Name = "MEAL_VOUCHER OFFLINE" },
                new IFoodPaymentType(){ Id = "VRO", Description = "VR ONLINE", Name = "MEAL_VOUCHER ONLINE" },
                new IFoodPaymentType(){ Id = "VSREST", Description = "CRÉDITO - VISA", Name = "CREDIT OFFLINE" },
                new IFoodPaymentType(){ Id = "VVREST", Description = "VALE - ALELO REFEIÇÃO / VISA VALE (CARTÃO)", Name = "MEAL_VOUCHER OFFLINE" }
            };
        }
    }
}
