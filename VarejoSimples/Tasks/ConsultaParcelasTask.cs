using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VarejoSimples.Controller;
using VarejoSimples.Enums;
using VarejoSimples.Model;
using VarejoSimples.Views.Parcela;

namespace VarejoSimples.Tasks
{
    public class ConsultaParcelasTask : AsyncTask<object[], int, List<ParcelaAdapter>>
    {
        private ConsultaParcelas View { get; set; }
        public ConsultaParcelasTask(ConsultaParcelas view)
        {
            View = view;
        }

        public override List<ParcelaAdapter> DoInBackGround(object[] param)
        {
            ParcelasController controller = new ParcelasController();

            List<Parcelas> list = controller.BuscaBasica((Tipo_parcela)param[0], (int)param[1], (int)param[2], (int)param[3]);
            List<ParcelaAdapter> listAdp = new List<ParcelaAdapter>();
            list.ForEach(e => listAdp.Add(new ParcelaAdapter(e, controller.GetContext())));

            return listAdp;
        }

        public override void OnPostExecute(List<ParcelaAdapter> result)
        {
            View.GridNavegacao.IsEnabled = true;
            View.dataGrid.ItemsSource = result;
            View.imgLoading.Visibility = System.Windows.Visibility.Hidden;
        }

        public override void OnPreExecute()
        {
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
