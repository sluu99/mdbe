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

        [TestMethod]
        public void GetAll_should_return_one()
        {
            var dir = Path.Combine(TestUtils.GetAssemblyDir(), "Test_Data", "One_Post");
            var provider = new MetaDataProvider(dir, 0);
            Assert.AreEqual(1, provider.GetAll().Count());
        }

        [TestMethod]
        public void GetAll_should_return_multiple()
        {
            var dir = Path.Combine(TestUtils.GetAssemblyDir(), "Test_Data", "Multiple_Posts");
            var provider = new MetaDataProvider(dir, 0);
            Assert.AreEqual(2, provider.GetAll().Count());
        }
        
        [TestMethod]
        public void GetAll_should_only_return_markdown_files()
        {
            var dir = Path.Combine(TestUtils.GetAssemblyDir(), "Test_Data", "Mixed_Files");
            var provider = new MetaDataProvider(dir, 0);
            Assert.AreEqual(1, provider.GetAll().Count());
        }

        [TestMethod]
        public void Get_should_return_null()
        {
            var dir = Path.Combine(TestUtils.GetAssemblyDir(), "Test_Data", "Multiple_Posts");
            var provider = new MetaDataProvider(dir, 0);
            Assert.IsNull(provider.Get("post-zero"));
        }

        [TestMethod]
        public void Get_should_return_MetaData()
        {
            var dir = Path.Combine(TestUtils.GetAssemblyDir(), "Test_Data", "Multiple_Posts");
            var provider = new MetaDataProvider(dir, 0);
            Assert.IsNotNull(provider.Get("post-one"));
            Assert.IsNotNull(provider.Get("post-two"));
        }

        [TestMethod]
        public void Get_should_return_the_same_object_as_GetAll()
        {
            var dir = Path.Combine(TestUtils.GetAssemblyDir(), "Test_Data", "One_Post");
            var provider = new MetaDataProvider(dir, 0);

            var first = provider.GetAll().First();
            Assert.IsNotNull(first);
            Assert.AreEqual(first, provider.Get(first.Slug));
        }

        [TestMethod]
        public void Missing_meta_data_should_be_inferred()
        {
            var dir = Path.Combine(TestUtils.GetAssemblyDir(), "Test_Data", "Add_New_Posts_Here");
            var provider = new MetaDataProvider(dir, 0);

            var post = provider.Get("no-meta-data");
            Assert.IsNotNull(post);
            Assert.AreEqual("no-meta-data", post.Title);
            Assert.AreEqual("no-meta-data", post.Slug);
            Assert.AreEqual(new FileInfo(post.FilePath).CreationTime, post.Date);
        }

        [TestMethod]
        public void Meta_data_should_be_parsed_when_available()
        {
            var dir = Path.Combine(TestUtils.GetAssemblyDir(), "Test_Data", "Add_New_Posts_Here");
            var provider = new MetaDataProvider(dir, 0);

            var post = provider.Get("all-the-meta-data");
            Assert.IsNotNull(post);
            Assert.AreEqual("all-the-meta-data", post.Slug);
            Assert.AreEqual("This file has all the meta data", post.Title);
            Assert.AreEqual(new DateTime(2013,12,14,17,30,0), post.Date);
        }
    }
}
