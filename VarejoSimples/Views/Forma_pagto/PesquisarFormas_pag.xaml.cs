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

namespace VarejoSimples.Views.Forma_pagto
{
    /// <summary>
    /// Lógica interna para PesquisarFormas_pag.xaml
    /// </summary>
    public partial class PesquisarFormas_pag : Window
    {
        public Formas_pagamento Selecionado = new Formas_pagamento();
        public PesquisarFormas_pag()
        {
            InitializeComponent();
            txPesquisa.Focus();
            dataGrid.AplicarPadroes();
            Pesquisar();
        }

        private void Pesquisar()
        {
            List<Formas_pagamento> list = new Formas_pagamentoController().Search(txPesquisa.Text);
            dataGrid.ItemsSource = list;
        }

        private void txPesquisa_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Pesquisar();
        }

        private void btSelecionar_Click(object sender, RoutedEventArgs e)
        {
            Selecionar();
        }

        private void Selecionar()
        {
            Formas_pagamento f = (Formas_pagamento)dataGrid.SelectedItem;
            if (f == null)
                return;
            if (f.Id == 0)
                return;

            Selecionado = f;
            Close();
        }

        private void btCancelar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void dataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Selecionar();
        }
    }
}
