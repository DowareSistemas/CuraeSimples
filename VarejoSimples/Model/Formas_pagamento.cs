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
    
    public partial class Formas_pagamento
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Formas_pagamento()
        {
            this.Itens_pagamento = new HashSet<Itens_pagamento>();
            this.Movimentos_caixas = new HashSet<Movimentos_caixas>();
        }
    
        public int Id { get; set; }
        public string Descricao { get; set; }
        public int Tipo_pagamento { get; set; }
        public int Tipo_intervalo { get; set; }
        public int Intervalo { get; set; }
        public int Dia_base { get; set; }
        public int Parcelas { get; set; }
        public int Operadora_cartao_id { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Itens_pagamento> Itens_pagamento { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Movimentos_caixas> Movimentos_caixas { get; set; }
    }
}
