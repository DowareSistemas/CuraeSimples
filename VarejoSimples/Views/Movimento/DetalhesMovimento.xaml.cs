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
using VarejoSimples.Enums;
using VarejoSimples.Model;

namespace VarejoSimples.Views.Movimento
{
    /// <summary>
    /// Lógica interna para DetalhesMovimento.xaml
    /// </summary>
    public partial class DetalhesMovimento : Window
    {
        MovimentosController controller;
        public DetalhesMovimento(int movimento_id)
        {
            InitializeComponent();

            dataGrid_produtos.AplicarPadroes();
            dataGrid_produtos.FontSize = 15;
            dataGrid_produtos.MinRowHeight = 20;
            dataGrid_produtos.AlternatingRowBackground = Brushes.Lavender;

            dataGrid_pagamentos.AplicarPadroes();
            dataGrid_pagamentos.FontSize = 15;
            dataGrid_pagamentos.MinRowHeight = 20;
            dataGrid_pagamentos.AlternatingRowBackground = Brushes.Lavender;

            dataGrid_parcelas.AplicarPadroes();
            dataGrid_parcelas.FontSize = 15;
            dataGrid_parcelas.MinRowHeight = 20;
            dataGrid_parcelas.AlternatingRowBackground = Brushes.Lavender;

            controller = new MovimentosController();
            LoadMov(movimento_id);
        }

        private void LoadMov(int movimento_id)
        {
            varejo_config context = new varejo_config();

            Movimentos movimento = controller.Find(movimento_id);

            txCod.Text = movimento.Id.ToString();
            txTipo_mov.Text = movimento.Tipos_movimento.Descricao;
            txCliente.Text = (movimento.Cliente_id == 0
                ? string.Empty
                : (from cliente in context.Clientes
                   where cliente.Id == movimento.Cliente_id
                   select cliente.Nome).SingleOrDefault());
            txFornecedor.Text = (movimento.Fornecedor_id == 0
                ? string.Empty
                : (from fornecedor in context.Fornecedores
                   where fornecedor.Id == movimento.Fornecedor_id
                   select fornecedor.Nome).SingleOrDefault());
            txData_mov.Text = movimento.Data.ToString("dd/MM/yyyy HH:mm:ss");
            txUsuario.Text = movimento.Usuarios.Nome;

            Caixas cx = (from caixa in context.Caixas
                            join mov_caixa in context.Movimentos_caixas on caixa.Id equals mov_caixa.Caixa_id
                            where mov_caixa.Movimento_id == movimento.Id

                            select caixa).FirstOrDefault();

            txCaixa.Text = (cx == null
                ? string.Empty
                : cx.Nome);

            dataGrid_produtos.ItemsSource = movimento.Itens_movimento;
            dataGrid_pagamentos.ItemsSource = movimento.Itens_pagamento;

            List<ParcelaAdapter> listAdp = new List<ParcelaAdapter>();
            List<Parcelas> parcelas = new ParcelasController().ListByItens_pagamento(movimento.Itens_pagamento.ToList());
            parcelas.ForEach(e => listAdp.Add(new ParcelaAdapter(e, context)));

            dataGrid_parcelas.ItemsSource = listAdp;
        }
    }

    public class ParcelaAdapter
    {
        public int Id { get; set; }
        public string Portador { get; set; }
        public string Num_documento { get; set; }
        public BitmapImage ImgStatus { get; set; }
        public string Forma_pagamento { get; set; }
        public string Tipo { get; set; }
        public decimal Valor { get; set; }
        public DateTime Data_vencimento { get; set; }

        public ParcelaAdapter(Parcelas parcela, varejo_config context)
        {
            Id = parcela.Id;
            Num_documento = parcela.Num_documento;
            Data_vencimento = parcela.Data_vencimento;

            if(parcela.Portador > 0)
            {
                Contas conta = context.Contas.Find(parcela.Portador);
                Portador = conta.Nome;
            }

            switch (parcela.Situacao)
            {
                case (int)Situacao_parcela.EM_ABERTO:
                    ImgStatus = new BitmapImage(new Uri(System.IO.Directory.GetCurrentDirectory() + System.IO.Path.DirectorySeparatorChar + "/Images/verde.png"));
                    break;

                case (int)Situacao_parcela.PAGA:
                    ImgStatus = new BitmapImage(new Uri(System.IO.Directory.GetCurrentDirectory() + System.IO.Path.DirectorySeparatorChar + "/Images/vermelho.png"));
                    break;

                case (int)Situacao_parcela.CANCELADA:
                    ImgStatus = new BitmapImage(new Uri(System.IO.Directory.GetCurrentDirectory() + System.IO.Path.DirectorySeparatorChar + "/Images/cinza.png"));
                    break;

                case (int)Situacao_parcela.RENEGOCIADA:
                    ImgStatus = new BitmapImage(new Uri(System.IO.Directory.GetCurrentDirectory() + System.IO.Path.DirectorySeparatorChar + "/Images/amarelo.png"));
                    break;
            }

            Valor = parcela.Valor;
            Forma_pagamento = (from fpg in context.Formas_pagamento
                               join itens_pg in context.Itens_pagamento on fpg.Id equals itens_pg.Forma_pagamento_id
                               join parc in context.Parcelas on itens_pg.Id equals parc.Item_pagamento_id
                               where parc.Id == parcela.Id

                               select fpg.Descricao).SingleOrDefault();

            switch (parcela.Tipo_parcela)
            {
                case (int)Tipo_parcela.PAGAR:
                    Tipo = "PAGAR";
                    break;

                case (int)Tipo_parcela.RECEBER:
                    Tipo = "RECEBER";
                    break;
            }
        }
    }
}
