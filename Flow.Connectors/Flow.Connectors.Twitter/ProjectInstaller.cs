using System.ComponentModel;
using System.Configuration.Install;

namespace Flow.Connectors.Twitter
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