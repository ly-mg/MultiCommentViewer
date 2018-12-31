using SitePlugin;
using Plugin;
using System.Collections.Generic;
using System.Linq;
using System;
using Common;
using System.Diagnostics;

namespace MultiCommentViewer
{
    public interface IPluginHost2: IPluginHost
    {
        ///// <summary>
        ///// 設定を保存するディレクトリの絶対パス
        ///// </summary>
        //string SettingsDirPath { get; }
        //double MainViewLeft { get; }
        //double MainViewTop { get; }
        //bool IsTopmost { get; }
        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="pluginName"></param>
        ///// <param name="s">serialized options</param>
        //void SaveOptions(string path, string s);
        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="pluginName"></param>
        ///// <returns>serialized options</returns>
        //string LoadOptions(string path);
        ///// <summary>
        ///// 全ての接続中のConnectionにコメントを投稿する
        ///// </summary>
        //void PostCommentToAll(string comment);
        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="guid"></param>
        ///// <param name="comment"></param>
        //void PostComment(string guid, string comment);
        //IEnumerable<IConnectionStatus> GetAllConnectionStatus();
    }
    public class PluginHost2: IPluginHost2
    {
        public string SettingsDirPath => _model.SettingsDirPath;

        public double MainViewLeft => _model.MainViewLeft;

        public double MainViewTop => _model.MainViewTop;
        public bool IsTopmost => _model.IsTopmost;
        public string LoadOptions(string path)
        {
            var s = _model.LoadPluginOptions(path);
            return s;
        }

        public void SaveOptions(string path, string s)
        {
            _model.SavePluginOptions(path, s);
            //_io.WriteFile(path, s);
        }

        public void PostCommentToAll(string comment)
        {
            _model.PostCommentToAll(comment);
            //foreach (var connection in _vm.Connections)
            //{
            //    try
            //    {
            //        var cp = connection.CommentProvider;
            //        await cp.PostCommentAsync(comment);
            //    }
            //    catch (Exception ex)
            //    {
            //        Debug.WriteLine($"PluginHost.PostCommentToAll(string) ConnectionName={connection.Name}, ex={ex.Message}");
            //        _logger.LogException(ex);
            //    }
            //}
        }

        public void PostComment(string guid, string comment)
        {
            _model.PostComment(guid, comment);
        }

        public IEnumerable<IConnectionStatus> GetAllConnectionStatus()
        {
            return _model.Connections.Cast<IConnectionStatus>();
        }

        //private readonly IOptions _options;
        //private readonly IIo _io;
        //private readonly ILogger _logger;
        private readonly Model _model;

        public PluginHost2(Model model)//, IOptions options, IIo io, ILogger logger)
        {
            _model = model;
            //_options = options;
            //_io = io;
            //_logger = logger;
        }
    }
    public class PluginHost : IPluginHost
    {
        public string SettingsDirPath => _options.SettingsDirPath;

        public double MainViewLeft => _options.MainViewLeft;

        public double MainViewTop => _options.MainViewTop;
        public bool IsTopmost => _options.IsTopmost;
        public string LoadOptions(string path)
        {
            var s = _io.ReadFile(path);
            return s;
        }

        public void SaveOptions(string path, string s)
        {
            _io.WriteFile(path, s);
        }

        public async void PostCommentToAll(string comment)
        {
            foreach(var connection in _vm.Connections)
            {
                try
                {
                    //var cp = connection.CommentProvider;
                    //await cp.PostCommentAsync(comment);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"PluginHost.PostCommentToAll(string) ConnectionName={connection.Name}, ex={ex.Message}");
                    _logger.LogException(ex);
                }
            }
        }

        public void PostComment(string guid, string comment)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<IConnectionStatus> GetAllConnectionStatus()
        {
            return _vm.Connections.Cast<IConnectionStatus>();
        }

        private readonly MainViewModel _vm;
        private readonly IOptions _options;
        private readonly IIo _io;
        private readonly ILogger _logger;

        public PluginHost(MainViewModel vm, IOptions options, IIo io, ILogger logger)
        {
            //TODO:MainViewModelを直接受け取るのではなく、interfaceを受け取りたい
            _vm = vm;
            _options = options;
            _io = io;
            _logger = logger;
        }
    }
}

