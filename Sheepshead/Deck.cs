using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheepshead
{
    public class Deck
    {
        public List<Card> Cards;
        public Deck()
        {
            Initialize();
        }
        private void Initialize()
        {
            Cards = new List<Card>();
            foreach (var suit in Enum.GetValues(typeof(CardSuit)))
            {
                foreach (var value in Enum.GetValues(typeof(CardValue)))
                {
                    Cards.Add(new Card
                    {
                        Suit = (CardSuit)suit,
                        Value = (CardValue)value
                    });
                }
            }
        }
        public void Display()
        {
            int i = 0;
            foreach (var card in Cards)
            {
                i++;
                Console.WriteLine(i + "\t" + card.Value + card.Suit);
            }
        }
    }
    public class Card
    {
        public static Card Parse(string s)
        {
            if (s.Length != 2) return null;
            else
            {
                s = s.ToUpper();
                var strValue = s.Substring(0, 1);
                var strSuit = s.Substring(1, 1);
                CardValue enumValue;
                CardSuit enumSuit;
                try
                {
                     enumValue = (CardValue)Enum.Parse(typeof(CardValue), strValue);
                     enumSuit = (CardSuit)Enum.Parse(typeof(CardSuit), strSuit);

                }
                catch
                {
                    return null;
                }
                return new Card { Suit = enumSuit, Value = enumValue };
            }
        }
        public bool Equals(Card toCompare)
        {
            if ((Suit == toCompare.Suit) && (Value == toCompare.Value)) return true;
            return false;
        }
        public CardSuit Suit { get; set; }
        public CardValue Value { get; set; }
        public CardSheepSuit SheepSuit
        {
            get
            {
                if ((Value == CardValue.Q) || (Value == CardValue.J) || (Suit == CardSuit.D)) return CardSheepSuit.T;
                else
                {
                    return (CardSheepSuit)Enum.Parse(typeof(CardSheepSuit),Enum.GetName(typeof(CardSuit), Suit));
                }
            }
        }
        public int TrumpRank
        {
            get
            {
                if (SheepSuit != CardSheepSuit.T) return 0;
                else
                {
                    if ((Value != CardValue.Q) && (Value != CardValue.J)) return (8 - (int)Value);
                    else
                    {
                        int rank;
                        if (Value == CardValue.J) rank = 7;
                        else rank = 11;
                        rank += (3 - (int)Suit);
                        return rank;
                    }
                }
            }
        }
        public int PointValue
        {
            get
            {
                switch (Value)
                {
                    case CardValue.A: return 11;
                    case CardValue.T: return 10;
                    case CardValue.K: return 4;
                    case CardValue.Q: return 3;
                    case CardValue.J: return 2;
                    default: return 0;
                }
            }
        }
    }
    public enum CardSuit
    {
        C,
        S,
        H,
        D
    };
    public enum CardValue
    {
        Q,
        J,
        A,
        T,
        K,
        N,
        E,
        S
    };
    public enum CardSheepSuit
    {
        T,
        C,
        S,
        H
    };
}
