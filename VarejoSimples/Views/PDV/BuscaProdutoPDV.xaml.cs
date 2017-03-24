using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using VarejoSimples.Controller;
using VarejoSimples.Model;
using VarejoSimples.Views.PDV.EventMonitors;

namespace VarejoSimples.Views.PDV
{
    /// <summary>
    /// Lógica interna para BuscaProdutoPDV.xaml
    /// </summary>
    public partial class BuscaProdutoPDV : Window
    {
        List<Estoque> ListEstoque = null;
        public BuscaProdutoPDV()
        {
            InitializeComponent();

            dataGrid.AplicarPadroes();
            txDescricao.Focus();
            Pesquisar();
        }

        private void Pesquisar()
        {
            EstoqueController ec = new EstoqueController();
            List<Estoque> list = ec.ListarEstoqueProdutos(txDescricao.Text, txMarca.Text, txFabricante.Text);

            List<EstoquePdvAdapter> adapters = new List<EstoquePdvAdapter>();
            list.ForEach(e => adapters.Add(new EstoquePdvAdapter(e)));

            dataGrid.ItemsSource = adapters.OrderBy(e => e.Estoque.Produto_id);
        }

        private void btFechar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();

            if (e.Key == Key.F5)
            {
                Pesquisar();

                if (dataGrid.Items.Count > 0)
                {
                    HabilitarCampos(false);

                    dataGrid.Focus();
                    dataGrid.SelectedIndex = 0;
                }
            }

            if (e.Key == Key.F1)
            {
                HabilitarCampos(true);
                txDescricao.Focus();
            }

            if (e.Key == Key.F2)
            {
                HabilitarCampos(true);
                txMarca.Focus();
            }

            if (e.Key == Key.F3)
            {
                HabilitarCampos(true);
                txFabricante.Focus();
            }
        }

        private void HabilitarCampos(bool enabled)
        {
            txDescricao.IsEnabled = enabled;
            txMarca.IsEnabled = enabled;
            txFabricante.IsEnabled = enabled;
        }

        private void btBuscar_Click(object sender, RoutedEventArgs e)
        {
            Pesquisar();
        }

        private void dataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            EstoquePdvAdapter adapter = (EstoquePdvAdapter)dataGrid.SelectedItem;
            if (adapter == null)
                return;

            if (adapter.Estoque.Id == 0)
                return;

            if (e.Key == Key.Insert)
                MonitorInsereRemove.Instance.AcionarInsercao(adapter.Estoque);

            if (e.Key == Key.Delete)
                MonitorInsereRemove.Instance.AcionarRemocao(adapter.Estoque);
        }

        private void btInserir_Click(object sender, RoutedEventArgs e)
        {
            EstoquePdvAdapter adapter = (EstoquePdvAdapter)dataGrid.SelectedItem;
            if (adapter == null)
                return;

            if (adapter.Estoque.Id == 0)
                return;

            MonitorInsereRemove.Instance.AcionarInsercao(adapter.Estoque);
        }

        private void btRemover_Click(object sender, RoutedEventArgs e)
        {
            Estoque estoque = (Estoque)dataGrid.SelectedItem;
            if (estoque == null)
                return;

            if (estoque.Id == 0)
                return;

            MonitorInsereRemove.Instance.AcionarRemocao(estoque);
        }
    }
    
    public class EstoquePdvAdapter
    {
        public Estoque Estoque { get; set; }
        public Grades_produtos Grade { get; set; }

        public EstoquePdvAdapter(Estoque estoque)
        {
            Estoque = estoque;
            if (estoque.Grade_id != null)
                Grade = new Grades_produtosController().Find(estoque.Grade_id);
        }
    }
}
