using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VarejoSimples.Controller;
using VarejoSimples.Model;
using VarejoSimples.Views.Lancamento_financ;

namespace VarejoSimples.Tasks
{
    public class LancamentosFinanceirosTask : AsyncTask<int[], int, object[]>
    {
        private Lancamentos View { get; set; }

        public LancamentosFinanceirosTask(Lancamentos view)
        {
            View = view;
        }

        public override object[] DoInBackGround(int[] param)
        {
            Lancamentos_financeirosController lController = new Lancamentos_financeirosController();
            ContasController cController = new ContasController();

            decimal saldoConta = cController.Find(param[3]).Saldo;

            List<Lancamentos_financeiros> list = lController.BuscaSimples(param[0], param[1], param[2], param[3]);
            List<Lancamentos_financeirosAdapter> listAdp = new List<Lancamentos_financeirosAdapter>();
            list.ForEach(e => listAdp.Add(new Lancamentos_financeirosAdapter(e, lController.GetContext())));

            return new object[] { saldoConta, listAdp };
        }

        public override void OnPostExecute(object[] results)
        {
            decimal saldoConta = (decimal)results[0];
            List<Lancamentos_financeirosAdapter> result = (List<Lancamentos_financeirosAdapter>)results[1];

            View.GridNavegacao.IsEnabled = true;
            View.dataGrid.ItemsSource = result;
            View.lbSaldoConta.Content = saldoConta.ToString("N2");
            View.imgLoading.Visibility = System.Windows.Visibility.Hidden;
        }

        public override void OnProgressUpdate(int progress)
        {
            throw new NotImplementedException();
        }

        public override void OnPreExecute()
        {
            View.GridNavegacao.IsEnabled = false;
            View.dataGrid.ItemsSource = null;
            View.imgLoading.Visibility = System.Windows.Visibility.Visible;
        }
    }
}