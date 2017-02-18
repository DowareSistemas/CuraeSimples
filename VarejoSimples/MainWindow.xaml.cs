using DarumaFramework_NFCe;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
using VarejoSimples.Model;
using VarejoSimples.Views;

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

        public MainWindow()
        {
         //   NFCe();
            varejo_config db = new varejo_config();
            //  db.Database.Connection.ConnectionString = @"data source=tcp:192.168.0.199,1433;initial catalog=bancoteste;user id=sa;password=81547686;multipleactiveresultsets=True;application name=EntityFramework";
            db.Database.CreateIfNotExists();

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
            Login l = new Login();
            l.ShowDialog();
            listView.SelectedIndex = 0;
            txNomeLoja.Text = (UsuariosController.LojaAtual.Nome_fantasia + $" ({UsuariosController.LojaAtual.Razao_social})");
            txUsuario.Text = UsuariosController.UsuarioAtual.Nome;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void listView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int menu = int.Parse((listView.SelectedItem as ListViewItem).Name.Replace("menu_", ""));
            List<Rotinas> list = new RotinasController().ListByMenu(menu);
            dataGrid.ItemsSource = list;
        }

        private void dataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Rotinas r = (Rotinas)dataGrid.SelectedItem;
                RotinasController rc = new RotinasController();
                rc.ShowWindow(r.Id);
            }
            catch
            {

            }
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
    }
}
