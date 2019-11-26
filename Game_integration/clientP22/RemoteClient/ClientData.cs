using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alcatraz
{
    class ClientData
    {
        private string address;
        private int playerID;
        private string uniqueName;
        private int port;
        private string urlAddition;

        public ClientData(string address, int port,string urlAddition,int playerID, string uniqueName)
        {
            this.address = address;
            this.port = port;
            this.uniqueName = uniqueName;      
            this.urlAddition = urlAddition;
            this.playerID = playerID;
        }

        public void setAddress(string address)
        {
            this.address = address;
        }
        public string getAddress()
        {
            return this.address;
        }
        public void setPlayerID(int playerID)
        {
            this.playerID = playerID;
        }
        public string getPlayerID()
        {
            return this.address;
        }
        public void setUniqueName(string uniqueName)
        {
            this.uniqueName = uniqueName;
        }
        public string getUniqueName()
        {
            return this.uniqueName;
        }
        public void setPort(int port)
        {
            this.port = port;
        }
        public int getPort()
        {
            return this.port;
        }
        public void setUrlAddition(string urlAddition)
        {
            this.urlAddition = urlAddition;
        }
        public string getUrlAddition()
        {
            return urlAddition;
        }

    }
}
