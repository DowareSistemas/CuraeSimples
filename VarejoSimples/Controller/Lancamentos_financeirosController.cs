using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Windows;
using VarejoSimples.Enums;
using VarejoSimples.Interfaces;
using VarejoSimples.Model;
using VarejoSimples.Repository;
using VarejoSimples.Views.Movimento.LancamentoCheque;
using VarejoSimples.Views.Movimento.RecebimentoCheques;

namespace VarejoSimples.Controller
{
    public class Lancamentos_financeirosController
    {
        private Lancamentos_financeirosRepository db;

        public Lancamentos_financeirosController()
        {
            db = new Lancamentos_financeirosRepository();
        }

        public bool Save(Lancamentos_financeiros lancamento_financeiro)
        {
            if (!Valid(lancamento_financeiro))
                return false;

            List<Pagamentos_lancamentos> pagamentos = lancamento_financeiro.Pagamentos_lancamentos.ToList();
            lancamento_financeiro.Pagamentos_lancamentos = null;
            UnitOfWork unit = new UnitOfWork();
            try
            {
                unit.BeginTransaction();
                db.Context = unit.Context;

                lancamento_financeiro.Id = db.NextId(e => e.Id);
                db.Save(lancamento_financeiro);
                db.Commit();

                pagamentos.ForEach(e => e.Lancamento_id = lancamento_financeiro.Id);
                pagamentos.ForEach(e => e.Formas_pagamento = null);

                Pagamentos_lancamentosController pl_controller = new Pagamentos_lancamentosController();
                pl_controller.SetContext(unit.Context);

                Formas_pagamentoController fpg_controller = new Formas_pagamentoController();
                fpg_controller.SetContext(unit.Context);

                ParcelasController parcController = new ParcelasController();
                parcController.SetContext(unit.Context);

                ContasController contasController = new ContasController();
                contasController.SetContext(unit.Context);

                Planos_contasController planoContasController = new Planos_contasController();
                planoContasController.SetContext(unit.Context);

                Contas conta = contasController.Find(lancamento_financeiro.Conta_id);
                Planos_contas plano_conta = planoContasController.Find(lancamento_financeiro.Plano_conta_id);

                int numero_parcela = 1;
                foreach (Pagamentos_lancamentos pagamento in pagamentos)
                {
                    if (!pl_controller.Save(pagamento))
                    {
                        unit.RollBack();
                        return false;
                    }

                    Formas_pagamento forma_pg = fpg_controller.Find(pagamento.Forma_pagamento_id);

                    #region DINHEIRO
                    if (forma_pg.Tipo_pagamento == (int)Tipo_pagamento.DINHEIRO)
                    {
                        if (lancamento_financeiro.Tipo == (int)Tipo_lancamento.ENTRADA)
                            conta.Saldo += pagamento.Valor;
                        else
                            conta.Saldo -= pagamento.Valor;

                        contasController.Save(conta);
                    }
                    #endregion

                    #region CHEQUE
                    if (forma_pg.Tipo_pagamento == (int)Tipo_pagamento.CHEQUE)
                    {
                        IRegistroCheques registroCheques;

                        if (lancamento_financeiro.Tipo == (int)Tipo_lancamento.ENTRADA)
                            registroCheques = new RecebimentoCheques();
                        else
                        {
                            registroCheques = new LancamentoCheque();
                            registroCheques.SetConta(conta);
                        }

                        registroCheques.Exibir(pagamento.Valor);

                        foreach (Cheque cheque in registroCheques.Cheques)
                        {
                            Parcelas parcela_cheque = new Parcelas();

                            parcela_cheque.Pagamento_lancamento_id = pagamento.Id;
                            parcela_cheque.Valor = cheque.Valor;
                            parcela_cheque.Situacao = (int)Situacao_parcela.EM_ABERTO;
                            parcela_cheque.Data_lancamento = lancamento_financeiro.Data;
                            parcela_cheque.Num_documento = lancamento_financeiro.Num_documento.PadLeft(8 - lancamento_financeiro.Num_documento.Length, '0') + "-" + numero_parcela;
                            parcela_cheque.Parcela_descricao = $"REFERENTE AO LANÇAMENTO FINANCEIRO {lancamento_financeiro.Id} ({plano_conta.Descricao})";
                            parcela_cheque.Data_vencimento = cheque.Data_deposito;

                            if (lancamento_financeiro.Tipo == (int)Tipo_lancamento.ENTRADA)
                            {
                                parcela_cheque.Tipo_parcela = (int)Tipo_parcela.RECEBER;
                                parcela_cheque.Cliente_id = lancamento_financeiro.Cliente_id;

                                parcela_cheque.Numero_cheque = cheque.Numero_cheque;
                                parcela_cheque.Banco = cheque.Banco;
                                parcela_cheque.Agencia = cheque.Agencia;
                                parcela_cheque.Dias_compensacao = cheque.Dias_compensacao;
                                parcela_cheque.Conta = cheque.Conta;
                            }

                            if (lancamento_financeiro.Tipo == (int)Tipo_lancamento.SAIDA)
                            {
                                parcela_cheque.Tipo_parcela = (int)Tipo_parcela.PAGAR;
                                parcela_cheque.Fornecedor_id = lancamento_financeiro.Fornecedor_id;

                                parcela_cheque.Portador = forma_pg.Conta_id;
                                parcela_cheque.Numero_cheque = cheque.Numero_cheque;
                                parcela_cheque.Banco = string.Empty;
                                parcela_cheque.Agencia = string.Empty;
                                parcela_cheque.Dias_compensacao = 0;
                                parcela_cheque.Conta = string.Empty;
                            }

                            if (!parcController.Save(parcela_cheque))
                            {
                                unit.RollBack();
                                return false;
                            }

                            numero_parcela++;
                        }

                    }
                    #endregion

                    #region CARTAO
                    if (forma_pg.Tipo_pagamento == (int)Tipo_pagamento.CARTAO)
                    {
                        Operadoras_cartaoController opController = new Operadoras_cartaoController();
                        opController.SetContext(unit.Context);

                        Operadoras_cartao operadora = opController.Find(forma_pg.Operadora_cartao_id);

                        Parcelas parcela_cartao = new Parcelas();
                        parcela_cartao.Pagamento_lancamento_id = pagamento.Id;
                        parcela_cartao.Valor = pagamento.Valor;
                        parcela_cartao.Situacao = (int)Situacao_parcela.EM_ABERTO;
                        parcela_cartao.Data_lancamento = lancamento_financeiro.Data;

                        parcela_cartao.Data_vencimento = (operadora.Tipo_recebimento == (int)Tipo_prazo_operadora.DIAS
                            ? lancamento_financeiro.Data.AddDays(operadora.Prazo_recebimento)
                            : lancamento_financeiro.Data.AddHours(operadora.Prazo_recebimento));
                        parcela_cartao.Portador = forma_pg.Conta_id;

                        if (lancamento_financeiro.Tipo == (int)Tipo_lancamento.ENTRADA)
                        {
                            parcela_cartao.Tipo_parcela = (int)Tipo_parcela.RECEBER;
                            parcela_cartao.Cliente_id = lancamento_financeiro.Cliente_id;
                        }

                        if (lancamento_financeiro.Tipo == (int)Tipo_lancamento.SAIDA)
                        {
                            parcela_cartao.Tipo_parcela = (int)Tipo_parcela.PAGAR;
                            parcela_cartao.Fornecedor_id = lancamento_financeiro.Cliente_id;
                        }

                        parcela_cartao.Num_documento = lancamento_financeiro.Num_documento.PadLeft(8 - lancamento_financeiro.Num_documento.Length, '0') + "-" + numero_parcela;
                        parcela_cartao.Parcela_descricao = $"REFERENTE AO LANÇAMENTO FINANCEIRO {lancamento_financeiro.Id} ({plano_conta.Descricao})";
                        parcela_cartao.Numero_cheque = string.Empty;
                        parcela_cartao.Banco = string.Empty;
                        parcela_cartao.Agencia = string.Empty;
                        parcela_cartao.Dias_compensacao = 0;
                        parcela_cartao.Conta = string.Empty;

                        if (!parcController.Save(parcela_cartao))
                        {
                            unit.RollBack();
                            return false;
                        }
                        numero_parcela++;
                    }
                    #endregion

                    #region PRAZO
                    if (forma_pg.Tipo_pagamento == (int)Tipo_pagamento.PRAZO)
                    {
                        DateTime data_base = (forma_pg.Tipo_intervalo == (int)Tipo_intervalo.DATA_BASE
                            ? DateTime.Now.AddMonths(1)
                            : DateTime.Now.AddDays(forma_pg.Intervalo)); //baseando a data para o mes sequente ao atual

                        for (int i = 0; i < forma_pg.Parcelas; i++)
                        {
                            Parcelas parcela_prazo = new Parcelas();

                            parcela_prazo.Pagamento_lancamento_id = pagamento.Id;
                            parcela_prazo.Valor = (pagamento.Valor / forma_pg.Parcelas);
                            parcela_prazo.Situacao = (int)Situacao_parcela.EM_ABERTO;
                            parcela_prazo.Data_lancamento = lancamento_financeiro.Data;
                            parcela_prazo.Parcela_descricao = $"REFERENTE AO LANÇAMENTO FINANCEIRO {lancamento_financeiro.Id} ({plano_conta.Descricao})";
                            parcela_prazo.Num_documento = lancamento_financeiro.Num_documento.PadLeft(8 - lancamento_financeiro.Num_documento.Length, '0') + "-" + numero_parcela;
                            parcela_prazo.Numero_cheque = string.Empty;
                            parcela_prazo.Banco = string.Empty;
                            parcela_prazo.Agencia = string.Empty;
                            parcela_prazo.Dias_compensacao = 0;
                            parcela_prazo.Conta = string.Empty;
                            parcela_prazo.Portador = lancamento_financeiro.Conta_id;

                            if (forma_pg.Tipo_intervalo == (int)Tipo_intervalo.DATA_BASE)
                            {
                                data_base = new DateTime(data_base.Year, data_base.Month, forma_pg.Dia_base);
                                parcela_prazo.Data_vencimento = data_base;
                                data_base = data_base.AddMonths(1);
                            }

                            if (forma_pg.Tipo_intervalo == (int)Tipo_intervalo.INTERVALO)
                            {
                                parcela_prazo.Data_vencimento = data_base;
                                data_base = data_base.AddDays(forma_pg.Intervalo);
                            }

                            if (lancamento_financeiro.Tipo == (int)Tipo_lancamento.ENTRADA)
                            {
                                parcela_prazo.Tipo_parcela = (int)Tipo_parcela.RECEBER;
                                parcela_prazo.Cliente_id = lancamento_financeiro.Cliente_id;
                            }

                            if (lancamento_financeiro.Tipo == (int)Tipo_lancamento.SAIDA)
                            {
                                parcela_prazo.Tipo_parcela = (int)Tipo_parcela.PAGAR;
                                parcela_prazo.Fornecedor_id = lancamento_financeiro.Fornecedor_id;
                            }

                            if (!parcController.Save(parcela_prazo))
                            {
                                unit.RollBack();
                                return false;
                            }

                            numero_parcela++;
                        }
                        #endregion
                    }
                }

                unit.Commit();
                BStatus.Success("Lançamento financeiro efetuado com sucesso");
                return true;
            }
            catch (Exception ex)
            {
                unit.RollBack();
                return false;
            }
        }

