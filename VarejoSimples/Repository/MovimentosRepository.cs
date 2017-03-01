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
            DateTime? data_inicio, DateTime? data_fim, varejo_config context,
            int pagina_atual, int numero_registros)
        {
            try
            {
                if (data_inicio == null && data_fim == null)
                {
                    MessageBox.Show("Deve ser informado ao menos uma data para o filtro", "Atenção", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return new List<Movimentos>();
                }

                int valor_numerico = 0;
                int.TryParse(busca, out valor_numerico);

                busca = $"%{ busca }%";
                string sql =
@"
SET dateformat YMD

SELECT
    MOVIMENTOS.* 
FROM 
    MOVIMENTOS

INNER JOIN USUARIOS            ON MOVIMENTOS.USUARIO_ID         = USUARIOS.ID
INNER JOIN TIPOS_MOVIMENTO     ON MOVIMENTOS.TIPO_MOVIMENTO_ID  = TIPOS_MOVIMENTO.ID
INNER JOIN PLANOS_CONTAS       ON MOVIMENTOS.PLANO_CONTA_ID     = PLANOS_CONTAS.ID
LEFT JOIN  CLIENTES            ON MOVIMENTOS.CLIENTE_ID         = CLIENTES.ID
LEFT JOIN  FORNECEDORES        ON MOVIMENTOS.FORNECEDOR_ID      = FORNECEDORES.ID 
LEFT JOIN  DOCUMENTOS_FISCAIS  ON MOVIMENTOS.DOCUMENTO_ID       = DOCUMENTOS_FISCAIS.ID

WHERE
    (USUARIOS.NOME               LIKE @AA1 OR
	TIPOS_MOVIMENTO.DESCRICAO   LIKE @AB1 OR
	PLANOS_CONTAS.Descricao     LIKE @AC1 OR
	CLIENTES.NOME               LIKE @AD1 OR
	FORNECEDORES.NOME           LIKE @AE1 OR
	(DOCUMENTOS_FISCAIS.NUMERO_DOCUMENTO = @AF1 OR DOCUMENTOS_FISCAIS.Chave_acesso = @AF2)) AND
";
                
                SqlParameter[] parameters = new SqlParameter[7];

                if (data_fim == null)
                    sql += $"(CONVERT(date, MOVIMENTOS.DATA) <= '{((DateTime)data_fim).ToString("yyyy-MM-dd")}'";

                if (data_inicio == null)
                    sql += $"(CONVERT(date, MOVIMENTOS.DATA) >= '{((DateTime)data_inicio).ToString("yyyy-MM-dd")}'";

                if (data_inicio != null && data_fim != null)
                    sql += $"(CONVERT(date, MOVIMENTOS.DATA)	BETWEEN  '{((DateTime)data_inicio).ToString("yyyy-MM-dd")}' AND '{((DateTime)data_fim).ToString("yyyy-MM-dd")}')";

                sql += $@"
ORDER BY MOVIMENTOS.ID
OFFSET {pagina_atual} ROWS FETCH NEXT {numero_registros} ROWS ONLY";

                parameters[0] = new SqlParameter("@AA1", busca);
                parameters[1] = new SqlParameter("@AB1", busca);
                parameters[2] = new SqlParameter("@AC1", busca);
                parameters[3] = new SqlParameter("@AD1", busca);
                parameters[4] = new SqlParameter("@AE1", busca);
                parameters[5] = new SqlParameter("@AF1", valor_numerico);
                parameters[6] = new SqlParameter("@AF2", valor_numerico);

                List<Movimentos> result = context.Database.SqlQuery<Movimentos>(sql, parameters).ToList();
                return result;
            }
            catch (Exception ex)
            {

            }

            return new List<Movimentos>();
        }

        public int CountPaginacao(string busca,
    DateTime? data_inicio, DateTime? data_fim, varejo_config context)
        {
            try
            {
                if (data_inicio == null && data_fim == null)
                {
                    MessageBox.Show("Deve ser informado ao menos uma data para o filtro", "Atenção", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return 0;
                }

                int valor_numerico = 0;
                int.TryParse(busca, out valor_numerico);

                busca = $"%{ busca }%";
                string sql =
@"
SET dateformat YMD

SELECT
    count(*)
FROM 
    MOVIMENTOS

INNER JOIN USUARIOS            ON MOVIMENTOS.USUARIO_ID         = USUARIOS.ID
INNER JOIN TIPOS_MOVIMENTO     ON MOVIMENTOS.TIPO_MOVIMENTO_ID  = TIPOS_MOVIMENTO.ID
INNER JOIN PLANOS_CONTAS       ON MOVIMENTOS.PLANO_CONTA_ID     = PLANOS_CONTAS.ID
LEFT JOIN  CLIENTES            ON MOVIMENTOS.CLIENTE_ID         = CLIENTES.ID
LEFT JOIN  FORNECEDORES        ON MOVIMENTOS.FORNECEDOR_ID      = FORNECEDORES.ID 
LEFT JOIN  DOCUMENTOS_FISCAIS  ON MOVIMENTOS.DOCUMENTO_ID       = DOCUMENTOS_FISCAIS.ID

WHERE
    (USUARIOS.NOME               LIKE @AA1 OR
	TIPOS_MOVIMENTO.DESCRICAO   LIKE @AB1 OR
	PLANOS_CONTAS.Descricao     LIKE @AC1 OR
	CLIENTES.NOME               LIKE @AD1 OR
	FORNECEDORES.NOME           LIKE @AE1 OR
	(DOCUMENTOS_FISCAIS.NUMERO_DOCUMENTO = @AF1 OR DOCUMENTOS_FISCAIS.Chave_acesso = @AF2)) AND
";

                SqlParameter[] parameters = new SqlParameter[7];

                if (data_fim == null)
                    sql += $"(CONVERT(date, MOVIMENTOS.DATA) <= '{((DateTime)data_fim).ToString("yyyy-MM-dd")}'";

                if (data_inicio == null)
                    sql += $"(CONVERT(date, MOVIMENTOS.DATA) >= '{((DateTime)data_inicio).ToString("yyyy-MM-dd")}'";

                if (data_inicio != null && data_fim != null)
                    sql += $"(CONVERT(date, MOVIMENTOS.DATA)	BETWEEN  '{((DateTime)data_inicio).ToString("yyyy-MM-dd")}' AND '{((DateTime)data_fim).ToString("yyyy-MM-dd")}')";
                
                parameters[0] = new SqlParameter("@AA1", busca);
                parameters[1] = new SqlParameter("@AB1", busca);
                parameters[2] = new SqlParameter("@AC1", busca);
                parameters[3] = new SqlParameter("@AD1", busca);
                parameters[4] = new SqlParameter("@AE1", busca);
                parameters[5] = new SqlParameter("@AF1", valor_numerico);
                parameters[6] = new SqlParameter("@AF2", valor_numerico);

                int result = context.Database.SqlQuery<int>(sql, parameters).First();
                return result;
            }
            catch (Exception ex)
            {

            }

            return 0;
        }
    }
}
