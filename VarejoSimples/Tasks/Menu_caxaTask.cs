using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VarejoSimples.Controller;
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

            Caixas caixaAtual = new CaixasController().Find(controller.Get_ID_CaixaAtualUsuario());

            return new object[] { list, caixaAtual.Nome, UsuariosController.UsuarioAtual.Nome };
        }

        public override void OnPostExecute(object[] result)
        {
            View.dataGrid.ItemsSource = (List<Movimentos_caixas>)result[0];
            View.lbNomeCaixa.Content = result[1].ToString();
            View.lbNomeUsuario.Content = result[2].ToString();

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
