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
using VarejoSimples.Views.VendaRapida.UCControllers;

namespace VarejoSimples.Views.VendaRapida
{
    /// <summary>
    /// Lógica interna para Venda.xaml
    /// </summary>
    public partial class Venda : Window
    {
        public Venda()
        {
            InitializeComponent();

            GridContainer.Children.Add(new LogoEmpresa());

            AuxiliarSp controllerSp = new AuxiliarSp();
            controllerSp.Initialize(sp_grupos, typeof(CardGrupo), true);

            List<Produtos> list = new ProdutosController().Search("");

            list.ForEach(e => controllerSp.AddObject(e));
            controllerSp.VerifyLastAndComplete();
        }
    }
}
