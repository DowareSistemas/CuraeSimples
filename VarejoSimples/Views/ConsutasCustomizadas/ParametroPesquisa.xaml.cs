using SQL2Search.Model;
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

namespace VarejoSimples.Views.ConsutasCustomizadas
{
    /// <summary>
    /// Interação lógica para ParametroPesquisa.xam
    /// </summary>
    public partial class ParametroPesquisa : UserControl
    {
        public string Nome { get; set; }
        public string Valor
        {
            get
            {
                return textBox.Text;
            }
        }

        public ParametroPesquisa(SQLField field)
        {
            InitializeComponent();

            label.Content = field.Description;
            Nome = field.Name;

            if (field.Type == "numeric")
                textBox.ToNumeric();

            if (field.Type == "money")
                textBox.ToMoney();
        }
    }
}
