using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface
{
    public class Players
    {
        public string[,] players;
        public Players()
        { }

        public Players(string[,] players)
        {
            this.players = players;
        }
        public string printPlayerCounter()
        {
            Console.WriteLine(this.players.Length);
            return this.players.Length.ToString();
        }
    }
}
