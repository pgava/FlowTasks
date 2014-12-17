using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using Moq;
using Flow.Docs.Data;
using Flow.Library;
using Flow.Docs.Data.Core.Interfaces;
using Flow.Docs.Data.Infrastructure;
using Hitone.Web.SqlServerUploader;
using Brettle.Web.NeatUpload;
using Flow.Docs.Contract.Message;
using Flow.Docs.Process;
using Flow.Docs.Service;
using Flow.Library.EF;

namespace Flow.Docs.DataTest.Integration
{
    [TestClass]
    public sealed class DocsService : IDisposable
    {

        private TestContext testContextInstance;

        #region IDisposable
        public void Dispose()
        {
            if (uow != null)
            {
                var disp = uow as FlowDocsUnitOfWork;
                disp.Dispose();
            }

            if (ctx != null)
            {
                ctx.Dispose();
            }

            if (adp != null)
            {
                adp.Dispose();
            }
        }
        #endregion

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

        private FlowDocsEntities ctx;
        private DbContextAdapter adp;
        IFlowDocsUnitOfWork uow;

        [TestInitialize()]
        public void MyTestInitialize()
        {
            ctx = new FlowDocsEntities();
            adp = new FlowDocsContextAdapter(ctx);
            uow = new FlowDocsUnitOfWork(adp);
        }

        [TestCleanup()]
        public void MyTestCleanup()
        {
            var docs = from d in uow.Documents.AsQueryable()
                       where Oids.Contains(d.OidDocument)
                       select d;

            foreach (var d in docs)
            {
                uow.Documents.Delete(d);
            }

            uow.Commit();

            var neatuploadFileBig = System.Configuration.ConfigurationManager.AppSettings["NeatuploadFileBig"];
            var neatuploadFileSmall = System.Configuration.ConfigurationManager.AppSettings["NeatuploadFileSmall"];
            var neatuploadFileSmall1 = System.Configuration.ConfigurationManager.AppSettings["NeatuploadFileSmall1"];
            var neatuploadFileTmp = System.Configuration.ConfigurationManager.AppSettings["NeatuploadFileTmp"];

            var filePath = Path.Combine(neatuploadFileTmp, Path.GetFileName(neatuploadFileBig));
            FileInfo f = new FileInfo(filePath);
            if (f.Exists) f.Delete();

            filePath = Path.Combine(neatuploadFileTmp, Path.GetFileName(neatuploadFileSmall));
            f = new FileInfo(filePath);
            if (f.Exists) f.Delete();

            filePath = Path.Combine(neatuploadFileTmp, Path.GetFileName(neatuploadFileSmall1));
            f = new FileInfo(filePath);
            if (f.Exists) f.Delete();
        }

        List<Guid> Oids = new List<Guid>();

        [TestMethod]
        public void Should_Upload_A_File_To_Server()
        {
            var sut = CreateSut();

            var neatuploadFile = System.Configuration.ConfigurationManager.AppSettings["NeatuploadFileBig"];

            var info = new DocumentInfo { DocumentName = Path.GetFileName(neatuploadFile), Path = Path.GetDirectoryName(neatuploadFile), Description = "desc-test", Owner = "owner", Version = 1 };

            long startTicks = DateTime.Now.Ticks;

            var OidUpload = sut.UploadDocument(info, neatuploadFile, DocumentUploadMode.NewVersion);
            Oids.Add(OidUpload);

            long endTicks = DateTime.Now.Ticks;

            var docs = from d in uow.Documents.AsQueryable()
                       where d.OidDocument == OidUpload
                       select d;

            var atchs = from a in uow.Attachments.AsQueryable()
                        where a.OidDocument == OidUpload
                        select a;

            Console.WriteLine("Upload file in: {0}", (endTicks - startTicks) / 10000);

            Assert.AreNotEqual(Guid.Empty, OidUpload);
            Assert.AreEqual(1, docs.Count());
            Assert.AreEqual("desc-test", docs.First().Description);
            Assert.AreEqual(Path.GetFileName(neatuploadFile), docs.First().DocumentName);
            Assert.AreEqual(Path.GetDirectoryName(neatuploadFile), docs.First().Path);
            Assert.AreEqual(1, atchs.Count());

        }

