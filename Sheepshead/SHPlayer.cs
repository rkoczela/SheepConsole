using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheepshead
{
    public class SHPlayer
    {
        public List<Card> Hand;
        public string Name = "";
        public List<SHTrick> PlTricks = new List<SHTrick>();
        public ISheepOutputter Displayer;
        public ISheepInputter Reader = null;
        public bool IsHuman = false;
        public SHPlayer()
        {
            Hand = new List<Card>();
        } 
        public virtual bool RequestPick()
        {
            Random rnd = new Random();
            int retVal = rnd.Next(0, 2);
            bool realRetVal = retVal==1;
            return realRetVal; 
        }
        public virtual void PutCardsInBlind(List<Card> Blind)
        {
            PlTricks.Add(new SHTrick
            {
                Cards = Blind
            });
        }
        public virtual void Sort()
        {
        }
        public virtual Card RequestCard()
        {
            Random rnd = new Random();
            int cardToPlay = rnd.Next(0, Hand.Count);
            return Hand[cardToPlay];
        }
        public int Score
        {
            get
            {
                return PlTricks.Sum(x => x.Cards.Sum(y => y.PointValue));
            }
        }
    }
}
