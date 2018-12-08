using SitePlugin;
using System;

namespace MultiCommentViewer
{
    public class CommentReceivedEventArgs : EventArgs
    {
        public ConnectionName2 ConnectionName { get; }
        public ICommentViewModel CommentContext { get; }
        public CommentReceivedEventArgs(ICommentViewModel cvm, ConnectionName2 connectionName)
        {
            CommentContext = cvm;
            ConnectionName = connectionName;
        }
    }
    public class MetadataUpdatedEventArgs : EventArgs
    {
        public ConnectionName2 ConnectionName { get; }
        public IMetadata Metadata { get; }
        public MetadataUpdatedEventArgs(IMetadata metadata, ConnectionName2 connectionName)
        {
            Metadata = metadata;
            ConnectionName = connectionName;
        }
    }
}
