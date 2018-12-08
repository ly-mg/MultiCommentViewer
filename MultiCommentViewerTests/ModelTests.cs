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
using Moq.Protected;
using CommentViewerCommon;
using ryu_s.BrowserCookie;
using static MultiCommentViewerTests.BrowserViewModelHelper;
namespace MultiCommentViewerTests
{
    public static class BrowserViewModelHelper
    {
        public static string CreateDisplayName(IBrowserProfile browserProfile)
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
    }
    [TestFixture]
    class ModelTests
    {
        Mock<IOptions> _optionsMock = new Mock<IOptions>();
        Mock<ILogger> _loggerMock = new Mock<ILogger>();
        Mock<IIo> _ioMock = new Mock<IIo>();
        Mock<ISitePluginLoader> _sitePluginLoaderMock = new Mock<ISitePluginLoader>();
        Mock<IBrowserLoader> _browserLoaderMock = new Mock<IBrowserLoader>();
        [SetUp]
        public void Setup()
        {

        }
        [Test]
        public void 存在しないブラウザ名が指定された場合は何もしない()
        {
            var connection = Test("Chrome(a)");
            Assert.AreEqual("Chrome(b)", BrowserViewModelHelper.CreateDisplayName(connection.CurrentBrowserProfile));
        }
        [Test]
        public void 存在するブラウザ名が指定された場合はそのブラウザを選択する()
        {
            var connection = Test("Chrome(c)");
            Assert.AreEqual("Chrome(c)", BrowserViewModelHelper.CreateDisplayName(connection.CurrentBrowserProfile));
        }
        public IConnection Test(string profileName)
        {
            var modelMock = new Mock<Model>(_optionsMock.Object, _loggerMock.Object, _ioMock.Object, _sitePluginLoaderMock.Object) { CallBase = true };

            var browserMockB = new Mock<IBrowserProfile>();
            browserMockB.Setup(b => b.ProfileName).Returns("b");

            var browserMockC = new Mock<IBrowserProfile>();
            browserMockC.Setup(b => b.ProfileName).Returns("c");

            _browserLoaderMock.Setup(b => b.LoadBrowsers()).Returns(new List<IBrowserProfile>
            {
                browserMockB.Object,
                browserMockC.Object,
            });
            modelMock.Protected().Setup<IBrowserLoader>("CreateBrowserLoader").Returns(_browserLoaderMock.Object);
            var model = modelMock.Object;
            model.Init();
            IConnection connection = null;
            model.ConnectionAdded += (s, e) =>
            {
                connection = e;
            };
            model.AddConnection("name", "url", "sitename", profileName, false);
            return connection;
        }
    }

}
