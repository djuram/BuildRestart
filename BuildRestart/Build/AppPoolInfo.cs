using Microsoft.Web.Administration;

namespace BuildRestart.Build
{
    public class AppPoolInfo
    {
        public AppPoolInfo(bool restart, ApplicationPool applicationPool)
        {
            ApplicationPool = applicationPool;
            Restart = restart;
        }

        public ApplicationPool ApplicationPool { get; }

        public bool Restart { get; }
    }
}
