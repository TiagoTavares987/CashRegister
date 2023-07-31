using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XDPeople.Entities.Brazil.IFood
{
    public static class IFoodCancellation
    {
        public static Dictionary<string, string> GetCancellationCodes() => new Dictionary<string, string>()
        {
            { "501", "PROBLEMAS DE SISTEMA" },
            { "502", "PEDIDO EM DUPLICIDADE" },
            { "503", "ITEM INDISPONÍVEL" },
            { "504", "RESTAURANTE SEM MOTOBOY" },
            { "505", "CARDÁPIO DESATUALIZADO" },
            { "506", "PEDIDO FORA DA ÁREA DE ENTREGA" },
            { "507", "CLIENTE GOLPISTA / TROTE" },
            { "508", "FORA DO HORÁRIO DO DELIVERY" },
            { "509", "DIFICULDADES INTERNAS DO RESTAURANTE" },
            { "511", "ÁREA DE RISCO" },
            { "512", "RESTAURANTE ABRIRÁ MAIS TARDE" },
            { "513", "RESTAURANTE FECHOU MAIS CEDO" },
            { "801", "FALHA GERAL" },
        };
    }
}
