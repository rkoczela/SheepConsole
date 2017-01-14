using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;
using Sheepshead;
using Microsoft.AspNet.Identity;
namespace WebSheepshead.Helpers
{
    public class ChatSession
    {
        public static List<ChatMessage> messages = new List<ChatMessage>();
        public static List<System.Security.Principal.IIdentity> People = new List<System.Security.Principal.IIdentity>();
        public static Thread ShThread;
        static ChatSession()
        {
            messages.Add(new ChatMessage {
                MessageNumber = 0,
                MessageText =  " Begin messages"
            });
            ShThread = new Thread(PlaySheepshead);
            ShThread.Start();
        }
        public static void AddPerson(System.Security.Principal.IIdentity per)
        {
            if (!People.Any(x => x.Name == per.Name))
            {
                ChatSession.People.Add(per);
            }
            messages.Add(new ChatMessage
            {
                MessageNumber = messages.Count,
                MessageText = per.Name+" logged in."
            });
        }
        public static void PlaySheepshead()
        {
            while (true)
            {
                var peopleOnline = new List<SHPlayer>();
                foreach (var per in People)
                {
                    var humPlayer = new HumanPlayer();
                    humPlayer.WebIdentity = per;
                    humPlayer.Name = per.Name;
                    var hmmbbq = new WebInputter();
                    hmmbbq.pl = humPlayer;
                    humPlayer.Reader = hmmbbq;
                    peopleOnline.Add(humPlayer);
                    if (peopleOnline.Count == 3) break;
                }
                if (peopleOnline.Count == 0) Thread.Sleep(10000);
                else
                {
                    for (int i = peopleOnline.Count; i < 3; i++)
                    {
                        peopleOnline.Add(new AIPlayer
                        {
                            Name = "AIPlayer " + i.ToString()
                        }
                            );
                    }
                    new Game(peopleOnline, new WebOutputter()).Start();
                }
            }
        }
        public static void ResetGame()
        {
           var oldThread = ShThread;
           ShThread = new System.Threading.Thread(Helpers.ChatSession.PlaySheepshead);
           messages.Add(new ChatMessage
            {
                MessageNumber = messages.Count,
                MessageText = "*** GAME RESET  *****"
            });
            ShThread.Start();
            oldThread.Abort();
        }
        public static void KickPlayer(string playerToKick)
        {
            System.Security.Principal.IIdentity toRemove = null;
            foreach (var pl in Helpers.ChatSession.People)
            {
                if (pl.Name == playerToKick)
                {
                    toRemove = pl;

                }
            }
            if (toRemove != null)
            {
                People.Remove(toRemove);
                messages.Add(new ChatMessage
                {
                    MessageNumber = messages.Count,
                    MessageText = "Kicked "+playerToKick
                });
            }
        }
    }
    public class WebOutputter : ISheepOutputter
    {
        public void Output(string message, SHPlayer pl = null)
        {
            if ((pl == null) || (pl.IsHuman))

            {
                var msg = new ChatMessage
                {
                    MessageNumber = ChatSession.messages.Count,
                    MessageText = message
                };
                if (pl!=null)
                {
                    if (pl.IsHuman)
                    {
                        msg.MessageRecipient = ((HumanPlayer)pl).WebIdentity;
                    }
                }
                ChatSession.messages.Add(msg);
            }
        }
    }
    public class WebInputter : ISheepInputter
    {
        public HumanPlayer pl;
        public string Input()
        {
            int oldCount = ChatSession.messages.Count;
            bool done = false;
            int waited = 0;
            while (!done)
            {
                if (ChatSession.messages.Count > oldCount)
                {
                    for (int i=oldCount;i<ChatSession.messages.Count;i++)
                    {
                        Thread.Sleep(500);//kludge - race condition here, need to do locking
                        var msg = ChatSession.messages[i];
                        if ((msg.MessageSender.Name == pl.WebIdentity.Name) && (msg.MessageText.StartsWith("/")))
                        {
                            done = true;
                            return msg.MessageText.Substring(1);
                        }
                        Thread.Sleep(1000);
                        oldCount = ChatSession.messages.Count;
                    }
                }
                Thread.Sleep(1000);
                waited++;
                if (waited == 120)
                {
                    done = true;
                    ChatSession.messages.Add(new ChatMessage
                    {
                        MessageNumber = ChatSession.messages.Count,
                        MessageText = pl.WebIdentity.Name + " is being kicked for inactivity."
                    });
                    ChatSession.KickPlayer(pl.WebIdentity.Name);
                    ChatSession.ResetGame();
                }
            }
            return "";
        }
    }
    public class ChatMessage
    {
        public int MessageNumber = 0;
        public string MessageText = "";
        public System.Security.Principal.IIdentity MessageSender = null;
        public System.Security.Principal.IIdentity MessageRecipient = null;
        public string DisplayText
        {
            get
            {
                return ((MessageSender!=null) ? (MessageSender.GetUserName() + ": "): "") +MessageText;
            }
        }
    }
}