using System.Data;
using System.Web.UI;
using DotNetNuke.Entities.Modules;
using System;
using DotNetNuke.Services.Localization;
using System.Configuration;

namespace DotNetNuke.Modules.uDebateThreads.Controls
{
    public partial class ForumBreadcrumb : PortalModuleBase
    {
        string Topic = ATC.Tools.IntURLParam("TopicID");
      
        protected void Page_Load(object sender, System.EventArgs e)
        {
            LocalResourceFile = Localization.GetResourceFile(this, "ForumBreadcrumb.ascx" /*+
                System.Threading.Thread.CurrentThread.CurrentCulture.Name */+ ".resx");

            string literal = "" + Localization.GetString("breadStart", LocalResourceFile) +

             ": <a href='" +
             ConfigurationManager.AppSettings["DomainName"] + /*"/" + System.Threading.Thread.CurrentThread.CurrentCulture.Name +*/

             "/udebate.aspx' class='bread_link'>" + Localization.GetString("debateStart", LocalResourceFile) + "</a>";

            if (Topic != string.Empty)
            {
                string TopicDesc = ATC.Database.sqlGetFirst("SELECT Description FROM uDebate_Forum_Topics WHERE ID = " + Topic);

                literal += " >  <strong>" + TopicDesc + "</strong>";

            }        
            Breadcrumb.Controls.Add(new LiteralControl(literal));
        }
    }
}