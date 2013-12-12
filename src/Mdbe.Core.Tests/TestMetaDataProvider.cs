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
        public void Empty_dir_should_return_nothing()
        {
            // directory created by post-build script
            var dir = Path.Combine(TestUtils.GetAssemblyDir(), "Test_Data", "Empty");
            var provider = new MetaDataProvider(dir, 0);

            var all = provider.GetAll();

            Assert.AreEqual(0, all.Count());
        }

        [TestMethod, ExpectedException(typeof(DirectoryNotFoundException))]
        public void Posts_dir_missing_should_throw_DirectoryNotFoundException()
        {
            // directory created by post-build script
            var dir = Path.Combine(TestUtils.GetAssemblyDir(), "Test_Data", "Posts_Folder_Missing");
            var provider = new MetaDataProvider(dir, 0);
        }

        [TestMethod, ExpectedException(typeof(DirectoryNotFoundException))]
        public void Nonexisting_dir_should_throw_DirectoryNotFoundException()
        {
            var dir = Path.Combine(TestUtils.GetAssemblyDir(), "Nonexisting");
            var provider = new MetaDataProvider(dir, 0);
        }
    }
}
