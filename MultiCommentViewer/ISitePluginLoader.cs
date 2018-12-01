using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SitePlugin;
using Common;
using System.Windows.Threading;
namespace MultiCommentViewer
{
    public interface ISitePluginLoader
    {
        void LoadSitePlugins(ICommentOptions options, ILogger logger);
        /// <summary>
        /// 終了処理
        /// 終了処理的な名前にしたい
        /// </summary>
        void Save();
        [Obsolete("ISiteContextを直接触れないようにしたい。直接の操作は全てISitePluginLoader内で行う")]
        ISiteContext GetSiteContext(Guid guid);
        IEnumerable<ISiteContext> GetSiteContexts();
        ICommentProvider CreateCommentProvider(Guid guid);
        Guid GetValidSiteGuid(string input);
        System.Windows.Controls.UserControl GetCommentPostPanel(Guid guid, ICommentProvider commentProvider);
        void LoadSiteOptions(Guid guid, string path, IIo io);
    }
    public class SitePluginInfo
    {
        public string DisplayName { get; }
        public Guid Guid { get; }
        public SitePluginInfo(string displayName, Guid guid)
        {
            DisplayName = displayName;
            Guid = guid;
        }
    }
}
