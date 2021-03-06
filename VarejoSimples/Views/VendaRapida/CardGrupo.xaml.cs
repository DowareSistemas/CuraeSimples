﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VarejoSimples.Model;
using VarejoSimples.Views.VendaRapida.UCControllers;

namespace VarejoSimples.Views.VendaRapida
{
    /// <summary>
    /// Interação lógica para CardGrupo.xam
    /// </summary>
    public partial class CardGrupo : UserControl, ICardPair
    {
        private Grupos_produtos Grupo1 { get; set; }
        private Grupos_produtos Grupo2 { get; set; }
        private Grupos_produtos Grupo3 { get; set; }

        public CardGrupo()
        {
            InitializeComponent();

            brd1.Visibility = Visibility.Hidden;
            brd2.Visibility = Visibility.Hidden;
            brd3.Visibility = Visibility.Hidden;
        }

        public bool HandleSelection { get; set; }

        public void FillNewObject(Object obj)
        {
            Grupos_produtos grupo = (Grupos_produtos)obj;

            if (brd1.Visibility == Visibility.Hidden)
            {
                brd1.Visibility = Visibility.Visible;
                lbName1.Text = grupo.Nome;
                Grupo1 = grupo;
                return;
            }

            if (brd2.Visibility == Visibility.Hidden)
            {
                brd2.Visibility = Visibility.Visible;
                lbName2.Text = grupo.Nome;
                Grupo2 = grupo;
                return;
            }

            if (brd3.Visibility == Visibility.Hidden)
            {
                brd3.Visibility = Visibility.Visible;
                lbName3.Text = grupo.Nome;
                Grupo3 = grupo;
            }
        }

        public UserControl CurrentUserControl
        {
            get
            {
                return this;
            }
        }

        public bool HasComplete
        {
            get
            {
                return (brd1.Visibility == Visibility.Visible &&
                    brd2.Visibility == Visibility.Visible &&
                    brd3.Visibility == Visibility.Visible);
            }
        }

        private void FindAndUnSelect()
        {
            StackPanel stackPanel = (Parent as StackPanel);
            foreach(var item in stackPanel.Children)
            {
                ICardPair cardPair = (ICardPair)item;
                cardPair.UnSelectAll();
            }
        }

        public void UnSelectAll()
        {
            brd1.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#FFB93A3A");
            lbName1.Foreground = (SolidColorBrush)new BrushConverter().ConvertFromString("White");

            brd2.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#FFB93A3A");
            lbName2.Foreground = (SolidColorBrush)new BrushConverter().ConvertFromString("White");

            brd3.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#FFB93A3A");
            lbName3.Foreground = (SolidColorBrush)new BrushConverter().ConvertFromString("White");
        }

        private void brd3_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (HandleSelection)
                FindAndUnSelect();
            
            brd3.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("White");
            lbName3.Foreground = (SolidColorBrush)new BrushConverter().ConvertFromString("#FFB93A3A");

            MonitorSelecaoGrupo.Instance.AcionarSelecao(Grupo3);
        }

        private void brd1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (HandleSelection)
                FindAndUnSelect();

            brd1.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("White");
            lbName1.Foreground = (SolidColorBrush)new BrushConverter().ConvertFromString("#FFB93A3A");

            MonitorSelecaoGrupo.Instance.AcionarSelecao(Grupo1);
        }

        private void brd2_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (HandleSelection)
                FindAndUnSelect();

            brd2.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("White");
            lbName2.Foreground = (SolidColorBrush)new BrushConverter().ConvertFromString("#FFB93A3A");

            MonitorSelecaoGrupo.Instance.AcionarSelecao(Grupo2);
        }
    }
}
