//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace VarejoSimples.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class Movimentos_contas
    {
        public int Id { get; set; }
        public int Conta_id { get; set; }
        public System.DateTime Data { get; set; }
        public int Tipo { get; set; }
        public int Forma_pagamento_id { get; set; }
        public int Plano_conta_id { get; set; }
        public int Origem { get; set; }
        public int Movimento_caixa_id { get; set; }
        public string Num_documento { get; set; }
        public decimal Valor_original { get; set; }
        public decimal Desconto { get; set; }
        public decimal Acrescimo { get; set; }
        public decimal Valor_final { get; set; }
    
        public virtual Contas Contas { get; set; }
        public virtual Formas_pagamento Formas_pagamento { get; set; }
        public virtual Planos_contas Planos_contas { get; set; }
    }
}
