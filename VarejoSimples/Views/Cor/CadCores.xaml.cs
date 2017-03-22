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

namespace VarejoSimples.Views.Cor
{
    /// <summary>
    /// Lógica interna para CadCores.xaml
    /// </summary>
    public partial class CadCores : Window
    {
        private CoresController controller = null;
        public CadCores()
        {
            InitializeComponent();

            controller = new CoresController();
            txDescricao.Focus();
        }

        private void FillCor(Cores cor)
        {
            if (cor == null)
                return;

            txCodigo.Text = cor.Id.ToString();
            txDescricao.Text = cor.Descricao.ToString();
        }

        private void next_Click(object sender, RoutedEventArgs e)
        {
            Cores cor = controller.Next(int.Parse(txCodigo.Text));
            FillCor(cor);
        }

        private void prev_Click(object sender, RoutedEventArgs e)
        {
            int id = int.Parse(txCodigo.Text);
            if ((id - 1) <= 0)
            {
                LimparCampos();
                return;
            }

            Cores cor = controller.Prev(id);
            FillCor(cor);
        }

        private void LimparCampos()
        {
            txCodigo.Text = "0";
            txDescricao.Text = string.Empty;
            txDescricao.Focus();
        }

        private void Salvar()
        {
            Cores cor = (int.Parse(txCodigo.Text) == 0
                ? new Cores()
                : controller.Find(int.Parse(txCodigo.Text)));

            cor.Descricao = txDescricao.Text;

            if (controller.Save(cor))
                LimparCampos();
        }

        private void btNovo_Click(object sender, RoutedEventArgs e)
        {
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
                PesquisarCor pc = new PesquisarCor();
                pc.ShowDialog();

                if(pc.Selecionado.Id > 0)
                FillCor(pc.Selecionado);
            }
        }
    }
}
