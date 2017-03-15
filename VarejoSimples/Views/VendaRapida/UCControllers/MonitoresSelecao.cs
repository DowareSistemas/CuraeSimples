using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VarejoSimples.Model;

namespace VarejoSimples.Views.VendaRapida.UCControllers
{
    public class MonitorSelecaoGrupo
    {
        public delegate void SelecaoGrupo(Grupos_produtos grupo);
        public event SelecaoGrupo GrupoSelecionado;

        private MonitorSelecaoGrupo()
        {

        }

        public void AcionarSelecao(Grupos_produtos grupo)
        {
            if (GrupoSelecionado != null)
                GrupoSelecionado(grupo);
        }

        private static MonitorSelecaoGrupo instance = null;
        public static MonitorSelecaoGrupo Instance
        {
            get
            {
                if (instance == null)
                    instance = new MonitorSelecaoGrupo();

                return instance;
            }
        }
    }

    public class MonitorSelecaoProduto
    {
        public delegate void SelecaoProduto(Produtos produto);
        public event SelecaoProduto ProdutoSelecionado;

        private MonitorSelecaoProduto()
        {

        }

        public void AcionarSelecao(Produtos produto)
        {
            if (ProdutoSelecionado != null)
                ProdutoSelecionado(produto);
        }

        private static MonitorSelecaoProduto instance = null;
        public static MonitorSelecaoProduto Instance
        {
            get
            {
                if (instance == null)
                    instance = new MonitorSelecaoProduto();

                return instance;
            }
        }
    }


}
