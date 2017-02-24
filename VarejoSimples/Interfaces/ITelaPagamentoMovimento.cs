using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VarejoSimples.Model;

namespace VarejoSimples.Interfaces
{
    public interface ITelaPagamentoMovimento
    {
        List<Itens_pagamento> Itens_pagamento { get; set; }
        void Exibir(decimal valor_movimento);
        bool Pago { get; set; }
        decimal Troco { get; set; }
    }
}
