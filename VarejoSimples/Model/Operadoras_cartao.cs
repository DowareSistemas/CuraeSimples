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
    
    public partial class Operadoras_cartao
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public bool Inativo { get; set; }
        public int Tipo_recebimento { get; set; }
        public int Prazo_recebimento { get; set; }
        public decimal Taxa { get; set; }
    }
}
