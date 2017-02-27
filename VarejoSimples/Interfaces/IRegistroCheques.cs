using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VarejoSimples.Model;

namespace VarejoSimples.Interfaces
{
    public interface IRegistroCheques
    {
        List<Cheque> Cheques { get; set; }

        void Exibir(decimal valor_pagamento);
    }

    public class Cheque
    {
        public string Numero_cheque { get; set; }
        public string Banco { get; set; }
        public string Agencia { get; set; }
        public DateTime Data_deposito { get; set; }
        public int Dias_compensacao { get; set; }
        public decimal Valor { get; set; }
    }
}
