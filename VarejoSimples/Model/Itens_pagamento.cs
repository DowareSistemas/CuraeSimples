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
    
    public partial class Itens_pagamento
    {
        public int Id { get; set; }
        public int Movimento_id { get; set; }
        public int Forma_pagamento_id { get; set; }
        public decimal Valor { get; set; }
    
        public virtual Formas_pagamento Formas_pagamento { get; set; }
        public virtual Movimentos Movimentos { get; set; }
    }
}
