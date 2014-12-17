using System.ComponentModel;
using System.Configuration.Install;

namespace Flow.Connectors.DocsOnFolder
{
   [RunInstaller(true)]
   public partial class ProjectInstaller : Installer
   {
      public ProjectInstaller()
      {
         InitializeComponent();
      }
   }
}