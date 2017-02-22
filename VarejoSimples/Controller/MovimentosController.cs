using DarumaFramework_NFCe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VarejoSimples.Enums;
using VarejoSimples.Model;
using VarejoSimples.Repository;

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

        List<Itens_movimento> itens_mov = null;
        List<Itens_pagamento> itens_pag = null;

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Código do movimento caso seja efetuado com sucesso. Caso haja erros, retornará 0.</returns>
        public int FechaMovimento()
        {
            try
            {
                if (itens_mov == null)
                    itens_mov = Movimento.Itens_movimento.ToList();
                Movimento.Itens_movimento.Clear();

                if (itens_pag == null)
                    itens_pag = Movimento.Itens_pagamento.ToList();
                Movimento.Itens_pagamento.Clear();

                db.Begin(System.Data.IsolationLevel.ReadUncommitted);

                Movimento.Id = db.NextId(e => e.Id);
                Movimento.Data = DateTime.Now;
                Movimento.Usuario_id = UsuariosController.UsuarioAtual.Id;
                Movimento.Loja_id = UsuariosController.LojaAtual.Id;

                db.Save(Movimento);

                Itens_movimentoController imc = new Itens_movimentoController();
                imc.SetContext(db.Context);

                EstoqueController estoque_controller = new EstoqueController();
                estoque_controller.SetContext(db.Context);

                Tipos_movimentoController tmc = new Tipos_movimentoController();
                tmc.SetContext(db.Context);
                Tipos_movimento tipo_mov = tmc.Find(Movimento.Tipo_movimento_id);

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
                    pc.SetContext(db.Context);
                    Produtos prod = pc.Find(e.Produto_id);

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
                                estoque_controller.Save(e);

                                sublote++;
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
                                    db.RollBack();
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
                                    db.RollBack();
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
                                db.RollBack();
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
                        db.RollBack();
                        return 0;
                    }
                }
                #endregion

                #region Itens do Pagamento
                foreach(Itens_pagamento item_pg in itens_pag)
                {
                    Formas_pagamentoController fpg_controller = new Formas_pagamentoController();
                    fpg_controller.SetContext(db.Context);

                    Movimentos_caixasController mov_caixa_c = new Movimentos_caixasController();
                    mov_caixa_c.SetContext(db.Context);

                    Formas_pagamento forma_pg = fpg_controller.Find(item_pg.Forma_pagamento_id);
                    Movimentos_caixas mov_caixa;

                    switch (forma_pg.Tipo_pagamento)
                    {
                        case (int)Tipo_pagamento.DINHEIRO:

                            mov_caixa = new Movimentos_caixas();
                            mov_caixa.Caixa_id = Movimento.Caixa_id;        

                            break;
                    }
                }
                #endregion

                db.Commit();
                BStatus.Success("Movimento salvo");
                return Movimento.Id;
            }
            catch (Exception ex)
            {
                db.RollBack();
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
    }
}
