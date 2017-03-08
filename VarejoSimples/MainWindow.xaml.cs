using DarumaFramework_NFCe;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Transactions;
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
using VarejoSimples.Interfaces;
using VarejoSimples.Model;
using VarejoSimples.Views;
using VarejoSimples.Views.Consultas;
using VarejoSimples.Views.ConsutasCustomizadas;
using VarejoSimples.Views.Movimento.LancamentoCheque;
using VarejoSimples.Views.Movimento.RecebimentoCheques;
using VarejoSimples.Views.VendaRapida;

namespace VarejoSimples
{
    /// <summary>
    /// Interação lógica para MainWindow.xam
    /// </summary>
    public partial class MainWindow : Window
    {
        private void NFCe()
        {
            Declaracoes.regAlterarValor_NFCe_Daruma("CONFIGURACAO\\EmpPK", "0oz/7sntevE3BkNUMV+GJA==");

            Declaracoes.aCFAbrir_NFCe_Daruma("17132107704", "Emerson T Almeida", "Rua Aimores", "15", "Retiro", "3306305",
                "Volta Redonda", "RJ", "27275350");

            Declaracoes.aCFVender_NFCe_Daruma("F1", "1,00", "5,89", "D%", "0,00", "123456789", "UN", "Suco de Laranja");
            Declaracoes.aCFTotalizar_NFCe_Daruma("D%", "0,00");
            Declaracoes.aCFEfetuarPagamento_NFCe_Daruma("Dinheiro", "5,89");
            int retorno = Declaracoes.tCFEncerrar_NFCe_Daruma("NFC-e emitida via Curae ERP - Mini");

            MessageBox.Show(Declaracoes.TrataRetorno(retorno));
        }

        public delegate void Complete();
        public event Complete OnComplete;

        public MainWindow(Iniciando ini)
        { 
            if (!Directory.Exists(@"C:\Temp\Curae"))
                Directory.CreateDirectory(@"C:\Temp\Curae");

            foreach (string filename in Directory.GetFiles(@"C:\Temp\Curae\"))
                File.Delete(filename);

            InitializeComponent();
            dataGrid.AplicarPadroes();
            BStatus.Attach(1, lbStatus, image);

            txCod_rotina.ToNumeric();
            txCod_rotina.Focus();
            this.Title = "Curae - Mini ERP";

            Login l = new Login(ini);
            l.Show();

            l.EfetuouLogin += L_EfetuouLogin;
        }

        private void L_EfetuouLogin()
        {
            listView.SelectedIndex = 0;
            txNomeLoja.Text = (UsuariosController.LojaAtual.Nome_fantasia + $" ({UsuariosController.LojaAtual.Razao_social})");
            txUsuario.Text = UsuariosController.UsuarioAtual.Nome;

            Parametros p = ParametrosController.FindParametro("CONS_CUST", true);
            if (p == null)
                listView.Items.Remove(mi_consultasCustomizadas);
            else
               if (p.Valor.Equals("N"))
                listView.Items.Remove(mi_consultasCustomizadas);

            //  this.Show();
            Venda venda = new Venda();
            venda.ShowDialog();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void listView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                int menu = int.Parse((listView.SelectedItem as ListViewItem).Name.Replace("menu_", ""));
                List<Rotinas> list = new RotinasController().ListByMenu(menu);
                dataGrid.ItemsSource = list;
            }
            catch
            {
                Parametros p = ParametrosController.FindParametro("CSQL_PATH");

                List<Rotinas> r = new List<Rotinas>();
                DirectoryInfo di = new DirectoryInfo(p.Valor);
                foreach (FileInfo file in di.GetFiles())
                {
                    if (!file.Name.EndsWith(".csql"))
                        continue;
                    r.Add(new Rotinas()
                    {
                        Id = 600,
                        Menu = 600,
                        Descricao = file.Name.Replace(".csql", "")
                    });
                }

                dataGrid.ItemsSource = r;
            }
        }

        private void dataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Rotinas r = (Rotinas)dataGrid.SelectedItem;
                if(r.Id == 600)
                {
                    CarregaConsultaCustomizada(r.Descricao);
                    return;
                }

                RotinasController rc = new RotinasController();
                rc.ShowWindow(r.Id);
            }
            catch
            {

            }
        }

        private void CarregaConsultaCustomizada(string nome)
        {
            Parametros p = ParametrosController.FindParametro("CSQL_PATH");

            SQL2Search.Compiler.SQLEntityDecompiler decompiler = new SQL2Search.Compiler.SQLEntityDecompiler();
            SQL2Search.Model.SQLEntity sqlEntity = decompiler.Decompile(p.Valor + nome + ".csql");

            if(sqlEntity.Provider == null)
            {
                AlertaDesconhecido ad = new AlertaDesconhecido(nome, (p.Valor + nome + ".csql"), sqlEntity.CreationTime);
                ad.ShowDialog();

                if (!ad.Permitido)
                    return;
            }

            ConsultaCustomizada cc = new ConsultaCustomizada();
            cc.SetSqlEntity(sqlEntity);
            cc.Start();
        }

        private void txCod_rotina_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                int id_rotina = 0;
                if (int.TryParse(txCod_rotina.Text, out id_rotina))
                {
                    RotinasController rc = new RotinasController();
                    rc.ShowWindow(id_rotina);
                }
                txCod_rotina.Text = string.Empty;
            }
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F3)
                txCod_rotina.Focus();
        }

        private void TreeViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            SaldosFisicosFinan sff = new SaldosFisicosFinan();
            sff.ShowDialog();
        }

        private void btSaldoLote_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            SaldosLotes sl = new SaldosLotes();
            sl.ShowDialog();
        }

        private void btValidade_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ProdutosVencendo pv = new ProdutosVencendo();
            pv.ShowDialog();
        }

        private void mi_consultasCustomizadas_MouseDown(object sender, MouseButtonEventArgs e)
        {
        }
    }
}
