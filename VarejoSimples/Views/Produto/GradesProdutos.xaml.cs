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

namespace VarejoSimples.Views.Produto
{
    /// <summary>
    /// Lógica interna para GradesProdutos.xaml
    /// </summary>
    public partial class GradesProdutos : Window
    {
        Grades_produtosController controller = null;
        private Produtos Produto { get; set; }
        public GradesProdutos(Produtos produto)
        {
            InitializeComponent();

            txProduto.Text = produto.Descricao;
            Produto = produto;
            controller = new Grades_produtosController();
            dataGrid.AplicarPadroes();
            ListarGrades();
        }

        private void btIncluir_Click(object sender, RoutedEventArgs e)
        {
            AdicionarItemGrade aig = new AdicionarItemGrade();
            aig.ShowDialog();

            if (aig.Cor == null || aig.Tamanho == null)
                return;

            if (aig.Cor.Id == 0 || aig.Tamanho.Id == 0)
                return;

            Grades_produtos gp = new Grades_produtos();
            gp.Produto_id = Produto.Id;
            gp.Cor_id = aig.Cor.Id;
            gp.Tamanho_id = aig.Tamanho.Id;

            controller.Save(gp);
            ListarGrades();
        }

        private void ListarGrades()
        {
            List<Grades_produtos> list = controller.ListByProduto(Produto.Id);
            dataGrid.ItemsSource = list;
        }

        private void btRemover_Click(object sender, RoutedEventArgs e)
        {
            Grades_produtos grade = (Grades_produtos)dataGrid.SelectedItem;
            if (grade == null)
                return;
            
            if (controller.RemoveGradeCompleto(grade.Identificador))
            {
                controller = new Grades_produtosController();
                ListarGrades();
            }
        }

        private void btFechar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
