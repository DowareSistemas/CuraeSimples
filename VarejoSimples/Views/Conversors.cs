using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;
using VarejoSimples.Enums;

namespace VarejoSimples.Views
{
    public class Tipo_movimentacaoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return value;

            switch(int.Parse(value.ToString()))
            {
                case (int)Tipo_movimentacao.ENTRADA: return "Entrada";
                case (int)Tipo_movimentacao.SAIDA: return "Saída";
                case (int)Tipo_movimentacao.NENHUM: return "Nenhum";
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
