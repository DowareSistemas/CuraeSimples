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
    
    public partial class Permissoes
    {
        public int Id { get; set; }
        public int Usuario_id { get; set; }
        public int Rotina_id { get; set; }
        public bool Acesso { get; set; }
        public bool Salvar { get; set; }
        public bool Excluir { get; set; }
    
        public virtual Rotinas Rotinas { get; set; }
        public virtual Usuarios Usuarios { get; set; }
    }
}
