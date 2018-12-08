using ryu_s.BrowserCookie;
using SitePlugin;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MultiCommentViewer
{
    class EmptySitePlugin : ISiteContext
    {
        public Guid Guid => new Guid("6B07E5E8-DFFC-45A9-855A-A6F646B56B2B");

        public string DisplayName => "(無し)";

        public IOptionsTabPage TabPanel => null;

        public ICommentProvider CreateCommentProvider()
        {
            return new EmptyCommentProvider();
        }
        class EmptyCommentProvider : ICommentProvider
        {
            #region ICommentProvider
            public bool CanConnect => false;
            public bool CanDisconnect => false;
#pragma warning disable 0067
            public event EventHandler<ConnectedEventArgs> Connected;
            public event EventHandler<List<ICommentViewModel>> InitialCommentsReceived;
            public event EventHandler<ICommentViewModel> CommentReceived;
            public event EventHandler<IMetadata> MetadataUpdated;
            public event EventHandler CanConnectChanged;
            public event EventHandler CanDisconnectChanged;
#pragma warning restore 0067

            public Task ConnectAsync(string input, IBrowserProfile browserProfile)
            {
                return Task.CompletedTask;
            }

            public void Disconnect()
            {
            }

            public Task<ICurrentUserInfo> GetCurrentUserInfo(IBrowserProfile browserProfile)
            {
                return Task.FromResult<ICurrentUserInfo>(null);
            }

            public IUser GetUser(string userId)
            {
                return null;
            }

            public Task PostCommentAsync(string text)
            {
                return Task.CompletedTask;
            }
            #endregion //ICommentProvider
        }

        public UserControl GetCommentPostPanel(ICommentProvider commentProvider) => null;

        public void Init()
        {
        }

        public bool IsValidInput(string input) => true;

        public void LoadOptions(string path, IIo io)
        {
        }

        public void Save()
        {
        }

        public void SaveOptions(string path, IIo io)
        {
        }
    }
}
