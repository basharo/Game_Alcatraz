using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alcatraz
{
    public class ClientData
    {
        public string address { get; set; }
        public int playerID { get; set; }
        public string uniqueName { get; set; }
        public int? port { get; set; }
        public string urlAddition { get; set; }

        public ClientData(string address, int? port, string urlAddition, int playerID, string uniqueName)
        {
            this.address = address;
            this.port = port;
            this.uniqueName = uniqueName;
            this.urlAddition = urlAddition;
            this.playerID = playerID;
        }

    }
}
