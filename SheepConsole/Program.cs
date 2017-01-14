using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sheepshead;

namespace SheepConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Sheepshead..AAAAHHH!");
            var peopleOnline = new List<SHPlayer>();
            peopleOnline.Add(new HumanPlayer
            {
                Name = "Human Player"
            });
            for (int i=0;i<2;i++)
            {
                peopleOnline.Add(new AIPlayer
                {
                    Name = "AIPlayer "+i.ToString()
                }
                    );
            }
            while (true)
            {
                new Game(peopleOnline).Start();
            }
            Console.ReadLine();
        }
    }
}
