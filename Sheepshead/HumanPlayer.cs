using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheepshead
{
    public class HumanPlayer:SHPlayer
    {
        public System.Security.Principal.IIdentity WebIdentity = null;
        public HumanPlayer():base()
        {

            IsHuman = true;
        }
        public override bool RequestPick()
        {
            DisplayHand();
            Displayer.Output("Pick? (Y/N)",this);
            string response = Reader.Input();
            if (response.ToLower().StartsWith("y")) return true;
            else
            {
                Displayer.Output("You mauer!",this);
                return false;
            }
        }
        public void DisplayHand()
        {
            string handStr = "";
            foreach (var card in Hand)
            {
                handStr+=String.Format("{0}{1}  ", card.Value, card.Suit);
            }
            Displayer.Output(handStr,this);
            Displayer.Output("",this);
        }
        public override void PutCardsInBlind(List<Card> Blind)
        {
            Hand = Hand.Concat(Blind).ToList();
            Blind.Clear();
            Sort();
            bool done = false;
            while (!done)
            {
                DisplayHand();
                Displayer.Output("Enter cards to put in blind, separated by comma:",this);
                string response = Reader.Input();
                string[] cards = response.Split(',');
                var card1 = Card.Parse(cards[0].Trim());
                var card2 = Card.Parse(cards[1].Trim());
                if ((card1 == null) || (card2==null))
                {
                    Displayer.Output("Invalid card.");
                }
                else
                {
                    bool found1 = false;
                    bool found2 = false;
                    Card handCard1=null, handCard2=null;
                    foreach (Card handCard in Hand)
                    {
                        if (handCard.Equals(card1))
                        {
                            found1 = true;
                            handCard1 = handCard;
                        }
                        if (handCard.Equals(card2))
                        {
                            found2 = true;
                            handCard2 = handCard;
                        } 
                    }
                    if (!(found1 && found2)) Displayer.Output("You do not have those cards.",this);
                    else
                    {
                        var blindTrick = new SHTrick();
                        blindTrick.Cards.Add(handCard1);
                        blindTrick.Cards.Add(handCard2);
                        PlTricks.Add(blindTrick);
                        Hand.Remove(handCard1);
                        Hand.Remove(handCard2);
                        done = true;
                    }
                }
            }
        }
        public override Card RequestCard()
        {
            bool done = false;
            while (!done)
            {
                Displayer.Output("Pick a card",this);
                DisplayHand();
                string response = Reader.Input();
                var playedCard = Card.Parse(response.Trim());
                if (playedCard != null)
                {
                    foreach (Card handCard in Hand)
                    {
                        if (handCard.Equals(playedCard)) return handCard;
                    }
                    Displayer.Output("You do not have that card.",this);
                }
                else Displayer.Output("Invalid card.",this);
            }
            return new Card();
        }
        public override void Sort()
        {
            Hand.Sort((x, y) =>
            {
                int TrumpDiff = y.TrumpRank - x.TrumpRank;
                if (TrumpDiff != 0) return Math.Sign(TrumpDiff);
                int SuitDiff = (int)x.Suit - (int)y.Suit;
                if (SuitDiff != 0) return Math.Sign(SuitDiff);
                return Math.Sign((int)x.Value - (int)y.Value); 
            });
        }
    }
    
}
