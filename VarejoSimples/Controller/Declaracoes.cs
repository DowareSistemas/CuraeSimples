using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace DarumaFramework_NFCe
{
   public class Declaracoes
    {
       public static int iRetorno;

        #region Métodos NFCe
        //Métodos de Status
        [DllImport("DarumaFrameWork.dll")]
        public static extern int rStatusWS_NFCe_Daruma();
        [DllImport("DarumaFrameWork.dll")]
        public static extern int rCFVerificarStatus_NFCe_Daruma();

        //Métodos de Emissão
        [DllImport("DarumaFrameWork.dll")]
        public static extern int aCFAbrir_NFCe_Daruma(string pszCPF, string pszNome, string pszLgr, string psznro, string pszBairro, string pszcMun, string pszMunicipio, string pszUF, string pszCEP);
        [DllImport("DarumaFrameWork.dll")]
        public static extern int aCFAbrirNumSerie_NFCe_Daruma(string StrNNF, string StrNSerie, string StrCPF, string StrNome, string StrLgr, string StrNro, string StrBairro, string StrcMun, string StrMunicipio, string StrUF, string StrCEP);        
        [DllImport("DarumaFrameWork.dll")]
        public static extern int aCFVender_NFCe_Daruma(string pszCargaTributaria, string pszQuantidade, string pszPrecoUnitario, string pszTipoDescAcresc, string pszValorDescAcresc, string pszCodigoItem, string pszUnidadeMedida, string pszDescricaoItem);
        [DllImport("DarumaFrameWork.dll")]
        public static extern int aCFVenderCompleto_NFCe_Daruma(string StrAliquota, string StrQuantidade, string StrPrecoUnitario, string StrTipoDescAcresc, string StrValorDescAcresc, string StrCodigoItem, string StrNCMm, string  StrCFOP, string StrUnidadeMedida, string StrDescricaoItem, String StrUsoFuturo);
        [DllImport("DarumaFrameWork.dll")]
        public static extern int aCFTotalizar_NFCe_Daruma(string pszTipoDescAcresc, string pszValorDescAcresc); 
        [DllImport("DarumaFrameWork.dll")]
        public static extern int aCFIdentificarConsumidor_NFCe_Daruma(string StrCPF, string StrNome, string StrLgr, string StrNro, string StrBairro, string StrcMun, string StrMunicipio, string StrUF, string StrCEP, string StrEmail);
        [DllImport("DarumaFrameWork.dll")]
        public static extern int aCFIdentificarTransportadora_NFCe_Daruma(String strCPF_CNPJ, String strNome, String strIE, String strEndereco, String strMunicipio, String strUF);
        [DllImport("DarumaFrameWork.dll")]
        public static extern int aCFValorLeiImposto_NFCe_Daruma(string pszValor); 
        [DllImport("DarumaFrameWork.dll")]
        public static extern int aCFEfetuarPagamento_NFCe_Daruma(string pszFormaPgto, string pszValor);
        [DllImport("DarumaFrameWork.dll")]
        public static extern int tCFEncerrar_NFCe_Daruma(string pszMsgPromocional);
        [DllImport("DarumaFrameWork.dll")]
        public static extern int aCFCancelarItemParcial_NFCe_Daruma(string StrNumItem, string StrQuant);
        [DllImport("DarumaFrameWork.dll")]
        public static extern int aCFCancelarItem_NFCe_Daruma(string StrNumItem);
        [DllImport("DarumaFrameWork.dll")]
        public static extern int aCFLancarAcrescimoItem_NFCe_Daruma(string StrNumItem, string StrTipoAcresc, string StrValorAcresc);
        [DllImport("DarumaFrameWork.dll")]
        public static extern int aCFLancarDescontoItem_NFCe_Daruma(string StrNumItem, string StrTipoDesc, string StrValorDesc);
        [DllImport("DarumaFrameWork.dll")]
        public static extern int aCFCancelarDescontoItem_NFCe_Daruma(string StrNumItem);
        [DllImport("DarumaFrameWork.dll")]
        public static extern int aCFCancelarAcrescimoItem_NFCe_Daruma(string StrNumItem);
        [DllImport("DarumaFrameWork.dll")]
        public static extern int tCFCancelar_NFCe_Daruma(string StrNNF, string StrNSerie, string StrChAcesso, string StrProtAutorizacao, string StrJustificativa);
        [DllImport("DarumaFrameWork.dll")]
        public static extern int aCFEstornarPagamento_NFCe_Daruma(string StrNFormaPagtoEstornado, string StrFormaPagtoEfetivado, string Valor);
        [DllImport("DarumaFrameWork.dll")]
        public static extern int tCFInutilizar_NFCe_Daruma(string StrnNFInic, string StrnNFFim, string StrNSerie, string StrJustificativa);
        
        //Métodos de Impressão
        [DllImport("DarumaFrameWork.dll")]
        public static extern int iCFImprimir_NFCe_Daruma(string pszPathXMLVenda, string strPathRetornoWS, string pszLinkQrCode, int iNumColunas, int iTipoNF);
        [DllImport("DarumaFrameWork.dll")]
        public static extern int iCFReImprimir_NFCe_Daruma(string Str_Param1, string Str_Param2, string Str_Param3);

        //Métodos de Retorno
        [DllImport("DarumaFrameWork.dll")]
        public static extern int rAvisoErro_NFCe_Daruma(StringBuilder StrCodigo, StringBuilder StrMensagem);
        [DllImport("DarumaFrameWork.dll")]
        public static extern int rInfoEstendida_NFCe_Daruma(String StrIndice, StringBuilder StrRetorno);
        [DllImport("DarumaFrameWork.dll")]
        public static extern int rURLQrcode_NFCe_Daruma(StringBuilder strRetornoUrl);
        [DllImport("DarumaFrameWork.dll")]
        public static extern int rRetornarInformacao_NFCe_Daruma(string pszTipoIntervalo, string pszInic, string pszFim, string pszSerie, string pszChaveAcesso, string pszInfoConsulta, StringBuilder pszResposta);
  

       
        //Métodos Impressora DUAL
        [DllImport("DarumaFrameWork.dll")]
        public static extern int iImprimirTexto_DUAL_DarumaFramework(string pszTexto, int iTam);
        [DllImport("DarumaFrameWork.dll")]
        public static extern int eBuscarPortaVelocidade_DUAL_DarumaFramework();
        [DllImport("DarumaFrameWork.dll")]
        public static extern int regAlterarValor_Daruma(string pszPathChave, string pszValor); 
        [DllImport("DarumaFrameWork.dll")]
        public static extern int  regRetornaValorChave_DarumaFramework(string pszProduto, string pszChave, StringBuilder pszValor);
        
        // Método de Configuração NFCe
        [DllImport("DarumaFrameWork.dll")]
        public static extern int regRetornarValor_NFCe_Daruma(string strPathChaveNFCe, StringBuilder strValorRetornado);
        [DllImport("DarumaFrameWork.dll")]
        public static extern int regAlterarValor_NFCe_Daruma(string strPathChaveNFCe, string strValor);

        
        
        
        #endregion

        #region Tratamento de Retorno

       /// <summary>
       /// Método responsavel por traduzir o retorno dos metodos da DFW, recebendo uma variavel inteira  
       /// com o retorno dometodo da DFW e retornando uma string com o significado do código.
       /// </summary>
       /// <param name="iRetMetodo"></param>
       /// <returns></returns>
        public static string TrataRetorno(int iRetMetodo)
        {
          string strRetorno = String.Empty;
            switch (iRetMetodo)
            {
                case 0:
                    strRetorno = "[0] - Não foi possível comunicar com a impressora não fiscal!";
                    break;
                case 1:
                    strRetorno = "[1] - Comando executado com sucesso!";
                    break;
                case -1:
                    strRetorno = "[-1] - Erro encontrado na execução do método!";
                    break;
                case -2:
                    strRetorno = "[-2] - Chave Inválida (Erro retornado pelo webservice)!";
                    break;
                case -3:
                    strRetorno = "[-3] - Falha no schema XML (Erro retornado pelo webservice)!";
                    break;
                case -4:
                    strRetorno = "[-4] - XML fora do padrão (Erro retornado pelo webservice)!";
                    break;
                case -5:
                    strRetorno = "[-5] - Erro genérico (Erro retornado pelo webservice)!";
                    break;
                case -6:
                    strRetorno = "[-6] - Nota já cadastrada na base de dados!";
                    break;
                case -8:
                    strRetorno = "[-8] - Usuário não Autorizado!";
                    break;
                case -9:
                    strRetorno = "[-9] - Usuário não Licenciado!";
                    break;
                case -10:
                    strRetorno = "[-10] - Documento e Ambiente não identificados!";
                    break;
                case -11:
                    strRetorno = "[-11] - Índice inexistente passado como parâmetro!";
                    break;
                case -12:
                    strRetorno = "[-12] - Pagamento informado não existe no cupom!";
                    break;
                case -13:
                    strRetorno = "[-13] - Tipo de Documento não identificado!";
                    break;
                case -14:
                    strRetorno = "[-14] -  Erro retornado pelo WebService!";
                    break;
                case -21:
                    strRetorno = "[-21] - Não existe acréscimo ou desconto a ser cancelado!";
                    break;
                case -30:
                    strRetorno = "[-30] - Lista não possui itens!";
                    break;
                case -31:
                    strRetorno = "[-31] - Quantidade informada não pode ser maior que a quantidade vendida!";
                    break;
                case -40:
                    strRetorno = "[-40] - Tag xml não encontrada!";
                    break;
                case -52:
                    strRetorno = "[-52] - Erro ao gravar em arquivo temporário!";
                    break;
                case -99:
                    strRetorno = "[-99] - Parâmetro inválido ou ponteiro nulo de parâmetro!";
                    break;
                case -100:
                    strRetorno = "[-100] - Estado do cupom inválido para execução o método!";
                    break;
                case -103:
                    strRetorno = "[-103] - Não foram encontradas as DLLs auxiliares (WS_Framework.dll e GNE_Framework.dll)!";
                    break;
                case -109:
                    strRetorno = "[-109] - Chave IDE\\indPres com preenchimento incorreto!";
                    break;
                case -120:
                    strRetorno = "[-120] - Encontrada tag inválida!";
                    break;
                case -121:
                    strRetorno = "[-121] - Estrutura Invalida!";
                    break;
                case -122:
                    strRetorno = "[-122] - Tag obrigatória não foi informada!";
                    break;
                case -123:
                    strRetorno = "[-123] - Tag obrigatória não tem valor preenchido!";
                    break;
                case -130:
                    strRetorno = "[-130] - NFCe ja aberta!";
                    break;
                case -131:
                    strRetorno = "[-131] - NFCe não aberta!";
                    break;
                case -132:
                    strRetorno = "[-132] - NFCe não em fase de venda!";
                    break;
                case -133:
                    strRetorno = "[-133] - NFCe não em fase de totalização!";
                    break;
                case -134:
                    strRetorno = "[-134] - NFCe não em fase de pagamento!";
                    break;
                case -135:
                    strRetorno = "[-135] - NFCe não em fase de encerramento!";
                    break;

                default:
                    strRetorno = "Erro desconhecido";
                    break;
            }
        
          return strRetorno;
         
        }
        #endregion

    }
}
