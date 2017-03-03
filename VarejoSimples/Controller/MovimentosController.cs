using DarumaFramework_NFCe;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Text;
using VarejoSimples.Enums;
using VarejoSimples.Interfaces;
using VarejoSimples.Model;
using VarejoSimples.Repository;
using VarejoSimples.Views.Movimento.LancamentoCheque;
using VarejoSimples.Views.Movimento.RecebimentoCheques;

namespace VarejoSimples.Controller
{
    public class MovimentosController
    {
        private MovimentosRepository db = null;

        public MovimentosController()
        {
            db = new MovimentosRepository();
        }

        private Movimentos Movimento { get; set; }

        public void AbreMovimento(int cliente_fornecedor_id, int tipo_movimento)
        {
            Movimento = new Movimentos();

            Tipos_movimento tipo = new Tipos_movimentoController().Find(tipo_movimento);

            if (tipo.Utiliza_fornecedor)
                InformarFornecedor(cliente_fornecedor_id);
            else
                InformarCliente(cliente_fornecedor_id);

            Movimento.Tipo_movimento_id = tipo_movimento;
            BStatus.Success("Movimento iniciado...");
        }

        internal Movimentos Find(int movimento_id)
        {
            return db.Find(movimento_id);
        }

        private List<Itens_movimento> itens_mov = null;
        private List<Itens_pagamento> itens_pag = null;
        private decimal Troco { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Código do movimento caso seja efetuado com sucesso. Caso haja erros, retornará 0.</returns>
        public int FechaMovimento()
        {
            UnitOfWork unit = new UnitOfWork();
            try
            {
                unit.BeginTransaction();
                db.Context = unit.Context;

                if (itens_mov == null)
                    itens_mov = Movimento.Itens_movimento.ToList();
                Movimento.Itens_movimento.Clear();

                if (itens_pag == null)
                    itens_pag = Movimento.Itens_pagamento.ToList();
                Movimento.Itens_pagamento.Clear();

                Movimentos_caixasController movimentos_caixaController = new Movimentos_caixasController();
                movimentos_caixaController.SetContext(unit.Context);

                Tipos_movimentoController tmc = new Tipos_movimentoController();
                tmc.SetContext(unit.Context);

                Tipos_movimento tipo_mov = tmc.Find(Movimento.Tipo_movimento_id);

                Movimento.Id = db.NextId(e => e.Id);
                Movimento.Data = DateTime.Now;
                Movimento.Usuario_id = UsuariosController.UsuarioAtual.Id;
                Movimento.Loja_id = UsuariosController.LojaAtual.Id;
                Movimento.Plano_conta_id = tipo_mov.Plano_conta_id;

                db.Save(Movimento);
                db.Commit();

                Itens_movimentoController imc = new Itens_movimentoController();
                imc.SetContext(unit.Context);

                EstoqueController estoque_controller = new EstoqueController();
                estoque_controller.SetContext(unit.Context);

                string lote = imc.GetLastLote(false);
                lote = estoque_controller.GeraProximoLote(lote);
                int sublote = 1;

                #region Itens do movimento
                foreach (Itens_movimento item in itens_mov)
                {
                    item.Produtos = null;
                    item.Unidades = null;
                    item.Movimento_id = Movimento.Id;

                    Estoque e = new Estoque();
                    e.Produto_id = item.Produto_id;
                    e.Loja_id = UsuariosController.LojaAtual.Id;
                    e.Quant = item.Quant;
                    e.Lote = (string.IsNullOrEmpty(item.Lote)
                            ? null
                            : (item.Lote + "SL" + item.Sublote));

                    ProdutosController pc = new ProdutosController();
                    pc.SetContext(unit.Context);
                    Produtos prod = pc.Find(e.Produto_id);

                    Produtos_fornecedoresController pForn_c = new Produtos_fornecedoresController();
                    pForn_c.SetContext(unit.Context);

                    switch (tipo_mov.Movimentacao_itens)
                    {
                        case (int)Tipo_movimentacao.ENTRADA:

                            /*
                                 O produto controla Lote, e a sua entrada é vinda de um Fornecedor.
                                 Neste caso é gerado o seu Lote e inserido no estoque da 
                                 loja em questão
                            */
                            if (tipo_mov.Utiliza_fornecedor && prod.Controla_lote)
                            {
                                e.Lote = lote;
                                e.Sublote = sublote.ToString();
                                e.Data_entrada = DateTime.Now;

                                Produtos_fornecedores pf = pForn_c.Find(item.Produto_id, Movimento.Fornecedor_id);
                                if (pf != null)
                                    item.Quant = (item.Quant * pf.Fator_conversao);

                                estoque_controller.Save(e);
                                sublote++;
                            }

                            if (tipo_mov.Utiliza_fornecedor && !prod.Controla_lote)
                            {
                                Produtos_fornecedores pf = pForn_c.Find(item.Produto_id, Movimento.Fornecedor_id);
                                if (pf != null)
                                    item.Quant = (item.Quant * pf.Fator_conversao);

                                if (!estoque_controller.InsereEstoque(item.Quant, item.Produto_id, Movimento.Loja_id, (item.Lote + "SL" + item.Sublote)))
                                {
                                    unit.RollBack();
                                    return 0;
                                }
                            }

                            /*
                                 O produto controla Lote, porém sua entrada NÃO é proveniennte de um fornecedor.
                                 Pode ser uma devolução, troca ou entrada por transferencia de loja.

                                 Neste caso, é verificado se existe o lote em questão.
                                 Caso não exista, será criado,
                                 Caso exista, o Saldo em Estoque do mesmo será atualizado
                            */
                            if ((!tipo_mov.Utiliza_fornecedor) && prod.Controla_lote)
                            {
                                if (!estoque_controller.ExisteLote(item.Lote, item.Sublote))
                                    estoque_controller.CriaLote(item.Produto_id, Movimento.Loja_id, item.Lote, item.Sublote);

                                if (!estoque_controller.InsereEstoque(item.Quant, item.Produto_id, Movimento.Loja_id, (item.Lote + "SL" + item.Sublote)))
                                {
                                    unit.RollBack();
                                    return 0;
                                }
                            }

                            /*
                                O produto NÃO controla lote, e sua entrada NÃO é proveniente de um fornecedor.
                                Neste caso, o estoque será inserido levando em consideração o produto pelo seu código.

                                OBS.: Quando um produto NÃO possui lotes em estoque, a tabela de Estoque só pode
                                conter um unico registro referente ao produto em questão, considerando a Loja.

                                Quando o produto POSSUI lotes em estoque, a tabela de estoque pode conter varios
                                registros referente ao produto em questão, levando em consideração o Lote, Sub-Lote
                                e respectiva Loja.
                            */
                            if (!tipo_mov.Utiliza_fornecedor && !prod.Controla_lote)
                            {
                                if (!estoque_controller.InsereEstoque(item.Quant, item.Produto_id, Movimento.Loja_id))
                                {
                                    unit.RollBack();
                                    return 0;
                                }
                            }

                            break;

                        case (int)Tipo_movimentacao.SAIDA:

                            string loteEst = (string.IsNullOrEmpty(e.Lote)
                                ? null
                                : e.Lote);

                            if (!estoque_controller.RetiraEstoque(e.Quant, e.Produto_id, e.Loja_id, loteEst))
                            {
                                unit.RollBack();
                                return 0;
                            }

                            break;
                    }

                    if (e.Lote != null)
                    {
                        if (e.Lote.Contains("SL"))
                        {
                            item.Lote = e.Lote.Substring(0, e.Lote.IndexOf("SL"));
                            item.Sublote = e.Lote.Substring(e.Lote.IndexOf("SL") + 2);
                        }
                        else
                        {
                            item.Lote = e.Lote;
                            item.Sublote = e.Sublote;
                        }
                    }

                    if (!imc.Save(item))
                    {
                        unit.RollBack();
                        return 0;
                    }
                }
                #endregion

                int numero_parcela = 1;
                
                #region Itens do Pagamento
                foreach (Itens_pagamento item_pg in itens_pag)
                {
                    if (tipo_mov.Movimentacao_valores == (int)Tipo_movimentacao.NENHUM)
                        continue;

                    item_pg.Movimento_id = Movimento.Id;

                    ContasController contas_controller = new ContasController();
                    contas_controller.SetContext(unit.Context);

                    Itens_pagamentoController ipc = new Itens_pagamentoController();
                    ipc.SetContext(unit.Context);

                    if (!ipc.Save(item_pg))
                    {
                        unit.RollBack();
                        return 0;
                    }

                    Formas_pagamentoController fpg_controller = new Formas_pagamentoController();
                    fpg_controller.SetContext(unit.Context);

                    Formas_pagamento forma_pagamento = fpg_controller.Find(item_pg.Forma_pagamento_id);

                    Movimentos_caixas movimento_caixa = new Movimentos_caixas();
                    movimento_caixa.Descricao = $"Movimento {Movimento.Id} ({(tipo_mov.Movimentacao_valores == (int)Tipo_movimentacao.ENTRADA ? "ENTRADA" : "SAIDA")})";
                    movimento_caixa.Caixa_id = movimentos_caixaController.GetCaixaAtualUsuario();
                    movimento_caixa.Data = Movimento.Data;
                    movimento_caixa.Movimento_id = Movimento.Id;
                    movimento_caixa.Usuario_id = Movimento.Usuario_id;
                    movimento_caixa.Forma_pagamento_id = item_pg.Forma_pagamento_id;
                    movimento_caixa.Loja_id = UsuariosController.LojaAtual.Id;

                    ParcelasController parcController = new ParcelasController();
                    parcController.SetContext(unit.Context);

                    switch (forma_pagamento.Tipo_pagamento)
                    {
                        #region DINHEIRO
                        case (int)Tipo_pagamento.DINHEIRO:

                            switch (tipo_mov.Movimentacao_valores)
                            {
                                case (int)Tipo_movimentacao.ENTRADA:
                                    movimento_caixa.Tipo_mov = (int)Tipo_movimentacao_caixa.ENTRADA;
                                    movimento_caixa.Valor = item_pg.Valor;
                                    break;

                                case (int)Tipo_movimentacao.SAIDA:
                                    movimento_caixa.Tipo_mov = (int)Tipo_movimentacao_caixa.SAIDA;
                                    movimento_caixa.Valor = (item_pg.Valor * (-1));
                                    break;
                            }

                            if (!movimentos_caixaController.Save(movimento_caixa))
                            {
                                unit.RollBack();
                                return 0;
                            }

                            break;
                        #endregion

                        #region CARTAO
                        case (int)Tipo_pagamento.CARTAO:

                            movimento_caixa.Valor = 0;

                            Operadoras_cartaoController opController = new Operadoras_cartaoController();
                            opController.SetContext(unit.Context);

                            Operadoras_cartao operadora = opController.Find(forma_pagamento.Operadora_cartao_id);

                            Parcelas parcela_cartao = new Parcelas();
                            parcela_cartao.Item_pagamento_id = item_pg.Id;
                            parcela_cartao.Valor = item_pg.Valor;
                            parcela_cartao.Situacao = (int)Situacao_parcela.EM_ABERTO;
                            parcela_cartao.Data_lancamento = Movimento.Data;

                            parcela_cartao.Data_vencimento = (operadora.Tipo_recebimento == (int)Tipo_prazo_operadora.DIAS
                                ? Movimento.Data.AddDays(operadora.Prazo_recebimento)
                                : Movimento.Data.AddHours(operadora.Prazo_recebimento));
                            parcela_cartao.Portador = forma_pagamento.Conta_id;

                            if (tipo_mov.Movimentacao_valores == (int)Tipo_movimentacao.ENTRADA)
                            {
                                parcela_cartao.Tipo_parcela = (int)Tipo_parcela.RECEBER;
                                parcela_cartao.Cliente_id = Movimento.Cliente_id;
                            }

                            if (tipo_mov.Movimentacao_valores == (int)Tipo_movimentacao.SAIDA)
                            {
                                parcela_cartao.Tipo_parcela = (int)Tipo_parcela.PAGAR;
                                parcela_cartao.Fornecedor_id = Movimento.Cliente_id;
                            }

                            parcela_cartao.Num_documento = Movimento.Id.ToString().PadLeft(8 - Movimento.Id.ToString().Length, '0') + "-" + numero_parcela;
                            parcela_cartao.Parcela_descricao = $"REFERENTE AO MOVIMENTO {Movimento.Id} ({tipo_mov.Descricao})";
                            parcela_cartao.Numero_cheque = string.Empty;
                            parcela_cartao.Banco = string.Empty;
                            parcela_cartao.Agencia = string.Empty;
                            parcela_cartao.Dias_compensacao = 0;
                            parcela_cartao.Conta = string.Empty;

                            if (!parcController.Save(parcela_cartao))
                            {
                                unit.RollBack();
                                return 0;
                            }
                            numero_parcela++;
                            break;
                        #endregion

                        #region CHEQUE
                        case (int)Tipo_pagamento.CHEQUE:

                            movimento_caixa.Valor = 0;
                            IRegistroCheques registroCheques;

                            if (tipo_mov.Movimentacao_valores == (int)Tipo_movimentacao.ENTRADA)
                                registroCheques = new RecebimentoCheques();
                            else
                            {
                                registroCheques = new LancamentoCheque();
                                registroCheques.SetConta(contas_controller.Find(forma_pagamento.Conta_id));
                            }

                            registroCheques.Exibir(item_pg.Valor);

                            foreach (Cheque cheque in registroCheques.Cheques)
                            {
                                Parcelas parcela_cheque = new Parcelas();

                                parcela_cheque.Item_pagamento_id = item_pg.Id;
                                parcela_cheque.Valor = cheque.Valor;
                                parcela_cheque.Situacao = (int)Situacao_parcela.EM_ABERTO;
                                parcela_cheque.Data_lancamento = Movimento.Data;
                                parcela_cheque.Num_documento = Movimento.Id.ToString().PadLeft(8 - Movimento.Id.ToString().Length, '0') + "-" + numero_parcela;
                                parcela_cheque.Parcela_descricao = $"REFERENTE AO MOVIMENTO {Movimento.Id} ({tipo_mov.Descricao})";
                                parcela_cheque.Data_vencimento = cheque.Data_deposito;

                                if(tipo_mov.Movimentacao_valores == (int)Tipo_movimentacao.ENTRADA)
                                {
                                    parcela_cheque.Tipo_parcela = (int)Tipo_parcela.RECEBER;
                                    parcela_cheque.Cliente_id = Movimento.Cliente_id;
                                }

                                if(tipo_mov.Movimentacao_valores == (int)Tipo_movimentacao.SAIDA)
                                {
                                    parcela_cheque.Tipo_parcela = (int)Tipo_parcela.PAGAR;
                                    parcela_cheque.Fornecedor_id = Movimento.Fornecedor_id;
                                }

                                parcela_cheque.Portador = forma_pagamento.Conta_id;

                                if (tipo_mov.Movimentacao_valores == (int)Tipo_movimentacao.ENTRADA)
                                {
                                    parcela_cheque.Tipo_parcela = (int)Tipo_parcela.RECEBER;
                                    parcela_cheque.Cliente_id = Movimento.Cliente_id;

                                    parcela_cheque.Numero_cheque = cheque.Numero_cheque;
                                    parcela_cheque.Banco = cheque.Banco;
                                    parcela_cheque.Agencia = cheque.Agencia;
                                    parcela_cheque.Dias_compensacao = cheque.Dias_compensacao;
                                    parcela_cheque.Conta = cheque.Conta;
                                }

                                if (tipo_mov.Movimentacao_valores == (int)Tipo_movimentacao.SAIDA)
                                {
                                    parcela_cheque.Tipo_parcela = (int)Tipo_parcela.PAGAR;
                                    parcela_cheque.Fornecedor_id = Movimento.Fornecedor_id;

                                    parcela_cheque.Numero_cheque = cheque.Numero_cheque;
                                    parcela_cheque.Banco = string.Empty;
                                    parcela_cheque.Agencia = string.Empty;
                                    parcela_cheque.Dias_compensacao = 0;
                                    parcela_cheque.Conta = string.Empty;
                                }

                                if (!parcController.Save(parcela_cheque))
                                {
                                    unit.RollBack();
                                    return 0;
                                }

                                numero_parcela++;
                            }

                            break;
                        #endregion

                        #region PRAZO
                        case (int)Tipo_pagamento.PRAZO:

                            DateTime data_base = (forma_pagamento.Tipo_intervalo == (int)Tipo_intervalo.DATA_BASE
                                ? DateTime.Now.AddMonths(1)
                                : DateTime.Now.AddDays(forma_pagamento.Intervalo)); //baseando a data para o mes sequente ao atual

                            for (int i = 0; i < forma_pagamento.Parcelas; i++)
                            {

                                Parcelas parcela_prazo = new Parcelas();

                                parcela_prazo.Item_pagamento_id = item_pg.Id;
                                parcela_prazo.Valor = (item_pg.Valor / forma_pagamento.Parcelas);
                                parcela_prazo.Situacao = (int)Situacao_parcela.EM_ABERTO;
                                parcela_prazo.Data_lancamento = Movimento.Data;
                                parcela_prazo.Parcela_descricao = $"REFERENTE AO MOVIMENTO {Movimento.Id} ({tipo_mov.Descricao})";
                                parcela_prazo.Num_documento = Movimento.Id.ToString().PadLeft(8 - Movimento.Id.ToString().Length, '0') + "-" + numero_parcela;
                                parcela_prazo.Numero_cheque = string.Empty ;
                                parcela_prazo.Banco = string.Empty;
                                parcela_prazo.Agencia = string.Empty;
                                parcela_prazo.Dias_compensacao = 0;
                                parcela_prazo.Conta = string.Empty;

                                if (forma_pagamento.Tipo_intervalo == (int)Tipo_intervalo.DATA_BASE)
                                {
                                    data_base = new DateTime(data_base.Year, data_base.Month, forma_pagamento.Dia_base);
                                    parcela_prazo.Data_vencimento = data_base;
                                    data_base = data_base.AddMonths(1);
                                }

                                if (forma_pagamento.Tipo_intervalo == (int)Tipo_intervalo.INTERVALO)
                                {
                                    parcela_prazo.Data_vencimento = data_base;
                                    data_base = data_base.AddDays(forma_pagamento.Intervalo);
                                }

                                if (tipo_mov.Movimentacao_valores == (int)Tipo_movimentacao.ENTRADA)
                                {
                                    parcela_prazo.Tipo_parcela = (int)Tipo_parcela.RECEBER;
                                    parcela_prazo.Cliente_id = Movimento.Cliente_id;
                                }

                                if (tipo_mov.Movimentacao_valores == (int)Tipo_movimentacao.SAIDA)
                                {
                                    parcela_prazo.Tipo_parcela = (int)Tipo_parcela.PAGAR;
                                    parcela_prazo.Fornecedor_id = Movimento.Fornecedor_id;
                                }

                                if (!parcController.Save(parcela_prazo))
                                {
                                    unit.RollBack();
                                    return 0;
                                }

                                numero_parcela++;
                            }
                            break;
                            #endregion
                    }
                }
                #endregion

                unit.Commit();
                BStatus.Success("Movimento salvo");
                return Movimento.Id;
            }
            catch (Exception ex)
            {
                unit.RollBack();
                return 0;
            }
        }

        internal void InformarFornecedor(int fornecedor_id)
        {
            Movimento.Fornecedor_id = fornecedor_id;
        }

        public List<Itens_movimento> Itens_movimento
        {
            get
            {
                return Movimento.Itens_movimento.ToList();
            }
        }

        public void RemoveItem(int item_id)
        {
            Movimento.Itens_movimento.Remove(Movimento.Itens_movimento.Where(e => e.Id == item_id).First());
        }

        public void IncrementaItem(int item_id)
        {
            Itens_movimento item = Movimento.Itens_movimento.First(e => e.Id == item_id);

            decimal valor_item = (item.Valor_final / item.Quant);
            item.Quant += 1;
            item.Valor_final += valor_item;
        }

        public void DecrementaItem(int item_id)
        {
            Itens_movimento item = Movimento.Itens_movimento.First(e => e.Id == item_id);

            decimal valor_item = (item.Valor_final / item.Quant);
            item.Quant -= 1;
            item.Valor_final -= valor_item;
        }

        public void InformarCliente(int cliente_id)
        {
            Movimento.Cliente_id = cliente_id;
        }

        public void AplicarDescontoReais(int item_id, decimal valor)
        {
            Itens_movimento item = Movimento.Itens_movimento.Where(e => e.Id == item_id).First();
            item.Valor_final -= valor;
        }

        public void AplicarDescontoPerc(int item_id, decimal percent)
        {
            Itens_movimento item = Movimento.Itens_movimento.Where(e => e.Id == item_id).First();
            item.Valor_final = (item.Valor_final - (item.Valor_final / 100 * percent));
        }

        public bool EfetuaPagamento(int forma_pagamento_id, decimal valor)
        {
            Itens_pagamento itemP = Movimento.Itens_pagamento
                .Where(e => e.Forma_pagamento_id == forma_pagamento_id).FirstOrDefault();

            if (itemP != null)
            {
                BStatus.Alert("Esta forma de pagamento ja foi informada");
                return false;
            }

            Movimento.Itens_pagamento.Add(new Itens_pagamento()
            {
                Forma_pagamento_id = forma_pagamento_id,
                Valor = valor
            });

            BStatus.Success("Forma de pagamento registrada");
            return true;
        }

        public void AdicionaItem(Itens_movimento item)
        {
            if (!ValidItem(item))
                return;

            int id = (Movimento.Itens_movimento.OrderByDescending(e => e.Id).FirstOrDefault() == null
                ? 1
                : Movimento.Itens_movimento.OrderByDescending(e => e.Id).FirstOrDefault().Id + 1);

            item.Id = id;
            Movimento.Itens_movimento.Add(item);
        }

        private bool ValidItem(Itens_movimento item)
        {
            if (item.Produto_id == 0)
            {
                BStatus.Alert("Informe o produto antes de adicionar um item");
                return false;
            }

            if (item.Valor_unit == 0)
            {
                BStatus.Alert("O preço unitário do item deve ser superior a 0");
                return false;
            }

            if (item.Cfop == 0)
            {
                BStatus.Alert("O item informado não possui CFOP. Verifique o Tipo de Movimento tente novamente");
                return false;
            }

            return true;
        }

        public int CountByTipo_movimento(int tipo_mov_id)
        {
            return db.Where(e => e.Tipo_movimento_id == tipo_mov_id).Count();
        }

        public int CountByCliente(int cliente_id)
        {
            return db.Where(m => m.Cliente_id == cliente_id).Count();
        }

        public List<Movimentos> BuscaGenerica(string search, DateTime? data_inicio, DateTime? data_fim, int pagina_atual, int numero_registros)
        {
            return db.BuscaGenericaMovimentos(search, data_inicio, data_fim, db.Context, pagina_atual, numero_registros);
        }

        public int CountPaginacao(string search, DateTime? data_inicio, DateTime? data_fim)
        {
            return db.CountPaginacao(search, data_inicio, data_fim, db.Context);
        }
    }
}
