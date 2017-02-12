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

namespace VarejoSimples.Views.Caixa
{
    /// <summary>
    /// Lógica interna para CadCaixas.xaml
    /// </summary>
    public partial class CadCaixas : Window
    {
        CaixasController contoller;
        public CadCaixas()
        {
            InitializeComponent();
            txCod.ToNumeric();
            txNome.Focus();
            this.contoller = new CaixasController();
        }

        private void FillCaixa(Caixas caixa)
        {
            if (caixa == null)
                return;

            txCod.Text = caixa.Id.ToString();
            txNome.Text = caixa.Nome;
        }

        private void LimparCampos()
        {
            txCod.Text = "0";
            txNome.Text = string.Empty;
            txNome.Focus();
        }

        private void Salvar()
        {
            Caixas c = (int.Parse(txCod.Text) == 0
                ? new Caixas()
                : contoller.Find(int.Parse(txCod.Text)));

            c.Id = int.Parse(txCod.Text);
            c.Nome = txNome.Text;

            if (contoller.Save(c))
                LimparCampos();
        }

        private void next_Click(object sender, RoutedEventArgs e)
        {
            Caixas c = contoller.Next(int.Parse(txCod.Text));
            FillCaixa(c);
        }

        private void prev_Click(object sender, RoutedEventArgs e)
        {
            if ((int.Parse(txCod.Text) - 1) <= 0)
            {
                LimparCampos();
                return;
            }

            Caixas c = this.contoller.Prev(int.Parse(txCod.Text));
            FillCaixa(c);
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
            if (this.contoller.Remove(int.Parse(txCod.Text)))
                LimparCampos();
        }

        private void btCancelar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
