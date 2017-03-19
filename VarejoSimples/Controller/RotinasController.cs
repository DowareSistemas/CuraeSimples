using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VarejoSimples.Model;
using VarejoSimples.Repository;
using VarejoSimples.Views.Caixa;
using VarejoSimples.Views.Cliente;
using VarejoSimples.Views.Configuracao;
using VarejoSimples.Views.Conta;
using VarejoSimples.Views.DocEntrada;
using VarejoSimples.Views.Fabricante;
using VarejoSimples.Views.Forma_pagto;
using VarejoSimples.Views.Fornecedor;
using VarejoSimples.Views.Lancamento_financ;
using VarejoSimples.Views.Loja;
using VarejoSimples.Views.Marca;
using VarejoSimples.Views.Movimento;
using VarejoSimples.Views.Operadora_cartao;
using VarejoSimples.Views.Parcela;
using VarejoSimples.Views.PDV;
using VarejoSimples.Views.Plano_conta;
using VarejoSimples.Views.Produto;
using VarejoSimples.Views.Produto_fornecedor;
using VarejoSimples.Views.Tipo_movimento;
using VarejoSimples.Views.Unidade;
using VarejoSimples.Views.Usuario;
using VarejoSimples.Views.Vendedor;

namespace VarejoSimples.Controller
{
    public class ListaRotinasCache
    {
        private static ListaRotinasCache instance = null;
        
        public static ListaRotinasCache Instance
        {
            get
            {
                if (instance == null)
                    instance = new ListaRotinasCache();

                return instance;
            }
        }

        private List<Rotinas> Rotinas { get; set; }

        public void Add(List<Rotinas> rotinas)
        {
            rotinas.ForEach(e => Rotinas.Add(e));
        }

        public List<Rotinas> Find(Func<Rotinas, bool> query)
        {
            if (Rotinas == null)
                Rotinas = new List<Model.Rotinas>();

            return Rotinas.Where(query).ToList();
        }
    }

    public class RotinasController
    {
        private RotinasRepository db = null;

        public RotinasController()
        {
            db = new RotinasRepository();
        }

        public List<Rotinas> ListByMenu(int menu)
        {
            List<Rotinas> cache = ListaRotinasCache.Instance.Find(e => e.Menu == menu);
            if (cache != null)
                if (cache.Count > 0)
                    return cache;

            cache =  db.Where(e => e.Menu == menu).ToList();
            ListaRotinasCache.Instance.Add(cache);

            return cache;
        }

        public List<Rotinas> ListAll()
        {
            return db.Where(e => e.Id > 0).ToList();
        }

        public void ShowWindow(int rotina_id)
        {
            try
            {
                switch (rotina_id)
                {
                    case 1:
                        CadLoja cl = new CadLoja();
                        cl.ShowDialog();
                        break;

                    case 2:
                        CadastroUsuarios cadUsu = new CadastroUsuarios();
                        cadUsu.ShowDialog();
                        break;

                    case 3:
                        CadPermissoes cp = new CadPermissoes();
                        cp.ShowDialog();
                        break;


                    case 4:
                        CadVendedor vc = new CadVendedor();
                        vc.ShowDialog();
                        break;

                    case 5:
                        CadCaixas cc = new CadCaixas();
                        cc.ShowDialog();
                        break;

                    case 6:
                        CadastroTmv ctmv = new CadastroTmv();
                        ctmv.ShowDialog();
                        break;

                    case 7:
                        CadFabricantes cf = new CadFabricantes();
                        cf.ShowDialog();
                        break;

                    case 8:
                        CadMarcas cm = new CadMarcas();
                        cm.ShowDialog();
                        break;

                    case 9:
                        CadOperadora op = new CadOperadora();
                        op.ShowDialog();
                        break;

                    case 10:
                        CadUnidade cadUn = new CadUnidade();
                        cadUn.ShowDialog();
                        break;

                    case 11:
                        CadProduto cadProd = new CadProduto();
                        cadProd.ShowDialog();
                        break;

                    case 12:
                        CadFornecedor cadFrn = new CadFornecedor();
                        cadFrn.ShowDialog();
                        break;

                    case 13:
                        CadCliente cCli = new CadCliente();
                        cCli.ShowDialog();
                        break;

                    case 14:
                        CadFormas_pag cadFpg = new CadFormas_pag();
                        cadFpg.ShowDialog();
                        break;

                    case 15:
                        LancamentoMovimentos lm = new LancamentoMovimentos();
                        lm.ShowDialog();
                        break;

                    case 16:
                        Produto_fornecedor pf = new Produto_fornecedor();
                        pf.ShowDialog();
                        break;

                    case 29:

                        Parametrizacao param = new Parametrizacao();
                        param.ShowDialog();
                        break;

                    case 30:

                        CadPlano_conta cpc = new CadPlano_conta();
                        cpc.ShowDialog();
                        break;

                    case 31:

                        CadContas cadconta = new CadContas();
                        cadconta.ShowDialog();
                        break;

                    case 32:
                        ConsultaMovimentos consMov = new ConsultaMovimentos();
                        consMov.ShowDialog();
                        break;

                    case 20:
                        ConsultaParcelas cp_cp = new ConsultaParcelas(Enums.Tipo_parcela.PAGAR);
                        cp_cp.ShowDialog();
                        break;

                    case 21:
                        ConsultaParcelas cp_cr = new ConsultaParcelas(Enums.Tipo_parcela.RECEBER);
                        cp_cr.ShowDialog();
                        break;

                    case 33:
                        Lancamentos lancamentos = new Lancamentos();
                        lancamentos.ShowDialog();
                        break;

                    case 34:
                        PDV pdv = new PDV();
                        pdv.ShowDialog();
                        break;

                    case 27:
                        DocumentoEntrada docEnt = new DocumentoEntrada();
                        docEnt.ShowDialog();
                        break;
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
