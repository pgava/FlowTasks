using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Flow.Docs.Data.Core;
using Flow.Docs.Data;

namespace Flow.Docs.DataTest
{
    /// <summary>
    /// Summary description for UsersFlowDBTest
    /// </summary>
    [TestClass]
    public class DBTest
    {
        
        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion


        [TestMethod]
        [Ignore]
        public void CreateDb()
        {
            var oid = Guid.NewGuid();
            var doc1 = new Document()
            {
                DocumentName = "Test_Document.doc",
                Owner = "SQL",
                OidDocument = oid,
                Path = "root/SQL",
                DateCreated = DateTime.Now,
                DateLastUpdated = DateTime.Now
            };

            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            byte[] bytes = encoding.GetBytes("this is the attachment");

            var atch1 = new Attachment()
            {
                DataField = bytes,
                DateCreated = DateTime.Now,
                FileName = "Test_Document.doc",
            };

            atch1.Document = doc1;

            using (var ctx = new FlowDocsEntities())
            {
                ctx.Attachments.Add(atch1);
                ctx.Documents.Add(doc1);
                ctx.SaveChanges();
            }
        }
    }
}
