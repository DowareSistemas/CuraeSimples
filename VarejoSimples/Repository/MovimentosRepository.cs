using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Windows;
using VarejoSimples.Interfaces;
using VarejoSimples.Model;

namespace VarejoSimples.Repository
{
    public class MovimentosRepository : RepositoryImpl<Movimentos>, IMovimentos
    {
        public List<Movimentos> BuscaGenericaMovimentos(string busca,
            DateTime? data_inicio, DateTime? data_fim,
            int pagina_atual, int numero_registros)
        {
            if (data_inicio == null && data_fim == null)
            {
                MessageBox.Show("Deve ser informado ao menos uma data para o filtro", "Atenção", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return new List<Movimentos>();
            }

            int valor_numerico = 0;
            int.TryParse(busca, out valor_numerico);
            var baseQuery = GetBaseQuery(busca, data_inicio, data_fim).
                OrderBy(e => e.Id).Skip(pagina_atual).Take(numero_registros).AsEnumerable();

            return baseQuery.ToList();
        }

        public int CountPaginacao(string busca,
    DateTime? data_inicio, DateTime? data_fim)
        {
            if (data_inicio == null && data_fim == null)
            {
                MessageBox.Show("Deve ser informado ao menos uma data para o filtro", "Atenção", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return 0;
            }

            int valor_numerico = 0;
            int.TryParse(busca, out valor_numerico);
            var query = GetBaseQuery(busca, data_inicio, data_fim).AsEnumerable();

            return query.Count();
        }

        private IQueryable<Movimentos> GetBaseQuery(string busca,
    DateTime? data_inicio, DateTime? data_fim)
        {
            if (data_fim != null)
                data_fim = ((DateTime)data_fim).AddHours(23).AddMinutes(59);

            int valor_numerico = 0;
            int.TryParse(busca, out valor_numerico);

            var q = (from movimentos in Context.Movimentos.AsNoTracking()
                     join usuarios in Context.Usuarios.AsNoTracking() on movimentos.Usuario_id equals usuarios.Id
                     join tipos_movimento in Context.Tipos_movimento.AsNoTracking() on movimentos.Tipo_movimento_id equals tipos_movimento.Id
                     join clientes in Context.Clientes.AsNoTracking() on movimentos.Cliente_id equals clientes.Id into cli
                     from clients in cli.DefaultIfEmpty()
                     join fornecedores in Context.Fornecedores.AsNoTracking() on movimentos.Fornecedor_id equals fornecedores.Id into frn
                     from forn in frn.DefaultIfEmpty()

                     where
                    (usuarios.Nome.Contains(busca) ||
                     tipos_movimento.Descricao.Contains(busca) ||
                     clients.Nome.Contains(busca) ||
                     forn.Nome.Contains(busca) ||

                     (data_inicio != null && data_fim == null) // Tem data inicio | Não tem data fim
                         ? movimentos.Data >= data_inicio // apartir da data inicio
                         : (data_inicio == null && data_fim != null) // Não tem data inicio | Tem data fim
                         ? movimentos.Data <= data_fim // menor que a data fim
                         : (movimentos.Data >= data_inicio && movimentos.Data <= data_fim)
                      )

                     select movimentos);

            return q;
        }
    }
}
