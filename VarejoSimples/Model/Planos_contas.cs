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
    
    public partial class Planos_contas
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Planos_contas()
        {
            this.Tipos_movimento = new HashSet<Tipos_movimento>();
            this.Movimentos = new HashSet<Movimentos>();
            this.Movimentos_contas = new HashSet<Movimentos_contas>();
        }
    
        public int Id { get; set; }
        public string Descricao { get; set; }
        public int Classe { get; set; }
        public int Tipo { get; set; }
        public int Conta_pai { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Tipos_movimento> Tipos_movimento { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Movimentos> Movimentos { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Movimentos_contas> Movimentos_contas { get; set; }
    }
}
