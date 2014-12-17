using Ninject.Modules;
using Hitone.Web.SqlServerUploader;
using Brettle.Web.NeatUpload;
using Flow.Docs.Contract;
using Flow.Docs.Process;

namespace Flow.Docs.Service
{
    public class FlowDocsOperationsModule : NinjectModule
    {
        public override void Load()
        {
            var provider = (SqlServerUploadStorageProvider)UploadStorage.ProviderFromSectionName("neatUpload");

            Bind<IFlowDocsOperations>().To<FlowDocsOperationsBase>();
            Bind<FlowDocsOperationsBase>().To<FlowDocsOperationsDatabase>().WithConstructorArgument("provider", provider);
        } 
    }
}