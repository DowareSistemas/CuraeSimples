using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace VarejoSimples.Views.VendaRapida.UCControllers
{
    public class AuxiliarSp
    {
        private StackPanel StackPanel { get; set; }
        private Type CardType { get; set; }
        private ICardPair cardPair = null;
        private bool HandleSelection = false;

        public List<ICardPair> CardPairs { get; set; }

        public void Initialize(StackPanel sp, Type cardType, bool handleSelection)
        {
            StackPanel = sp;
            CardType = cardType;
            CardPairs = new List<ICardPair>();
            this.HandleSelection = handleSelection;
        }

        public void AddObject(Object obj)
        {
            if (cardPair == null)
            {
                cardPair = (ICardPair)Activator.CreateInstance(CardType);
                cardPair.HandleSelection = this.HandleSelection;
            }

            if (cardPair.HasComplete)
            {
                CardPairs.Add(cardPair);
                StackPanel.Children.Add(cardPair.CurrentUserControl);
                cardPair = null;
                return;
            }

            cardPair.FillNewObject(obj);

            if (cardPair.HasComplete)
            {
                CardPairs.Add(cardPair);
                StackPanel.Children.Add(cardPair.CurrentUserControl);
                cardPair = null;
            }
        }

        public void VerifyLastAndComplete()
        {
            if(cardPair != null)
                StackPanel.Children.Add(cardPair.CurrentUserControl);
        }
    }
}
