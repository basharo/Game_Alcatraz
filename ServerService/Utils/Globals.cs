using Akka.Actor;

namespace ServerService.Utils
{
    public static class Globals
    {
        public static ActorSystem mainActorSystem { get; set; }
        public static int groupSize { get; set; }

    }
}
