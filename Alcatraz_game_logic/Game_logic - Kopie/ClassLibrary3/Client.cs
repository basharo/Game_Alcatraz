using Alcatraz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlcatrazGame
{
    class Client
    {
        private ClientClass clientClass;
        private Alcatraz alcatraz;
        private int playerID;

        private string address;
        private int port;


        public Client()
        {
            
        }
        public Client(ClientClass clientClass,Alcatraz alcatraz, int playerID, string address, int port)
        {
            this.clientClass = clientClass;
            this.alcatraz = alcatraz;
            this.playerID = playerID;
            this.address = address;
            this.port = po
        }

        public void setPlayerID(int playerID)
        {
            this.playerID = playerID;
        }
        public int getPlayerID()
        {
            return playerID;
        }        

        public ClientClass getClientClass()
        {
            return clientClass;
        }
        public Alcatraz getAlcatraz()
        {
            return alcatraz;
        }
    }
}
