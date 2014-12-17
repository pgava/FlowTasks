using System;
using System.Collections.Generic;

using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Brettle.Web.NeatUpload;

namespace NeatUploadTest
{
    public partial class DemoUpload : System.Web.UI.Page
    {
        //protected override void OnInit(EventArgs e)
        //{
        //    InitializeComponent();
        //    base.OnInit(e);
        //}

        //private void InitializeComponent()
        //{
        //    this.Load += new System.EventHandler(this.Page_Load);
        //}

        private void Page_Load(object sender, EventArgs e)
        {
            submitButtonSpan.Visible = (buttonTypeDropDown.SelectedValue == "Button");
            linkButtonSpan.Visible = (buttonTypeDropDown.SelectedValue == "LinkButton");
            commandButtonSpan.Visible = (buttonTypeDropDown.SelectedValue == "CommandButton");
            htmlInputButtonButtonSpan.Visible = (buttonTypeDropDown.SelectedValue == "HtmlInputButtonButton");
            htmlInputButtonSubmitSpan.Visible = (buttonTypeDropDown.SelectedValue == "HtmlInputButtonSubmit");

            inlineProgressBarDiv.Visible = (progressBarLocationDropDown.SelectedValue == "Inline");
            popupProgressBarDiv.Visible = (progressBarLocationDropDown.SelectedValue == "Popup");

            submitButton.Click += new System.EventHandler(this.Button_Clicked);
            linkButton.Click += new System.EventHandler(this.Button_Clicked);
            commandButton.Click += new System.EventHandler(this.Button_Clicked);
            htmlInputButtonButton.ServerClick += new System.EventHandler(this.Button_Clicked);
            htmlInputButtonSubmit.ServerClick += new System.EventHandler(this.Button_Clicked);

            /*
                        // Instead of setting the Triggers property of the 
                        // ProgressBar element in the aspx file, you can put lines like
                        // the following in your code-behind:
                        progressBar.AddTrigger(submitButton);
                        progressBar.AddTrigger(linkButton);
                        progressBar.AddTrigger(commandButton);
                        progressBar.AddTrigger(htmlInputButtonButton);
                        progressBar.AddTrigger(htmlInputButtonSubmit);
                        inlineProgressBar.AddTrigger(submitButton);
                        inlineProgressBar.AddTrigger(linkButton);
                        inlineProgressBar.AddTrigger(commandButton);
                        inlineProgressBar.AddTrigger(htmlInputButtonButton);
                        inlineProgressBar.AddTrigger(htmlInputButtonSubmit);

                        // The temp directory used by the default FilesystemUploadStorageProvider can be configured on a
                        // per-control basis like this (see documentation for details).  Note that if the temp directory
                        // is within the application's directory hierarchy (except under App_Data) ASP.NET may restart
                        // the application when NeatUpload writes the temp files to the directory.
                        if (!IsPostBack)
                        {
                            inputFile.StorageConfig["tempDirectory"] = Path.Combine("App_Data", "file1temp");
                            inputFile2.StorageConfig["tempDirectory"] = Path.Combine("App_Data", "file2temp");
                            multiFile.StorageConfig["tempDirectory"] = Path.Combine("App_Data", "file1temp");
                            multiFile2.StorageConfig["tempDirectory"] = Path.Combine("App_Data", "file2temp");
                        }
            */
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            if (!this.IsValid)
            {
                bodyPre.InnerText = "Page is not valid!";
                return;
            }
            bodyPre.InnerText = "";
            //if (multiFile.Files.Length > 0)
            //{
            //    bodyPre.InnerText += "Uploaded " + multiFile.Files.Length + " files with MultiFile:\n";
            //    foreach (UploadedFile file in multiFile.Files)
            //    {
            //        /* 
            //            In a real app, you'd do something like:
            //            file.MoveTo(Path.Combine(Request.PhysicalApplicationPath, file.FileName), 
            //                             MoveToOptions.Overwrite);
            //        */
            //        bodyPre.InnerText += file.FileName + "\n";
            //    }
            //}

            //if (multiFile2.Files.Length > 0)
            //{
            //    bodyPre.InnerText += "Uploaded " + multiFile2.Files.Length + " files with MultiFile2:\n";
            //    foreach (UploadedFile file in multiFile2.Files)
            //    {
            //        /* 
            //            In a real app, you'd do something like:
            //            file.MoveTo(Path.Combine(Request.PhysicalApplicationPath, file.FileName), 
            //                             MoveToOptions.Overwrite);
            //        */
            //        bodyPre.InnerText += file.FileName + "\n";
            //    }
            //}


            if (inputFile.HasFile)
            {
                /* 
                    In a real app, you'd do something like:
                    inputFile.MoveTo(Path.Combine(Request.PhysicalApplicationPath, inputFile.FileName), 
                                     MoveToOptions.Overwrite);
                */
                bodyPre.InnerText += "File #1:\n";
                bodyPre.InnerText += "  Name: " + inputFile.FileName + "\n";
                bodyPre.InnerText += "  Size: " + inputFile.ContentLength + "\n";
                bodyPre.InnerText += "  Content type: " + inputFile.ContentType + "\n";
            }
            //if (inputFile2.HasFile)
            //{
            //    /* 
            //        In a real app, you'd do something like:
            //        inputFile2.MoveTo(Path.Combine(Request.PhysicalApplicationPath, inputFile2.FileName), 
            //                          MoveToOptions.Overwrite);
            //    */
            //    bodyPre.InnerText += "File #2:\n";
            //    bodyPre.InnerText += "  Name: " + inputFile2.FileName + "\n";
            //    bodyPre.InnerText += "  Size: " + inputFile2.ContentLength + "\n";
            //    bodyPre.InnerText += "  Content type: " + inputFile2.ContentType + "\n";
            //}
        }
    }
}