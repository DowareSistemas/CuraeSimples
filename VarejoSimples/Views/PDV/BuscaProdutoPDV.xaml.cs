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

namespace VarejoSimples.Views.PDV
{
    /// <summary>
    /// Lógica interna para BuscaProdutoPDV.xaml
    /// </summary>
    public partial class BuscaProdutoPDV : Window
    {
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

            dataGrid.ItemsSource = list;
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

        private void dataGrid_KeyDown(object sender, KeyEventArgs e)
        {
   
        }

        private void dataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            Estoque estoque = (Estoque)dataGrid.SelectedItem;
            if (estoque == null)
                return;

            if (estoque.Id == 0)
                return;

            if (e.Key == Key.Insert)
                MonitorInsereRemove.Instance.AcionarInsercao(estoque);

            if (e.Key == Key.Delete)
                MonitorInsereRemove.Instance.AcionarRemocao(estoque);
        }

        private void btInserir_Click(object sender, RoutedEventArgs e)
        {
            Estoque estoque = (Estoque)dataGrid.SelectedItem;
            if (estoque == null)
                return;

            if (estoque.Id == 0)
                return;

            MonitorInsereRemove.Instance.AcionarInsercao(estoque);
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

    public class MonitorInsereRemove
    {
        public delegate void InsereItem(Estoque estoque);
        public event InsereItem ItemInserido;

        public delegate void RemoveItem(Estoque estoque);
        public event RemoveItem ItemRemovido;

        public void AcionarInsercao(Estoque estoque)
        {
            if (ItemInserido != null)
                ItemInserido(estoque);
        }

        public void AcionarRemocao(Estoque estoque)
        {
            if (ItemRemovido != null)
                ItemRemovido(estoque);
        }

        private static MonitorInsereRemove instance = null;

        private MonitorInsereRemove()
        {

        }

        public static MonitorInsereRemove Instance
        {
            get
            {
                if (instance == null)
                    instance = new MonitorInsereRemove();

                return instance;
            }
        }
    }
}
