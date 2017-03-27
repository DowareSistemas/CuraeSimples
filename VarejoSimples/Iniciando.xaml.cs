using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace VarejoSimples
{
    /// <summary>
    /// Lógica interna para Iniciando.xaml
    /// </summary>
    public partial class Iniciando : Window
    {
        public Iniciando()
        {
            InitializeComponent();
            
        }
        private bool feito = false;

        private void Start()
        {
            if (feito)
                return;

            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += (o, ea) =>
            {
                
                //long-running-process code
                System.Threading.Thread.Sleep(500);

                this.Dispatcher.BeginInvoke((Action)(() =>
                {
                    new MainWindow(this);
                }));
            };

            worker.RunWorkerCompleted += (o, ea) =>
            {
              //  this.Close();
            };
            worker.RunWorkerAsync();

            feito = true;
        }

        private void M_OnComplete()
        {
            this.Close();
        }

        public bool EnabledClose = false;

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (!EnabledClose)
                e.Cancel = true;
        }

        private void fadeCompleted(object sender, EventArgs e)
        {
            Start();
        }
    }
}
