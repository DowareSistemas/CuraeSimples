using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

                if (auto_commit)
                    db.Commit();
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        public int CountByCaixa(int caixa_id)
        {
            return db.Where(m => m.Caixa_id == caixa_id).Count();
        }

        public void SetContext(varejo_config context)
        {
            auto_commit = false;
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
            mc.Forma_pagamento_id = forma_pagamento_id;
            mc.Tipo_mov = (int)Tipo_movimentacao_caixa.ABERTURA;
            mc.Loja_id = UsuariosController.LojaAtual.Id;

            if (!Save(mc))
                BStatus.Error("Ocorreu um problema ao abrir o caixa. Acione o suporte Doware.");
        }

        public bool MovimentarCaixa(Tipo_movimentacao_caixa tipo_mov, decimal valor, int forma_pagamento_id, int movimento_id, string descricao)
        {
            Movimentos_caixas mc = new Movimentos_caixas();
            mc.Caixa_id = GetCaixaAtualUsuario();
            mc.Forma_pagamento_id = forma_pagamento_id;
            mc.Loja_id = UsuariosController.LojaAtual.Id;
            mc.Usuario_id = UsuariosController.UsuarioAtual.Id;
            mc.Valor = valor;
            mc.Movimento_id = movimento_id;
            mc.Tipo_mov = (int)tipo_mov;
            mc.Descricao = descricao;

            return Save(mc);
        }

        public int GetCaixaAtualUsuario()
        {
            int ultimaAbertura = db.Where(m =>
                    m.Usuario_id == UsuariosController.UsuarioAtual.Id &&
                    m.Tipo_mov == (int)Tipo_movimentacao_caixa.ABERTURA
                ).Max(m => m.Id);

            return db.Find(ultimaAbertura).Id;
        }

        private void FechaCaixa(int caixa_id, int usuario_id)
        {

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

            if ((ultimoFechamento == null) && (ultimaAbertura > 0))
                return true;

            if ((ultimoFechamento == null) && (ultimaAbertura == null))
                return false;

            return (ultimaAbertura < ultimoFechamento);
        }
    }
}
