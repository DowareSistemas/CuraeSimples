using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace VarejoSimples.Views.VendaRapida.UCControllers
{
    public interface ICardPair
    {
        UserControl CurrentUserControl { get; }
        bool HasComplete { get; }
        bool HandleSelection { get; set; }
        void FillNewObject(Object obj);
        void UnSelectAll();
    }
}
