using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface
{
    public class Client
    {

        public int SequenceID { get; set; }              //seqence ID assigned by Server at registration
        public string UniqueName { get; set; }           // unique player name   
        public int PreferredGroupSize { get; set; }      //  preferred group size            
        public string Address { get; set; }              // address of client to contact him back         
                                                         //public int port { get; }                        //port which is used for the

        public Client(int sequenceID, string uniqueName, int preferredGroupSize, String Address /*,int port*/)
        {
            this.SequenceID = sequenceID;
            this.UniqueName = uniqueName;
            this.PreferredGroupSize = preferredGroupSize;
            this.Address = Address;
            //this.port = port;
        }
    }
}
