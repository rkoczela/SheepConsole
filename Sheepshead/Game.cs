using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheepshead
{
    public class DefaultOutputter:ISheepOutputter
    {

        public void Output(string s, SHPlayer pl = null)
        {
            if ((pl == null) || (pl.IsHuman))
            {
                Console.WriteLine(s);
            }
        }
    }
    public class DefaultInputter : ISheepInputter
    {
        public string Input()
        {
            return Console.ReadLine();
        }
    }
    public class Game
    {
        public Deck GameDeck;
        public List<SHPlayer> GamePlayers;
        public List<Card> Blind;
        public SHTrick TableTrick;
        public ISheepOutputter Displayer;
        public ISheepInputter Reader;
        int Dealer = 2;
        int Picker = -1;
        int CurrentLeader = 1;
        public Game(List<SHPlayer> AvailablePlayers,ISheepOutputter outputter = null, ISheepInputter inputter = null)
        {
            Displayer = outputter ?? new DefaultOutputter();
            Reader = inputter ?? new DefaultInputter();
            GameDeck = new Deck();
            GamePlayers = AvailablePlayers;
            Blind = new List<Card>();
        }
        public void Output (string s, SHPlayer pl = null)
        {
            Displayer.Output(s, pl);
        }
        public void Start()
        {
            DealCards();
            Displayer.Output("*****   NEW GAME STARTING ******");
            Displayer.Output("Players:");
            foreach (var pl in GamePlayers)
            {
                Displayer.Output(pl.Name);
                pl.Displayer = Displayer;
                if (pl.Reader == null) pl.Reader = Reader;
                pl.Sort();
                pl.PlTricks.Clear();
            }
            //GameDeck.Display();
            //Console.WriteLine();
            CallForPicks();
            if (Picker!=-1)
            {
                GamePlayers[Picker].PutCardsInBlind(Blind);
            }
            while (GamePlayers[0].Hand.Count >0)
            {
                PlayTrick();
                Output("");
                CurrentLeader = WhoTookTrick();
                GamePlayers[CurrentLeader].PlTricks.Add(TableTrick);
            }
            if (Picker == -1)
            {
                var winner = GamePlayers.Find(x=> x.Score == GamePlayers.Min(y => y.Score));
                Output(winner.Name + " won with a score of " + winner.Score);
            }
            else
            {
                var picker = GamePlayers[Picker];
                string evtString = "";
                if (picker.PlTricks.Count == 0) evtString = "got no-tricked";
                else if (picker.Score < 31) evtString = "got schneidered";
                else if (picker.Score < 61) evtString = "lost";
                else if (picker.Score < 91) evtString = "won";
                else if (picker.Score < 120) evtString = "schneidered them";
                else evtString = "no-tricked them";
                Output(String.Format("{0} {1} with a score of {2}",picker.Name, evtString, picker.Score));
            }
            //DisplayBlind();
            //DisplayAllHands();
        }
        private int WhoTookTrick()
        {
            bool trumpPresent = false;
            int currentlyTaking = 0;
            trumpPresent = TableTrick.Cards.Any(x => x.SheepSuit == CardSheepSuit.T);
            if (trumpPresent)
            {
                int maxTrumpRank = 0;
                for (int i=0;i<TableTrick.Cards.Count;i++)
                {
                    if (TableTrick.Cards[i].TrumpRank > maxTrumpRank)
                    {
                        currentlyTaking = i;
                        maxTrumpRank = TableTrick.Cards[i].TrumpRank;
                    }
                }
            }
            else
            {
                CardSuit ledSuit = TableTrick.Cards[0].Suit;
                int minValue = 8;
                for (int i = 0; i < TableTrick.Cards.Count; i++)
                {
                    if (TableTrick.Cards[i].Suit == ledSuit)
                    {
                        if ((int)TableTrick.Cards[i].Value < minValue)
                        {
                            currentlyTaking = i;
                            minValue = (int)TableTrick.Cards[i].Value;
                        }
                    }
                }
            }
            Output(GamePlayers[TableTrick.PlayerIndexes[currentlyTaking]].Name + " took trick with " + TableTrick.Cards[currentlyTaking].Value +
                TableTrick.Cards[currentlyTaking].Suit);
            Output("");

            return TableTrick.PlayerIndexes[currentlyTaking];
        }
        private void PlayTrick()
        {
            TableTrick = new SHTrick();
            int i = CurrentLeader;
            bool done = false;
            while (!done)
            {
                bool haveCard = false;
                while (!haveCard)
                {
                    Card playedCard = GamePlayers[i].RequestCard();
                    if (IsValidCard(GamePlayers[i],playedCard))
                    {
                        haveCard = true;
                        Output(String.Format("{0} played {1}{2}", GamePlayers[i].Name, playedCard.Value, playedCard.Suit));
                        GamePlayers[i].Hand.Remove(playedCard);
                        TableTrick.Cards.Add(playedCard);
                        TableTrick.PlayerIndexes.Add(i);
                    }
                    else
                    {
                        Output("Invalid card", GamePlayers[i]);
                    }
                }
                i++;
                if (i == GamePlayers.Count) i = 0;
                if (i == CurrentLeader) done = true;
            }
        }
        private bool IsValidCard(SHPlayer cardPlayer, Card cardToCheck)
        {
            if (TableTrick.Cards.Count == 0)
            {
                return true;
            }
            else
            {
                Card ledCard = TableTrick.Cards[0];
                CardSheepSuit ledSuit = ledCard.SheepSuit;
                if (cardToCheck.SheepSuit == ledSuit) return true;
                else
                {
                    foreach (var handCard in cardPlayer.Hand)
                    {
                        if (handCard.SheepSuit == ledSuit) return false;
                    }
                    return true;
                }
            }
        }
        private void CallForPicks()
        {
            int i = Dealer;
            bool done = false;
            bool dealerDone = false;
            while (!done)
            {
                i++;
                if (i == GamePlayers.Count) i = 0;
                if (GamePlayers[i].RequestPick())
                {
                    Picker = i;
                    done = true;
                    Output(String.Format("{0} picked.",GamePlayers[i].Name));
                }
                else
                {
                    Output(String.Format("{0} mauered.",GamePlayers[i].Name));
                }
                if ((i == Dealer) && (dealerDone)) done = true;
                dealerDone = true;
            }
            if (Picker == -1) Output("Leaster ahhhh");
        }
        private void DealCards()
        {
            GameDeck.Cards.Shuffle<Card>();
            int playerIndex = 0;
            int cardCount = 0;
            foreach (var cardToDeal in GameDeck.Cards)
            {
                if (cardCount > 29)
                {
                    Blind.Add(cardToDeal);
                }
                else
                {
                    var currentPlayer = GamePlayers[playerIndex];
                    currentPlayer.Hand.Add(cardToDeal);
                    playerIndex++;
                    if (playerIndex == GamePlayers.Count) playerIndex = 0;
                }
                cardCount++;
            }
        }
        private void DisplayBlind()
        {
            Console.WriteLine("BLIND:");
            Console.WriteLine("---------------");
            foreach (var card in Blind)
            {
                Console.WriteLine("{0}{1}", card.Value, card.Suit);
            }
        }
        private void DisplayAllHands()
        {
            int i = 1;
            foreach (var pl in GamePlayers)
            {
                Console.WriteLine();
                Console.WriteLine("Player {0}: {1}", i, pl.Name);
                Console.WriteLine("------------------------------------");
                foreach (var card in pl.Hand)
                {
                    Console.WriteLine("{0}{1}", card.Value, card.Suit);
                }
                i++;
            }
        }
        
    }
    static class MyExtensions
    {
        private static Random rng = new Random();

        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }

}
