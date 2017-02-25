using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using VarejoSimples.Enums;
using VarejoSimples.Model;
using VarejoSimples.Repository;

namespace VarejoSimples.Controller
{
    public class ContasController
    {
        private ContasRepository db = null;

        public ContasController()
        {
            db = new ContasRepository();
        }

        public bool Save(Contas conta)
        {
            try
            {
                if (!Valid(conta))
                    return false;

                if (db.Find(conta.Id) == null)
                {
                    conta.Id = db.NextId(e => e.Id);
                    db.Save(conta);
                }
                else
                    db.Update(conta);

                db.Commit();
                BStatus.Success("Conta salva.");
                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool Valid(Contas conta)
        {
            if(string.IsNullOrWhiteSpace(conta.Nome))
            {
                BStatus.Alert("Informe o nome da conta");
                return false;
            }

            if(conta.Tipo == (int)Tipo_conta.CONTA_BANCARIA)
            {
                if(string.IsNullOrWhiteSpace(conta.Titular))
                {
                    BStatus.Alert("Informe o titular");
                    return false;
                }

                if(conta.Banco_numero == 0)
                {
                    BStatus.Alert("O número do banco é obrigatório");
                    return false;
                }

                if(string.IsNullOrWhiteSpace(conta.Nome_banco))
                {
                    BStatus.Alert("Informe o nome do banco");
                    return false;
                }

                if(string.IsNullOrWhiteSpace(conta.Agencia))
                {
                    BStatus.Alert("Informe a agência");
                    return false;
                }

                if (string.IsNullOrWhiteSpace(conta.Conta))
                {
                    BStatus.Alert("Informe o numero da conta");
                    return false;
                }

                if(string.IsNullOrWhiteSpace(conta.Carteira))
                {
                    BStatus.Alert("Informe a carteira");
                    return false;
                }

                if(string.IsNullOrWhiteSpace(conta.Nosso_numero))
                {
                    BStatus.Alert("Informe o Nosso Número");
                    return false;
                }
            }

            return true;
        }

        public Contas Find(int id)
        {
            return db.Find(id);
        }

        public Contas Next(int current_id)
        {
            return db.Where(e => e.Id > current_id).FirstOrDefault();
        }

        public Contas Prev(int current_id)
        {
            return db.Where(e => e.Id < current_id).OrderByDescending(e => e.Id).FirstOrDefault();
        }

        public bool Remove(int id)
        {
            try
            {
                Contas conta = Find(id);
                if(conta.Movimentos_contas.Count > 0)
                {
                    BStatus.Alert("Não é possível excluir esta conta. Ela está presente em uma ou mais movimentações.");
                    return false;
                }

                db.Remove(conta);
                db.Commit();
                BStatus.Success("A conta foi removida");

                return true;
            }
            catch
            {
                return false;
            }
        }

        public List<Contas> Search(string search, bool inativos = false)
        {
            Expression<Func<Contas, bool>> query = (e =>
                e.Nome.Contains(search) ||
                e.Nome_banco.Contains(search) ||
                e.Conta.Equals(search) ||
                e.Id.ToString().Equals(search) ||
                e.Titular.Contains(search));

            if (!inativos)
                query = query.And(e => e.Inativa == false);

            return db.Where(query).ToList();
        }
    }
}
