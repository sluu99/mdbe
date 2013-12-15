using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using Mdbe.Core.Blog;

namespace Mdbe.Core.Tests
{
    [TestClass]
    public class TestPostProvider
    {
        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void Should_throw_ArgumentNullException_for_MetaDataProvider()
        {
            new PostProvider(null);
        }

        [TestMethod]
        public void Get_should_return_posts_supplied_by_MetaDataProvider()
        {
            var dir = Path.Combine(TestUtils.GetAssemblyDir(), "Test_Data", "Multiple_Posts");
            var mdp = new MetaDataProvider(dir, 0);
            var pp = new PostProvider(mdp);

            Assert.IsNotNull(pp.Get("post-one"));
            Assert.IsNotNull(pp.Get("post-two"));
        }

        [TestMethod]
        public void Get_should_return_posts_with_the_same_MetaData_instance()
        {
            var dir = Path.Combine(TestUtils.GetAssemblyDir(), "Test_Data", "Multiple_Posts");
            var mdp = new MetaDataProvider(dir, 0);
            var pp = new PostProvider(mdp);

            Assert.AreEqual(mdp.Get("post-one"), pp.Get("post-one").MetaData);
            Assert.AreEqual(mdp.Get("post-two"), pp.Get("post-two").MetaData);
        }

        [TestMethod]
        public void Get_should_return_null()
        {
            var dir = Path.Combine(TestUtils.GetAssemblyDir(), "Test_Data", "Multiple_Posts");
            var mdp = new MetaDataProvider(dir, 0);
            var pp = new PostProvider(mdp);

            Assert.IsNull(pp.Get("post-zero"));
        }

        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void Get_should_throw_ArgumentException()
        {
            var dir = Path.Combine(TestUtils.GetAssemblyDir(), "Test_Data", "Multiple_Posts");
            var mdp = new MetaDataProvider(dir, 0);
            var pp = new PostProvider(mdp);

            pp.Get(null);
        }
    }
}