        internal varejo_config GetContext()
        {
            return db.Context;
        }

        internal Lancamentos_financeiros Find(int lancamento_id)
        {
            return db.Find(lancamento_id);
        }

        private bool Valid(Lancamentos_financeiros lf)
        {
            if (lf.Conta_id == 0)
            {
                BStatus.Alert("Informe uma conta para efetuar o lançamento");
                return false;
            }

            if (lf.Data == null)
            {
                BStatus.Alert("A data do lançamento é obrigatória");
                return false;
            }

            if (lf.Valor_original == 0)
            {
                BStatus.Alert("Informe o valor original do lançamento");
                return false;
            }

            if (lf.Plano_conta_id == 0)
            {
                BStatus.Alert("Informe o plano de contas");
                return false;
            }

            if (!string.IsNullOrEmpty(lf.Num_documento))
                if (db.Where(e => e.Num_documento.Equals(lf.Num_documento)).FirstOrDefault() != null)
                {
                    BStatus.Alert("Já existe um lançamento com o número de documento informado. Informe outro numero de documento.");
                    return false;
                }

            return true;
        }

        public int CountBusca(
            int mes,
            int conta_id)
        {
            return db.CountBusca(mes, conta_id);
        }

        public List<Lancamentos_financeiros> BuscaSimples(
            int pagina_atual,
            int numero_registros,
            int mes,
            int conta_id)
        {
            return db.BuscaSimples(pagina_atual, numero_registros, mes, conta_id);
        }
    }
}
