using ryu_s.BrowserCookie;
using System;

namespace MultiCommentViewer
{
    public class CurrentBrowserEventArgs : EventArgs
    {
        public IBrowserProfile BrowserProfile { get; }
        public Guid ConnectionGuid { get; }
        public CurrentBrowserEventArgs(Guid connectionGuid, IBrowserProfile browserProfile)
        {
            ConnectionGuid = connectionGuid;
            BrowserProfile = browserProfile;
        }
    }
}
