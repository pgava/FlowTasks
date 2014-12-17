using System.ServiceProcess;
using Ninject;
using System.Reflection;
using Flow.Docs.Contract.Interface;
using Flow.Tasks.Contract;

namespace Flow.Connectors.DocsOnFolder
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        private static void Main()
        {
            var kernel = new StandardKernel();
            kernel.Load(Assembly.GetExecutingAssembly());

            var docsDependency = kernel.Get<IFlowDocsDocument>();

            ServiceBase[] ServicesToRun;

            // More than one user Service may run within the same process. To add
            // another service to this process, change the following line to
            // create a second service object. For example,
            //
            //   ServicesToRun = new ServiceBase[] {new Service1(), new MySecondUserService()};
            //
            ServicesToRun = new ServiceBase[] { new DocsOnFolderService { _processDocs = docsDependency } };

            ServiceBase.Run(ServicesToRun);
        }
    }
}