        [TestMethod]
        public void Should_Overwrite_A_File_To_Server_Without_Previous_Oid()
        {
            var sut = CreateSut();
            var bigFile = System.Configuration.ConfigurationManager.AppSettings["NeatuploadFileBig"];
            var smallFile = System.Configuration.ConfigurationManager.AppSettings["NeatuploadFileSmall"];

            Guid firstVer = SetupTest_UploadDoc(sut, bigFile, DocumentUploadMode.NewVersion);
            Guid secondVer = SetupTest_UploadDoc(sut, bigFile, DocumentUploadMode.Overwrite);

            var docs1 = from d in uow.Documents.AsQueryable()
                        where d.OidDocument == firstVer
                        select d;

            var docs2 = from d in uow.Documents.AsQueryable()
                        where d.OidDocument == secondVer
                        select d;

            Assert.AreEqual(0, docs1.Count());
            Assert.AreEqual(1, docs2.Count());
            Assert.AreEqual(Path.GetFileName(bigFile), docs2.First().DocumentName);
            Assert.AreEqual(Path.GetDirectoryName(bigFile), docs2.First().Path);
        }

        [TestMethod]
        public void Should_Overwrite_A_File_To_Server_With_Previous_Oid()
        {
            var sut = CreateSut();
            var bigFile = System.Configuration.ConfigurationManager.AppSettings["NeatuploadFileBig"];
            var smallFile = System.Configuration.ConfigurationManager.AppSettings["NeatuploadFileSmall"];

            Guid firstVer = SetupTest_UploadDoc(sut, bigFile, DocumentUploadMode.NewVersion);

            var info = new DocumentInfo { DocumentName = Path.GetFileName(bigFile), Path = Path.GetDirectoryName(bigFile), Description = "desc-test", Owner = "owner", PreviousOid = firstVer };
            Guid secondVer = SetupTest_UploadDoc(sut, info, bigFile, DocumentUploadMode.Overwrite);

            var docs1 = from d in uow.Documents.AsQueryable()
                        where d.OidDocument == firstVer
                        select d;

            var docs2 = from d in uow.Documents.AsQueryable()
                        where d.OidDocument == secondVer
                        select d;

            Assert.AreEqual(0, docs1.Count());
            Assert.AreEqual(1, docs2.Count());
            Assert.AreEqual(Path.GetFileName(bigFile), docs2.First().DocumentName);
            Assert.AreEqual(Path.GetDirectoryName(bigFile), docs2.First().Path);
        }

        [TestMethod]
        public void Should_Been_Able_To_Upload_A_New_Version()
        {
            var sut = CreateSut();
            var smallFile = System.Configuration.ConfigurationManager.AppSettings["NeatuploadFileSmall"];

            Guid firstVer = SetupTest_UploadDoc(sut, smallFile, DocumentUploadMode.NewVersion);
            Guid secondVer = SetupTest_UploadDoc(sut, smallFile, DocumentUploadMode.NewVersion);

            var docs1 = from d in uow.Documents.AsQueryable()
                        where d.OidDocument == firstVer
                        select d;

            var docs2 = from d in uow.Documents.AsQueryable()
                        where d.OidDocument == secondVer
                        select d;

            Assert.AreEqual(1, docs1.Count());
            Assert.AreEqual(1, docs2.Count());
            Assert.AreEqual(Path.GetFileName(smallFile), docs2.First().DocumentName);
            Assert.AreEqual(Path.GetDirectoryName(smallFile), docs2.First().Path);
            Assert.AreEqual(firstVer, docs2.First().DocumentPrevious.OidDocument);
            Assert.AreEqual(2, docs2.First().Version);
        }

        [TestMethod]
        public void Should_Download_A_File_From_Server()
        {
            var sut = CreateSut();

            var neatuploadFile = System.Configuration.ConfigurationManager.AppSettings["NeatuploadFileBig"];
            Guid oid = SetupTest_UploadDoc(sut, neatuploadFile, DocumentUploadMode.NewVersion);

            var neatuploadFileTmp = System.Configuration.ConfigurationManager.AppSettings["NeatuploadFileTmp"];

            var info = new DocumentInfo
            {
                OidDocument = oid,
                Version = 1
            };

            sut.DownloadDocument(info, neatuploadFileTmp, DocumentDownloadMode.LastVersion);

            var filePath = Path.Combine(neatuploadFileTmp, Path.GetFileName(neatuploadFile));
            FileInfo f = new FileInfo(filePath);

            Assert.IsTrue(f.Exists);

        }

