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
    
    public partial class Estoque
    {
        public int Id { get; set; }
        public int Produto_id { get; set; }
        public int Loja_id { get; set; }
        public decimal Quant { get; set; }
        public string Lote { get; set; }
        public string Sublote { get; set; }
        public Nullable<System.DateTime> Data_entrada { get; set; }
        public Nullable<System.DateTime> Data_validade { get; set; }
        public string Grade_id { get; set; }
    
        public virtual Lojas Lojas { get; set; }
        public virtual Produtos Produtos { get; set; }
    }
}
