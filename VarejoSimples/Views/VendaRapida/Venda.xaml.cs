using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using VarejoSimples.Controller;
using VarejoSimples.Model;
using VarejoSimples.Views.VendaRapida.UCControllers;

namespace VarejoSimples.Views.VendaRapida
{
    /// <summary>
    /// Lógica interna para Venda.xaml
    /// </summary>
    public partial class Venda : Window
    {
        bool VendaAberta { get; set; }
        private ItensVenda ItensVenda { get; set; }
        public Venda()
        {
            InitializeComponent();

            GridContainer.Children.Add(new LogoEmpresa());

            /* GRUPOS PRODUTOS */
            AuxiliarSp controllerSp = new AuxiliarSp();
            controllerSp.Begin(sp_grupos, typeof(CardGrupo), true);

            List<Grupos_produtos> list = new Grupos_produtosController().Search("");

            list.ForEach(e => controllerSp.AddObject(e));
            controllerSp.End();
            MonitorSelecaoGrupo.Instance.GrupoSelecionado += Instance_GrupoSelecionado;
            MonitorSelecaoProduto.Instance.ProdutoSelecionado += Instance_ProdutoSelecionado;
            VendaAberta = false;
        }

        private void Instance_GrupoSelecionado(Grupos_produtos grupo)
        {
            sp_produtos.Children.Clear();

            AuxiliarSp controllerSp = new AuxiliarSp();
            controllerSp.Begin(sp_produtos, typeof(CardProdutos), false);

            foreach (Produtos produto in grupo.Produtos)
                controllerSp.AddObject(produto);

            controllerSp.End();
        }

        private void Instance_ProdutoSelecionado(Produtos produto)
        {
            if(!VendaAberta)
            {
                GridContainer.Children.Clear();
                ItensVenda = new ItensVenda();
                GridContainer.Children.Add(ItensVenda);

                VendaAberta = true;
            }

            ItensVenda.AdicionaItem(produto, 1);
        }

        private void btFecharVenda_Click(object sender, RoutedEventArgs e)
        {
            GridContainer.Children.Clear();
            GridContainer.Children.Add(new LogoEmpresa());
            VendaAberta = false;
        }
    }
}
