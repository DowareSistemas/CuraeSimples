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
using System.Windows.Navigation;
using System.Windows.Shapes;
using VarejoSimples.Controller;
using VarejoSimples.Tasks;

namespace VarejoSimples.Views.PDV.MenuPDV
{
    /// <summary>
    /// Interação lógica para Menu_Caixa.xam
    /// </summary>
    public partial class Menu_Caixa : UserControl
    {
        Movimentos_caixasController controller = null;
        public Menu_Caixa()
        {
            InitializeComponent();
            controller = new Movimentos_caixasController();

            dataGrid.AplicarPadroes();

            Menu_CaixaTask task = new Menu_CaixaTask(this);
            task.Execute(0);
        }
    }
}
