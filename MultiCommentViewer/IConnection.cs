using ryu_s.BrowserCookie;
using SitePlugin;
using System;
using System.Collections.Generic;

namespace MultiCommentViewer
{
    public interface IConnection
    {
        event EventHandler<NameChangedEventArgs> NameChanged;
        event EventHandler<CurrentSiteEventArgs> CurrentSiteChanged;
        event EventHandler<IBrowserProfile> CurrentBrowserChanged;
        event EventHandler<IBrowserProfile> BrowserAdded;
        event EventHandler<SitePluginInfo> SiteAdded;
        event EventHandler<Guid> SiteRemoved;
        event EventHandler<ICommentViewModel> CommentReceived;
        event EventHandler CanConnectChanged;
        event EventHandler CanDisconnectChanged;
        event EventHandler<IMetadata> MetadataUpdated;

        Guid Guid { get; }
        bool CanConnect { get; }
        bool CanDisconnect { get; }
        void Connect();
        void Disconnect();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="isAutoSiteSelectionEnabled">入力された値を解析して最適な配信サイトを自動的に選択するか</param>
        void SetInput(string input, bool isAutoSiteSelectionEnabled);

        ConnectionName2 ConnectionName { get; }
        string Input { get; }
        Guid CurrentSiteGuid { get; set; }
        string Name { get; set; }
        bool NeedSave { get; set; }
        IBrowserProfile CurrentBrowserProfile { get; set; }
        IReadOnlyList<ISiteContext> Sites { get; }
        IReadOnlyList<IBrowserProfile> Browsers { get; }
        void AddBrowser(IBrowserProfile browserProfile);
        void AddSiteContext(ISiteContext sitePlugin);
    }
}
