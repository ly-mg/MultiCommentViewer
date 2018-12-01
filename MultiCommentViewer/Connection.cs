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
            public bool CanConnect => false;

            public bool CanDisconnect => false;

            public event EventHandler<ConnectedEventArgs> Connected;
            public event EventHandler<List<ICommentViewModel>> InitialCommentsReceived;
            public event EventHandler<ICommentViewModel> CommentReceived;
            public event EventHandler<IMetadata> MetadataUpdated;
            public event EventHandler CanConnectChanged;
            public event EventHandler CanDisconnectChanged;

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
    public class Connection:IConnection
    {
        public event EventHandler<NameChangedEventArgs> NameChanged;
        public event EventHandler<CurrentSiteEventArgs> CurrentSiteChanged;
        public event EventHandler<IBrowserProfile> CurrentBrowserChanged;
        public event EventHandler<IBrowserProfile> BrowserAdded;
        public string Name
        {
            get => ConnectionName2.Name;
            set
            {
                if(ConnectionName2.Name == value)
                {
                    return;
                }
                ConnectionName2.Name = value;
                NameChanged?.Invoke(this, new NameChangedEventArgs(_beforeName, ConnectionName2.Name));
                _beforeName = ConnectionName2.Name;
            }
        }
        private string _beforeName;
        public Guid Guid => ConnectionName2.Guid;

        private ISiteContext _selectedSite;
        public Guid SelectedSiteGuid
        {
            get => _selectedSite.Guid;
            set
            {
                if (_selectedSite != null && _selectedSite.Guid.Equals(value))
                {
                    return;
                }
                var sitePluginInfo = _siteDitct[value];
                _selectedSite = sitePluginInfo;
                CurrentSiteChanged?.Invoke(this, new CurrentSiteEventArgs(Guid, value));
            }
        }
        public event EventHandler<SitePluginInfo> SiteAdded;
        public event EventHandler<Guid> SiteRemoved;
        List<ISiteContext> _sites = new List<ISiteContext>();
        public List<ISiteContext> Sites
        {
            get
            {
                //if(_siteDitct.Count > 0)
                //{
                    return _sites;
                //}
                //else
                //{
                //    return new List<ISiteContext> { _emptySitePlugin };
                //}
            }
        }
        Dictionary<Guid, ISiteContext> _siteDitct = new Dictionary<Guid, ISiteContext>();
        public void AddSiteContext(ISiteContext sitePlugin)
        {
            _sites.Add(sitePlugin);
            _siteDitct.Add(sitePlugin.Guid, sitePlugin);
            SiteAdded?.Invoke(this, new SitePluginInfo(sitePlugin.DisplayName, sitePlugin.Guid));
        }
        public void AddSiteContext(List<ISiteContext> sitePlugins)
        {
            foreach(var site in sitePlugins)
            {
                AddSiteContext(site);
            }
        }
        public void RemoveSite(ISiteContext siteContext)
        {
            _sites.Remove(siteContext);
            _siteDitct.Remove(siteContext.Guid);
            SiteRemoved?.Invoke(this, siteContext.Guid);
        }
        List<IBrowserProfile> _browsers = new List<IBrowserProfile>();
        public List<IBrowserProfile> Browsers
        {
            get
            {
                return _browsers;
            }
        }
        public void AddBrowser(IBrowserProfile browserProfile)
        {
            _browsers.Add(browserProfile);
            if(_currentBrowserProfile == null)
            {
                _currentBrowserProfile = browserProfile;
            }
            BrowserAdded?.Invoke(this, browserProfile);
        }
        public void AddBrowser(List<IBrowserProfile> browserProfiles)
        {
            foreach(var browser in browserProfiles)
            {
                AddBrowser(browser);
            }
        }
        IBrowserProfile _currentBrowserProfile;
        public IBrowserProfile CurrentBrowserProfile
        {
            get => _currentBrowserProfile;
            set
            {
                if (_currentBrowserProfile != null && _currentBrowserProfile.Equals(value))
                    return;
                _currentBrowserProfile = value;
                CurrentBrowserChanged?.Invoke(this, _currentBrowserProfile);
            }
        }
        private string _input;
        /// <summary>
        /// 入力値
        /// 値をセットする場合はSetInput()を使用する
        /// </summary>
        public string Input
        {
            get => _input;
            private set
            {
                if (_input == value) return;
                _input = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="isAutoSiteSelectionEnabled">入力された値を解析して最適な配信サイトを自動的に選択するか</param>
        public void SetInput(string input, bool isAutoSiteSelectionEnabled)
        {
            if (isAutoSiteSelectionEnabled)
            {
                foreach(var site in _sites)
                {
                    var b = site.IsValidInput(input);
                    if (b)
                    {
                        SelectedSiteGuid = site.Guid;
                        break;
                    }
                }
            }
            Input = input;
        }
        ConnectionName2 ConnectionName2 { get; } = new ConnectionName2();
        public bool CanConnect { get; internal set; }
        public bool CanDisconnect { get; internal set; }

        ICommentProvider _cp;
        public Connection()
        {
            CanConnect = true;
            CanDisconnect = false;
            _beforeName = ConnectionName2.Name;
        }
    }
    class AutoSiteSelector
    {

    }
}
