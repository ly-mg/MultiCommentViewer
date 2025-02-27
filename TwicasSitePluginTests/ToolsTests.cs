﻿using Common;
using NUnit.Framework;
using SitePlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwicasSitePlugin;
using TwicasSitePlugin.LowObject;

namespace TwicasSitePluginTests
{
    [TestFixture]
    class ToolsTests
    {
        [Test]
        public void IsKiitosTest()
        {
            var data = "528091930	0	186	2567	4	5562	452				0	3639	0	0			%203638%09https%3A%2F%2Fs01.twitcasting.tv%2Fimg%2Fitem_tea.png%09%09https%3A%2F%2Fimagegw02.twitcasting.tv%2Fimage3s%2Fpbs.twimg.com%2Fprofile_images%2F994966873480380416%2Ft6IPFRYj_normal.jpg%09eW91NTMxMjIz%09tea%09like%093%09%091%09you531223%09994965646646525952%0944Gh44GE44GT%0944GK6Iy2%092%09%0A%203639%09https%3A%2F%2Ftwitcasting.tv%2Fimg%2Fitem_funding_stamp.png%096YGU5oiQOiAxMDAlIDog8J%2BSm%2BODquOCueODiuODvOOBleOCk%2BOBruOCs%2BODoeODs%2BODiOOBp%2BS4ieinkuOCs%2BODvOODiuODvOmjr%2BS9nOOCi%2FCfkps%3D%09https%3A%2F%2Ftwitcasting.tv%2Fimg%2Fitem_funding_user.png%09a2lpdG9zX2Nhcw%3D%3D%09event.base%09stamp%090%09c3RhbXBpdGVtKCJodHRwczovL3R3aXRjYXN0aW5nLnR2L2ltZy9zdGFtcC9zdGFtcF9mdW5kaW5nXzEwMC5wbmciLDEsMzAwMCk%3D%091%09kiitos_cas%09965900769839915008%0944Kt44O844OI44K5%095pSv5o%2B0OiAxMDAw5YaG%093%09https%3A%2F%2Ftwitcasting.tv%2Fimg%2Fstamp%2Fstamp_funding_100.png%0A		0	0";
            var c = StreamChecker2.ParseStreamChcker(data);
            var item = c.Items[1];
            Assert.IsTrue(Tools.IsKiitos(item));
        }
        [Test]
        public void CreateKiitosMessageTest()
        {
            var data = "528091930	0	186	2567	4	5562	452				0	3639	0	0			%203638%09https%3A%2F%2Fs01.twitcasting.tv%2Fimg%2Fitem_tea.png%09%09https%3A%2F%2Fimagegw02.twitcasting.tv%2Fimage3s%2Fpbs.twimg.com%2Fprofile_images%2F994966873480380416%2Ft6IPFRYj_normal.jpg%09eW91NTMxMjIz%09tea%09like%093%09%091%09you531223%09994965646646525952%0944Gh44GE44GT%0944GK6Iy2%092%09%0A%203639%09https%3A%2F%2Ftwitcasting.tv%2Fimg%2Fitem_funding_stamp.png%096YGU5oiQOiAxMDAlIDog8J%2BSm%2BODquOCueODiuODvOOBleOCk%2BOBruOCs%2BODoeODs%2BODiOOBp%2BS4ieinkuOCs%2BODvOODiuODvOmjr%2BS9nOOCi%2FCfkps%3D%09https%3A%2F%2Ftwitcasting.tv%2Fimg%2Fitem_funding_user.png%09a2lpdG9zX2Nhcw%3D%3D%09event.base%09stamp%090%09c3RhbXBpdGVtKCJodHRwczovL3R3aXRjYXN0aW5nLnR2L2ltZy9zdGFtcC9zdGFtcF9mdW5kaW5nXzEwMC5wbmciLDEsMzAwMCk%3D%091%09kiitos_cas%09965900769839915008%0944Kt44O844OI44K5%095pSv5o%2B0OiAxMDAw5YaG%093%09https%3A%2F%2Ftwitcasting.tv%2Fimg%2Fstamp%2Fstamp_funding_100.png%0A		0	0";
            var c = StreamChecker2.ParseStreamChcker(data);
            var item = c.Items[1];
            var message = Tools.CreateKiitosMessage(item);
            Assert.AreEqual(new List<IMessagePart> { Common.MessagePartFactory.CreateMessageText("キートス") }, message.NameItems);
            Assert.AreEqual(new List<IMessagePart> { Common.MessagePartFactory.CreateMessageText("[支援: 1000円] 達成: 100% : 💛リスナーさんのコメントで三角コーナー飯作る💛\r\n"),
                new MessageImage
                {
                    Url = "https://twitcasting.tv/img/item_funding_stamp.png",
                    Alt  = "キートス",
                    Height =40,
                    Width =40,
                }
            }, message.CommentItems);
            Assert.AreEqual("kiitos_cas", message.UserId);
            Assert.AreEqual("キートス", message.ItemName);
        }
        [Test]
        public void Test()
        {
            Assert.AreEqual("flowitem(\"https://twitcasting.tv/img/anim/anim_tea_10\", 3000, 1, 1, 5)", Tools.DecodeBase64("Zmxvd2l0ZW0oImh0dHBzOi8vdHdpdGNhc3RpbmcudHYvaW1nL2FuaW0vYW5pbV90ZWFfMTAiLCAzMDAwLCAxLCAxLCA1KQ=="));
        }
    }
}
