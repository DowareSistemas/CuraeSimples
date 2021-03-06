﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace VarejoSimples.Controller
{
    public class ParameterRebinder : ExpressionVisitor
    {
        private readonly Dictionary<ParameterExpression, ParameterExpression> map;

        public ParameterRebinder(Dictionary<ParameterExpression, ParameterExpression> map)
        {
            this.map = map ?? new Dictionary<ParameterExpression, ParameterExpression>();
        }

        public static Expression ReplaceParameters(Dictionary<ParameterExpression, ParameterExpression> map, Expression exp)
        {
            return new ParameterRebinder(map).Visit(exp);
        }

        protected override Expression VisitParameter(ParameterExpression p)
        {
            ParameterExpression replacement;
            if (map.TryGetValue(p, out replacement))
                p = replacement;

            return base.VisitParameter(p);
        }
    }

    public static class Extensions
    {

        public static void ToMoney(this TextBox txInput)
        {
            txInput.TextWrapping = System.Windows.TextWrapping.NoWrap;
            txInput.AcceptsReturn = false;
            txInput.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Right;
            txInput.PreviewTextInput += TxInput_PreviewTextInput;
            txInput.GotFocus += TxInput_GotFocus;
            txInput.LostFocus += TxInput_LostFocus1;
            txInput.Text = "0,00";
        }

        private static void TxInput_LostFocus1(object sender, System.Windows.RoutedEventArgs e)
        {
            TextBox textBox = (sender as TextBox);
            if (string.IsNullOrWhiteSpace(textBox.Text))
                textBox.Text = "0,00";

            if (string.IsNullOrEmpty(textBox.Text))
                textBox.Text = "0,00";
        }

        public static void ToNumeric(this TextBox txInput, bool acceptTrace = false)
        {
            if (acceptTrace)
                txInput.PreviewTextInput += TxInput_PreviewTextInput2;
            else
                txInput.PreviewTextInput += TxInput_PreviewTextInput1;

            txInput.GotFocus += TxInput_GotFocus;
        }

        private static void TxInput_GotFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            TextBox tx = (TextBox)sender;
            tx.SelectAll();
        }

        private static void TxInput_PreviewTextInput2(object sender, TextCompositionEventArgs e)
        {
            if (e.Text.Last() == '-')
                return;

            Regex rgxNumbers = new Regex("[^0-9]+");
            e.Handled = (rgxNumbers.IsMatch(e.Text));
        }

        private static void TxInput_PreviewTextInput1(object sender, TextCompositionEventArgs e)
        {
            Regex rgxNumbers = new Regex("[^0-9]+");
            e.Handled = (rgxNumbers.IsMatch(e.Text));
        }

        private static void TxInput_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            try
            {
                TextBox txInput = (sender as TextBox);
                txInput.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Right;
                txInput.LostFocus += TxInput_LostFocus;

                Regex rgxNumbers = new Regex("[^0-9]+");
                e.Handled = (rgxNumbers.IsMatch(e.Text) || (e.Text.Equals(",")));
                if (e.Text.Equals(",") || e.Text.Equals("."))
                {
                    if (e.Text.Equals(",") && txInput.Text.Contains(","))
                        return;

                    if (e.Text.Equals(".") && txInput.Text.Contains(","))
                        return;

                    if (e.Text.Equals(",") && txInput.Text.Contains(","))
                        return;

                    if (e.Text.Equals(".") && (txInput.Text.Last().Equals('.') || txInput.Text.Last().Equals(',')))
                        return;

                    if (e.Text.Equals(",") && (txInput.Text.Last().Equals('.') || txInput.Text.Last().Equals(',')))
                        return;

                    txInput.Text += e.Text;
                    txInput.SelectionStart = txInput.Text.Length; // add some logic if length is 0
                    txInput.SelectionLength = 0;

                }
                if (e.Text.Equals("-"))
                {
                    if (txInput.Text.Contains("-"))
                        return;

                    txInput.Text = "-";
                    txInput.SelectionStart = txInput.Text.Length; // add some logic if length is 0
                    txInput.SelectionLength = 0;
                }
            }
            catch (Exception ex)
            {

            }
        }

        private static void TxInput_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                TextBox txInput = (sender as TextBox);
                decimal content = decimal.Parse(txInput.Text);
                txInput.Text = content.ToString("N2");
            }
            catch { }
        }

        public static void AplicarPadroes(this DataGrid dt, bool isRead = true)
        {
            dt.AutoGenerateColumns = false;
            dt.IsReadOnly = isRead;
            dt.CanUserDeleteRows = !isRead;
            dt.CanUserAddRows = !isRead;
            dt.CanUserResizeRows = false;
            dt.SelectionMode = DataGridSelectionMode.Single;
            dt.SelectionUnit = DataGridSelectionUnit.FullRow;
            dt.FontSize = 18;
            dt.MinRowHeight = 30;
            dt.Cursor = Cursors.Hand;
            dt.HorizontalGridLinesBrush = Brushes.LightGray;
            dt.VerticalGridLinesBrush = Brushes.LightGray;

            dt.KeyDown += Dt_KeyDown;
            dt.PreviewKeyDown += Dt_PreviewKeyDown;
        }

        private static void Dt_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                e.Handled = true;
        }

        private static void Dt_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                DataGrid dataGrid = (sender as DataGrid);

                if (dataGrid.Items.Count == 0)
                    return;

                if (e.Key == Key.Insert)
                {
                    if ((dataGrid.SelectedIndex - 1) < 0)
                        return;

                    dataGrid.SelectedItem = dataGrid.SelectedItems[dataGrid.SelectedIndex + 1];
                }

                if (e.Key == Key.Enter)
                {
                    if ((dataGrid.SelectedIndex - 1) < 0)
                        return;
                    dataGrid.SelectedIndex -= 1;
                }

                if (e.Key == Key.Down)
                {
                    if ((dataGrid.SelectedIndex + 1) > (dataGrid.Items.Count - 1))
                        return;

                    dataGrid.SelectedIndex += 1;
                }
                else
                {
                    if ((dataGrid.SelectedIndex - 1) < 0)
                        return;
                    dataGrid.SelectedIndex -= 1;
                }
            }
            catch { }
        }

        public static Expression<T> Compose<T>(this Expression<T> first, Expression<T> second, Func<Expression, Expression, Expression> merge)
        {
            // build parameter map (from parameters of second to parameters of first)
            var map = first.Parameters.Select((f, i) => new { f, s = second.Parameters[i] }).ToDictionary(p => p.s, p => p.f);

            // replace parameters in the second lambda expression with parameters from the first
            var secondBody = ParameterRebinder.ReplaceParameters(map, second.Body);

            // apply composition of lambda expression bodies to parameters from the first expression 
            return Expression.Lambda<T>(merge(first.Body, secondBody), first.Parameters);
        }

        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            return first.Compose(second, Expression.And);
        }

        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            return first.Compose(second, Expression.Or);
        }
    }
}
