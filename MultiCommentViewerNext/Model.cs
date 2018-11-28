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
    interface IModel
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


        event EventHandler<ISiteContext> SitePluginLoaded;
        event EventHandler<IBrowserProfile> BrowserProfileLoaded;
        void Init();
    }
    class Model:IModel
    {
        public event EventHandler<ISiteContext> SitePluginLoaded;
        public event EventHandler<IBrowserProfile> BrowserProfileLoaded;
        private string GetSiteOptionsPath(string displayName)
        {
            var path = System.IO.Path.Combine(_options.SettingsDirPath, displayName + ".txt");
            return path;
        }
        public void Init()
        {
            //サイトプラグインを読み込む
            var xs = _sitePluginLoader.LoadSitePlugins(_options, _logger);
            foreach (var (displayName, guid) in xs)
            {
                try
                {
                    var path = GetSiteOptionsPath(displayName);
                    var siteContext = _sitePluginLoader.GetSiteContext(guid);
                    siteContext.LoadOptions(path, _io);
                    _siteContexts.Add(siteContext);
                    SitePluginLoaded?.Invoke(this, siteContext);
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
        protected virtual IBrowserLoader CreateBrowserLoader()
        {
            return new BrowserLoader(_logger);
        }
        public Model(ISitePluginLoader SitePluginLoader, IOptions options, ILogger logger, IIo io)
        {
            _sitePluginLoader = SitePluginLoader;
            _options = options;
            _logger = logger;
            _io = io;
        }

        private readonly ISitePluginLoader _sitePluginLoader;
        private readonly IOptions _options;
        private readonly ILogger _logger;
        private readonly IIo _io;
        List<ISiteContext> _siteContexts = new List<ISiteContext>();
        
    }
    interface IConnection
    {

    }
    class Connecion:IConnection
    {

    }

}
