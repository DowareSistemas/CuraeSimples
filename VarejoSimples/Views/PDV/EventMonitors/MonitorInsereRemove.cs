using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VarejoSimples.Model;

namespace VarejoSimples.Views.PDV.EventMonitors
{
    public class MonitorInsereRemove
    {
        public delegate void InsereItem(Estoque estoque);
        public event InsereItem ItemInserido;

        public delegate void RemoveItem(Estoque estoque);
        public event RemoveItem ItemRemovido;

        public void AcionarInsercao(Estoque estoque)
        {
            if (ItemInserido != null)
                ItemInserido(estoque);
        }

        public void AcionarRemocao(Estoque estoque)
        {
            if (ItemRemovido != null)
                ItemRemovido(estoque);
        }

        private static MonitorInsereRemove instance = null;

        private MonitorInsereRemove()
        {

        }

        public static MonitorInsereRemove Instance
        {
            get
            {
                if (instance == null)
                    instance = new MonitorInsereRemove();

                return instance;
            }
        }
    }
}
