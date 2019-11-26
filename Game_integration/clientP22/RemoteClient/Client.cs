using Alcatraz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alcatraz
{
    public class Client
    {
        private Alcatraz alcatraz;
        private int playerID;

       // private string address;
       // private int port;

        public Client()
        {
            
        }
        public Client(Alcatraz alcatraz, int playerID)
        {
            //this.clientClass = clientClass;
            this.alcatraz = alcatraz;
            this.playerID = playerID;
         //   this.port = port;
           // this.address = address;
        }

        public void setPlayerID(int playerID)
        {
            this.playerID = playerID;
        }
        public int getPlayerID()
        {
            return playerID;
        }        

        //public void setPort(int port)
        //{
        //    this.port = port;
        //}
        //public int getPort()
        //{
        //    return this.port;
        //}

        public Alcatraz getAlcatraz()
        {
            return alcatraz;
        }
    }
}
