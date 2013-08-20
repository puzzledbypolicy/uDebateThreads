using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using ATC.WebControls;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Services.Localization;
using System.Configuration;
using Telerik.Web.UI;
using System.Web.UI.HtmlControls;

namespace DotNetNuke.Modules.uDebateThreads
{
    public partial class View : uDebateThreadsModuleBase
    {
        ImageButtonExt myExtBtn = new ImageButtonExt();
        string newThreadUrl = string.Empty;
        public string culture = System.Threading.Thread.CurrentThread.CurrentCulture.Name;

        /* Change the title of the page to the title of the current thread.
         * Also used for facebook like button, which uses this as the title 
         * to the newsitem it publishes to the timeline
         */
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            string title = getDecriptionOfTopic(ATC.Tools.URLParam("TopicID"));
            if(!title.Equals(""))
                ((DotNetNuke.Framework.CDefault)this.Page).Title = title;
        }

        protected void Page_Load(object sender, EventArgs e)
        {            
            string TopicID = ATC.Tools.URLParam("TopicID");
            
            /* IF no topic is passed we redirect the user to the udebta start page*/ 
            if (TopicID.Equals(""))
            {
                Response.Redirect(ConfigurationManager.AppSettings["DomainName"] + /*"/" + culture +*/ "/udebate.aspx");
            }

            SqluDebateThreads.SelectParameters["TopicID"].DefaultValue = TopicID;

           // if (DotNetNuke.Security.PortalSecurity.IsInRoles("Pilot Leaders"))
            /* OGP registered users can create new threads */
            if (Request.IsAuthenticated )
            {
                SqluDebateThreads.SelectParameters["ThreadStatus"].DefaultValue = "1";
                /* Add new thread link */
                if (!newThreadLink.Visible)
                {
                    newThreadLink.NavigateUrl = ConfigurationManager.AppSettings["DomainName"] + "/tabid/" +
                        PortalSettings.ActiveTab.TabID + "/ctl/Edit/mid/" + ModuleId +
                   "/Topic/" + TopicID + "/ThreadID/-1" /*+
                        "/language/" + culture */+ "/default.aspx";

                    newThreadLink.CssClass = "forum_button_thread";
                    newThreadLink.Text = " ";
                    newThreadLink.Attributes.Add("onclick", "return " + UrlUtils.PopUpUrl(newThreadLink.NavigateUrl, this, PortalSettings, true, false));
                    newThreadLink.Visible = true;
                }               
            }

            /* OGP pilot leaders can edit topics */
            if (DotNetNuke.Security.PortalSecurity.IsInRoles("Pilot Leaders"))
            {
                /* Edit current topic link */
                if (!editTopikLink.Visible)
                {         
                    editTopikLink.NavigateUrl = ConfigurationManager.AppSettings["DomainName"] + "/tabid/" +
                     PortalSettings.ActiveTab.TabID + "/ctl/EditForum/mid/" + ModuleId +
                     "/ForumID/" + getForumId(ATC.Tools.URLParam("TopicID")) +
                   "/TopicID/" + TopicID + "/editItem/TOPIC/language/" /*+ culture */+ "/default.aspx";

                    editTopikLink.ImageUrl = ATC.Tools.GetParam("RootURL") + "Images/manage-icn.png";
                    editTopikLink.Attributes.Add("onclick", "return " + UrlUtils.PopUpUrl(editTopikLink.NavigateUrl, this, PortalSettings, true, false));
                    editTopikLink.Visible = true;
                }
            }

            LinkButton orderPopular = ThreadsListView.FindControl("orderPopular") as LinkButton;
            LinkButton orderDate = ThreadsListView.FindControl("orderDate") as LinkButton;
            orderPopular.CssClass = "";
            orderDate.CssClass = "";

            threadDesc.Text = getDecriptionOfTopic(ATC.Tools.URLParam("TopicID"));

            /* must be uncommented in multilingual sites*/
            //LocalResourceFile = Localization.GetResourceFile(this, "View.ascx." + culture + ".resx");
        }
                   
        public string getUserIdByThread(string thread_id)
        {
            string sUserId = ATC.Database.sqlGetFirst("SELECT [UserID] FROM [uDebate_Forum_Threads] where [ID]=" + thread_id);
            return sUserId;
        }
        public string getUserIdByTopic()
        {
            string sUserId = ATC.Database.sqlGetFirst("SELECT [UserID] FROM [uDebate_Forum_Topics] where [ID]=" + ATC.Tools.URLParam("Thread"));
            return sUserId;
        }

