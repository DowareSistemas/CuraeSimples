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
    /// Interação lógica para Introducao.xam
    /// </summary>
    public partial class Introducao : UserControl, ITelaTransferenciaConta
    {
        List<Requisito> requisitos = new List<Requisito>();

        public Introducao()
        {
            InitializeComponent();
            
            CheckRequisitos();
        }

        public bool RequisitosAtendidos
        {
            get
            {
                return requisitos.Count(e => e.Atendido) == requisitos.Count;
            }
        }

        private void CheckRequisitos()
        {
            Formas_pagamentoController fpg = new Formas_pagamentoController();
            if (fpg.Get(e => e.Tipo_pagamento == (int)Tipo_pagamento.DINHEIRO) != null)
                requisitos.Add(new Requisito(true, "Possuir uma condição de pagamento do tipo DINHEIRO"));
            else
                requisitos.Add(new Requisito(false, "Possuir uma condição de pagamento do tipo DINHEIRO"));

            ContasController contas = new ContasController();
            if (contas.Count(e => e.Inativa == false) > 1)
                requisitos.Add(new Requisito(true, "Possuir mais de uma conta cadastrada"));
            else
                requisitos.Add(new Requisito(false, "Possuir mais de uma conta cadastrada"));

            if (contas.Count(e => e.Saldo > 0) > 0)
                requisitos.Add(new Requisito(true, "Possuir uma conta com saldo positivo"));
            else
                requisitos.Add(new Requisito(false, "Possuir uma conta com saldo positivo"));


            dataGrid.ItemsSource = requisitos;
            dataGrid.AplicarPadroes();
            dataGrid.FontSize = 13;
            dataGrid.MinRowHeight = 15;
        }

        public int Conta_id
        {
            get
            {
                return 0;
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
                return 0;
            }
        }

        public Tipo_lancamento Tipo_lancamento
        {
            get
            {
                return Tipo_lancamento.ENTRADA;
            }
        }

        public decimal Valor
        {
            get
            {
                return 0;
            }
            set
            {

            }
        }
    }

    public class Requisito
    {
        public BitmapImage Img { get; set; }
        public string Descricao { get; set; }
        public bool Atendido { get; set; }

        public Requisito(bool atendido, string descricao)
        {
            Atendido = atendido;

            if (atendido)
                Img = new BitmapImage(new Uri(System.IO.Directory.GetCurrentDirectory() + System.IO.Path.DirectorySeparatorChar + "/Images/aprovado.png"));
            else
                Img = new BitmapImage(new Uri(System.IO.Directory.GetCurrentDirectory() + System.IO.Path.DirectorySeparatorChar + "/Images/reprovado.png"));

            Descricao = descricao;
        }
    }
}
