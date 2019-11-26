
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Alcatraz
{
    class ClientClass : MoveListener
    {

        private int numPlayer;
        private Alcatraz[] other;

        public ClientClass(int numberOfPlayers)
        {
            other = new Alcatraz[numberOfPlayers];
            numPlayer = numberOfPlayers;
        }

        public Client initializeClient(int playerID, int numberOfPlayer)
        {
            ClientClass clientClass = new ClientClass(numberOfPlayer);
            Alcatraz clientAlcatraz = new Alcatraz();

            clientClass.setNumPlayer(numPlayer);
            clientAlcatraz.init(numPlayer, playerID);

            for (int j = 1; j < numPlayer + 1; j++)
            {
                int help = j - 1;
                clientAlcatraz.getPlayer(help).Name = "Player " + j;
            }
            return new Client(clientClass, clientAlcatraz, playerID);

        }

        public void setOther(int i, Alcatraz t)
        {
            this.other[i] = t;
        }
        public int getNumPlayer()
        {
            return numPlayer;
        }

        public void setNumPlayer(int numPlayer)
        {
            this.numPlayer = numPlayer;
        }


        public void doMove(Player player, Prisoner prisoner, int rowOrCol, int row, int col)
        {
            Console.WriteLine("moving " + prisoner + " to " + (rowOrCol == Alcatraz.ROW ? "row" : "col") + " " + (rowOrCol == Alcatraz.ROW ? row : col));
            Console.WriteLine("ID" + player.Id);

           //Move move = new Move(player, prisoner, rowOrCol, row, col, player.Id);
            //akka send to other players


           /* for (int i = 0; i < getNumPlayer() - 1; i++)
            {
                other[i].doMove(other[i].getPlayer(player.Id), other[i].getPrisoner(prisoner.Id), rowOrCol, row, col);
                Console.WriteLine("Player " + other[i].getPlayer(player.Id) + "Prisoner " + prisoner + "row " + row + "col" + col);
            }*/
        }

        public void undoMove()
        {
            Console.WriteLine("Undoing move");
        }

        public void gameWon(Player player)
        {
            Console.WriteLine("Player " + player.Id + " wins.");
        }

        public static void main(String[] args)
        {
            Console.WriteLine("main main main");
        }
    }
}
