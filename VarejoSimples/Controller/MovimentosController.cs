using Base.Controller_Reports;
using DarumaFramework_NFCe;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using VarejoSimples.DataSets;
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
        private Pedidos_venda Pedido_venda { get; set; }

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

        internal varejo_config GetContext()
        {
            return db.Context;
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
        public int FechaMovimento(decimal troco)
        {
            UnitOfWork unit = new UnitOfWork(true);
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

                Formas_pagamentoController fpg_controller = new Formas_pagamentoController();
                fpg_controller.SetContext(unit.Context);

                Tipos_movimentoController tmc = new Tipos_movimentoController();
                tmc.SetContext(unit.Context);

                Tipos_movimento tipo_mov = tmc.Find(Movimento.Tipo_movimento_id);

                Movimento.Id = db.NextId(e => e.Id);
                Movimento.Data = DateTime.Now;
                Movimento.Usuario_id = UsuariosController.UsuarioAtual.Id;
                Movimento.Loja_id = UsuariosController.LojaAtual.Id;

                db.Save(Movimento);
                db.Commit();

                if (troco > 0)
                {
                    int tipo_pg_dinheiro = (int)Tipo_pagamento.DINHEIRO;
                    Formas_pagamento fpgTroco = fpg_controller.Get(e => e.Tipo_pagamento == tipo_pg_dinheiro);

                    Movimentos_caixas mcTroco = new Movimentos_caixas();
                    mcTroco.Descricao = $"Movimento {Movimento.Id} (TROCO)";
                    mcTroco.Caixa_id = movimentos_caixaController.Get_ID_CaixaAtualUsuario();
                    mcTroco.Data = Movimento.Data;
                    mcTroco.Movimento_id = Movimento.Id;
                    mcTroco.Usuario_id = Movimento.Usuario_id;
                    mcTroco.Forma_pagamento_id = fpgTroco.Id;
                    mcTroco.Loja_id = UsuariosController.LojaAtual.Id;
                    mcTroco.Tipo_mov = (int)Tipo_movimentacao_caixa.TROCO;
                    mcTroco.Valor = (troco * (-1));

                    if (!movimentos_caixaController.Save(mcTroco))
                    {
                        unit.RollBack();
                        return 0;
                    }
                }

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
                    e.Grade_id = item.Grade_id;

                    ProdutosController pc = new ProdutosController();
                    pc.SetContext(unit.Context);
                    Produtos prod = pc.Find(item.Produto_id);

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
                            #region tipo_mov.Utiliza_fornecedor && prod.Controla_lote
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

                                break;
                            }
                            #endregion

                            #region tipo_mov.Utiliza_fornecedor && !prod.Controla_lote
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

                                break;
                            }
                            #endregion

                            /*
                                 O produto controla Lote, porém sua entrada NÃO é proveniennte de um fornecedor.
                                 Pode ser uma devolução, troca ou entrada por transferencia de loja.

                                 Neste caso, é verificado se existe o lote em questão.
                                 Caso não exista, será criado,
                                 Caso exista, o Saldo em Estoque do mesmo será atualizado
                            */
                            #region (!tipo_mov.Utiliza_fornecedor) && prod.Controla_lote
                            if ((!tipo_mov.Utiliza_fornecedor) && prod.Controla_lote)
                            {
                                if (!estoque_controller.ExisteLote(item.Lote, item.Sublote))
                                    estoque_controller.CriaLote(item.Produto_id, Movimento.Loja_id, item.Lote, item.Sublote);

                                if (!estoque_controller.InsereEstoque(item.Quant, item.Produto_id, Movimento.Loja_id, (item.Lote + "SL" + item.Sublote)))
                                {
                                    unit.RollBack();
                                    return 0;
                                }

                                break;
                            }
                            #endregion

                            #region prod.Controla_grade
                            if (prod.Controla_grade)
                            {
                                if (!estoque_controller.InsereEstoque(item.Quant, item.Produto_id, Movimento.Loja_id, null, item.Grade_id))
                                {
                                    unit.RollBack();
                                    return 0;
                                }

                                break;
                            }
                            #endregion

                            /*
                                O produto NÃO controla lote, e sua entrada NÃO é proveniente de um fornecedor.
                                Neste caso, o estoque será inserido levando em consideração o produto pelo seu código.

                                OBS.: Quando um produto NÃO possui lotes em estoque, a tabela de Estoque só pode
                                conter um unico registro referente ao produto em questão, considerando a Loja.

                                Quando o produto POSSUI lotes em estoque, a tabela de estoque pode conter varios
                                registros referente ao produto em questão, levando em consideração o Lote, Sub-Lote
                                e respectiva Loja.
                            */
                            #region !tipo_mov.Utiliza_fornecedor && !prod.Controla_lote
                            if (!tipo_mov.Utiliza_fornecedor && !prod.Controla_lote)
                            {
                                if (!estoque_controller.InsereEstoque(item.Quant, item.Produto_id, Movimento.Loja_id))
                                {
                                    unit.RollBack();
                                    return 0;
                                }

                                break;
                            }
                            #endregion

                            break;

                        case (int)Tipo_movimentacao.SAIDA:

                            string loteEst = (string.IsNullOrEmpty(e.Lote)
                                ? null
                                : e.Lote);

                            if (!estoque_controller.RetiraEstoque(e.Quant, e.Produto_id, e.Loja_id, loteEst, item.Grade_id))
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

                    Formas_pagamento forma_pagamento = fpg_controller.Find(item_pg.Forma_pagamento_id);

                    Movimentos_caixas movimento_caixa = new Movimentos_caixas();
                    movimento_caixa.Descricao = $"Movimento {Movimento.Id} ({(tipo_mov.Movimentacao_valores == (int)Tipo_movimentacao.ENTRADA ? "ENTRADA" : "SAIDA")})";
                    movimento_caixa.Caixa_id = movimentos_caixaController.Get_ID_CaixaAtualUsuario();
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

                                movimento_caixa.Tipo_mov = (int)Tipo_movimentacao_caixa.ENTRADA;
                                movimento_caixa.Valor = item_pg.Valor;
                            }

                            if (tipo_mov.Movimentacao_valores == (int)Tipo_movimentacao.SAIDA)
                            {
                                parcela_cartao.Tipo_parcela = (int)Tipo_parcela.PAGAR;
                                parcela_cartao.Fornecedor_id = Movimento.Cliente_id;

                                movimento_caixa.Tipo_mov = (int)Tipo_movimentacao_caixa.SAIDA;
                                movimento_caixa.Valor = item_pg.Valor * (-1);
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
                            {
                                registroCheques = new RecebimentoCheques();
                                movimento_caixa.Tipo_mov = (int)Tipo_movimentacao_caixa.ENTRADA;
                                movimento_caixa.Valor = item_pg.Valor;
                            }
                            else
                            {
                                registroCheques = new LancamentoCheque();
                                registroCheques.SetConta(contas_controller.Find(forma_pagamento.Conta_id));

                                movimento_caixa.Tipo_mov = (int)Tipo_movimentacao_caixa.SAIDA;
                                movimento_caixa.Valor = item_pg.Valor * (-1);
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

                                    parcela_cheque.Portador = forma_pagamento.Conta_id;
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
                                parcela_prazo.Numero_cheque = string.Empty;
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

                                    movimento_caixa.Tipo_mov = (int)Tipo_movimentacao_caixa.ENTRADA;
                                    movimento_caixa.Valor = item_pg.Valor;
                                }

                                if (tipo_mov.Movimentacao_valores == (int)Tipo_movimentacao.SAIDA)
                                {
                                    parcela_prazo.Tipo_parcela = (int)Tipo_parcela.PAGAR;
                                    parcela_prazo.Fornecedor_id = Movimento.Fornecedor_id;

                                    movimento_caixa.Tipo_mov = (int)Tipo_movimentacao_caixa.SAIDA;
                                    movimento_caixa.Valor = item_pg.Valor * (-1);
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

                    if (!movimentos_caixaController.Save(movimento_caixa))
                    {
                        unit.RollBack();
                        return 0;
                    }
                }
                #endregion

                if (Pedido_venda != null)
                {
                    Pedidos_vendaController pedidosController = new Pedidos_vendaController();
                    pedidosController.SetContext(unit.Context);

                    pedidosController.RemovePedido(Pedido_venda.Id);
                }

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

        private void LogNFCe(string msg)
        {
            StreamWriter writer = null;
            try
            {
                if (!Directory.Exists(@"C:\Temp\"))
                    Directory.CreateDirectory(@"C:\Temp\");

                string fileName = $@"C:\Temp\MOV{Movimento.Id}-NFCe.log";

                writer = (File.Exists(fileName)
                    ? File.AppendText(fileName)
                    : new StreamWriter(fileName));

                writer.WriteLine($" [{DateTime.Now.ToString()}]: {msg}");
                writer.Close();
            }
            catch (Exception ex)
            {
                if (writer != null)
                    writer.Close();
            }
        }

        public void NFCe()
        {
            int retorno = 0;
            //  Declaracoes.tCFCancelar_NFCe_Daruma("", "", "", "", "");
            Clientes cliente = new ClientesController().Find(Movimento.Cliente_id);

            if (cliente == null || cliente.Cpf.Equals("___.___.___-__"))
                retorno = Declaracoes.aCFAbrir_NFCe_Daruma("", "", "", "", "", "",
                  "", "", "");
            else
                retorno = Declaracoes.aCFAbrir_NFCe_Daruma(cliente.Cpf, cliente.Nome, cliente.Logradouro, cliente.Numero.ToString(), cliente.Bairro, "",
                    cliente.Municipio, cliente.Uf, cliente.Cep);

            LogNFCe($"aCFAbrir_NFCe_Daruma - {Declaracoes.TrataRetorno(retorno)}");

            if (retorno != 1)
            {
                MessageBox.Show("Ocorreu um problema ao emitir a NFC-e. \nAcione o suporte Doware.", "Erro NFC-e", MessageBoxButton.OK, MessageBoxImage.Error);
                Declaracoes.tCFCancelar_NFCe_Daruma("", "", "", "", "");
                return;
            }

            foreach (Itens_movimento item in Itens_movimento)
            {
                Produtos produto = new ProdutosController().Find(item.Produto_id);

                string aliquota = (produto.Aliquota == 0
                    ? "F1"
                    : produto.Aliquota.ToString("N2").Replace(",", "."));

                string tipoDescAcresc = (item.Desconto == 0
                    ? "A$"
                    : "D$");

                string valorDescAcresc = (item.Desconto == 0
                    ? item.Acrescimo.ToString("N2")
                    : item.Desconto.ToString("N2"));

                string codigoItem = (produto.Controla_lote
                    ? item.Lote + "SL" + item.Sublote
                    : produto.Id.ToString());

                retorno = Declaracoes.aCFVenderCompleto_NFCe_Daruma(aliquota, item.Quant.ToString("N2"),
                      item.Valor_unit.ToString("N2").Replace(".", ""), tipoDescAcresc, valorDescAcresc, codigoItem, produto.Ncm, item.Cfop.ToString(), produto.Unidades.Sigla,
                      produto.Descricao, "");

                string msg = $@"aCFVenderCompleto_NFCe_Daruma - {Declaracoes.TrataRetorno(retorno)}
    Aliquita.....: {aliquota}
    Quant........: {item.Quant}
    Valor_unit...: {item.Valor_unit.ToString("N2")}
    Tp. Desc/Acr.: {tipoDescAcresc}
    Vl. Desc/Acr.: {valorDescAcresc}
    Cod. Item....: {codigoItem}
    NCM..........: {produto.Ncm}
    CFOP.........: {item.Cfop}
    Unidade......: {produto.Unidades.Sigla}
    Produto......: {produto.Descricao}
";
                LogNFCe(msg);

                if (retorno != 1)
                {
                    MessageBox.Show("Ocorreu um problema ao emitir a NFC-e. Acione o suporte Doware.", "Erro NFC-e", MessageBoxButton.OK, MessageBoxImage.Exclamation);

                    Declaracoes.tCFCancelar_NFCe_Daruma("", "", "", "", "");
                    return;
                }
            }

            retorno = Declaracoes.aCFTotalizar_NFCe_Daruma("D%", "0,00");
            LogNFCe("aCFTotalizar_NFCe_Daruma - " + Declaracoes.TrataRetorno(retorno));
            List<Itens_pagamento> itens_pagamento = itens_pag;

            foreach (Itens_pagamento item in itens_pagamento)
            {
                Formas_pagamento fpg = new Formas_pagamentoController().Find(item.Forma_pagamento_id);
                retorno = Declaracoes.aCFEfetuarPagamento_NFCe_Daruma(fpg.Descricao, item.Valor.ToString("N2").Replace(".", ""));

                string msg = $@"aCFEfetuarPagamento_NFCe_Daruma - {Declaracoes.TrataRetorno(retorno)}

    Condição pgto.....: {fpg.Descricao}
    Valor.............: {item.Valor.ToString("N2")}
";

                LogNFCe(msg);

                if (retorno != 1)
                {
                    MessageBox.Show("Ocorreu um problema ao emitir a NFC-e. \nAcione o suporte Doware.", "Erro NFC-e", MessageBoxButton.OK, MessageBoxImage.Error);
                    Declaracoes.tCFCancelar_NFCe_Daruma("", "", "", "", "");
                    return;
                }
            }

            retorno = Declaracoes.tCFEncerrar_NFCe_Daruma("NFC-e emitida via Curae ERP - Doware Sistemas");
            LogNFCe($@"tCFEncerrar_NFCe_Daruma - {Declaracoes.TrataRetorno(retorno)}");

            if (retorno != 1)
            {
                StringBuilder sbCodigo = new StringBuilder(10);
                StringBuilder sbMensagem = new StringBuilder(1000);

                Declaracoes.rAvisoErro_NFCe_Daruma(sbCodigo, sbMensagem);
                LogNFCe($@"tCFEncerrar_NFCe_Daruma - ERRO

Codigo.....: {sbCodigo.ToString()}
Mensagem...: {sbMensagem.ToString()}");

                MessageBox.Show($"A NFC-e não foi autorizada! \nErro: {sbCodigo.ToString()} \nMensagem SEFAZ: {sbMensagem.ToString()}");
                retorno = Declaracoes.tCFCancelar_NFCe_Daruma("", "", "", "", "");
                LogNFCe($"tCFCancelar_NFCe_Daruma - {Declaracoes.TrataRetorno(retorno)}");
                return;
            }

            if (retorno == 1)
            {
                string diretorio = @"C:\NFC-e\DANFEs\";
                var directory = new DirectoryInfo(diretorio);
                FileInfo danfe = directory.GetFiles()
                    .Where(f => f.Name.Contains("DANFE"))
                    .OrderByDescending(f => f.LastWriteTime)
                    .First();

                Parametros parametro = ParametrosController.FindParametro("NF_IMPPADRAO", true);
                if (parametro == null)
                {
                    MessageBox.Show("Não foi possível imprimir a DANFE por que o parâmetro de sistema 'NF_IMPPADRAO' não foi informado ou seu valor não pode ser reconhecido. \n\nAcione o suporte Doware.", "NF_IMPPADRAO", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                if (parametro.Valor == null || parametro.Valor == "")
                {
                    MessageBox.Show("Não foi possível imprimir a DANFE por que o parâmetro de sistema 'NF_IMPPADRAO' não foi informado ou seu valor não pode ser reconhecido. \n\nAcione o suporte Doware.", "NF_IMPPADRAO", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                string parametrosSpool = $@"{danfe.FullName};{parametro.Valor}";

                File.WriteAllText(Directory.GetCurrentDirectory() + @"\PARAMS.txt", "");
                StreamWriter writer = new StreamWriter(Directory.GetCurrentDirectory() + @"\PARAMS.txt");
                writer.WriteLine(parametrosSpool);
                writer.Close();

                System.Diagnostics.Process.Start(Directory.GetCurrentDirectory() + @"\Utilitarios\NFCe_Spool.exe");
            }
        }

        public void CupomNaoFiscal()
        {
            DsCupomNaoFiscal dataSet = new DsCupomNaoFiscal();
            DataTable dtMovimento = dataSet.Tables["Movimento"];

            EstoqueController eController = new EstoqueController();
            Grades_produtosController gController = new Grades_produtosController();
            UnidadesController unController = new UnidadesController();
            UsuariosController usuariosController = new UsuariosController();
            ClientesController clientesController = new ClientesController();
            Tipos_movimentoController tipoMovController = new Tipos_movimentoController();
            Formas_pagamentoController fpgController = new Formas_pagamentoController();

            if (Movimento.Usuarios == null)
                Movimento.Usuarios = new UsuariosController().Find(Movimento.Usuario_id);

            #region MOVIMENTO
            dtMovimento.Rows.Add(
                Movimento.Id,
                UsuariosController.LojaAtual.Nome_fantasia,
                Movimento.Usuarios.Vendedores.Count == 0 ? Movimento.Usuarios.Nome : Movimento.Usuarios.Vendedores.First().Nome,
                Movimento.Cliente_id == 0 ? "Não identificado" : clientesController.Find(Movimento.Cliente_id).Nome,
                GetTotalParcial(),
                GetTotalDesconto(),
                Movimento.Data,
                "Mensagem promocional",
                $"{UsuariosController.LojaAtual.Logradouro}, {UsuariosController.LojaAtual.Bairro} - {UsuariosController.LojaAtual.Municipio}",
                tipoMovController.Find(Movimento.Tipo_movimento_id).Descricao,
                UsuariosController.LojaAtual.Cnpj);
            #endregion

            #region ITENS_PAGAMENTO
            DataTable dtItens_pag = dataSet.Tables["Itens_pagamento"];
            itens_pag.ForEach(e => dtItens_pag.Rows.Add(
                    fpgController.Find(e.Forma_pagamento_id).Descricao,
                    e.Valor
                 ));
            #endregion

            #region ITENS_MOVIMENTO
            DataTable dtItens_mov = dataSet.Tables["Itens_movimento"];
            foreach (Itens_movimento item in Itens_movimento)
            {
                string cod_prod = string.Empty;
                string descricaoProdo = item.Produtos.Descricao;
                string descricao_valor_unitario = string.Empty;

                if (!string.IsNullOrEmpty(item.Lote))
                    cod_prod = item.Lote + "SL" + item.Sublote;
                else if (item.Grade_id != null)
                {
                    Grades_produtos grade = gController.Find(item.Grade_id);

                    cod_prod = item.Grade_id;
                    descricaoProdo += $" {grade.Cores.Descricao} {grade.Tamanhos.Descricao}";
                }
                else cod_prod = item.Produtos.Ean;

                if (item.Unidades == null)
                    item.Unidades = unController.Find(item.Unidade_id);
                descricao_valor_unitario = $"{item.Quant} {item.Unidades.Sigla} x R${item.Valor_unit}";
                dtItens_mov.Rows.Add(item.Id, cod_prod, descricaoProdo, descricao_valor_unitario, item.Quant, item.Valor_final);
            }
            #endregion

            IControllerReport rController = ReportController.GetInstance();
            rController.AddDataSource("Itens_movimento", dtItens_mov);
            rController.AddDataSource("Itens_pagamento", dtItens_pag);
            rController.AddDataSource("Movimento", dtMovimento);

            rController.ShowReport("MOVIMENTO", "MOV001");
        }

        public decimal GetTotalDesconto()
        {
            return Itens_movimento.Sum(e => e.Desconto);
        }

        public decimal GetTotalParcial()
        {
            return Itens_movimento.Sum(e => e.Valor_final);
        }

        public int GetClienteId()
        {
            return Movimento.Cliente_id;
        }

        internal void InformarFornecedor(int fornecedor_id)
        {
            Movimento.Fornecedor_id = fornecedor_id;
        }

        public List<Itens_movimento> Itens_movimento
        {
            get
            {
                return Movimento.Itens_movimento.OrderBy(e => e.Id).ToList();
            }
        }

        public void RemoveItem(int item_id)
        {
            Itens_movimento imv = Movimento.Itens_movimento.Where(e => e.Id == item_id).FirstOrDefault();
            if (imv == null)
                return;

            Movimento.Itens_movimento.Remove(imv);

            int id = 0;
            foreach (Itens_movimento item in Itens_movimento)
                item.Id = (id += 1);
        }

        public void IncrementaItem(int item_id)
        {
            Itens_movimento item = Movimento.Itens_movimento.FirstOrDefault(e => e.Id == item_id);
            if (item == null)
                return;

            decimal valor_item = (item.Valor_final / item.Quant);
            item.Quant += 1;
            item.Valor_final += valor_item;
        }

        public void DecrementaItem(int item_id)
        {
            Itens_movimento item = Movimento.Itens_movimento.FirstOrDefault(e => e.Id == item_id);
            if (item == null)
                return;

            if ((item.Quant - 1) == 0)
            {
                RemoveItem(item_id);
                return;
            }

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
            Itens_movimento item = Movimento.Itens_movimento.Where(e => e.Id == item_id).FirstOrDefault();
            if (item == null)
                return;

            item.Valor_final -= valor;
        }

        public void AplicarDescontoPerc(int item_id, decimal percent)
        {
            Itens_movimento item = Movimento.Itens_movimento.Where(e => e.Id == item_id).FirstOrDefault();
            if (item == null)
                return;

            item.Valor_final = (item.Valor_final - (item.Valor_final / 100 * percent));
        }

        public void AplicarDescontoGeralPercent(decimal percent)
        {
            Itens_movimento.ForEach(e => AplicarDescontoPerc(e.Id, percent));
        }

        public void AplicarDescontoGeralReais(decimal valor)
        {
            Itens_movimento.ForEach(e => AplicarDescontoReais(e.Id, valor));
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

        public void CancelaPagamento(int forma_pagamento_id)
        {
            Itens_pagamento item = Movimento.Itens_pagamento.FirstOrDefault(e => e.Forma_pagamento_id == forma_pagamento_id);
            if (item != null)
                Movimento.Itens_pagamento.Remove(item);
        }

        public void AdicionaItem(Itens_movimento item)
        {
            if (!ValidItem(item))
                return;

            int id = (Movimento.Itens_movimento.OrderByDescending(e => e.Id).FirstOrDefault() == null
                ? 1
                : Movimento.Itens_movimento.OrderByDescending(e => e.Id).FirstOrDefault().Id + 1);

            item.Id = id;

            Itens_movimento itemExistente = Movimento.Itens_movimento.FirstOrDefault(e =>
            e.Produto_id == item.Produto_id &&
            (e.Lote == item.Lote && e.Sublote == item.Sublote) &&
            e.Grade_id == item.Grade_id);

            if (itemExistente != null)
            {
                decimal valor_item = (itemExistente.Valor_final / itemExistente.Quant);
                itemExistente.Quant += item.Quant;
                itemExistente.Valor_final += (valor_item * item.Quant);
            }
            else
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

        public void DecrementaItem(Estoque estoque)
        {
            Itens_movimento item = null;
            if (estoque.Produtos.Controla_lote)
                item = Itens_movimento.FirstOrDefault(e => e.Lote.Equals(estoque.Lote) && e.Sublote.Equals(estoque.Sublote));
            else
                item = Itens_movimento.FirstOrDefault(e => e.Produto_id == estoque.Produto_id);

            if (item != null)
                DecrementaItem(item.Id);
        }

        public List<Movimentos> BuscaGenerica(string search, DateTime? data_inicio, DateTime? data_fim, int pagina_atual, int numero_registros)
        {
            return db.BuscaGenericaMovimentos(search, data_inicio, data_fim, pagina_atual, numero_registros);
        }

        public int CountPaginacao(string search, DateTime? data_inicio, DateTime? data_fim)
        {
            return db.CountPaginacao(search, data_inicio, data_fim);
        }

        public int MovimentoParaPedido()
        {
            Pedidos_vendaController pedidoController = new Pedidos_vendaController();
            pedidoController.AbrePedido(Movimento.Cliente_id);

            Itens_movimento.ForEach(e => pedidoController.AdicionaItem(pedidoController.ConvertToItemPedido(e)));
            return pedidoController.FecharPedido();
        }

        public Itens_movimento ItemMovimentoFromItemPedido(Itens_pedido item_pedido)
        {
            Itens_movimento item = new Model.Itens_movimento();

            item.Produto_id = item_pedido.Produto_id;
            item.Produtos = item_pedido.Produtos;
            item.Lote = item_pedido.Lote;
            item.Sublote = item_pedido.Lote;
            item.Quant = item_pedido.Quant;
            item.Valor_unit = item_pedido.Valor_unit;
            item.Aliquota = item_pedido.Aliquota;
            item.Desconto = item_pedido.Desconto;
            item.Acrescimo = item_pedido.Acrescimo;
            item.Frete = item_pedido.Frete;
            item.Outros_valores = item_pedido.Outros_valores;
            item.Cfop = item_pedido.Cfop;
            item.Vendedor_id = item_pedido.Vendedor_id;
            item.Unidades = item_pedido.Unidades;
            item.Unidade_id = item_pedido.Unidade_id;
            item.Grade_id = item_pedido.Grade_id;
            item.Valor_final = item_pedido.Valor_final;

            return item;
        }

        public Movimentos PedidoParaMovimento(Pedidos_venda pedido, int tipo_movimento_id)
        {
            AbreMovimento(0, tipo_movimento_id);
            InformarCliente(pedido.Cliente_id);
            pedido.Itens_pedido.ToList().ForEach(e => AdicionaItem(ItemMovimentoFromItemPedido(e)));
            Pedido_venda = pedido;
            return Movimento;
        }
    }
}