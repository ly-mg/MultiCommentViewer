using CommentViewerCommon;
using Common;
using Moq;
using Moq.Protected;
using MultiCommentViewer;
using NUnit.Framework;
using ryu_s.BrowserCookie;
using SitePlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiCommentViewerTests
{
    [TestFixture]
    class ConnectionTests
    {
        ILogger logger = new Mock<ILogger>().Object;
        [Test]
        public void ChangeNameTest()
        {
            var connection = new Connection(logger);
            var actualOld = "";
            var actualNew = "";
            connection.NameChanged += (s, e) =>
            {
                actualOld = e.OldName;
                actualNew = e.NewName;
            };
            connection.Name = "a";
            Assert.IsNull(actualOld);
            Assert.AreEqual("a", actualNew);
        }
        [Test]
        public void SetInputの第二引数にfalseを渡したらサイトの自動選択は発動しない()
        {
            var connection = new Connection(logger);
            var before = connection.CurrentSiteGuid;
            connection.SetInput("a", false);
            Assert.AreEqual("a", connection.Input);
            Assert.AreEqual(before, connection.CurrentSiteGuid);
        }
        [Test]
        public void SitePluginやBrowserProfileが登録されていない場合は代替のものを使用()
        {
            var ioMock = new Mock<IIo>();
            var loggerMock = new Mock<ILogger>();
            var optionsMock = new Mock<IOptions>();
            var sitePluginLoaderMock = new Mock<ISitePluginLoader>();

            var options = optionsMock.Object;
            var logger = loggerMock.Object;
            var io = ioMock.Object;
            var sitepluginLoader = sitePluginLoaderMock.Object;

            var model = new Model(options, logger, io, sitepluginLoader);
            IConnection connection = null;
            model.ConnectionAdded += (s, e) =>
            {
                connection = e;
            };
            model.AddConnection();
            Assert.AreEqual("#1", connection.Name);
            Assert.AreEqual(1, connection.Sites.Count);
            Assert.AreEqual("(無し)", connection.Sites[0].DisplayName);

            Assert.AreEqual(1, connection.Browsers.Count);
            Assert.AreEqual("(無し)", connection.Browsers[0].ProfileName);
        }
        [Test]
        public void SitePluginやBrowserProfileが事前に登録されている場合はそれを使用()
        {
            var ioMock = new Mock<IIo>();
            var loggerMock = new Mock<ILogger>();
            var optionsMock = new Mock<IOptions>();
            optionsMock.Setup(o => o.SettingsDirPath).Returns("");
            var commentProviderMock = new Mock<ICommentProvider>();
            var siteContextMock1 = new Mock<ISiteContext>();
            siteContextMock1.Setup(s => s.DisplayName).Returns("site1");
            siteContextMock1.Setup(s=> s.CreateCommentProvider()).Returns(commentProviderMock.Object);
            var sitePluginLoaderMock = new Mock<ISitePluginLoader>();
            sitePluginLoaderMock.Setup(s => s.GetSiteContexts()).Returns(new List<ISiteContext>
            {
                siteContextMock1.Object,
            });
            var browserProfileMock = new Mock<IBrowserProfile>();
            browserProfileMock.Setup(b => b.ProfileName).Returns("browser1");
            var browserLoaderMock = new Mock<IBrowserLoader>();
            browserLoaderMock.Setup(b => b.LoadBrowsers()).Returns(new List<IBrowserProfile>
            {
                browserProfileMock.Object,
            });

            var options = optionsMock.Object;
            var logger = loggerMock.Object;
            var io = ioMock.Object;
            var sitepluginLoader = sitePluginLoaderMock.Object;
            var browserLoader = browserLoaderMock.Object;

            var modelMock = new Mock<Model>(options, logger, io, sitepluginLoader) { CallBase = true };
            modelMock.Protected().Setup<IBrowserLoader>("CreateBrowserLoader").Returns(browserLoader);
            var model = modelMock.Object;

            model.Init();
            IConnection connection = null;
            model.ConnectionAdded += (s, e) =>
            {
                connection = e;
            };
            model.AddConnection();
            Assert.AreEqual("#1", connection.Name);
            Assert.AreEqual(1, connection.Sites.Count);
            Assert.AreEqual("site1", connection.Sites[0].DisplayName);

            Assert.AreEqual(1, connection.Browsers.Count);
            Assert.AreEqual("browser1", connection.Browsers[0].ProfileName);
        }
        [Test]
        public void SitePluginやBrowserProfileが後から登録されたらそれを使用()
        {
            var ioMock = new Mock<IIo>();
            var loggerMock = new Mock<ILogger>();
            var optionsMock = new Mock<IOptions>();
            optionsMock.Setup(o => o.SettingsDirPath).Returns("");
            var commentProviderMock = new Mock<ICommentProvider>();
            var siteContextMock1 = new Mock<ISiteContext>();
            siteContextMock1.Setup(s => s.DisplayName).Returns("site1");
            siteContextMock1.Setup(s => s.CreateCommentProvider()).Returns(commentProviderMock.Object);
            var sitePluginLoaderMock = new Mock<ISitePluginLoader>();
            sitePluginLoaderMock.Setup(s => s.GetSiteContexts()).Returns(new List<ISiteContext>
            {
            });
            var browserProfileMock = new Mock<IBrowserProfile>();
            browserProfileMock.Setup(b => b.ProfileName).Returns("browser1");
            var browserLoaderMock = new Mock<IBrowserLoader>();
            browserLoaderMock.Setup(b => b.LoadBrowsers()).Returns(new List<IBrowserProfile>
            {
            });

            var options = optionsMock.Object;
            var logger = loggerMock.Object;
            var io = ioMock.Object;
            var sitepluginLoader = sitePluginLoaderMock.Object;
            var browserLoader = browserLoaderMock.Object;

            var modelMock = new Mock<Model>(options, logger, io, sitepluginLoader) { CallBase = true };
            modelMock.Protected().Setup<IBrowserLoader>("CreateBrowserLoader").Returns(browserLoader);
            var model = modelMock.Object;

            model.Init();
            IConnection connection = null;
            model.ConnectionAdded += (s, e) =>
            {
                connection = e;
            };
            model.AddConnection();
            model.AddSitePlugin(siteContextMock1.Object);
            model.AddBrowserProfile(browserProfileMock.Object);
            Assert.AreEqual("#1", connection.Name);
            Assert.AreEqual(1, connection.Sites.Count);
            Assert.AreEqual("site1", connection.Sites[0].DisplayName);

            Assert.AreEqual(1, connection.Browsers.Count);
            Assert.AreEqual("browser1", connection.Browsers[0].ProfileName);
        }
        [Test]
        public void sksksk()
        {
            var connection = new Connection(logger);
            //connection.SelectedSiteGuid
            Guid removedGuid=new Guid();
            string addedSiteDisplayName = null;

            connection.SiteRemoved += (s, e) =>
            {
                removedGuid = e;
            };
            connection.SiteAdded += (s, e) =>
            {
                addedSiteDisplayName = e.DisplayName;
            };
            var siteContextMock1 = new Mock<ISiteContext>();
            siteContextMock1.Setup(s => s.DisplayName).Returns("site1");
            var commentProviderMock = new Mock<ICommentProvider>();
            siteContextMock1.Setup(s => s.CreateCommentProvider()).Returns(commentProviderMock.Object);
            var siteContext = siteContextMock1.Object;
            connection.AddSiteContext(siteContext);

            Assert.AreEqual(new EmptySitePlugin().Guid, removedGuid);
            Assert.AreEqual("site1", addedSiteDisplayName);
        }
    }
}
