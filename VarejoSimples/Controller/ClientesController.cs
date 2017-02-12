using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VarejoSimples.Model;
using VarejoSimples.Repository;

namespace VarejoSimples.Controller
{
    public class ClientesController
    {
        private ClientesRepository db = null;

        public ClientesController()
        {
            db = new ClientesRepository();
        }

        public bool Save(Clientes c)
        {
            try
            {
                if (!Valid(c))
                    return false;

                if (db.Find(c.Id) == null)
                {
                    c.Id = db.NextId(e => e.Id);
                    db.Save(c);
                }
                else
                    db.Update(c);
                db.Commit();
                BStatus.Success("Cliente salvo");
                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool Valid(Clientes c)
        {
            if (string.IsNullOrWhiteSpace(c.Nome))
            {
                BStatus.Alert("O nome do cliente é obrigatório");
                return false;
            }

            return true;
        }

        public Clientes Find(int id)
        {
            return db.Find(id);
        }

        public Clientes Next(int current_id)
        {
            return db.Where(c => c.Id > current_id).FirstOrDefault();
        }

        public Clientes Prev(int current_id)
        {
            return db.Where(c => c.Id < current_id).OrderByDescending(c => c.Id).FirstOrDefault();
        }

        public List<Clientes> Search(string search)
        {
            return db.Where(e =>
                        e.Nome.Contains(search) ||
                        e.Email.Contains(search) ||
                        e.Cpf.Contains(search) ||
                        e.Municipio.Contains(search) ||
                        e.Logradouro.Contains(search) ||
                        e.Uf.Contains(search) ||
                        e.Id.ToString().Equals(search)).ToList();
        }

        public bool Remove(int id)
        {
            Clientes c = Find(id);
            if (new MovimentosController().CountByCliente(id) > 0)
            {
                BStatus.Alert("Não é possível excluir este cliente. Existem um ou mais movimentos relacionados a ele");
                return false;
            }

            db.Remove(c);
            db.Commit();
            BStatus.Success("Cliente removido");

            return true;
        }
    }
}
