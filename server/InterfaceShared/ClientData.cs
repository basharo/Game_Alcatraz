using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface
{
    public class ClientData
    {

        public string playerName { get; set; }
        public string address { get; set; }
        public int playerID { get; set; }


        public ClientData(string playerName, string address, int playerID)
        {
            this.playerName = playerName;
            this.address = address;
            this.playerID = playerID;
        }

        public ClientData()
        {
        }
    }
}
