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
using VarejoSimples.Enums;
using VarejoSimples.Interfaces;

namespace VarejoSimples.Views.Lancamento_financ.Transferencia
{
    /// <summary>
    /// Interação lógica para Confirmar.xam
    /// </summary>
    public partial class Confirmar : UserControl, ITelaTransferenciaConta
    {
        public Confirmar(ITelaTransferenciaConta t1, ITelaTransferenciaConta t2)
        {
            InitializeComponent();

            if (t1.Conta_id == 0)
                return;
            lbConta1.Content = new ContasController().Find(t1.Conta_id).Nome;
            lbInfoConta1.Content = ((t1.Tipo_lancamento == Tipo_lancamento.ENTRADA
                ? "ENTRADA"
                : "SAIDA") + " - " + new Planos_contasController().Find(t1.Plano_conta_id).Descricao);
            lbValorConta1.Content = $"R$ {t1.Valor.ToString("N2")}";

            lbConta2.Content = new ContasController().Find(t2.Conta_id).Nome;
            lbInfoConta2.Content = ((t2.Tipo_lancamento == Tipo_lancamento.ENTRADA
                ? "ENTRADA"
                : "SAIDA") + " - " + new Planos_contasController().Find(t2.Plano_conta_id).Descricao);
            lbValorConta2.Content = $"R$ {t2.Valor.ToString("N2")}";
        }

        public int Conta_id
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public UserControl CurrentView
        {
            get
            {
                return this;
            }
        }

        public int Plano_conta_id
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public Tipo_lancamento Tipo_lancamento
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public decimal Valor
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {

            }
        }
    }
}
