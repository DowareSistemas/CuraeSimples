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

namespace VarejoSimples.Views.PDV
{
    /// <summary>
    /// Lógica interna para NFCe_Log.xaml
    /// </summary>
    public partial class NFCe_Log : Window
    {
        List<OperacaoEmissao> Operacoes { get; set; }

        public NFCe_Log()
        {
            InitializeComponent();

            Operacoes = new List<OperacaoEmissao>();

            Operacoes.Add(new OperacaoEmissao(1, "Abrindo nota..."));
            Operacoes.Add(new OperacaoEmissao(2, "Adicionando itens..."));
            Operacoes.Add(new OperacaoEmissao(3, "Totalizando..."));
            Operacoes.Add(new OperacaoEmissao(4, "Efetuando pagamento..."));
            Operacoes.Add(new OperacaoEmissao(5, "Encerrando..."));

            dataGrid.AplicarPadroes();
            dataGrid.ItemsSource = Operacoes;
        }


    }

    public class OperacaoEmissao
    {
        public int Id { get; set; }
        public int Status { get; set; }
        public BitmapImage Img { get; set; }
        public string Descricao { get; set; }

        public OperacaoEmissao(int id, string descricao)
        {
            Id = id;
            Descricao = descricao;
            Img = new BitmapImage(new Uri(System.IO.Directory.GetCurrentDirectory() + System.IO.Path.DirectorySeparatorChar + "/Images/questao.png"));
        }
    }
}
