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
    
    public partial class Movimentos_caixas
    {
        public int Id { get; set; }
        public int Caixa_id { get; set; }
        public int Tipo_mov { get; set; }
        public decimal Valor { get; set; }
        public int Forma_pagamento_id { get; set; }
        public int Usuario_id { get; set; }
        public int Loja_id { get; set; }
        public System.DateTime Data { get; set; }
        public int Movimento_id { get; set; }
    
        public virtual Caixas Caixas { get; set; }
        public virtual Formas_pagamento Formas_pagamento { get; set; }
        public virtual Lojas Lojas { get; set; }
        public virtual Usuarios Usuarios { get; set; }
    }
}
