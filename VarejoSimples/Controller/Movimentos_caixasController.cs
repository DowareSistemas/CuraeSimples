using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Windows;
using VarejoSimples.Enums;
using VarejoSimples.Model;
using VarejoSimples.Repository;

namespace VarejoSimples.Controller
{
    public class Movimentos_caixasController
    {
        private Movimentos_caixasRepository db = null;
        private bool auto_commit = true;

        public Movimentos_caixasController()
        {
            db = new Movimentos_caixasRepository();
        }

        public bool Save(Movimentos_caixas mc)
        {
            try
            {
                mc.Id = db.NextId(e => e.Id);
                db.Save(mc);

                db.Commit();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public decimal GetTotalMovimentacoesCaixaAtual(Tipo_movimentacao_caixa tipo)
        {
            int Caixa_id = Get_ID_CaixaAtualUsuario();
            int ultimaAbertura = GetUltimoMovimentoAbertura().Id;
            if (Caixa_id == 0)
                return 0;

            int tipo_mov = (int)tipo;

            decimal? result = db.Where(e =>
                e.Caixa_id == Caixa_id &&
                e.Id > ultimaAbertura &&
                e.Tipo_mov == tipo_mov).Sum(e => (decimal?)e.Valor);

            return (result == null
                ? 0
                : (decimal)result);
        }

        public List<KeyValuePair<Formas_pagamento, decimal>> GetTotaisPorFormaPagamentoCaixaAtual()
        {
            int ultmaAbertura = GetUltimoMovimentoAbertura().Id;
            int caixa_id = Get_ID_CaixaAtualUsuario();
            int tipoMov_abertura = (int)Tipo_movimentacao_caixa.ABERTURA;

            List<Movimentos_caixas> movimentos = GetMovimentosCaixaAtual();
            List<Movimentos_caixas> distinctFormaPg = movimentos.GroupBy(e => e.Forma_pagamento_id).Select(mov => mov.First()).ToList();
            List<KeyValuePair<Formas_pagamento, decimal>> result = new List<KeyValuePair<Formas_pagamento, decimal>>();

            Formas_pagamentoController fpgController = new Formas_pagamentoController();

            foreach (Movimentos_caixas mov in distinctFormaPg)
            {
                Formas_pagamento fpg = fpgController.Find(mov.Forma_pagamento_id);
                result.Add(new KeyValuePair<Formas_pagamento, decimal>(fpg, movimentos.Where(e => e.Forma_pagamento_id == mov.Forma_pagamento_id).Sum(e => e.Valor)));
            }

            return result;
        }

        public decimal GetTotalCaixa()
        {
            int caixa_id = Get_ID_CaixaAtualUsuario();
            int ultimaAbertura = GetUltimoMovimentoAbertura().Id;
            if (caixa_id == 0)
                return 0;

            decimal result = db.Where(e =>
                e.Caixa_id == caixa_id &&
                e.Id >= ultimaAbertura).Sum(e => e.Valor);

            return result;
        }

        internal void DisableAntiTracking()
        {
            db.DisableAntiTracking();
        }

        public int CountByCaixa(int caixa_id)
        {
            return db.Where(m => m.Caixa_id == caixa_id).Count();
        }

        public void SetContext(varejo_config context)
        {
            db.Context = context;
        }

        public static bool CaixaAbertoUsuario(int usuario_id)
        {
            return new Movimentos_caixasController().CaixaAberto(usuario_id);
        }

        public void AbreCaixa(int caixa_id, decimal fundo_troco, int usuario_id, int forma_pagamento_id)
        {
            Movimentos_caixas mc = new Movimentos_caixas();
            mc.Caixa_id = caixa_id;
            mc.Valor = fundo_troco;
            mc.Usuario_id = usuario_id;
            mc.Data = DateTime.Now;
            mc.Forma_pagamento_id = forma_pagamento_id;
            mc.Tipo_mov = (int)Tipo_movimentacao_caixa.ABERTURA;
            mc.Loja_id = UsuariosController.LojaAtual.Id;
            mc.Descricao = "ABERTURA DO CAIXA";

            if (!Save(mc))
                BStatus.Error("Ocorreu um problema ao abrir o caixa. Acione o suporte Doware.");
        }

        public bool MovimentarCaixa(Tipo_movimentacao_caixa tipo_mov,
            decimal valor,
            int forma_pagamento_id,
            int usuario_id,
            int movimento_id,
            string descricao)
        {

            if (string.IsNullOrWhiteSpace(descricao))
            {
                MessageBox.Show("Informe o motivo", "Atenção", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }

            if (usuario_id == 0)
            {
                MessageBox.Show("Informe o usuário", "Atenção", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }

            if (forma_pagamento_id == 0)
            {
                MessageBox.Show("Informe a condição de pagamento", "Atenção", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }

            if (tipo_mov == Tipo_movimentacao_caixa.SAIDA)
            {
                Formas_pagamento fpg = new Formas_pagamentoController().Find(forma_pagamento_id);
                if (fpg.Tipo_pagamento != (int)Tipo_pagamento.DINHEIRO)
                {
                    MessageBox.Show("A condição de pagamento para movimentações de saída no caixa deve ser do tipo 'DINHEIRO'", "Confição de pagamento incompatível", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return false;
                }
            }

            Movimentos_caixas mc = new Movimentos_caixas();
            mc.Caixa_id = Get_ID_CaixaAtualUsuario();
            mc.Forma_pagamento_id = forma_pagamento_id;
            mc.Loja_id = UsuariosController.LojaAtual.Id;
            mc.Usuario_id = usuario_id;
            mc.Valor = (tipo_mov == Tipo_movimentacao_caixa.ABERTURA || tipo_mov == Tipo_movimentacao_caixa.ENTRADA
                ? valor
                : valor * (-1));
            mc.Data = DateTime.Now;
            mc.Movimento_id = movimento_id;
            mc.Tipo_mov = (int)tipo_mov;
            mc.Descricao = descricao;

            return Save(mc);
        }

        public int Get_ID_CaixaAtualUsuario()
        {
            int? ultimaAbertura = db.Where(m =>
                    m.Usuario_id == UsuariosController.UsuarioAtual.Id &&
                    m.Tipo_mov == (int)Tipo_movimentacao_caixa.ABERTURA
                ).Max(m => (int?)m.Id);

            return (ultimaAbertura == null
                ? 0
                : db.Find(ultimaAbertura).Caixa_id);
        }

        public Movimentos_caixas GetUltimoMovimentoAbertura()
        {
            try
            {
                int Caixa_atual_usuario_id = Get_ID_CaixaAtualUsuario();
                if (Caixa_atual_usuario_id == 0)
                    return null;

                int mov_abertura = (int)Tipo_movimentacao_caixa.ABERTURA;

                int? ultimaAbertura = db.Where(m =>
                            m.Caixa_id == Caixa_atual_usuario_id &&
                            m.Tipo_mov == mov_abertura
                        ).Max(m => (int?)m.Id);

                return (ultimaAbertura == null
                    ? null
                    : db.Find(ultimaAbertura));
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<Movimentos_caixas> GetMovimentosCaixaAtual()
        {
            Movimentos_caixas ultimoMovAbertura = GetUltimoMovimentoAbertura();
            if (ultimoMovAbertura == null)
                return new List<Movimentos_caixas>();

            return db.Where(e =>
                e.Caixa_id == ultimoMovAbertura.Caixa_id &&
                e.Id >= ultimoMovAbertura.Id
            ).ToList();
        }

        public bool CaixaAberto(int usuario_id)
        {
            int? ultimoFechamento = db.Where(m =>
                    m.Usuario_id == usuario_id &&
                    m.Tipo_mov == (int)Tipo_movimentacao_caixa.FECHAMENTO
                ).Max(m => (int?)m.Id);

            int? ultimaAbertura = db.Where(m =>
                    m.Usuario_id == usuario_id &&
                    m.Tipo_mov == (int)Tipo_movimentacao_caixa.ABERTURA
                ).Max(m => (int?)m.Id);

            if ((ultimoFechamento == null) && (ultimaAbertura == null))
                return false;

            if ((ultimoFechamento == null) && (ultimaAbertura > 0))
                return true;

            if (ultimoFechamento > ultimaAbertura)
                return false;

            if (ultimaAbertura > ultimoFechamento)
                return true;

            if (ultimaAbertura > 0 && ultimoFechamento == null)
                return true;

            return false;
        }
    }
}