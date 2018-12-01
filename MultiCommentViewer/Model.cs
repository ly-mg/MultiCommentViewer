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
        void Init();
        IReadOnlyList<SitePluginInfo> SitePlugins { get; }
        IReadOnlyList<IBrowserProfile> BrowserProfiles { get; }

        //戻り値があるメソッド。MVVMの責務の分離ルールを侵害してしまっているだろうか。一応Modelオブジェクトの中身は変更しておらず、単なるステートの取得なのだが。
        //C++であればメソッドにconstを付けているところ。
        IOptionsTabPage GetSitePluginTabPanel(Guid guid);
        void SaveSitePluginOptions();
        void SetInput(object connection, string value);
        void AddConnection();
    }
    public class Model
    {
        public event EventHandler<SitePluginInfo> SitePluginLoaded;
        public event EventHandler<IBrowserProfile> BrowserProfileLoaded;
        //public event EventHandler<Guid> ConnectionNameChanged;
        public event EventHandler<Connection> ConnectionAdded;

        static readonly EmptySitePlugin _emptySitePlugin = new EmptySitePlugin();
        static readonly EmptyBrowserProfile _emptyBrowserProfile = new EmptyBrowserProfile();
        private string GetSiteOptionsPath(string displayName)
        {
            var path = System.IO.Path.Combine(_options.SettingsDirPath, displayName + ".txt");
            return path;
        }
        private SitePluginInfo _defaultSitePluginInfo =new SitePluginInfo(_emptySitePlugin.DisplayName,_emptySitePlugin.Guid);
        private IBrowserProfile _defaultBrowser = _emptyBrowserProfile;
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
                    if(_defaultSitePluginInfo.Equals(new Guid()))
                    {
                        _defaultSitePluginInfo = new SitePluginInfo(sitePluginInfo.DisplayName, sitePluginInfo.Guid);
                    }
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
                    if(_defaultBrowser == null)
                    {
                        _defaultBrowser = browser;
                    }
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
            return connection.SelectedSiteGuid;
        }

        internal void AddBrowserProfile(IBrowserProfile browserProfile)
        {
            _browserProfiles.Add(browserProfile);
            foreach(var connection in Connections)
            {
                connection.AddBrowser(browserProfile);
            }
        }

        internal void AddSitePlugin(ISiteContext siteContext)
        {
            _sitePlugins.Add(siteContext);
            foreach (var connection in Connections)
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

        private Connection GetConnection(Guid guid1)
        {
            foreach(var connection in Connections)
            {
                if (connection.Guid.Equals(guid1))
                {
                    return connection;
                }
            }
            throw new BugException("")
            {
                 Details = $"guid={guid1}",
            };
        }

        protected virtual IBrowserLoader CreateBrowserLoader()
        {
            return new BrowserLoader(_logger);
        }

        public IOptionsTabPage GetSitePluginTabPanel(Guid guid)
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
            var name = GetDefaultName(Connections.Select(c => c.Name));
            SitePluginInfo sitePluginInfo;
            if(_sitePlugins.Count == 0)
            {
                sitePluginInfo = _defaultSitePluginInfo;
            }
            else
            {
                sitePluginInfo = new SitePluginInfo(_sitePlugins[0].DisplayName, _sitePlugins[0].Guid);
            }

            IBrowserProfile browser;
            if(_browserProfiles.Count == 0)
            {
                browser = _defaultBrowser;
            }
            else
            {
                browser = _browserProfiles[0];
            }
            AddConnection(name, "", sitePluginInfo.DisplayName, browser.ProfileName, false);
        }
        internal void AddConnection(string name, string url, string siteName, string browserName, bool needSave)
        {
            var connection = new Connection()
            {
                Name=name,
            };
            connection.SetInput(url, false);
            Connections.Add(connection);
            ConnectionAdded?.Invoke(this, connection);
            if (_sitePlugins.Count == 0)
            {
                connection.AddSiteContext(_emptySitePlugin);
            }
            else
            {
                connection.AddSiteContext(_sitePlugins);
            }
            if(_browserProfiles.Count == 0)
            {
                connection.AddBrowser(_emptyBrowserProfile);
            }
            else
            {
                connection.AddBrowser(_browserProfiles);
            }

            
            IBrowserProfile browserProfile = _defaultBrowser;
            foreach(var browser in _browserProfiles)
            {
                if(browser.ProfileName == browserName)
                {
                    browserProfile = browser;
                    break;
                }
            }
            connection.CurrentBrowserProfile = browserProfile;

            ISiteContext sitePlugin = _emptySitePlugin;
            foreach (var site in _sitePlugins)
            {
                if (site.DisplayName == siteName)
                {
                    sitePlugin = site;
                    break;
                }
            }
            connection.SelectedSiteGuid = sitePlugin.Guid;
        }
        protected virtual List<Connection> Connections { get; } = new List<Connection>();

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
    interface IConnection
    {

    }

    public class NameChangedEventArgs:EventArgs
    {
        public string OldName{ get; }
        public string NewName { get; }
        public NameChangedEventArgs(string oldName, string newName)
        {
            OldName = oldName;
            NewName = newName;
        }
    }
}
