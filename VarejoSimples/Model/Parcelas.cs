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
    
    public partial class Parcelas
    {
        public int Id { get; set; }
        public int Item_pagamento_id { get; set; }
        public string Num_documento { get; set; }
        public int Tipo_parcela { get; set; }
        public int Portador { get; set; }
        public int Cliente_id { get; set; }
        public int Fornecedor_id { get; set; }
        public decimal Valor { get; set; }
        public System.DateTime Data_lancamento { get; set; }
        public System.DateTime Data_vencimento { get; set; }
        public string Parcela_descricao { get; set; }
        public int Dias_tolerancia { get; set; }
        public decimal Juros_atraso { get; set; }
        public int Situacao { get; set; }
        public int Parcela_anterior { get; set; }
        public string Numero_cheque { get; set; }
        public string Banco { get; set; }
        public string Agencia { get; set; }
        public string Conta { get; set; }
        public int Dias_compensacao { get; set; }
        public int Pagamento_lancamento_id { get; set; }
    }
}