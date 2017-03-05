using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using VarejoSimples.Enums;

namespace VarejoSimples.Interfaces
{
    public interface ITelaTransferenciaConta
    {
        int Conta_id { get; }
        int Plano_conta_id { get; }
        Tipo_lancamento Tipo_lancamento { get; }
        decimal Valor { get; set; }
        UserControl CurrentView { get; }
    }
}
