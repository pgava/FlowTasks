namespace Flow.Connectors.Twitter
{
   partial class ProjectInstaller
   {
      /// <summary>
      /// Required designer variable.
      /// </summary>
      private System.ComponentModel.IContainer components = null;

      /// <summary> 
      /// Clean up any resources being used.
      /// </summary>
      /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
      protected override void Dispose(bool disposing)
      {
         if (disposing && (components != null))
         {
            components.Dispose();
         }
         base.Dispose(disposing);
      }

      #region Component Designer generated code

      /// <summary>
      /// Required method for Designer support - do not modify
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent()
      {
         this.callidenServiceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
         this.callidenServiceInstaller = new System.ServiceProcess.ServiceInstaller();
         // 
         // callidenServiceProcessInstaller
         // 
         this.callidenServiceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
         this.callidenServiceProcessInstaller.Password = null;
         this.callidenServiceProcessInstaller.Username = null;
         // 
         // callidenServiceInstaller
         // 
         this.callidenServiceInstaller.Description = "Twitter connector for FlowTasks.";
         this.callidenServiceInstaller.DisplayName = "TwitterConnector";
         this.callidenServiceInstaller.ServiceName = "TwitterConnector";
         this.callidenServiceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
         // 
         // ProjectInstaller
         // 
         this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.callidenServiceProcessInstaller,
            this.callidenServiceInstaller});

      }

      #endregion

      private System.ServiceProcess.ServiceProcessInstaller callidenServiceProcessInstaller;
      private System.ServiceProcess.ServiceInstaller callidenServiceInstaller;
   }
}