        [TestMethod]
        public void Should_Be_Able_To_Download_The_Latest_Version()
        {
            var sut = CreateSut();
            var smallFile = System.Configuration.ConfigurationManager.AppSettings["NeatuploadFileSmall"];
            var smallFile1 = System.Configuration.ConfigurationManager.AppSettings["NeatuploadFileSmall1"];
            var neatuploadFileTmp = System.Configuration.ConfigurationManager.AppSettings["NeatuploadFileTmp"];

            Guid firstVer = SetupTest_UploadDoc(sut, smallFile, DocumentUploadMode.NewVersion);

            var upInfo = new DocumentInfo { DocumentName = Path.GetFileName(smallFile), Path = Path.GetDirectoryName(smallFile), Description = "desc-test", Owner = "owner" };
            Guid secondVer = SetupTest_UploadDoc(sut, upInfo, smallFile1, DocumentUploadMode.NewVersion);

            var info = new DocumentInfo
            {
                OidDocument = secondVer
            };

            sut.DownloadDocument(info, neatuploadFileTmp, DocumentDownloadMode.LastVersion);

            var filePath = Path.Combine(neatuploadFileTmp, Path.GetFileName(smallFile));

            FileInfo f = new FileInfo(filePath);
            Assert.IsTrue(f.Exists);
            
            var lines = File.ReadAllLines(filePath);
            Assert.AreEqual(lines[0], "text line1 ");            
        }

        [TestMethod]
        public void Should_Be_Able_To_Download_The_Version_Specified()
        {
            var sut = CreateSut();
            var smallFile = System.Configuration.ConfigurationManager.AppSettings["NeatuploadFileSmall"];
            var smallFile1 = System.Configuration.ConfigurationManager.AppSettings["NeatuploadFileSmall1"];
            var neatuploadFileTmp = System.Configuration.ConfigurationManager.AppSettings["NeatuploadFileTmp"];

            Guid firstVer = SetupTest_UploadDoc(sut, smallFile, DocumentUploadMode.NewVersion);

            var upInfo = new DocumentInfo { DocumentName = Path.GetFileName(smallFile), Path = Path.GetDirectoryName(smallFile), Description = "desc-test", Owner = "owner" };
            Guid secondVer = SetupTest_UploadDoc(sut, upInfo, smallFile1, DocumentUploadMode.NewVersion);

            var info = new DocumentInfo
            {
                OidDocument = secondVer,
                Version = 1
            };

            sut.DownloadDocument(info, neatuploadFileTmp, DocumentDownloadMode.SpecifiedVersion);

            var filePath = Path.Combine(neatuploadFileTmp, Path.GetFileName(smallFile));

            FileInfo f = new FileInfo(filePath);
            Assert.IsTrue(f.Exists);

            var lines = File.ReadAllLines(filePath);
            Assert.AreEqual(lines[0], "text line ");
        }

        private Guid SetupTest_UploadDoc(FlowDocsDocument sut, string fileName, DocumentUploadMode mode)
        {
            var info = new DocumentInfo { DocumentName = Path.GetFileName(fileName), Path = Path.GetDirectoryName(fileName), Description = "desc-test", Owner = "owner"};

            return SetupTest_UploadDoc(sut, info, fileName, mode);
        }

        private Guid SetupTest_UploadDoc(FlowDocsDocument sut, DocumentInfo info, string fileName, DocumentUploadMode mode)
        {
            var OidUpload = sut.UploadDocument(info, fileName, mode);

            Oids.Add(OidUpload);

            return OidUpload;
        }

        private FlowDocsDocument CreateSut()
        {
            var neatuploadConfig = System.Configuration.ConfigurationManager.AppSettings["NeatuploadConfig"];

            var provider = (SqlServerUploadStorageProvider)UploadStorage.ProviderConfig(neatuploadConfig);

            var serviceDatabase = new FlowDocsOperationsDatabase(uow, provider);
            var sut = new FlowDocsDocument(serviceDatabase);

            return sut;
        }


    }
}
