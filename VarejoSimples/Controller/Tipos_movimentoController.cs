﻿using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using VarejoSimples.Model;
using VarejoSimples.Repository;

namespace VarejoSimples.Controller
{
    public class Tipos_movimentoController
    {
        private Tipos_movimentoRepository db = null;

        public Tipos_movimentoController()
        {
            db = new Tipos_movimentoRepository();
        }

        private bool auto_commit = true;

        public bool Save(Tipos_movimento tm)
        {
            try
            {
                if (!Valid(tm))
                    return false;

                if (db.Find(tm.Id) == null)
                {
                    tm.Id = db.NextId(t => t.Id);
                    db.Save(tm);
                }
                else
                    db.Update(tm);

                if (auto_commit)
                    db.Commit();
                BStatus.Success("Tipo de movimento salvo");
                return true;
            }
            catch
            {
                return false;
            }
        }

        public int CountByPlano_conta(int plano_conta_id)
        {
            return db.Where(e => e.Plano_conta_id == plano_conta_id).Count();
        }

        private bool Valid(Tipos_movimento tm)
        {
            if (tm.Plano_conta_id == 0)
            {
                BStatus.Alert("O plano de conta é obrigatório");
                return false;
            }

            if (string.IsNullOrWhiteSpace(tm.Descricao))
            {
                BStatus.Alert("A descrição do tipo de movimento é obrigatória");
                return false;
            }

            return true;
        }

        public bool Remove(int id)
        {
            try
            {
                if (!ValidRemove(id))
                    return false;

                db.Remove(Find(id));
                db.Commit();
                BStatus.Success("Tipo de movimento removido");
                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool ValidRemove(int id)
        {
            if (new MovimentosController().CountByTipo_movimento(id) > 0)
            {
                BStatus.Alert("Não é possível excluir este tipo de movimento. Existem um ou mais movimentos relacionados a ele");
                return false;
            }

            return true;
        }

        public void SetContext(varejo_config v)
        {
            db.Context = v;
        }

        public Tipos_movimento Find(int id)
        {
            return db.Find(id);
        }

        public List<Tipos_movimento> Search(string search)
        {
            return db.Where(e => e.Descricao.Contains(search)).ToList();
        }

        public Tipos_movimento Next(int current_id)
        {
            return db.Where(t => t.Id > current_id).FirstOrDefault();
        }

        public Tipos_movimento Prev(int current_id)
        {
            return db.Where(t => t.Id < current_id).OrderByDescending(t => t.Id).FirstOrDefault();
        }
    }
}
