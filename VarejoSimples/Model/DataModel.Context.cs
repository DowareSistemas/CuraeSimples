﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class varejo_config : DbContext
    {
        public varejo_config()
            : base("name=varejo_config")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Caixas> Caixas { get; set; }
        public virtual DbSet<Clientes> Clientes { get; set; }
        public virtual DbSet<Documentos_fiscais> Documentos_fiscais { get; set; }
        public virtual DbSet<Estoque> Estoque { get; set; }
        public virtual DbSet<Fabricantes> Fabricantes { get; set; }
        public virtual DbSet<Formas_pagamento> Formas_pagamento { get; set; }
        public virtual DbSet<Fornecedores> Fornecedores { get; set; }
        public virtual DbSet<Itens_movimento> Itens_movimento { get; set; }
        public virtual DbSet<Itens_pagamento> Itens_pagamento { get; set; }
        public virtual DbSet<Lojas> Lojas { get; set; }
        public virtual DbSet<Marcas> Marcas { get; set; }
        public virtual DbSet<Movimentos> Movimentos { get; set; }
        public virtual DbSet<Movimentos_caixas> Movimentos_caixas { get; set; }
        public virtual DbSet<Operadoras_cartao> Operadoras_cartao { get; set; }
        public virtual DbSet<Parametros> Parametros { get; set; }
        public virtual DbSet<Parcelas> Parcelas { get; set; }
        public virtual DbSet<Permissoes> Permissoes { get; set; }
        public virtual DbSet<Planos_contas> Planos_contas { get; set; }
        public virtual DbSet<Produtos> Produtos { get; set; }
        public virtual DbSet<Produtos_fornecedores> Produtos_fornecedores { get; set; }
        public virtual DbSet<Rotinas> Rotinas { get; set; }
        public virtual DbSet<Tipos_movimento> Tipos_movimento { get; set; }
        public virtual DbSet<Unidades> Unidades { get; set; }
        public virtual DbSet<Usuarios> Usuarios { get; set; }
        public virtual DbSet<Vendedores> Vendedores { get; set; }
        public virtual DbSet<Contas> Contas { get; set; }
        public virtual DbSet<Movimentos_contas> Movimentos_contas { get; set; }
    }
}