        public DataRow getLatestPostOfThread(string threadID)
        {
            DataRow result = null;

            string sSQL = @"SELECT TOP (1) posts.Subject, posts.PostDate, posts.UserID,Users.Username
                            FROM uDebate_Forum_Posts AS posts INNER JOIN
                            Users ON posts.UserID = Users.UserID
                            WHERE     (posts.ThreadID =" + threadID +
                            ") ORDER BY posts.PostDate DESC ";
            try
            {
                result = ATC.Database.sqlExecuteDataRow(sSQL);
            }
            catch (Exception x)
            {
            }
            return result;
        }

        public String getDecriptionOfTopic(string topicID)
        {
            DataRow result = null;
            string Description = String.Empty;
            string sSQL = @"SELECT Description
                            FROM uDebate_Forum_Topics 
                            WHERE ID =" + topicID;
            try
            {
                result = ATC.Database.sqlExecuteDataRow(sSQL);
                Description = result["Description"].ToString();
            }
            catch (Exception x)
            {
            }
            return Description;
        }

        public String getForumId(string topicID)
        {
            DataRow result = null;
            string ForumID = String.Empty;
            string sSQL = @"SELECT ForumID
                            FROM uDebate_Forum_Topics 
                            WHERE ID =" + topicID;
            try
            {
                result = ATC.Database.sqlExecuteDataRow(sSQL);
                ForumID = result["ForumID"].ToString();
            }
            catch (Exception x)
            {
            }
            return ForumID;
        }

        

        /* Threads characterised as hot are trheads having at least 
         * 1 new post in past week or at least 5 new posts in past month
         */
        public bool isThreadHot(string threadID)
        {
            DataRow result = null;

            string sSQL = @"SELECT TOP (5) posts.PostDate
                            FROM uDebate_Forum_Posts AS posts 
                            WHERE     (posts.ThreadID =" + threadID +
                            " AND PostDate>DATEADD(MONTH, -1, GETDATE())) ORDER BY posts.PostDate DESC ";
            try
            {
                DataSet ds = ATC.Database.sqlExecuteDataSet(sSQL);
                if (ds.Tables[0].Rows.Count >= 5)
                    return true;
                else if (ds.Tables[0].Rows.Count > 0)
                {
                    DateTime postDate = DateTime.Parse(ds.Tables[0].Rows[0]["PostDate"].ToString());
                    return (postDate - DateTime.Now).Days < 5;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception x)
            {
                return false;
            }
        }


        public static string TruncateAtWord(string input, int length)
        {
            string result = String.Empty;
            if (input == null || input.Length < length)
                result = input;
            else
            {
                int iNextSpace = input.LastIndexOf(" ", length);
                result = string.Format("{0}...", input.Substring(0, (iNextSpace > 0) ? iNextSpace : length).Trim());
            }
            return result.Replace("<p>", "").Replace("</p>", "");
        }

        protected void ThreadsListView_ItemDataBound(object sender, RadListViewItemEventArgs e)
        {
            if (e.Item is RadListViewDataItem)
            {
                RadListViewDataItem item = e.Item as RadListViewDataItem;
                DataRowView rowView = item.DataItem as DataRowView;
                String Thread_ID = rowView["Thread_ID"].ToString();
                String TopicID = ATC.Tools.URLParam("TopicID");

                /* Check if the thread is inactive and display the relevant label*/
                if (rowView["ThreadActive"].ToString().Equals("0"))
                {
                    Label inactive = item.FindControl("InactiveThreadLbl") as Label;
                    inactive.Visible = true;
                }

                /* Check if hotness icon should be displayed */                
               /* if (isThreadHot(Thread_ID))
                {                  
                    HtmlGenericControl hotThread = item.FindControl("hotThread") as HtmlGenericControl;
                    hotThread.Visible = true;
                    hotThread.Attributes.Add("class", "hotness_" + culture);
                    hotThread.Attributes.Add("title", Localization.GetString("hotness", LocalResourceFile));
                }*/

                 /* Check which flag to display */ 
               /* HtmlGenericControl topicLang = item.FindControl("topicLang") as HtmlGenericControl;
                Label nationalLbl = item.FindControl("nationalLbl") as Label;

                if (rowView["EuRelated"].ToString() == "True")
                {
                    nationalLbl.Text = Localization.GetString("ThreadEu", LocalResourceFile);
                    topicLang.Attributes.Add("class", "Topicflag tf_en-GB");
                }
                else
                {
                    nationalLbl.Text = Localization.GetString("ThreadNational", LocalResourceFile);
                    topicLang.Attributes.Add("class", "Topicflag tf_" + rowView["Language"]);
                }      */    
               
                /* Set the url of the user's profile link */
                HyperLink Thread_UserLink = item.FindControl("Thread_UserLink") as HyperLink;
                Thread_UserLink.NavigateUrl = DotNetNuke.Common.Globals.UserProfileURL(Convert.ToInt32(rowView["Thread_User_ID"].ToString()));
                

                /* Show edit Topic Link  if the user has edit permissions */
                if (DotNetNuke.Security.PortalSecurity.IsInRoles("Pilot Leaders") ||
                    (Request.IsAuthenticated && getUserIdByThread(Thread_ID).Equals(UserId.ToString())))
                {
                    /* Edit thread link */
                    HyperLink editLink = item.FindControl("EditLink") as HyperLink;
                    editLink.NavigateUrl = ConfigurationManager.AppSettings["DomainName"] + "/tabid/" +
                        PortalSettings.ActiveTab.TabID + "/ctl/Edit/mid/" + ModuleId +
                                "/TopicID/" + TopicID + "/ThreadID/" + Thread_ID +
                            /*"/language/" + culture + */"/default.aspx";
                    editLink.ImageUrl = ATC.Tools.GetParam("RootURL") + "Images/manage-icn.png";
                    editLink.Attributes.Add("onclick", "return " + UrlUtils.PopUpUrl(editLink.NavigateUrl, this, PortalSettings, true, false));
                    editLink.Visible = true;                   
                }

                /* Ask for details of the latest post of this Topic */
                DataRow lastPost = getLatestPostOfThread(Thread_ID);
                string lastMessage = String.Empty;
                string lastMessageUsername = String.Empty;
                string lastMessageDate = String.Empty;
                string lastMessageThreadTitle = String.Empty;
                string lastMessageThreadId = String.Empty;
                int lastMessageUserID = 1;
                bool hideLatestLabels = false;

                if (lastPost != null)
                {
                    lastMessage = TruncateAtWord(Server.HtmlDecode(lastPost["Subject"].ToString()), 90);
                    if (lastMessage != "")
                    {
                        lastMessageUsername = lastPost["Username"].ToString();
                        lastMessageDate = lastPost["PostDate"].ToString();
                        lastMessageUserID = Convert.ToInt32(lastPost["UserID"]);


                        Label LastMessage = item.FindControl("LastMessage") as Label;
                        Label LastMsgDate = item.FindControl("LastMsgDate") as Label;

                        HyperLink userProfile = item.FindControl("userProfile") as HyperLink;
                        HyperLink ThreadofPost = item.FindControl("ThreadofPost") as HyperLink;

                        LastMessage.Text = "\"" + lastMessage + "\"";
                        LastMsgDate.Text = lastMessageDate;
                        userProfile.NavigateUrl = DotNetNuke.Common.Globals.UserProfileURL(lastMessageUserID);
                        userProfile.Text = lastMessageUsername;
                    }
                    else
                        hideLatestLabels = true;
                }
                else
                {
                    hideLatestLabels = true;                    
                }
                if (hideLatestLabels)
                {
                    Label latspostLbl = item.FindControl("latspostLbl") as Label;
                    Label postBy = item.FindControl("postBy") as Label;
                    Label separator1 = item.FindControl("separator1") as Label;
                    latspostLbl.Visible = false;
                    postBy.Visible = false;
                    separator1.Visible = false;
                }
            }
        }

        protected void Sorting_Click(object sender, EventArgs e)
        {
            RadListViewSortExpression expression;
            LinkButton orderby = sender as LinkButton;
            ThreadsListView.SortExpressions.Clear();

            expression = new RadListViewSortExpression();
            expression.FieldName = orderby.CommandName;
            expression.SortOrder = orderby.CommandArgument == "asc" ? RadListViewSortOrder.Ascending : RadListViewSortOrder.Descending;
            orderby.CommandArgument = orderby.CommandArgument == "asc" ? "desc" : "asc";
            ThreadsListView.SortExpressions.AddSortExpression(expression);
            orderby.CssClass = "orderby";
            ThreadsListView.Rebind();
        }    

    }
}