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

namespace VarejoSimples.Views.Tamanho
{
    /// <summary>
    /// Lógica interna para CadTamanhos.xaml
    /// </summary>
    public partial class CadTamanhos : Window
    {
        private TamanhosController controller = null;
        public CadTamanhos()
        {
            InitializeComponent();

            controller = new TamanhosController();
            txDescricao.Focus();
        }

        private void FillTam(Tamanhos tamanho)
        {
            if (tamanho == null)
                return;

            txCodigo.Text = tamanho.Id.ToString();
            txDescricao.Text = tamanho.Descricao;
        }

        private void next_Click(object sender, RoutedEventArgs e)
        {
            int id = int.Parse(txCodigo.Text);
            Tamanhos tam = controller.Next(id);

            FillTam(tam);
        }

        private void prev_Click(object sender, RoutedEventArgs e)
        {
            int id = int.Parse(txCodigo.Text);
            if ((id - 1) <= 0)
            {
                LimparCampos();
                return;
            }

            Tamanhos tam = controller.Prev(id);
            FillTam(tam);
        }

        private void LimparCampos()
        {
            txCodigo.Text = "0";
            txDescricao.Text = string.Empty;
            txDescricao.Focus();
        }

        private void btNovo_Click(object sender, RoutedEventArgs e)
        {
            LimparCampos();
        }

        private void Salvar()
        {
            Tamanhos tam = (int.Parse(txCodigo.Text) == 0
                ? new Tamanhos()
                : controller.Find(int.Parse(txCodigo.Text)));

            tam.Descricao = txDescricao.Text;

            if (controller.Save(tam))
                LimparCampos();
        }

        private void btSalvar_Click(object sender, RoutedEventArgs e)
        {
            Salvar();
        }

        private void btExcluir_Click(object sender, RoutedEventArgs e)
        {
            int id = int.Parse(txCodigo.Text);
            if (id == 0)
                return;

            if (controller.Remove(id))
                LimparCampos();
        }

        private void btCancelar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F3)
            {
                PesquisarTamanho pt = new PesquisarTamanho(); //PT, credo ×
                pt.ShowDialog();

                if (pt.Selecionado.Id > 0)
                    FillTam(pt.Selecionado);
            }
        }
    }
}
