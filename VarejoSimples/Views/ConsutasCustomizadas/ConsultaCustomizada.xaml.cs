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
using SQL2Search.Model;

namespace VarejoSimples.Views.ConsutasCustomizadas
{
    /// <summary>
    /// Lógica interna para ConsultaCustomizada.xaml
    /// </summary>
    public partial class ConsultaCustomizada : Window
    {
        List<ParametroPesquisa> parametros = new List<ParametroPesquisa>();

        private SQLEntity SQLEntity { get; set; }
        public ConsultaCustomizada()
        {
            InitializeComponent();
        }

        public void Start()
        {
            SQLEntity.Fields.ForEach(e => parametros.Add(new ParametroPesquisa(e)));
            parametros.ForEach(e => stackPanel.Children.Add(e));

            this.ShowDialog();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
        }

        internal void SetSqlEntity(SQLEntity sqlEntity)
        {
            SQLEntity = sqlEntity;
        }
    }
}
