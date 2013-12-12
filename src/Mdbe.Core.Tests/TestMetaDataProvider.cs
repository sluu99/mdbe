using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using System.IO;
using Mdbe.Core.Blog;
using System.Linq;

namespace Mdbe.Core.Tests
{
    [TestClass]
    public class TestMetaDataProvider
    {
        [TestMethod]
        public void Test_empty_dir_should_return_nothing()
        {
            var dir = Path.Combine(TestUtils.GetAssemblyDir(), "Test_Data", "Empty");
            var provider = new MetaDataProvider(dir, 0);

            var all = provider.GetAll();

            Assert.AreEqual(0, all.Count());
        }

        [TestMethod, ExpectedException(typeof(DirectoryNotFoundException))]
        public void Test_posts_dir_missing_should_throw_DirectoryNotFoundException()
        {
            var dir = Path.Combine(TestUtils.GetAssemblyDir(), "Test_Data", "Posts_Folder_Missing");
            var provider = new MetaDataProvider(dir, 0);
        }
    }
}
