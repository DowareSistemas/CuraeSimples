using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Windows;
using VarejoSimples.Model;
using VarejoSimples.Repository;

namespace VarejoSimples.Controller
{
    public class ParametrosController
    {
        private ParametrosRepository db = null;

        public ParametrosController()
        {
            db = new ParametrosRepository();
        }

        public static Parametros FindParametro(string nome, bool filtrar_computador = false)
        {
            return new ParametrosController().FindParametroLojaAtual(nome, filtrar_computador);
        }

        public Parametros FindParametroLojaAtual(string nome, bool filtrar_computador)
        {
            Expression< Func<Parametros, bool>> query = (
                    e => e.Nome.Equals(nome) &&
                    e.Loja_id == UsuariosController.LojaAtual.Id);

            if (filtrar_computador)
                query = query.And(p => p.Computador.Equals(Environment.MachineName));

            return db.Where(query).FirstOrDefault();
        }

        public bool InsereParametro(string parametro, string computador, string valor)
        {
            if (string.IsNullOrWhiteSpace(computador))
            {
                MessageBox.Show(@"O nome do computador dedicado para a nova regra não foi informado.
O recurso de criação de novas regras de parâmetro, servem para atender ambientes onde
existe uma mesma regra de negócio, porém,
em alguns setores da empresa, essa regra pode ser ignorada.

Neste caso o sistema diferencia pelo nome do computador em rede.
Aplicar uma regra sem informar o computador, quebraria os outros 
fatores que diferenciam a mesma.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }

            if (db.Where(p => p.Nome.Equals(parametro) && p.Computador.Equals(computador)).Count() > 0)
            {
                MessageBox.Show("O parâmetro informado já foi atribuido para o mesmo computador. Informe outro computador ou outro parâmetro.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }

            Parametros baseP = FindParametroLojaAtual(parametro, false);

            Parametros param = new Parametros();
            param.Nome = parametro;
            param.Descricao = baseP.Descricao;
            param.Computador = computador;
            param.Valor = valor;
            param.Permite_multi = true;
            param.Loja_id = UsuariosController.LojaAtual.Id;

            try
            {
                db.Save(param);
                db.Commit();
                MessageBox.Show("Nova regra de parâmetro adicionada!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocorreu um problema ao inserir a nova regra de parâmetro. Acione o suporte Doware.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public void ExcluirRegra(string parametro, string computador)
        {
            try
            {
                if(db.Where(p => p.Nome.Equals(parametro)).Count() == 1)
                {
                    MessageBox.Show("Não é possível excluir esta regra.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                string sql = $"DELETE FROM Parametros WHERE Nome = '{parametro}' AND Computador = '{computador}'";
                if (db.ExecSQL(sql))
                    MessageBox.Show("Regra removida!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                else
                    MessageBox.Show("Ocorreu um problema ao excluir a regra. Acione o suporte Doware.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch
            {
                MessageBox.Show("Ocorreu um problema ao excluir a regra. Acione o suporte Doware.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void SetValorParametro(string parametro, string old_computador, string computador, string valor)
        {
            Parametros param = db.Where(p => p.Nome.Equals(parametro) && p.Computador.Equals(old_computador)).First();
            param.Computador = (string.IsNullOrWhiteSpace(computador)
               ? "T"
               : computador);
            param.Valor = valor;

            try
            {
                string sql = "update Parametros set Valor = @pValor, Computador = @pNewPC where Nome = @pNome and Computador = @oOldPC";
                SqlParameter[] aSql = new SqlParameter[4];
                aSql[0] = new SqlParameter("@pValor", valor);
                aSql[1] = new SqlParameter("@pNewPC", computador);
                aSql[2] = new SqlParameter("@pNome", parametro);
                aSql[3] = new SqlParameter("@oOldPC", old_computador);

                db.ExecSQL(sql, aSql);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocorreu um problema ao alteraro o parâmetro. Acione o suporte Doware.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(computador))
                MessageBox.Show("O computador dedicado para o parâmetro não foi informado. \nNote que nesse caso, é aplicado o atributo 'T', que indica todos os computadores da rede da empresa", "Atenção", MessageBoxButton.OK, MessageBoxImage.Exclamation);

            MessageBox.Show("Parâmetro de sistema alterado com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        internal List<Parametros> Search(string text)
        {
            return db.Where(
                p => p.Nome.Equals(text) ||
                p.Descricao.Contains(text) ||
                p.Computador.Equals(text)).ToList();
        }
    }
}
