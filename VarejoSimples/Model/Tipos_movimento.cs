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
    
    public partial class Tipos_movimento
    {
        public int Id { get; set; }
        public string Descricao { get; set; }
        public int Movimentacao_itens { get; set; }
        public int Movimentacao_valores { get; set; }
        public bool Gera_comissao { get; set; }
        public int Cfop { get; set; }
        public bool Utiliza_fornecedor { get; set; }
        public bool Transferencia_loja { get; set; }
        public Nullable<int> Plano_conta_id { get; set; }
    
        public virtual Planos_contas Planos_contas { get; set; }
    }
}
