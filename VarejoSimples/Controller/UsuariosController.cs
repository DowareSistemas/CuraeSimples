using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Windows;
using VarejoSimples.Model;
using VarejoSimples.Repository;
using VarejoSimples.Views.Usuario;

namespace VarejoSimples.Controller
{
    public class UsuariosController
    {
        public static Usuarios UsuarioAtual { get; set; }
        public static Lojas LojaAtual { get; set; }

        private UsuariosRepository db = null;
        public UsuariosController()
        {
            db = new UsuariosRepository();
        }

        public bool EfetuaLogin(string usuario, string senha, string loja_id)
        {
            int loja = 0;
            if(!(int.TryParse(loja_id, out loja)))
            {
                MessageBox.Show("Informe a loja!", "Atenção", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }

            if(loja == 0)
            {
                MessageBox.Show("Informe a loja!", "Atenção", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }
            
            List<Usuarios> list = db.Where(i => i.Nome.Equals(usuario) && i.Senha.Equals(senha)).ToList();
            if (list.Count == 1)
            {
                UsuarioAtual = list.First();
                LojaAtual = new LojasController().Find(loja);
                if (UsuarioAtual.Alteracao_pendente)
                    new AlterarSenha(UsuarioAtual).ShowDialog();
                return true;
            }

            return false;
        }

        public bool Save(Usuarios usuario)
        {
            try
            {
                if (!Valid(usuario))
                    return false;

                if (db.Find(usuario.Id) == null)
                {
                    usuario.Alteracao_pendente = true;
                    usuario.Id = db.NextId(e => e.Id);
                    db.Save(usuario);
                }
                else
                    db.Update(usuario);
                db.Commit();
                BStatus.Success("Usuário salvo");
                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool Valid(Usuarios usuario)
        {
            if (string.IsNullOrWhiteSpace(usuario.Nome))
            {
                BStatus.Alert("O nome do usuário é obrigatório");
                return false;
            }

            if (string.IsNullOrEmpty(usuario.Senha))
            {
                BStatus.Alert("A senha do usuário é obrigatória");
                return false;
            }

            return true;
        }

        public bool Delete(int id)
        {
            try
            {
                if (!ValidDelete(id))
                    return false;

                DbContextTransaction transaction = db.Begin(System.Data.IsolationLevel.ReadUncommitted);

                PermissoesController pc = new PermissoesController();
                pc.DeleteByUsuario(id, db.Context);

                Usuarios usuario = Find(id);
                db.Remove(usuario);

                db.Commit();
                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool ValidDelete(int usuario_id)
        {
            if (new VendedoresController().CountByUsuario(usuario_id) > 0)
            {
                BStatus.Alert("Não é possível remover este usuário. Existe um vendedor relacionado a ele.");
                return false;
            }
            return true;
        }

        internal Usuarios Find(int id)
        {
            return db.Find(id);
        }

        public IQueryable<Usuarios> Get(Expression<Func<Usuarios, bool>> query)
        {
            return db.Where(query);
        }

        public List<Usuarios> Search(string search, bool inativos = false)
        {
            int id = 0;

            Expression<Func<Usuarios, bool>> expression = (e => e.Nome.Contains(search));

            if (int.TryParse(search, out id))
                expression = expression.And(e => e.Id == id);

            if (inativos)
                expression = expression.And(e => e.Inativo == false && e.Inativo == true);
            else
                expression = expression.And(e => e.Inativo == false);

            return db.Where(expression).ToList();
        }
    }
}
