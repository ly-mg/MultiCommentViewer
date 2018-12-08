using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MultiCommentViewer;
using Moq;
using System.ComponentModel;
using SitePlugin;
using NUnit.Framework;
using Common;

namespace MultiCommentViewerTests
{
    [TestFixture]
    class ConnectionViewModelTests
    {
        //[Test]
        //public void ConnectionViewModel_RaiseRenamedWhenNameChanged()
        //{
        //    var name = new ConnectionName();
        //    var conn = new ConnectionViewModel(name.Guid, new List<SiteViewModel>(), new List<BrowserViewModel>(), null, null);
        //    var newName = "new";
        //    var b = false;
        //    conn.Renamed += (s, e) =>
        //    {
        //        Assert.IsTrue(string.IsNullOrEmpty(e.OldValue));
        //        Assert.AreEqual(newName, e.NewValue);
        //        b = true;
        //    };
        //    conn.Name = newName;
        //    Assert.IsTrue(b);
        //}
        [Test]
        public void ConnectionでNameChangedが起きたらPropertyChangedが発生するか()
        {
            var connectionMock = new Mock<IConnection>();
            connectionMock.SetupGet(c => c.Name).Returns("a");
            connectionMock.SetupGet(c => c.Sites).Returns(new List<ISiteContext>());
            var connection = connectionMock.Object;

            var loggerMock = new Mock<ILogger>();
            
            var logger = loggerMock.Object;
            var called = false;
            var connectionViewModel = new ConnectionViewModel(connection, logger);
            connectionViewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(ConnectionViewModel.Name))
                {
                    called = true;
                }
            };
            connectionMock.SetupGet(c => c.Name).Returns("b");
            connectionMock.Raise(c => c.NameChanged += null, new NameChangedEventArgs("a", "b"));
            Assert.IsTrue(called);

        }
    }
}
