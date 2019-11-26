using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface
{
    public static class Globals
    {
        public static ActorSystem mainActorSystem { get; set; }
        public static int groupSize { get; set; }
    }
}
