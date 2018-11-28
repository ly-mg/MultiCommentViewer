using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using ryu_s.BrowserCookie;
using SitePlugin;
using GalaSoft.MvvmLight.CommandWpf;
using System.Diagnostics;
using System.Collections.ObjectModel;
using Common;

namespace MultiCommentViewer
{
    interface IMainViewModel
    {
        event EventHandler<EventArgs> CloseRequested;
        void RequestClose();
        Task InitializeAsync();
    }
	class MainViewModel:IMainViewModel
    {
        #region Commands
        public ICommand ActivatedCommand { get; }
        public ICommand LoadedCommand { get; }
        public ICommand MainViewContentRenderedCommand { get; }
        public ICommand MainViewClosingCommand { get; }
        public ICommand ShowOptionsWindowCommand { get; }
        public ICommand ExitCommand { get; }
        public ICommand ShowWebSiteCommand { get; }
        public ICommand ShowDevelopersTwitterCommand { get; }
        public ICommand CheckUpdateCommand { get; }
        public ICommand ShowUserInfoCommand { get; }
        public ICommand RemoveSelectedConnectionCommand { get; }
        public ICommand AddNewConnectionCommand { get; }
        public ICommand ClearAllCommentsCommand { get; }
        public ICommand CommentCopyCommand { get; }
        public ICommand OpenUrlCommand { get; }
        #endregion //Commands
        private readonly IModel _model;
        private readonly ILogger _logger;

        public event EventHandler<EventArgs> CloseRequested;
		public void RequestClose()
		{
			OnCloseRequested(EventArgs.Empty);
		}

		protected virtual void OnCloseRequested(EventArgs e)
		{
            CloseRequested?.Invoke(this, e);
        }

		public async Task InitializeAsync()
		{
            //use this to test the exception handling
            //throw new NotImplementedException();
            await Task.Delay(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
		}
        public MainViewModel(IModel model, ILogger logger)
        {
            _model = model;
            _logger = logger;
            model.SitePluginLoaded += Model_SitePluginLoaded;
            model.BrowserProfileLoaded += Model_BrowserProfileLoaded;
            AddNewConnectionCommand = new RelayCommand(AddNewConnection);
        }

        private void Model_BrowserProfileLoaded(object sender, IBrowserProfile e)
        {
            var browserProfile = e;

        }

        private void Model_SitePluginLoaded(object sender, ISiteContext e)
        {
            var siteContext = e;

        }
        ObservableCollection<SiteViewModel> _siteVms=new ObservableCollection<SiteViewModel>();
        ObservableCollection<BrowserViewModel> _browserVms = new ObservableCollection<BrowserViewModel>();
        public MainViewModel() : this(new DesignTimeModel())
        {
        }

        private void AddNewConnection()
        {
            try
            {
                var name = GetDefaultName(Connections.Select(c => c.Name));
                var connectionName = new ConnectionName { Name = name };
                var connection = new ConnectionViewModel(connectionName, _siteVms, _browserVms, _logger, _sitePluginLoader);
                connection.Renamed += Connection_Renamed;
                connection.CommentReceived += Connection_CommentReceived;
                connection.InitialCommentsReceived += Connection_InitialCommentsReceived;
                connection.MetadataReceived += Connection_MetadataReceived;
                connection.SelectedSiteChanged += Connection_SelectedSiteChanged;
                var metaVm = new MetadataViewModel(connectionName);
                _metaDict.Add(connection, metaVm);
                MetaCollection.Add(metaVm);
                Connections.Add(connection);
                OnConnectionAdded(connection);
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
                Debug.WriteLine(ex.Message);
                Debugger.Break();
            }
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
    }
    class DesignTimeModel : IModel
    {
        public event EventHandler<ISiteContext> SitePluginLoaded;
        public event EventHandler<IBrowserProfile> BrowserProfileLoaded;

        public void Init()
        {
        }
    }
}
