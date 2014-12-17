using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using Moq;
using Flow.Docs.Data;
using Flow.Docs.Contract;
using Flow.Docs.Process;
using Flow.Docs.Contract.Message;
using Flow.Library;

namespace Flow.Docs.DataTest
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class DataTest
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
        public void Should_Split_A_File_In_Chunks_For_Writing()
        {    
            var mockDataService = new Mock<IFlowDocsOperations>();
            var sut = new FlowDocsDocument(mockDataService.Object);
            sut.ChunkSize = 10;

            var doc = "this is the document to send!."; //30 chars

            var docBytes = Encoding.Unicode.GetBytes(doc);

            var oid = Guid.NewGuid();
            var uploadResponse = new UploadDocumentResponse { OidDocument = oid };

            mockDataService.Setup(x => x.UploadDocument(Moq.It.IsAny<UploadDocumentRequest>()))
                .Returns(uploadResponse);

            var info = new DocumentInfo { DocumentName = "Name", Path = "path", Description = "desc", Owner = "owner", Version = 1 };
            sut.UploadDocument(info, docBytes, DocumentUploadMode.NewVersion);

            mockDataService.Verify(x => x.UploadDocument(Moq.It.IsAny<UploadDocumentRequest>()), Times.Exactly(6));
        }

        [TestMethod]
        public void Should_Split_A_File_In_Chunks_For_Reading()
        {
            var mockDataService = new Mock<IFlowDocsOperations>();
            var sut = new FlowDocsDocument(mockDataService.Object);
            sut.ChunkSize = 10;

            var response = new DownloadDocumentResponse
            {
                ChunkNumber = 1,
                ChunkSize = 10,
                ChunkTotal = 10,
                DataField = ASCIIEncoding.ASCII.GetBytes("0123456789"),
                Description = "desc",
                DocumentName = "testfile.txt",
                OidDocument = Guid.Empty,
                Owner = "me",
                Path = "",
                Version = 1,
                FileHash = Md5Hash.CreateMd5Hash(ASCIIEncoding.ASCII.GetBytes("0123456789"))
            };
            mockDataService.Setup(x => x.DownloadDocument(Moq.It.IsAny<DownloadDocumentRequest>()))
                .Returns(response);

            var doc = new DocumentInfo
            {
                OidDocument = Guid.Empty,
                Version = 1
            };

            var tmpFile = System.Configuration.ConfigurationManager.AppSettings["TmpFile"];

            sut.DownloadDocument(doc, tmpFile, DocumentDownloadMode.LastVersion);
            mockDataService.Verify(x => x.DownloadDocument(Moq.It.IsAny<DownloadDocumentRequest>()), Times.Exactly(10));

        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void Should_Return_Exception_If_Hash_Doesnt_Match()
        {
            var mockDataService = new Mock<IFlowDocsOperations>();
            var sut = new FlowDocsDocument(mockDataService.Object);
            sut.ChunkSize = 10;

            var response = new DownloadDocumentResponse
            {
                ChunkNumber = 1,
                ChunkSize = 10,
                ChunkTotal = 10,
                DataField = ASCIIEncoding.ASCII.GetBytes("0123456789"),
                Description = "desc",
                DocumentName = "testfile.txt",
                OidDocument = Guid.Empty,
                Owner = "me",
                Path = "",
                Version = 1,
                FileHash = Md5Hash.CreateMd5Hash(ASCIIEncoding.ASCII.GetBytes("000111333"))
            };
            mockDataService.Setup(x => x.DownloadDocument(Moq.It.IsAny<DownloadDocumentRequest>()))
                .Returns(response);

            var doc = new DocumentInfo
            {
                OidDocument = Guid.Empty,
                Version = 1
            };

            var tmpFile = System.Configuration.ConfigurationManager.AppSettings["TmpFile"];

            sut.DownloadDocument(doc, tmpFile, DocumentDownloadMode.LastVersion);

        }

    }
}
