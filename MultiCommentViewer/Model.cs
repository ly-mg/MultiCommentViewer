using CommentViewerCommon;
using Common;
using ryu_s.BrowserCookie;
using SitePlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiCommentViewer
{
    public interface IModel
    {
        //2018/11/26記
        //設計の方針
        //View,ViewModel,Modelの役割をしっかりと分けたい。
        //これまではとりあえずある程度の状態まで完成させるのを優先してM,V,VMの責務を一部曖昧にして実装してしまった。
        //そもそもどのように分けたら良いかもあんまり良くわかっていなかった部分も大きい。
        //↓のサイトに明確な指針が書いてあって、そのように実装したくなった。
        //http://ugaya40.hateblo.jp/entry/model-mistake
        //
        //Model
        //ViewModelに公開するインタフェースは以下の2つしかない
        //・Modelのステートの公開とその変更通知
        //・Modelの操作のための戻り値のない（void）メソッド
        //また、異常系も当然ViewModel側でハンドリングされることはありません。想定された例外はModel内のステートの変化をもたらすだけ
        //
        //ViewModel
        //ViewModelはModelの影

        //考える必要のあること
        //
        //新しいウインドウを開き、ComboBoxに利用可能なBrowserを列挙したい場合
        //案
        //Modelは1つBrowserが利用可能なことが判明したらその都度eventでViewModelに通知する。
        //ViewModelはその通知を元にBrowserViewModelを構築し、ObservableCollection<BrowserViewModel>に追加する
        //この仕様でMVVMの責務の分離は達成できるだろうか？


        //プラグイン
        //プラグインからほとんど全ての機能を実行できるようにしたい。
        //MainViewのMenuにはPluginの項目があって、Pluginを列挙する。よって明らかにMainView(ViewModel)とPluginの関係は対等ではない。

        event EventHandler<SitePluginInfo> SitePluginLoaded;
        event EventHandler<IBrowserProfile> BrowserProfileLoaded;
        event EventHandler<MetadataUpdatedEventArgs> MetadataUpdated;
        void Init();
        IReadOnlyList<SitePluginInfo> SitePlugins { get; }
        IReadOnlyList<IBrowserProfile> BrowserProfiles { get; }
        IReadOnlyList<IConnection> Connections { get; }
        //戻り値があるメソッド。MVVMの責務の分離ルールを侵害してしまっているだろうか。一応Modelオブジェクトの中身は変更しておらず、単なるステートの取得なのだが。
        //C++であればメソッドにconstを付けているところ。
        IOptionsTabPage GetSitePluginTabPanel(Guid siteContextGuid);
        void SaveSitePluginOptions();
        void SetInput(object connection, string value);
        void AddConnection();


    }
    public class Model
    {
        public event EventHandler<SitePluginInfo> SitePluginLoaded;
        public event EventHandler<IBrowserProfile> BrowserProfileLoaded;
        //public event EventHandler<Guid> ConnectionNameChanged;
        public event EventHandler<IConnection> ConnectionAdded;
        public event EventHandler<Guid> ConnectionRemoved;
        public event EventHandler<CommentReceivedEventArgs> CommentReceived;
        public event EventHandler<MetadataUpdatedEventArgs> MetadataUpdated;
        //static readonly EmptySitePlugin _emptySitePlugin = new EmptySitePlugin();
        //static readonly EmptyBrowserProfile _emptyBrowserProfile = new EmptyBrowserProfile();
        //private SitePluginInfo _defaultSitePluginInfo = new SitePluginInfo(_emptySitePlugin.DisplayName, _emptySitePlugin.Guid);
        //private IBrowserProfile _defaultBrowser = _emptyBrowserProfile;
        private string GetSiteOptionsPath(string displayName)
        {
            var path = System.IO.Path.Combine(_options.SettingsDirPath, displayName + ".txt");
            return path;
        }

        public void Init()
        {
            //サイトプラグインを読み込む
             _sitePluginLoader.LoadSitePlugins(_options, _logger);
            var xs = _sitePluginLoader.GetSiteContexts();
            foreach (var sitePluginInfo in xs)
            {
                try
                {
                    var path = GetSiteOptionsPath(sitePluginInfo.DisplayName);
                    _sitePluginLoader.LoadSiteOptions(sitePluginInfo.Guid, path, _io);
                    //if(_defaultSitePluginInfo.Equals(new Guid()))
                    //{
                    //    _defaultSitePluginInfo = new SitePluginInfo(sitePluginInfo.DisplayName, sitePluginInfo.Guid);
                    //}
                    _sitePlugins.Add(sitePluginInfo);
                    SitePluginLoaded?.Invoke(this, new SitePluginInfo(sitePluginInfo.DisplayName, sitePluginInfo.Guid));
                }
                catch (Exception ex)
                {
                    _logger.LogException(ex);
                }
            }

            //ブラウザを読み込む
            var browserLoader = CreateBrowserLoader();
            var browsers = browserLoader.LoadBrowsers();
            if (browsers.Count() > 0)
            {
                foreach(var browser in browsers)
                {
                    //if(_defaultBrowser == null)
                    //{
                    //    _defaultBrowser = browser;
                    //}
                    _browserProfiles.Add(browser);
                    BrowserProfileLoaded?.Invoke(this, browser);
                }
            }
            else
            {

            }

            //プラグインを読み込む


            //Connectionを読み込む


            //アップデートチェック

        }
        public Guid GetSelectedSite(Guid connectionGuid)
        {
            var connection = GetConnection(connectionGuid);
            return connection.CurrentSiteGuid;
        }

        internal void AddBrowserProfile(IBrowserProfile browserProfile)
        {
            _browserProfiles.Add(browserProfile);
            foreach(var connection in _Connections)
            {
                connection.AddBrowser(browserProfile);
            }
        }

        internal void AddSitePlugin(ISiteContext siteContext)
        {
            _sitePlugins.Add(siteContext);
            foreach (var connection in _Connections)
            {
                connection.AddSiteContext(siteContext);
            }
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="guid1">Connection Guid</param>
        ///// <param name="guid2">SitePlugin Guid</param>
        //internal void SetCurrentSite(Guid guid1, Guid guid2)
        //{
        //    var connection = GetConnection(guid1);
        //    if (connection.SelectedSiteGuid.Equals(guid2))
        //    {
        //        return;
        //    }
        //    connection.SelectedSiteGuid = guid2;
        //    CurrentSiteChanged?.Invoke(this, new CurrentSiteEventArgs(connection.Guid, connection.SelectedSiteGuid));
        //}
        //public void SetCurrentBrowser(Guid connectionGuid, IBrowserProfile browserProfile)
        //{
        //    var connection = GetConnection(connectionGuid);
        //    if (connection.CanConnect.Equals(browserProfile))
        //    {
        //        return;
        //    }
        //    connection.CurrentBrowserProfile = browserProfile;
        //    CurrentBrowserChanged?.Invoke(this, new CurrentBrowserEventArgs(connectionGuid, browserProfile));            
        //}

        private IConnection GetConnection(Guid guid1)
        {
            foreach(var connection in _Connections)
            {
                if (connection.Guid.Equals(guid1))
                {
                    return connection;
                }
            }
            //guidは必ず一致するものが無ければならない。無ければバグ
            throw new BugException("")
            {
                 Details = $"guid={guid1}",
            };
        }

        protected virtual IBrowserLoader CreateBrowserLoader()
        {
            return new BrowserLoader(_logger);
        }

        public IOptionsTabPage GetSitePluginTabPanel(Guid siteContextGuid)
        {
            throw new NotImplementedException();
        }

        public void SaveSitePluginOptions()
        {
            throw new NotImplementedException();
        }
        private string GetDefaultName(IEnumerable<string> existingNames)
        {
            for (var n = 1; ; n++)
            {
                var testName = "#" + n;
                if (!existingNames.Contains(testName))
                {
                    return testName;
                }
            }
        }
        public void AddConnection()
        {
            var name = GetDefaultName(_Connections.Select(c => c.Name));
            SitePluginInfo sitePluginInfo;
            if(_sitePlugins.Count == 0)
            {
                sitePluginInfo = null;
            }
            else
            {
                sitePluginInfo = new SitePluginInfo(_sitePlugins[0].DisplayName, _sitePlugins[0].Guid);
            }

            IBrowserProfile browser;
            if(_browserProfiles.Count == 0)
            {
                browser = null;
            }
            else
            {
                browser = _browserProfiles[0];
            }
            AddConnection(name, "", sitePluginInfo?.DisplayName, browser?.ProfileName, false);
        }
        public void RemoveConnection(Guid connectionGuid)
        {
            var connection = GetConnection(connectionGuid);
            connection.CommentReceived -= Connection_CommentReceived;
            connection.MetadataUpdated -= Connection_MetadataUpdated;


            _Connections.Remove(connection);
            ConnectionRemoved?.Invoke(this, connectionGuid);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="url"></param>
        /// <param name="siteName">null可</param>
        /// <param name="browserName">null可</param>
        /// <param name="needSave"></param>
        internal void AddConnection(string name, string url, string siteName, string browserName, bool needSave)
        {
            var connection = new Connection(_logger)
            {
                Name=name,
            };
            connection.CommentReceived += Connection_CommentReceived;
            connection.MetadataUpdated += Connection_MetadataUpdated;
            connection.SetInput(url, false);
            _Connections.Add(connection);
            ConnectionAdded?.Invoke(this, connection);
            if (_sitePlugins.Count > 0)
            {
                connection.AddSiteContext(_sitePlugins);
            }
            if(_browserProfiles.Count > 0)
            {
                connection.AddBrowser(_browserProfiles);
            }
            
            IBrowserProfile browserProfile = null;
            foreach(var browser in _browserProfiles)
            {
                if(GetBrowserDisplayName(browser) == browserName)
                {
                    browserProfile = browser;
                    break;
                }
            }
            if (browserProfile != null)
            {
                connection.CurrentBrowserProfile = browserProfile;
            }

            ISiteContext sitePlugin = null;
            foreach (var site in _sitePlugins)
            {
                if (site.DisplayName == siteName)
                {
                    sitePlugin = site;
                    break;
                }
            }
            if (sitePlugin != null)
            {
                connection.CurrentSiteGuid = sitePlugin.Guid;
            }
        }

        private void Connection_MetadataUpdated(object sender, IMetadata e)
        {
            var connection = sender as Connection;
            MetadataUpdated?.Invoke(this, new MetadataUpdatedEventArgs(e, connection.ConnectionName));
        }

        private string GetBrowserDisplayName(IBrowserProfile browserProfile)
        {
            if (string.IsNullOrEmpty(browserProfile.ProfileName))
            {
                return $"{browserProfile.Type}";
            }
            else
            {
                return $"{browserProfile.Type}({browserProfile.ProfileName})";
            }
        }
        private void Connection_CommentReceived(object sender, ICommentViewModel e)
        {
            var connection = sender as Connection;
            CommentReceived?.Invoke(this, new CommentReceivedEventArgs(e, connection.ConnectionName));
        }

        protected virtual List<IConnection> _Connections { get; } = new List<IConnection>();
        public IReadOnlyList<IConnection> Connections => _Connections;

        public Model(IOptions options, ILogger logger, IIo io, ISitePluginLoader sitePluginLoader)
        {
            _options = options;
            _logger = logger;
            _io = io;
            _sitePluginLoader = sitePluginLoader;
        }

        private readonly IOptions _options;
        private readonly ILogger _logger;
        private readonly IIo _io;
        private readonly ISitePluginLoader _sitePluginLoader;

        private List<ISiteContext> _sitePlugins { get; } = new List<ISiteContext>();
        public IReadOnlyList<ISiteContext> SitePlugins => _sitePlugins;
        private List<IBrowserProfile> _browserProfiles { get; } = new List<IBrowserProfile>();
        public IReadOnlyList<IBrowserProfile> BrowserProfiles => _browserProfiles;

        public string GetInput(Guid guid)
        {
            var connection = GetConnection(guid);
            return connection.Input;
        }

        public string GetConnectionName(Guid guid)
        {
            var connection = GetConnection(guid);
            return connection.Name;
        }


    }
}
