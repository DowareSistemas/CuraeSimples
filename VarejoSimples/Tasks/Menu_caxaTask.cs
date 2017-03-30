using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VarejoSimples.Controller;
using VarejoSimples.Enums;
using VarejoSimples.Model;
using VarejoSimples.Views.PDV.MenuPDV;

namespace VarejoSimples.Tasks
{
    public class Menu_CaixaTask : AsyncTask<int, int, object[]>
    {
        private Menu_Caixa View { get; set; }
        public Menu_CaixaTask(Menu_Caixa mc)
        {
            View = mc;
        }

        public override object[] DoInBackGround(int param)
        {
            Movimentos_caixasController controller = new Movimentos_caixasController();
            List<Movimentos_caixas> list = controller.GetMovimentosCaixaAtual();

            foreach (Movimentos_caixas mov in list)
                if (mov.Tipo_mov == (int)Tipo_movimentacao_caixa.SAIDA || mov.Tipo_mov == (int)Tipo_movimentacao_caixa.TROCO || mov.Tipo_mov == (int)Tipo_movimentacao_caixa.FECHAMENTO)
                    mov.Valor = mov.Valor;

            decimal valorAbertura = controller.GetUltimoMovimentoAbertura().Valor;
            decimal totalSaidas = controller.GetTotalMovimentacoesCaixaAtual(Tipo_movimentacao_caixa.SAIDA);
            decimal totalEntradas = controller.GetTotalMovimentacoesCaixaAtual(Tipo_movimentacao_caixa.ENTRADA);
            decimal totalCaixa = controller.GetTotalCaixa();

            Caixas caixaAtual = new CaixasController().Find(controller.Get_ID_CaixaAtualUsuario());

            return new object[]
            {
                list,
                caixaAtual.Nome,
                UsuariosController.UsuarioAtual.Nome,
                valorAbertura,
                totalEntradas,
                totalSaidas * (-1),
                totalCaixa
            };
        }

        public override void OnPostExecute(object[] result)
        {
            View.dataGrid.ItemsSource = (List<Movimentos_caixas>)result[0];
            View.lbNomeCaixa.Content = result[1].ToString();
            View.lbNomeUsuario.Content = result[2].ToString();
            View.lbValor_abertura.Content = decimal.Parse(result[3].ToString()).ToString("N2");
            View.lbTotal_entradas.Content = decimal.Parse(result[4].ToString()).ToString("N2");
            View.lbTotal_saidas.Content = decimal.Parse(result[5].ToString()).ToString("N2");
            View.lbTotal_caixa.Content = decimal.Parse(result[6].ToString()).ToString("N2");

            View.imgLoading.Visibility = System.Windows.Visibility.Hidden;
            View.dataGrid.Visibility = System.Windows.Visibility.Visible;
            View.btEntrada.Visibility = System.Windows.Visibility.Visible;
            View.btFecharCaixa.Visibility = System.Windows.Visibility.Visible;
            View.btRetirada.Visibility = System.Windows.Visibility.Visible;
        }

        public override void OnPreExecute()
        {
            View.imgLoading.Visibility = System.Windows.Visibility.Visible;
            View.dataGrid.Visibility = System.Windows.Visibility.Hidden;
            View.btEntrada.Visibility = System.Windows.Visibility.Hidden;
            View.btFecharCaixa.Visibility = System.Windows.Visibility.Hidden;
            View.btRetirada.Visibility = System.Windows.Visibility.Hidden;
            View.lbNomeCaixa.Content = string.Empty;
            View.lbNomeUsuario.Content = string.Empty;
        }

        public override void OnProgressUpdate(int progress)
        {
            throw new NotImplementedException();
        }
    }
}
