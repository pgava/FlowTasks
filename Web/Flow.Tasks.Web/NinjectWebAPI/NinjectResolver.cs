using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ninject.Syntax;
using System.Web.Http.Dependencies;
using Ninject.Activation;
using Ninject.Parameters;
using Ninject;

namespace Flow.Tasks.Web.NInjectWebAPI
{
    public class NinjectResolver : NinjectScope, IDependencyResolver
    {
        private IKernel _kernel;

        public NinjectResolver(IKernel kernel)
            : base(kernel)
        {
            _kernel = kernel;
        }

        public IDependencyScope BeginScope()
        {
            return new NinjectScope(_kernel.BeginBlock());
        }
    }
}