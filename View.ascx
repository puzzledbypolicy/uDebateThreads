<%@ Control Language="C#" AutoEventWireup="True" CodeBehind="View.ascx.cs" Inherits="DotNetNuke.Modules.uDebateThreads.View" %>
<%@ Register TagPrefix="uDebateThreads" TagName="Breadcrump" Src="~/DesktopModules/uDebateThreads/controls/ForumBreadcrumb.ascx" %>
<%@ Register Assembly="DotNetNuke.Web" Namespace="DotNetNuke.Web.UI.WebControls"
    TagPrefix="telerik" %>
<% if (Request.IsAuthenticated)
   {%>

<script type="text/javascript">
    jQuery(document).ready(function() {
        jQuery(".proposeSlider").show();
        jQuery(".proposeSlider").click(function() {
            jQuery(".ProposeTopic").toggle("slow");
            jQuery(this).toggleClass("active");
            return false;
        });
    });    
</script>

<script type="text/javascript">

    /* * * CONFIGURATION VARIABLES: EDIT BEFORE PASTING INTO YOUR WEBPAGE * * */

    var locale = "<%=System.Threading.Thread.CurrentThread.CurrentCulture.Name %>";
    if (locale == "es-ES") {
        var disqus_shortname = 'puzzledbypolicyes';
        var disqus_identifier = "spanish";
    }
    else if (locale == "el-GR") {
        var disqus_shortname = 'puzzledbypolicyel';
        var disqus_identifier = "greek";
    }
    else if (locale == "it-IT") {
        var disqus_shortname = 'joinpuzzledbypolicyitalian';
        var disqus_identifier = "italian";
    }
    else if (locale == "hu-HU") {
        var disqus_shortname = 'puzzledbypolicyhu';
        var disqus_identifier = "hungarian";
    }
    else {
        var disqus_shortname = 'puzzledbypolicy';
        var disqus_identifier = "english";
    }

    var disqus_developer = 1;

    /* * * DON'T EDIT BELOW THIS LINE * * */
    (function() {
        var dsq = document.createElement('script'); dsq.type = 'text/javascript'; dsq.async = true;
        dsq.src = 'http://' + disqus_shortname + '.disqus.com/embed.js';
        (document.getElementsByTagName('head')[0] || document.getElementsByTagName('body')[0]).appendChild(dsq);
    })();   
    
</script>

<noscript>
    Please enable JavaScript to view the <a href="http://disqus.com/?ref_noscript">comments
        powered by Disqus.</a></noscript>
<% }%>
<div id="fb-root">
</div>

<script>    (function(d, s, id) {
        var js, fjs = d.getElementsByTagName(s)[0];
        if (d.getElementById(id)) return;
        js = d.createElement(s); js.id = id;
        js.src = "//connect.facebook.net/en_US/all.js#xfbml=1&appId=274597442564126";
        fjs.parentNode.insertBefore(js, fjs);
    } (document, 'script', 'facebook-jssdk'));</script>

