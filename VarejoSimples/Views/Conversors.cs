using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;
using VarejoSimples.Enums;

namespace VarejoSimples.Views
{
    public class Tipo_mov_caixaConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return value;

            switch(int.Parse(value.ToString()))
            {
                case (int) Tipo_movimentacao_caixa.ABERTURA: return "ABERTURA";
                case (int)Tipo_movimentacao_caixa.FECHAMENTO: return "FECHAMENTO";
                case (int)Tipo_movimentacao_caixa.ENTRADA: return "ENTRADA";
                case (int)Tipo_movimentacao_caixa.SAIDA: return "SAÍDA";
                case (int)Tipo_movimentacao_caixa.TROCO: return "TROCO";
            }

            return "NÃO IDENTIFICADO";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class Tipo_contaConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return value;

            switch(int.Parse(value.ToString()))
            {
                case (int)Tipo_conta.CONTA_BANCARIA: return "CONTA BANCÁRIA";
                case (int)Tipo_conta.CONTA_CAIXA: return "CONTA CAIXA";
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class Classe_plano_contaConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return value;

            switch(int.Parse(value.ToString()))
            {
                case (int)Classe_plano_conta.DESPESA: return "Despesa";
                case (int)Classe_plano_conta.RECEITA: return "Receita";
                case (int)Classe_plano_conta.NAO_CLASSIFICADO: return "Não classificado";
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class Tipo_plano_contaConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return value;

            switch(int.Parse(value.ToString()))
            {
                case (int)Tipo_plano_conta.ANALITICO: return "Analítico";
                case (int)Tipo_plano_conta.SINTETICO: return "Sintético";
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

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
