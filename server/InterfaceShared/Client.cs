using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alcatraz;

namespace Alcatraz
{
    public class Client
    {

        public Alcatraz alcatraz { get; set; }
        public int playerID { get; set; }
        public ClientData clientData { get; set; }

        public Client() { }

        public Client(Alcatraz alcatraz, ClientData clientData)
        {
            this.alcatraz = alcatraz;
            this.playerID = clientData.playerID;
            this.clientData = clientData;
           
        }


    }
}
