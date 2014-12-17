using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Configuration;

namespace ApplyJob
{
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void UploadButton_Click(object sender, EventArgs e)
        {
            if (fulResume.HasFile)
            {
                try
                {
                    string filename = Path.GetFileName(fulResume.FileName);

                    var uploadedFilePath = Path.Combine(ConfigurationManager.AppSettings["ResumeLoadPath"], filename);
                    fulResume.SaveAs(uploadedFilePath);

                    StatusLabel.Text = "Upload status: File uploaded!";
                }
                catch (Exception ex)
                {
                    StatusLabel.Text = "Upload status: The file could not be uploaded. The following error occured: " + ex.Message;
                }
            }
        }
    }
}
