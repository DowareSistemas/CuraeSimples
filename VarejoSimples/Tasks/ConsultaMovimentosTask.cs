using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VarejoSimples.Controller;
using VarejoSimples.Model;
using VarejoSimples.Views.Movimento;

namespace VarejoSimples.Tasks
{
    public class ConsultaMovimentosTask : AsyncTask<object[], int, List<MovimentosAdapter>>
    {
        private ConsultaMovimentos View { get; set; }
        public ConsultaMovimentosTask(ConsultaMovimentos view)
        {
            View = view;
        }

        public override List<MovimentosAdapter> DoInBackGround(object[] param)
        {
            string busca = param[0].ToString();
            DateTime? data_inicio = (DateTime?)param[1];
            DateTime? data_fim = (DateTime?)param[2];
            int pagina_atual = (int)param[3];
            int numero_registros = (int)param[4];

            MovimentosController movController = new MovimentosController();
            List<Movimentos> list = movController.BuscaGenerica(busca, data_inicio, data_fim, pagina_atual, numero_registros);
            List<MovimentosAdapter> adapters = new List<MovimentosAdapter>();
            list.ForEach(e => adapters.Add(new MovimentosAdapter(e, movController.GetContext())));

            return adapters;
        }

        public override void OnPostExecute(List<MovimentosAdapter> result)
        {
            View.txPesquisa.IsEnabled = true;
            View.txPesquisa.Focus();
            View.GridNavegacao.IsEnabled = true;
            View.imgLoading.Visibility = System.Windows.Visibility.Hidden;
            View.dataGrid.ItemsSource = result;
        }

        public override void OnPreExecute()
        {
            View.txData_inicio.Focus();
            View.txPesquisa.IsEnabled = false;
            View.GridNavegacao.IsEnabled = false;
            View.dataGrid.ItemsSource = null;
            View.imgLoading.Visibility = System.Windows.Visibility.Visible;
        }

        public override void OnProgressUpdate(int progress)
        {
            throw new NotImplementedException();
        }
    }
}
