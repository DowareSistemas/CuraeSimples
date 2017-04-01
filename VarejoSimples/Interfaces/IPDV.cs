using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VarejoSimples.Model;

namespace VarejoSimples.Interfaces
{
    public interface IPDV
    {
        void VendeItem(Estoque estoque = null);
        void CancelarVenda();
        IPainelVenda PainelVenda { get; set; }
    }
}
