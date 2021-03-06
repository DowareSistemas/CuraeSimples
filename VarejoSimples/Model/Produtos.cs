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
    
    public partial class Produtos
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Produtos()
        {
            this.Estoque = new HashSet<Estoque>();
            this.Grades_produtos = new HashSet<Grades_produtos>();
            this.Itens_movimento = new HashSet<Itens_movimento>();
            this.Itens_pedido = new HashSet<Itens_pedido>();
            this.Produtos_fornecedores = new HashSet<Produtos_fornecedores>();
        }
    
        public int Id { get; set; }
        public string Descricao { get; set; }
        public string Referencia { get; set; }
        public string Ean { get; set; }
        public string Ncm { get; set; }
        public decimal Valor_unit { get; set; }
        public int Unidade_id { get; set; }
        public decimal Aliquota { get; set; }
        public int Fabricante_id { get; set; }
        public int Marca_id { get; set; }
        public string Localizacao { get; set; }
        public byte[] Foto { get; set; }
        public bool Controla_lote { get; set; }
        public int Grupo_id { get; set; }
        public bool Controla_grade { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Estoque> Estoque { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Grades_produtos> Grades_produtos { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Itens_movimento> Itens_movimento { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Itens_pedido> Itens_pedido { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Produtos_fornecedores> Produtos_fornecedores { get; set; }
        public virtual Unidades Unidades { get; set; }
    }
}
