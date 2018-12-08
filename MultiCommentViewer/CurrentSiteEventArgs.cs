using System;

namespace MultiCommentViewer
{
    public class CurrentSiteEventArgs : EventArgs
    {
        public Guid ConnectionGuid { get; }
        public Guid CurrentSiteGuid { get; }
        public CurrentSiteEventArgs(Guid connectionGuid, Guid siteGuid)
        {
            ConnectionGuid = connectionGuid;
            CurrentSiteGuid = siteGuid;
        }
    }
}