<uDebateThreads:Breadcrump ID="ctlBreadcrump" runat="server" />
<div class="dnnClear">
</div>
<div id="forumContainer">
    <div id="TopicTitle">
        <asp:HyperLink ID="editTopikLink" CssClass="floaLeft" runat="server" Visible="false" />
        <div class="facebook-box">
            <fb:like send="false" width="80" show_faces="false" layout="button_count"></fb:like>
        </div>
        <h1>
            <asp:Label ID="threadDesc" runat="server" /></h1>        
        <asp:HyperLink ID="newThreadLink" runat="server" Visible="false" />
    </div>
    <telerik:DnnListView ID="ThreadsListView" runat="server" DataSourceID="SqluDebateThreads"
        ItemPlaceholderID="PlaceHolder1" AllowPaging="true" OnItemDataBound="ThreadsListView_ItemDataBound">
        <LayoutTemplate>
            <table width="100%" cellspacing="0" cellpadding="0">
                <tr>                   
                    <td width="63%" class="forum_topics_header" style="text-align: left">
                        <asp:Label ID="topicLbl" runat="server" resourcekey="Threads"></asp:Label>
                    </td>
                    <td width="12%" class="forum_topics_header">
                        <asp:Label ID="openedLbl" runat="server" resourcekey="OpenedDate"></asp:Label>
                    </td>
                    <td width="5%" class="forum_topics_header">
                        <asp:Label ID="openedbyLbl" runat="server" resourcekey="PostBy"></asp:Label>
                    </td>
                    <td width="5%" class="forum_topics_header">
                        <asp:Label ID="postsLbl" runat="server" resourcekey="Posts"></asp:Label>
                    </td>
                    <td width="5%" class="forum_topics_header">
                        <asp:Label ID="viewLbl" runat="server" resourcekey="Views"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td colspan="5" class="ordering">
                        <asp:Label ID="orderLbl" runat="server" resourcekey="Orderby"></asp:Label>
                        <asp:LinkButton ID="orderPopular" runat="server" OnClick="Sorting_Click" CommandArgument="desc"
                            CommandName="PostsCount" ForeColor="#003399" resourcekey="popularity" />
                        |
                        <asp:LinkButton ID="orderDate" runat="server" OnClick="Sorting_Click" CommandArgument="desc"
                            CommandName="Opened_Date" ForeColor="#003399" resourcekey="date" />
                    </td>
                </tr>
                <asp:PlaceHolder ID="PlaceHolder1" runat="server"></asp:PlaceHolder>
            </table>
            <telerik:DnnDataPager ID="RadDataPager1" runat="server" PageSize="8" Skin="Transparent">
                <Fields>
                    <telerik:DnnDataPagerSliderField />
                </Fields>
            </telerik:DnnDataPager>
        </LayoutTemplate>
        <ItemTemplate>
            <tr>
                
                <!-- culture param must be added to link if multilingual site -->
                
                <td class="topic_Sumary">
                    <a href="<%#ConfigurationManager.AppSettings["DomainName"] %>/udebatediscussion.aspx?Thread=<%#Eval("Thread_ID") %>"
                        class="topic_Descr_link">
                        <asp:Label ID="Thread_DescLabel" runat="server" Text='<%#Eval("Thread_Desc") %>'></asp:Label>   
                        <asp:HyperLink ID="EditLink" runat="server" ToolTip="Edit this thread" Visible="false" />                     
                    </a>
                    <br />
                    <asp:Label ID="Thread_SubtitleLbl" runat="server" CssClass="topic_Descr_sub" Text='<%#Eval("Thread_Subtitle") %>'></asp:Label>
                    <asp:Label ID="InactiveThreadLbl" runat="server" CssClass="forum_topics_inactive_span"
                        Text='Inactive' resourcekey="Inactive" Visible="false"></asp:Label> 
                </td>
                <td class="forum_topics_cell">
                    <asp:Label ID="Opened_DateLabel" runat="server" Text='<%#Eval("Opened_Date","{0:dd-MM-yyyy}") %>'></asp:Label>
                </td>
                <td class="forum_topics_cell">
                    <asp:HyperLink ID="Thread_UserLink" CssClass="threadCreator" runat="server" Text='<%#Eval("Thread_User") %>'></asp:HyperLink>
                </td>
                <td class="forum_topics_cell">
                    <asp:Label ID="PostsCountLabel" runat="server" Text='<%#(String.IsNullOrEmpty(Eval("PostsCount").ToString()) ? "0" : Eval("PostsCount")) %>'></asp:Label>
                </td>
                <td class="forum_topics_cell">
                    <asp:Label ID="Views_CountLabel" runat="server" Text='<%#Eval("Views_Count") %>'></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="topic_latestpost" colspan="5">
                    <asp:Label ID="latspostLbl" runat="server" resourcekey="LatestPost"></asp:Label>
                    <asp:Label ID="LastMessage" runat="server" Font-Italic="true"></asp:Label>&nbsp;
                    <asp:Label ID="separator1" runat="server" Text="| "></asp:Label>
                    <asp:Label ID="LastMsgDate" runat="server"></asp:Label>
                    <asp:Label ID="postBy" runat="server" resourcekey="PostBy"></asp:Label>
                    <asp:HyperLink ID="userProfile" runat="server" ForeColor="#003399" />
                </td>
            </tr>
            <tr>
                <td colspan="6" height="10px">
                </td>
            </tr>
        </ItemTemplate>
    </telerik:DnnListView>
</div>
<asp:SqlDataSource ID="SqluDebateThreads" runat="server" ConnectionString="<%$ ConnectionStrings:SiteSqlServer %>"
    SelectCommand=" SELECT SUM(ThreadPosts.Posts) AS PostsCount, 
    Threads.ID AS Thread_ID, Threads.Description AS Thread_Desc,Threads.Summary AS Thread_Subtitle, Threads.EuRelated,Threads.Language, 
    Threads.CREATED AS Opened_Date, Threads.UserID AS Thread_User_ID, Threads.Active as ThreadActive,
    Threads.Closed_Date AS Closed_Date,Threads.Status AS Thread_Status, Threads.View_Count AS Views_Count,Threads.Position AS Position,
    User_Thread.LastName AS Surname, User_Thread.FirstName AS Name, User_Thread.Username AS Thread_User     
    FROM
    (SELECT COUNT(Posts.ID) AS 'Posts',Posts.ThreadID,Threads.View_Count
        FROM uDebate_Forum_Posts Posts
        JOIN uDebate_Forum_Threads Threads on Posts.ThreadID = Threads.ID
        GROUP BY Posts.ThreadID,Threads.View_Count) as ThreadPosts
 	RIGHT JOIN uDebate_Forum_Threads Threads on ThreadPosts.ThreadID = Threads.ID   
    INNER JOIN Users AS User_Topic ON Threads.UserID = User_Topic.UserID   
    LEFT OUTER JOIN Users AS User_Thread ON Threads.UserID = User_Thread.UserID   
   
    WHERE  Threads.TopicID = @TopicID AND ((Threads.Active = 1 AND Threads.Status=1) OR 1=@ThreadStatus) 
   
   GROUP BY Threads.ID,Threads.Description,Threads.Summary,
        Threads.EuRelated,Threads.CREATED,Threads.Language,User_Thread.Username,
        Threads.UserID,Threads.Active,Threads.Status,Threads.View_Count,Threads.Closed_Date,Threads.Position,
        User_Thread.LastName,User_Thread.FirstName  
   ORDER BY Position, PostsCount DESC">
    <SelectParameters>
        <asp:Parameter Name="ThreadStatus" Type="Int32" DefaultValue="0" />
        <asp:Parameter Name="TopicID" Type="Int32" />
    </SelectParameters>
</asp:SqlDataSource>
