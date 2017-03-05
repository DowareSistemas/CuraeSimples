using System;
using System.Collections.Generic;
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
using VarejoSimples.Controller;
using VarejoSimples.Enums;
using VarejoSimples.Interfaces;
using VarejoSimples.Model;

namespace VarejoSimples.Views.Lancamento_financ.Transferencia
{
    /// <summary>
    /// Lógica interna para Transferencia.xaml
    /// </summary>
    public partial class Transferencia : Window
    {
        int selectedIndex = 0;
        List<ITelaTransferenciaConta> telas = new List<ITelaTransferenciaConta>();

        Thread thread_transferencia;

        public Transferencia()
        {
            InitializeComponent();

            telas.Add(new Introducao());
            telas.Add(new ContaOrigem());
            telas.Add(new ContaDestino());
            telas.Add(new Confirmar(telas[1], telas[2]));
            GridContainer.Children.Add(telas[0].CurrentView);

            btAnterior.IsEnabled = (telas[0] as Introducao).RequisitosAtendidos;
            btProximo.IsEnabled = (telas[0] as Introducao).RequisitosAtendidos;
        }

        private void Avancar()
        {
        }

        private void EfetuarTransferencia()
        {
            ITelaTransferenciaConta t1 = telas[1];
            ITelaTransferenciaConta t2 = telas[2];

            progressBar.Dispatcher.Invoke(new Action<ProgressBar>(pb => progressBar.Visibility = Visibility.Visible), progressBar);
          
            Lancamentos_financeirosController controller = new Lancamentos_financeirosController();
            ContasController contasC = new ContasController();
            Formas_pagamento fpg = new Formas_pagamentoController().Get(e => e.Tipo_pagamento == (int)Tipo_pagamento.DINHEIRO);

            Lancamentos_financeiros lancamento1 = new Lancamentos_financeiros();
            Lancamentos_financeiros lancamento2 = new Lancamentos_financeiros();

            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                lancamento1.Data = DateTime.Now;
                lancamento1.Conta_id = t1.Conta_id;
                lancamento1.Plano_conta_id = t1.Plano_conta_id;
                lancamento1.Tipo = (int)t1.Tipo_lancamento;
                lancamento1.Valor_original = t1.Valor;
                lancamento1.Valor_final = t1.Valor;
                lancamento1.Usuario_id = UsuariosController.LojaAtual.Id;
                lancamento1.Num_documento = "";
                lancamento1.Pagamentos_lancamentos.Add(new Pagamentos_lancamentos()
                {
                    Forma_pagamento_id = fpg.Id,
                    Valor = lancamento1.Valor_final
                });
                
                lancamento2.Data = DateTime.Now;
                lancamento2.Conta_id = t2.Conta_id;
                lancamento2.Plano_conta_id = t2.Plano_conta_id;
                lancamento2.Tipo = (int)t2.Tipo_lancamento;
                lancamento2.Valor_original = t2.Valor;
                lancamento2.Valor_final = t2.Valor;
                lancamento2.Num_documento = "";
                lancamento2.Usuario_id = UsuariosController.LojaAtual.Id;
                lancamento2.Pagamentos_lancamentos.Add(new Pagamentos_lancamentos()
                {
                    Forma_pagamento_id = fpg.Id,
                    Valor = lancamento2.Valor_final
                });
                
                lancamento1.Descricao = $"TRANSFERÊNCIA '{contasC.Find(lancamento1.Conta_id).Nome}' -> '{contasC.Find(lancamento2.Conta_id).Nome}' - {lancamento1.Data.ToString("dd/MM/yyyy HH:mm:ss")} ({(lancamento1.Tipo == (int)Tipo_lancamento.ENTRADA ? "ENTRADA" : "SAIDA")})";
                lancamento2.Descricao = $"TRANSFERÊNCIA '{contasC.Find(lancamento1.Conta_id).Nome}' -> '{contasC.Find(lancamento2.Conta_id).Nome}' - {lancamento2.Data.ToString("dd/MM/yyyy HH:mm:ss")} ({(lancamento2.Tipo == (int)Tipo_lancamento.ENTRADA ? "ENTRADA" : "SAIDA")})";

                if (!controller.Save(lancamento1))
                {
                    MessageBox.Show("Ocorreu um problema ao executar o procedimento. Acione o suporte Doware.", "ERRO", MessageBoxButton.OK, MessageBoxImage.Error);
                    thread_transferencia.Abort();
                }

                if (!controller.Save(lancamento2))
                {
                    MessageBox.Show("Ocorreu um problema ao executar o procedimento. Acione o suporte Doware.", "ERRO", MessageBoxButton.OK, MessageBoxImage.Error);
                    thread_transferencia.Abort();
                }

                MessageBox.Show("Transferência efetuada com sucesso", "Concluído", MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
                thread_transferencia.Abort();
            }));
        }

        private void btProximo_Click(object sender, RoutedEventArgs e)
        {
            if (selectedIndex == 3)
            {
                thread_transferencia = new Thread(EfetuarTransferencia);
                thread_transferencia.Start();
                return;
            }

            if (selectedIndex != 0)
            {
                if (telas[selectedIndex].Valor == 0)
                {
                    MessageBox.Show("Informe o valor da movimentação", "Atenção", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                if (telas[selectedIndex].Conta_id == 0)
                {
                    MessageBox.Show("Selecione a conta", "Atenção", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                if (telas[selectedIndex].Plano_conta_id == 0)
                {
                    MessageBox.Show("Seleciona o plano de contas", "Atenção", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                if (telas[1].Conta_id == telas[2].Conta_id)
                {
                    MessageBox.Show("As contas devem ser diferentes", "Atenção", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }


                Contas conta = new ContasController().Find(telas[selectedIndex].Conta_id);
                if (telas[selectedIndex].Tipo_lancamento == Enums.Tipo_lancamento.SAIDA)
                {
                    if (conta.Saldo <= 0)
                    {
                        Parametros p = ParametrosController.FindParametro("TCNT_SLDZRO");
                        if (p.Valor.Equals("N"))
                        {
                            MessageBox.Show(@"Não é possível realizar uma movimentação de saída nesta
conta por que o saldo atual está igual ou inferior a zero, e o sistema
está atualmente configurado para bloquear está ação.", "TCNT_SLDZRO", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                            return;
                        }

                        if (!p.Valor.Equals("S"))
                        {
                            MessageBox.Show(@"Não é possível completar a ação por que o valor informado
no parâmetro de sistema 'TCNT_SLDZRO' não foi reconhecido.", "Erro de configuração", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                    }

                    if ((conta.Saldo - telas[selectedIndex].Valor) <= 0)
                    {
                        Parametros p = ParametrosController.FindParametro("TCNT_RSLDZRO");
                        if (p.Valor.Equals("N"))
                        {
                            MessageBox.Show(@"Não é possível realizar uma movimentação de saída nesta conta 
por que o saldo da conta será igual ou inferior a zero após a movimentação, e o sistema está atualmente 
configurado para bloquear esta ação.", "TCNT_RSLDZRO", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                            return;
                        }

                        if (!p.Valor.Equals("S"))
                        {
                            MessageBox.Show(@"Não é possível completar a ação por que o valor informado
no parâmetro de sistema 'TCNT_RSLDZRO' não foi reconhecido.", "Erro de configuração", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                    }
                }
            }

            if (selectedIndex == 2)
            {
                btProximo.Content = "Concluir";
                telas[3] = new Confirmar(telas[1], telas[2]);
                selectedIndex++;
                GridContainer.Children.Clear();
                GridContainer.Children.Add(telas[selectedIndex].CurrentView);

                return;
            }

            selectedIndex++;

            if (selectedIndex == 2)
                telas[selectedIndex].Valor = telas[1].Valor;

            GridContainer.Children.Clear();
            GridContainer.Children.Add(telas[selectedIndex].CurrentView);
        }

        private void btAnterior_Click(object sender, RoutedEventArgs e)
        {
            if (selectedIndex == 0)
            {

                return;
            }

            btProximo.Content = "Próximo";
            selectedIndex--;
            GridContainer.Children.Clear();
            GridContainer.Children.Add(telas[selectedIndex].CurrentView);
        }

        private void btCancelar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
