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
using VarejoSimples.Enums;
using VarejoSimples.Model;
using VarejoSimples.Views.Forma_pagto;
using VarejoSimples.Views.Usuario;

namespace VarejoSimples.Views.PDV
{
    /// <summary>
    /// Interaction logic for EntradaValorCaixa.xaml
    /// </summary>
    public partial class EntradaValorCaixa : Window
    {
        private Tipo_movimentacao_caixa Tipo_movimentacao { get; set; }
        Formas_pagamentoController formaPagController = null;
        public EntradaValorCaixa(Tipo_movimentacao_caixa tipo)
        {
            InitializeComponent();

            txValor_movimentacao.ToMoney();
            txCod_usuario.Text = UsuariosController.UsuarioAtual.Id.ToString();
            txNome_usuario.Text = UsuariosController.UsuarioAtual.Nome;

            formaPagController = new Formas_pagamentoController();

            int tpDinheiro = (int)Tipo_pagamento.DINHEIRO;
            Formas_pagamento formaPg = formaPagController.Get(e => e.Tipo_pagamento == tpDinheiro);
            txCod_forma_pagamento.Text = formaPg.Id.ToString();
            txDescricao_forma_pagamento.Text = formaPg.Descricao;

            txDescricao_movimento.Focus();
            Tipo_movimentacao = tipo;

            if (tipo == Tipo_movimentacao_caixa.ENTRADA)
                lbTitulo.Content = "Entrada de valor no caixa";

            if (tipo == Tipo_movimentacao_caixa.SAIDA)
                lbTitulo.Content = "Retirada de valor no caixa";
        }

        private void btFechar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btConfirmar_Click(object sender, RoutedEventArgs e)
        {
            Confirmar();
        }

        private void btCancelar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();

            if (e.Key == Key.F1)
                Confirmar();
        }

        private void Confirmar()
        {
            Movimentos_caixasController controller = new Movimentos_caixasController();

            if (controller.MovimentarCaixa(
                Tipo_movimentacao,
                decimal.Parse(txValor_movimentacao.Text),
                int.Parse(txCod_usuario.Text),
                int.Parse(txCod_forma_pagamento.Text),
                0,
                txDescricao_movimento.Text))
            {
                Close();
            }
        }

        private void btSelecionar_usuario_Click(object sender, RoutedEventArgs e)
        {
            PesquisarUsuario pu = new PesquisarUsuario();
            pu.ShowDialog();

            if (pu.Selecionado.Id > 0)
            {
                if (pu.Selecionado.Id == int.Parse(txCod_usuario.Text))
                    return;

                DigitarSenhaUsuario dsu = new DigitarSenhaUsuario(pu.Selecionado);
                dsu.ShowDialog();

                if (!dsu.Cancelado)
                {
                    if (!dsu.Autenticado)
                    {
                        MessageBox.Show("Não autorizado", "Autenticação", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    txCod_usuario.Text = pu.Selecionado.Id.ToString();
                    txNome_usuario.Text = pu.Selecionado.Nome;
                    txValor_movimentacao.Focus();
                }
            }
        }

        private void btSelecionar_forma_pagamento_Click(object sender, RoutedEventArgs e)
        {
            PesquisarFormas_pag pfg = new PesquisarFormas_pag();
            pfg.ShowDialog();

            if (pfg.Selecionado.Id > 0)
            {
                if (pfg.Selecionado.Tipo_pagamento != (int)Tipo_pagamento.DINHEIRO)
                {
                    MessageBox.Show("A condição de pagamento para movimentações de caixa deve ser do tipo 'DINHEIRO'", "Condição de pagamento inválida", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                txCod_forma_pagamento.Text = pfg.Selecionado.Id.ToString();
                txDescricao_forma_pagamento.Text = pfg.Selecionado.Descricao;
                txValor_movimentacao.Focus();
            }
        }

        private void txValor_movimentacao_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Confirmar();
        }
    }
}
