using OpenQA.Selenium.PhantomJS;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using SeleniumExtras.WaitHelpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Web;
using System.Windows.Forms;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Interactions;
using Newtonsoft.Json;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text.RegularExpressions;

namespace fetlife
{
    public class Locate_m
    {
        #region fields

        private string username;

        private string password;

        private string SignInUrl;

        private string HomePageUrl;

        private string userid;

        private string user_id;

        private string UserPageUrl;

        private string name_user;

        private TextBox status_text;

        [Obsolete]
        private PhantomJSDriver driver;

        private string filepath;

        private string imgpath;

        User us;
        bool show_images_open_pictures = true;


        public int wait_time = 20;
        public int submit_time = 50;

        public int loop_value = 1;

        private Logger _log;
        #endregion



        #region constructors

        public Locate_m(string user, string pass, string id, string path)
        {
            username = user;
            password = pass;
            userid = id;
            filepath = path;
            us = new User();
            _log = new Logger(us.errors_list);
            try
            {
                if (driver != null)
                {
                    driver.Quit();
                }
                var service = PhantomJSDriverService.CreateDefaultService(Environment.CurrentDirectory);
                service.WebSecurity = false;
                service.HideCommandPromptWindow = true;
                driver = new PhantomJSDriver(service, new PhantomJSOptions(), TimeSpan.FromSeconds(submit_time));
                driver.Manage().Window.Size = new System.Drawing.Size(1240, 1240);

            }
            catch(Exception ex)
            {
                _log.Error("Locate_m constructor", ex);
                throw;
            }
            SignInUrl = "https://fetlife.com/users/sign_in";
            HomePageUrl = "https://fetlife.com/home";
            UserPageUrl = "https://fetlife.com/users/";
        }

        public void consoleprint(string msg)
        {
            //Console.WriteLine(msg);
        }

        #endregion



        #region Events
        public delegate void ShowMessagebox(string data);
        public ShowMessagebox showMessage = delegate { };
        private readonly object _us;

        #endregion

        #region functions
        public int Login()
        {

            try
            {
                //navigation_url_cond(SignInUrl,submit_time,10,"")
                driver.Navigate().GoToUrl(SignInUrl);
                ShowDriverState();
                var pathElement = FindElementIfExists(By.Id("user_login"));
                var pass = FindElementIfExists(By.Id("user_password"));
                if (pathElement == null)
                {
                    return 0;
                }
                if (pass == null)
                {
                    return 0;
                }
                pathElement.SendKeys(username);
                pass.SendKeys(password);
                var signin = driver.FindElementByCssSelector("#new_user > div.pa3 > button");
                signin.Click();
                var body = new WebDriverWait(driver, TimeSpan.FromSeconds(submit_time)).Until(ExpectedConditions.UrlContains("Home"));
                takescreenshot("afterLoginScreen");
                if (!driver.Url.Contains(HomePageUrl))
                {
                    consoleprint(driver.Title);
                    consoleprint(driver.Url);
                    return 2;
                }
                return 1;
            }
            catch (Exception ex)
            {
                _log.Error("Login Error", ex);
                takescreenshot("exception login");
                consoleprint(driver.Url);
                consoleprint(ex.Message);
            }
            return 2;
        }

        internal void destroy()
        {
            if (driver != null)
                driver.Quit();
        }

        public bool searchUser(string user_id)
        {
            bool flag = false;
            try
            {
                var pathElement = FindElementIfExists(By.CssSelector("body > nav.fixed.top-0.right-0.left-0.z-9999.flex.justify-between.items-center.h-48.pa0.bg-black > div.flex.flex-auto.self-start.items-center.h-48 > form > input"));

                pathElement.SendKeys(user_id);
                pathElement.SendKeys("\r");
                var body = new WebDriverWait(driver, TimeSpan.FromSeconds(submit_time)).Until(ExpectedConditions.UrlContains("Home"));
                if (driver.Url != HomePageUrl)
                {
                    consoleprint(driver.Title);
                    consoleprint(driver.Url);
                    flag = false;
                }
                else
                {
                    consoleprint(driver.Title);
                    consoleprint(driver.Url);

                    flag = true;
                }
            }
            catch (Exception ex)
            {
                _log.Error("Search User 1st time", ex);
                IWebElement try_diff_search_user;
                try
                {
                    try_diff_search_user = FindElementIfExists(By.CssSelector("#open_menu > div > form > input"));
                    try_diff_search_user.SendKeys(userid);
                    try_diff_search_user.SendKeys("\r");
                    var body = new WebDriverWait(driver, TimeSpan.FromSeconds(submit_time)).Until(ExpectedConditions.UrlContains("Home"));
                    if (driver.Url != HomePageUrl)
                    {
                        consoleprint(driver.Title);
                        consoleprint(driver.Url);
                        flag = false;
                    }
                    if (try_diff_search_user == null)
                    {
                        return false;
                    }
                    consoleprint(driver.Title);
                    consoleprint(driver.Url);
                    return true;

                }
                catch (Exception e)
                {
                    _log.Error("Search User 2nd time", ex);
                    return false;
                }
                flag = false;
            }
            //selectuser();
            // takescreenshot("searchuser");
            return flag;

        }
        public bool selectuser()
        {
            bool flag = false;
            try
            {
                var pathElement = FindElementIfExists(By.CssSelector("#main-content > div > form > div > div > main > div > div > div:nth-child(2) > div > div > div > div > div.flex-auto.mw-100 > div > div.relative.flex-auto.mw-100.mw-none-ns > div:nth-child(1) > a"));
                if (pathElement == null)
                {
                    MessageBox.Show("There is no user against this username " + userid);
                    return flag;
                }
                else
                {
                    pathElement.Click();
                    var body = new WebDriverWait(driver, TimeSpan.FromSeconds(submit_time)).Until(ExpectedConditions.UrlContains("user"));
                    takescreenshot("Select User");
                    
                    UserPageUrl = driver.Url;
                    Regex re = new Regex(@"\d+");
                    user_id = re.Match(UserPageUrl).ToString();
                    Console.WriteLine("This is " + user_id);
                    flag = true;
                }

            }
            catch (Exception ex)
            {
                _log.Error("Select User", ex);
                takescreenshot("Exception select user");
                consoleprint(driver.Url);
                consoleprint(ex.Message);

            }
            return flag;
        }

        public bool download_user_data()
        {
            bool flag = false;
            try
            {
                var name = FindElementIfExists(By.CssSelector("#main_content > div > div.span-13.append-1 > h2"));
                var location = FindElementIfExists(By.CssSelector("#main_content > div > div.span-13.append-1 > p > em"));
                var orientation = FindElementIfExists(By.CssSelector("#main_content > div > div.span-13.append-1 > table:nth-child(4) > tbody > tr:nth-child(1) > td"));
                var active = FindElementIfExists(By.CssSelector("div.span-13:nth-child(2) > table:nth-child(4) > tbody:nth-child(1) > tr:nth-child(2) > td:nth-child(2)"));
                var relationship_status = FindElementIfExists(By.CssSelector(".no_bullets > li:nth-child(1)"));
                var islooking = FindElementIfExists(By.CssSelector("div.span-13:nth-child(2) > table:nth-child(4) > tbody:nth-child(1) > tr:nth-child(3) > td:nth-child(2)"));
                //var about_me = FindElementIfExists(By.CssSelector("#profile > div.container > div.span-14.border > div"));
                var latest_activity = FindElementIfExists(By.CssSelector("#profile > div.container > div.span-14.border > div > p > a"));
                var about_me = FindElementsIfExists(By.CssSelector("#profile > div.container > div.span-14.border > div > p"));
                var joined_groups = FindElementsIfExists(By.ClassName("breakword"));
               // var find_wall = FindElementIfExists(By.CssSelector("#wall"));
                
               
                if (name == null)
                {
                    return flag;
                }
                if (location == null)
                {
                    return flag;
                }
                if (orientation == null)
                {
                    us.orientation = "No Orientation";
                }
                if (relationship_status == null)
                {
                    us.relationship_status = "No RelationShip Status";
                }
                if (relationship_status != null)
                {
                    us.relationship_status = relationship_status.Text.Replace("\r\n", " ");
                }
                if (islooking == null)
                {
                    us.is_lookingfor = "No isLooking For";
                }

                if (islooking != null)
                {
                    us.is_lookingfor = islooking.Text.Replace("\r\n", " ");
                }
                if (active == null)
                {
                    us.active = "No Active Data";
                }
                if (active != null)
                {
                    us.active = active.Text.Replace("\r\n", " ");

                }
                if (about_me == null)
                {
                    us.about_me.Add("There is No about me Section");
                }
                if (about_me != null)
                {
                    foreach (var abt in about_me)
                    {
                        us.about_me.Add(abt.Text.Replace("\r\n", " "));
                    }
                }
                if (joined_groups == null)
                {
                    us.groups.Add("No Groups Joined ");
                }
                if (joined_groups != null)
                {
                    foreach (var gp in joined_groups)
                    {
                        us.groups.Add(gp.Text.Replace("\r\n", " "));
                    }
                }

                
                us.name = name.Text;
                name_user = name.Text.Replace("/", " ");
                consoleprint("This is User Name" + name_user);
                us.location = location.Text;
                us.orientation = orientation.Text.Replace("\r\n", " ");
                consoleprint("This is user name" + us.name);
                if (!Directory.Exists(filepath + "\\" + name_user))
                    Directory.CreateDirectory(filepath + "\\" + name_user);
                List<IWebElement> event_going_to = null;
                try
                {
                    for(int k = 1; k<=loop_value; k++)
                    {
                        event_going_to = FindElementsIfExists(By.CssSelector("#profile > div.container > div.span-5.append-1.small > ul:nth-child(15) > li"));
                        if (event_going_to != null)
                        {
                            break;
                        }
                    }
                    foreach (var e in event_going_to)
                    {
                        var find_li = HttpUtility.HtmlEncode(e.FindElement(By.CssSelector("li> a")).Text.Replace("\r\n"," "));
                        if (find_li != null)
                            us.events_going_to.Add(find_li);
                    }
                }
                catch(Exception es)
                {
                    _log.Error("Download User Data", es);
                    us.events_going_to.Add("No Events");
                }
                List<IWebElement> event_may_be_going_to = null;
                try
                {
                    for(int i= 1; i<=loop_value; i++)
                    {
                        event_may_be_going_to = FindElementsIfExists(By.CssSelector("#profile > div.container > div.span-5.append-1.small > ul:nth-child(19) > li"));
                        if(event_may_be_going_to != null)
                        {
                            break;
                        }
                    }
                    foreach (var e in event_may_be_going_to)
                    {
                        var find_li = HttpUtility.HtmlEncode(e.FindElement(By.CssSelector("a")).Text.Replace("\r\n", " "));
                        if (find_li != null)
                            us.events_may_be_going_to.Add(find_li);
                    }
                }
                catch (Exception es)
                {
                    _log.Error("Download User Data", es);
                    us.events_may_be_going_to.Add("No Events");
                }

                List<IWebElement> groups_lead = null;
                try
                {
                    for (int i = 0; i<=loop_value; i++)
                    {
                        groups_lead = FindElementsIfExists(By.CssSelector("#profile > div.container > div.span-5.append-1.small > ul:nth-child(20) > li"),5);
                        if (groups_lead != null)
                        {
                            break;
                        }

                    }
                    foreach (var e in groups_lead)
                    {
                        var find_li = HttpUtility.HtmlEncode(e.FindElement(By.CssSelector("a")).Text.Replace("\r\n", " "));
                        if (find_li != null)
                            us.group_leads.Add(find_li);
                    }
                }
                catch (Exception es)
                {
                    _log.Error("Download user data", es);
                    us.group_leads.Add("No Leading Groups");
                }

                    flag = true;
            }
            catch (Exception ex)
            {
                _log.Error("download user data", ex);
                takescreenshot("exception download_data");
                consoleprint(driver.Url);
                consoleprint(ex.Message);
                flag = false;
            }

            return flag;
        }

        public void wall_messages()
        {
            var wall_url = UserPageUrl + "/wall_posts";

            if (navigation_url_cond(wall_url, submit_time, 5, "users") == 1)
            {

                try
                {
                    Regex re = new Regex(@"\d+");
                    string anchor_id = re.Match(FindElementIfExists(By.CssSelector("#wall-posts-app > div > div > div > main > div > div:nth-child(1) > header > div.flex > h3")).Text.ToString()).ToString();
                    int wall_counts = int.Parse(anchor_id);

                    List<IWebElement> wall_post_div = null;
                    do
                    {
                        driver.ExecuteScript("window.scrollBy(0,1000)");
                        wall_post_div = FindElementsIfExists(By.CssSelector("#wall-posts-app > div > div > div > main > div > div")).Skip(2).ToList();

                    } while (wall_post_div != null && wall_post_div.Count() != wall_counts + 2);



                    Wall wl1;
                    foreach (var w in wall_post_div)
                    {
                        wl1 = new Wall();
                        try
                        {
                            var titler = FindElementOnObject(By.CssSelector("div > article > div.flex-auto > div:nth-child(1) > a"), w);
                            if (titler != null)
                                wl1.wall_message_given_by = HttpUtility.HtmlEncode(titler.Text.Replace("\r\n", " "));
                            var message_is = FindElementOnObject(By.CssSelector("div > article > div.flex-auto > div.breakword.mb2.comment__copy > p"), w);
                            if (message_is != null)
                                wl1.wall_message = HttpUtility.HtmlEncode(message_is.Text.Replace("\r\n", " "));
                            us.wall_list.Add(wl1);
                        }
                        catch (Exception ess)
                        {
                            _log.Error("Wall messages error", ess);
                        }

                    }
                }

                catch (Exception e)
                {
                    _log.Error("Walll Messages Error", e);
                }
            }

        }

        public void open_pictures() 
        {
            bool flag = false;
            IWebElement open_link_pic;
            try
            {
                open_link_pic = FindElementIfExists(By.CssSelector("#user_pictures_link"));
                if (open_link_pic != null)
                {
                    var counter_img = FindElementIfExists(By.CssSelector("#main_content > div > div.span-6 > div"));
                    var start_index = counter_img.Text.IndexOf('(') + 1;
                    var end_index = counter_img.Text.IndexOf(')');
                    var img_count = int.Parse(counter_img.Text.Substring(start_index, end_index - start_index));
                    try
                    {
                        open_link_pic.Click();
                    }
                    catch (Exception ex)
                    {
                        _log.Error("open pictures open link click ", ex);
                    }
                    try
                    {
                        var body = new WebDriverWait(driver, TimeSpan.FromSeconds(submit_time)).Until(ExpectedConditions.ElementExists(By.Id("pictures")));
                    }
                    catch(Exception ex)
                    {
                        _log.Error("open pictures web driver wait for pictures", ex);
                        try
                        {
                            var body = new WebDriverWait(driver, TimeSpan.FromSeconds(submit_time)).Until(ExpectedConditions.ElementExists(By.Id("pictures")));
                        }
                        catch(Exception ex1)
                        {
                            _log.Error("open pictures web driver wait for pictures second time", ex1);
                            throw ex1;
                        }
                    }
                    takescreenshot("images page", show_images_open_pictures);
                    List<IWebElement> select_pic = null;
                    List<string> urls = new List<string>();
                    IWebElement next;
                    try
                    {
                        var footer = FindElementIfExists(By.CssSelector("#pictures > footer"));
                        if(footer.GetCssValue("display") == "none")
                        {
                            next = null;
                        }
                        else
                        {
                            next = FindElementIfExists(By.CssSelector("#pictures > footer > div > a.next_page"));
                        }
                        if (next != null)
                        {
                            do
                            {
                                takescreenshot("image scroll");
                                driver.FindElementByTagName("body").SendKeys("t");
                                driver.ExecuteScript("window.scrollBy(10,5000)");

                                for(int l= 1; l<=loop_value; l++)
                                {
                                    select_pic = FindElementsIfExists(By.CssSelector("#pictures > ul > li"));
                                    if (select_pic != null)
                                    {
                                        break;
                                    }
                                }
                                

                                foreach (var picLi in select_pic)
                                {
                                    var anch = FindElementOnObject(By.CssSelector("a"), picLi);
                                    if (anch != null)
                                        urls.Add(anch.GetAttribute("href"));

                                }
                                if (next != null)
                                    next.Click();
                            } while (next != null);
                        }
                        else
                        {
                            us.errors_list.Add("open pictures next not found scrolling down");
                            do
                            {
                                driver.ExecuteScript("window.scrollBy(0,1000)");
                                select_pic = FindElementsIfExists(By.CssSelector("#pictures > ul > li"));
                            } while (select_pic != null && select_pic.Count() != img_count);

                            if(select_pic != null)
                                foreach (var picLi in select_pic)
                                {
                                    var anch = FindElementOnObject(By.CssSelector("a"), picLi);
                                    if (anch != null)
                                        urls.Add(anch.GetAttribute("href"));
                                }
                        }
                    }
                    catch(Exception exx)
                    {
                        takescreenshot("open pictures error in finding pics", show_images_open_pictures);
                        us.errors_list.Add("open pictures error in finding pics " + exx.Message + exx.StackTrace);
                        _log.Error("open pictures picking up lics", exx);
                    }
                    
                    
                    int i = 0;

                    imgpath = filepath + "\\" + name_user + "\\images";

                    if (!Directory.Exists(imgpath))
                        Directory.CreateDirectory(imgpath);
                    int c = 1;
                    foreach (var url in urls)
                    {
                        i++;
                        consoleprint("this is url " + url + "\n");
                        navigation_url_cond(url, submit_time, 5, "pictures/");
                        
                            var selector = "#user_picture > div > figure > a > img";
                            var pic_address = FindElementIfExists(By.CssSelector("#user_picture > div > figure > a > img"));
                            var alt_attribute = removeIlegalCharaters(pic_address.GetAttribute("alt").ToString());
                            var file_alt_attribute = alt_attribute;
                            if (alt_attribute == "")
                            {
                                alt_attribute = "Image ";
                            }
                            if (alt_attribute != "" && alt_attribute.Length >= 30)
                            {
                                alt_attribute = alt_attribute.Substring(0, 30);
                            }
                            alt_attribute = (i.ToString().PadLeft(3, '0')) + " - " + alt_attribute;
                            var filename = filepath + "\\" + name_user + "\\images" + "\\" + alt_attribute;
                            getImage(filename, i.ToString());

                            consoleprint("this is src  " + pic_address.GetAttribute("src"));

                            Images_class i_local_obj;
                            i_local_obj = new Images_class();
                            i_local_obj.img = alt_attribute;
                            i_local_obj.alt_atr = file_alt_attribute;

                            List<IWebElement> how_many_comment = null;
                            try
                            {
                                Comments c_local_obj;
                                
                                for (int k = 1; k <= loop_value; k++)
                                {
                                    how_many_comment = FindElementsIfExists(By.CssSelector("#comments > article"), 2);
                                    if (how_many_comment != null)
                                    { 
                                        break;
                                    }
                                }
                                foreach (var cmt in how_many_comment)
                                {
                                    c_local_obj = new Comments();

                                    var cmt_given_by = FindElementOnObject(By.CssSelector("div.fl-flag__body > header"), cmt);
                                    if (cmt_given_by != null)
                                        c_local_obj.comment_given_by = HttpUtility.HtmlEncode(cmt_given_by.Text.Replace("\r\n", " "));

                                    var cmt_description = FindElementOnObject(By.CssSelector("div.fl-flag__body > div > p"), cmt);
                                    if (cmt_description != null)
                                        c_local_obj.comment_description = HttpUtility.HtmlEncode(cmt_description.Text.Replace("\r\n", " "));

                                    var cmt_date = FindElementOnObject(By.CssSelector("div.fl-flag__body > footer > span:nth-child(1) > a > time"), cmt);
                                    if (cmt_date != null)
                                        c_local_obj.comment_date = HttpUtility.HtmlEncode(cmt_date.GetAttribute("datetime").ToString());

                                    i_local_obj.comments.Add(c_local_obj);
                                }
                                us.imgs_list.Add(i_local_obj);

                            }
                            catch (Exception el)
                            {
                                takescreenshot("open pictures error in getting comments of picture " + i.ToString(), show_images_open_pictures);
                                _log.Error("open pictures filling comments", el);
                                us.errors_list.Add("Line 476 : This is error thrown by Pics " + el.Message + "\n" + el.StackTrace);
                                 i_local_obj.comments.Add(new Comments());
                            us.imgs_list.Add(i_local_obj);
                        }
                       
                    }
                    us.img_count = urls.Count();
                   
                }
            }
            catch (Exception e)
            {
                _log.Error("open pictures", e);
                us.errors_list.Add("Line 686: This is error thrown by Pics " + e.Message);
                //us.img_count = 0;
            }


        }

        public bool goToUserProfile(string id)
        {
            navigation_url_cond("https://fetlife.com/users/" + id, submit_time, 3, id);
            //driver.Navigate().GoToUrl("https://fetlife.com/users/" + id);
            //var body = new WebDriverWait(driver, TimeSpan.FromSeconds(submit_time)).Until(ExpectedConditions.UrlContains(id));
            var page_not_found = FindElementIfExists(By.CssSelector("#main-content > div > div.container-fluid.mw1200 > div > div > div:nth-child(1) > div > div > div.f2.f1-m.f-subheadline-l.b.secondary"));
            if (page_not_found != null)
            {
                return false;
            }
            UserPageUrl = UserPageUrl + id;
            user_id = id;
            return true;
        }


        public bool posts_section()
        {
            var posts_url = UserPageUrl + "/posts";
            var chk_url_change = UserPageUrl;
            consoleprint("This is Post page url" + posts_url);
            if(navigation_url_cond(posts_url, submit_time,5, "users") == 1) { 
                Posts ps_local_obj;
                    int i = 1;
                    IWebElement find_next;
                    do
                    {
                        List<IWebElement> find_articles = null;
                        for (int s = 1; s<=loop_value; s++)
                        {
                            find_articles = FindElementsIfExists(By.CssSelector("#maincontent > div:nth-child(2) > section > article"), 5);
                            if (find_articles != null)
                            {
                                break;
                               // findtwo = find_articles;
                            }
                        }
                    
                        foreach (var pt in find_articles)
                        {

                            ps_local_obj = new Posts();
                            var title = FindElementOnObject(By.CssSelector("#maincontent > div:nth-child(2) > section > article > header > div > h2 > a"), pt);
                            if (title != null)
                                ps_local_obj.title = removeIlegalCharaters(title.Text.Replace("\r\n", " "));

                            var time = FindElementOnObject(By.CssSelector("#maincontent > div:nth-child(2) > section > article > header > div > p > time"), pt);
                            if (time != null)
                                ps_local_obj.time = time.GetAttribute("datetime").ToString();

                            var des = FindElementOnObject(By.CssSelector("#maincontent > div:nth-child(2) > section > article > div"), pt);
                            if (des != null)
                                ps_local_obj.description = removeIlegalCharaters(des.Text.Replace("\r\n", " ")); // \r\nremove kerna ha

                            IWebElement anchor;
                            try
                            {
                                anchor = pt.FindElement(By.CssSelector("#maincontent > div:nth-child(2) > section > article > header > div > p > a:nth-child(2)"));
                                ps_local_obj.comment_url = anchor.GetAttribute("href").ToString();

                            }
                            catch (Exception exx)
                            {
                                _log.Error("posts_section finding comment anchor", exx);
                                us.errors_list.Add("posts_section finding comment anchor " + exx.Message + exx.StackTrace);
                            anchor = null;
                            }
                            ps_local_obj.posts_counter = removeIlegalCharaters(title.Text.Replace("\r\n", " "));
                            us.posts_list.Add(ps_local_obj);
                            i++;
                        }
                        find_next = FindElementIfExists(By.CssSelector("#maincontent > div:nth-child(2) > section > footer > div > a.next_page"), 15);
                        if (find_next != null)
                        {
                            find_next.Click();
                            Thread.Sleep(1000);
                        }
                    } while (find_next != null);


                    foreach (var c in us.posts_list)
                    {
                        if (c.comment_url == null)
                        {
                            var comment = new Comments();
                            //comment.comment_description="";
                            c.comments.Add(comment);
                        }
                        else
                        {
                            navigation_url_cond(c.comment_url, submit_time, 5, "users");
                            List<IWebElement> comments_main_div = null;
                            try
                            {
                                for (int l = 1; l <= loop_value; l++)
                                {
                                    comments_main_div = FindElementsIfExists(By.CssSelector("#comments > div"), 5);
                                    if (comments_main_div != null)
                                    {
                                        break;
                                    }
                                }

                                foreach (var cm in comments_main_div)
                                {
                                    var comment = new Comments();
                                    comment.comment_given_by = removeIlegalCharaters(cm.FindElement(By.CssSelector("#comments > div > div.js-comment-not-loved > div:nth-child(3) > article > div.flex-auto.relative.pu3 > div:nth-child(1) > a")).Text);
                                    comment.comment_description = removeIlegalCharaters(cm.FindElement(By.CssSelector("#comments > div > div.js-comment-not-loved > div:nth-child(3) > article > div.flex-auto.relative.pu3 > div.breakword.mb2.comment__copy > p")).Text);
                                    comment.comment_date = cm.FindElement(By.CssSelector("#comments > div > div.js-comment-not-loved > div:nth-child(3) > article > div.flex-auto.relative.pu3 > div.flex.items-center.flex-wrap.w-100.f6.lh-copy > a.link.mid-gray.hover-gray > time")).GetAttribute("datetime").ToString();
                                    c.comments.Add(comment);
                                }
                            }
                            catch (Exception e)
                            {
                                _log.Error("posts_section filling comment", e);
                                us.errors_list.Add("posts_section filling comment " + e.Message + e.StackTrace);
                                comments_main_div = null;
                            }
                        }

                    }
                
                }


            return false;
        }
        public void Conversations()
        {
            var conversations_url = "https://fetlife.com/inbox/with?with=" + user_id;
//var chk_url_change = driver.Url;
            consoleprint("This is Conversation page url" + conversations_url);

            if (navigation_url_cond(conversations_url, submit_time, 5, "inbox") == 1)
            {
                List<IWebElement> main_divs = null;
                try
                {
                    scroll_to_end();
                    for (int s = 1; s <= loop_value; s++)
                    {
                        main_divs = FindElementsIfExists(By.CssSelector("#chat > div.flex.flex-column.w-100.mt0 > div"));
                        if (main_divs != null)
                        {
                            break;
                        }
                    }

                    takescreenshot("After Scroll To");
                    foreach (var divs in main_divs)
                    {
                        var div = divs.FindElement(By.CssSelector("#chat > div.flex.flex-column.w-100.mt0 > div > div"));
                        var oncl = div.GetAttribute("onclick");
                        Regex re = new Regex(@"\d+");
                        string anchor_id = re.Match(oncl).ToString();
                        var conver = new Conversation_Class();
                        conver.conversation_url = "https://fetlife.com/conversations/" + anchor_id + "#newest_message";
                        us.conv_list.Add(conver);
                    }
                    int i = 1;
                    foreach (var s in us.conv_list)
                    {
                        navigation_url_cond(s.conversation_url, submit_time, 15, "conversations");

                        var title = removeIlegalCharaters(FindElementIfExists(By.CssSelector("#main-content > div > div > div.fixed.top-0.left-0.left-250-l.right-250-xl.right-0.z-4.bg-mid-primary.shadow-4.bb.b--primary.items-center.flex.top-48 > div > h6 > span")).Text);
                        if (title == " ")
                        {
                            title = "No Subject";
                        }
                        title = (i.ToString().PadLeft(3, '0')) + " - " + title;
                        s.title = title;
                        List<IWebElement> multi_divs_load = null;
                        try
                        {
                            for (int k = 1; k <= loop_value; k++)
                            {
                                multi_divs_load = FindElementsIfExists(By.CssSelector("#main-content > div > div > main > div.ph3.ph4-ns > div > div.pb1.pb0-ns > div"));
                                if (multi_divs_load != null)
                                {
                                    break;
                                }
                            }
                            foreach (var d in multi_divs_load)
                            {
                                var cn = new Conversation_fields();
                                var sender = removeIlegalCharaters(d.FindElement(By.CssSelector("#main-content > div > div > main > div.ph3.ph4-ns > div > div.pb1.pb0-ns > div > div > div.flex.items-baseline > a")).Text);
                                var message = removeIlegalCharaters(d.FindElement(By.CssSelector("#main-content > div > div > main > div.ph3.ph4-ns > div > div.pb1.pb0-ns > div > div > div.db.comment__copy.pt1")).Text.Replace("\r\r", " "));
                                cn.sender_name = sender;
                                cn.message = message;
                                s.conversations.Add(cn);
                            }

                        }
                        catch (Exception e)
                        {
                            _log.Error("conversations messages", e);
                            multi_divs_load = null;
                            var cn = new Conversation_fields();
                            var sender = removeIlegalCharaters(FindElementIfExists(By.CssSelector("#main-content > div > div > main > div.ph3.ph4-ns > div > div.pb1.pb0-ns > div > div > div.flex.items-baseline > a")).Text);
                            var message = removeIlegalCharaters(FindElementIfExists(By.CssSelector("#main-content > div > div > main > div.ph3.ph4-ns > div > div.pb1.pb0-ns > div > div > div.db.comment__copy.pt1")).Text);
                            cn.sender_name = sender;
                            cn.message = message;
                            s.conversations.Add(cn);
                        }
                        i++;

                    }
                }
                catch (Exception e)
                {
                    _log.Error("conversations complete", e);
                    main_divs = null;

                }
            }

        }

        public void scroll_to_end()
        {
            try
            {
                long lastHeight = (long)(driver.ExecuteScript("return document.getElementById('chat').scrollHeight"));
                int scroll_height = 1500;
                driver.ExecuteScript("document.getElementById('chat').style.height = '1000px'");
                int k = 0;
                while (k < 10)
                {

                    driver.ExecuteScript("document.getElementById('chat').scroll(1," + scroll_height + ")");
                    Thread.Sleep(5000);
                    takescreenshot("scroll");
                    long newHeight = (long)(driver.ExecuteScript("return document.getElementById('chat').scrollHeight"));
                    if (newHeight == lastHeight)
                    {
                        Thread.Sleep(5000);
                        newHeight = (long)(driver.ExecuteScript("return document.getElementById('chat').scrollHeight"));
                        if (newHeight == lastHeight)
                        {
                            break;
                        }
                    }
                    lastHeight = newHeight;
                    scroll_height += 2000;
                    k++;
                }

            }
            catch (Exception e)
            {
                _log.Error("scroll to end", e);
                MessageBox.Show(e.Message);
            }
        }

        public void latest_activities()
        {
            var latest_activity_url = UserPageUrl + "/activity";

            consoleprint("This is User page url" + latest_activity_url);
            navigation_url_cond(latest_activity_url, submit_time, 2, "users");
            //driver.Navigate().GoToUrl(latest_activity_url);
            //var sc = driver.GetScreenshot();
            //sc.SaveAsFile("navigate.jpg");
            //var body = new WebDriverWait(driver, TimeSpan.FromSeconds(submit_time)).Until(ExpectedConditions.UrlContains("users"));
            IWebElement find_activity = null;
            try
            {
                find_activity = FindElementIfExists(By.CssSelector("#mini_feed"));
                if (find_activity != null)
                {
                    if (click_again_and_again() == false)
                    {
                        //var sc1 = driver.GetScreenshot();
                        //sc1.SaveAsFile("afterclick.jpg");
                        List<IWebElement> activities_li = null;
                        for(int l=1; l<=loop_value; l++)
                        {
                            activities_li = FindElementsIfExists(By.CssSelector("#mini_feed > li"), 100);
                            if (activities_li != null)
                            {
                                break;
                            }
                        }
                        foreach (var act in activities_li)
                        {
                            us.activites_list.Add(removeIlegalCharaters(act.Text.Replace("\r\n", " ")));
                           
                        }
                    }

                }
                else
                {
                    us.activites_list.Add("No Activities Found");
                }


            }
            catch (Exception ex)
            {
                _log.Error("latest activities", ex);
                find_activity = null;
            }
        }

        public void mutual_friends_data()
        {
            var curr_url = driver.Url;
            var next_url = UserPageUrl + "/friends/mutual";
            navigation_url_cond(next_url, submit_time, 20, "friends");
             try
                {
                    IWebElement find_next;
                
                    do
                    {
                            List<IWebElement> find_divs = null;
                            for (int i = 1; i<=loop_value; i++)
                            {
                                find_divs = FindElementsIfExists(By.CssSelector("#main-content > div > div.w-100.center.ph4-l.mw1200.ph3.pt4 > div > main > div > div"));
                                if (find_divs != null)
                                {
                                    break;  
                                }
                            }
                    
                            foreach (var fr in find_divs)
                            {
                                try
                                {
                                    var friends_obj = new Mutual_Friends();
                                    var name = removeIlegalCharaters(fr.FindElement(By.CssSelector("div > div > div > div.flex-auto.mw-100 > div > div.relative.flex-auto.mw-100.mw-none-ns > div > a")).Text.Replace("\r\n", " "));
                                    var loc = removeIlegalCharaters(fr.FindElement(By.CssSelector("div > div > div > div.flex-auto.mw-100 > div > div.relative.flex-auto.mw-100.mw-none-ns > div.f6.lh-copy.fw4.silver.nowrap.truncate")).Text.Replace("\r\n", " "));
                                    var posts_pic_vids = removeIlegalCharaters(fr.FindElement(By.CssSelector("div > div > div > div.flex-auto.mw-100 > div > div.relative.flex-auto.mw-100.mw-none-ns > div.relative.pd1.f6.fw4.lh-copy.mid-gray.nowrap.truncate")).Text.Replace("\r\n", " "));
                                    if (name != null)
                                        friends_obj.name = name;
                                    if (loc != null)
                                        friends_obj.loc = loc;
                                    if (posts_pic_vids != null)
                                        friends_obj.posts_pic_vids = posts_pic_vids;

                                    us.mutual_friends.Add(friends_obj);

                                }
                                catch (Exception el)
                                {
                                    _log.Error("mutauls friends", el);
                                }

                            }
       
                            find_next = FindElementIfExists(By.CssSelector("#main-content > div > div.w-100.center.ph4-l.mw1200.ph3.pt4 > div > main > footer > div.pagination > a.next_page"));
                            if (find_next != null)
                            {
                                find_next.Click();
                                Thread.Sleep(1000);
                            }

                        } while (find_next != null);
            }
            catch (Exception ee)
            {
                _log.Error("mutauls friends", ee);
            }
            

        }

        public int navigation_url_cond(string url,int w_time, int l_time,string contains)
        {
            bool flag = true;
            var curr_url = driver.Url;
            driver.Navigate().GoToUrl(url);
            for (int i = 1; i <= l_time; i++)
            {
                try
                {
                    var body = new WebDriverWait(driver, TimeSpan.FromSeconds(w_time)).Until(ExpectedConditions.UrlContains(contains));
                    if (curr_url != driver.Url)
                    {
                        flag = false;
                        break;
                        return 1;
                    }
                }
                catch(Exception e)
                {
                    _log.Error("navigation url cond", e);
                }
                
            }
            if(flag == true)
            {
                MessageBox.Show("Check Your Internet Connection", "Network Problem");
            }
            return 0;
        }
        public void friends_user()
        {
            navigation_url_cond(UserPageUrl + "/friends",submit_time,5,"friends"); 
            takescreenshot("Friends Page");
            try
            {
                IWebElement find_next;
                do
                {
                    List<IWebElement> find_divs = null;
                    for(int i= 1; i<=loop_value; i++)
                    {
                        find_divs =  FindElementsIfExists(By.CssSelector("#main-content > div > div.w-100.center.ph4-l.mw1200.ph3.pt4 > div > main > div > div"));
                        if (find_divs != null)
                        {
                            break;
                        } 
                    }
                   foreach (var fr in find_divs)
                        {
                            try
                            {
                                var friends_obj = new List_Friends();
                                var name = removeIlegalCharaters(fr.FindElement(By.CssSelector("div > div > div > div.flex-auto.mw-100 > div > div.relative.flex-auto.mw-100.mw-none-ns > div > a")).Text.Replace("\r\n", " "));
                                var loc = removeIlegalCharaters(fr.FindElement(By.CssSelector("div > div > div > div.flex-auto.mw-100 > div > div.relative.flex-auto.mw-100.mw-none-ns > div.f6.lh-copy.fw4.silver.nowrap.truncate")).Text.Replace("\r\n", " "));
                                var posts_pic_vids = removeIlegalCharaters(fr.FindElement(By.CssSelector("div > div > div > div.flex-auto.mw-100 > div > div.relative.flex-auto.mw-100.mw-none-ns > div.relative.pd1.f6.fw4.lh-copy.mid-gray.nowrap.truncate")).Text.Replace("\r\n", " "));
                                if (name != null)
                                    friends_obj.name = name;
                                if (loc != null)
                                    friends_obj.loc = loc;
                                if (posts_pic_vids != null)
                                    friends_obj.posts_pic_vids = posts_pic_vids;

                                us.list_friends.Add(friends_obj);

                            }
                            catch (Exception el)
                            {
                                _log.Error("friends user", el);
                            }

                        }
                   
                    find_next = FindElementIfExists(By.CssSelector("#main-content > div > div.w-100.center.ph4-l.mw1200.ph3.pt4 > div > main > footer > div.pagination > a.next_page"));
                    if (find_next != null)
                    {
                        find_next.Click();
                        Thread.Sleep(1000);
                    }

                } while (find_next != null);
            }
            catch (Exception ee)
            {
                _log.Error("friends user", ee);
            }
        }

        public void following_friends_data()
        {
            var next_url = UserPageUrl + "/following";
            if (navigation_url_cond(next_url, submit_time, 5, "following") == 1)
            {
                //driver.Navigate().GoToUrl(next_url);
                //var body = new WebDriverWait(driver, TimeSpan.FromSeconds(submit_time)).Until(ExpectedConditions.UrlContains("following"));
                takescreenshot("Following Friends");
                try
                {
                    IWebElement find_next;
                    do
                    {
                        List<IWebElement> find_divs = null;
                        for (int i = 1; i <= loop_value; i++)
                        {
                            find_divs = FindElementsIfExists(By.CssSelector("#main-content > div > div.w-100.center.ph4-l.mw1200.ph3.pt4 > div > main > div > div"));
                            if (find_divs != null)
                            {
                                break;
                            }
                        }
                        foreach (var fr in find_divs)
                        {
                            try
                            {
                                var friends_obj = new Following();
                                var name = removeIlegalCharaters(fr.FindElement(By.CssSelector("div > div > div > div.flex-auto.mw-100 > div > div.relative.flex-auto.mw-100.mw-none-ns > div > a")).Text.Replace("\r\n", " "));
                                var loc = removeIlegalCharaters(fr.FindElement(By.CssSelector("div > div > div > div.flex-auto.mw-100 > div > div.relative.flex-auto.mw-100.mw-none-ns > div.f6.lh-copy.fw4.silver.nowrap.truncate")).Text.Replace("\r\n", " "));
                                var posts_pic_vids = removeIlegalCharaters(fr.FindElement(By.CssSelector("div > div > div > div.flex-auto.mw-100 > div > div.relative.flex-auto.mw-100.mw-none-ns > div.relative.pd1.f6.fw4.lh-copy.mid-gray.nowrap.truncate")).Text.Replace("\r\n", " "));
                                if (name != null)
                                    friends_obj.name = name;
                                if (loc != null)
                                    friends_obj.loc = loc;
                                if (posts_pic_vids != null)
                                    friends_obj.posts_pic_vids = posts_pic_vids;
                                us.following_friends.Add(friends_obj);

                            }
                            catch (Exception el)
                            {
                                _log.Error("following friends", el);
                            }

                        }

                        find_next = FindElementIfExists(By.CssSelector("#main-content > div > div.w-100.center.ph4-l.mw1200.ph3.pt4 > div > main > footer > div.pagination > a.next_page"));
                        if (find_next != null)
                        {
                            find_next.Click();
                            Thread.Sleep(1000);
                        }

                    } while (find_next != null);
                }
                catch (Exception ee)
                {
                    _log.Error("following friends", ee);
                }

            }
        }

        public void follower_friends_data()
        {
            
            var next_url = UserPageUrl + "/followers";
            if (navigation_url_cond(UserPageUrl + "/followers", submit_time, 5, "followers") == 1)
            {
                //driver.Navigate().GoToUrl(next_url);
                //var body = new WebDriverWait(driver, TimeSpan.FromSeconds(submit_time)).Until(ExpectedConditions.UrlContains("followers"));
                takescreenshot("Following Friends");
                try
                {
                    IWebElement find_next;
                    do
                    {
                        List<IWebElement> find_divs = null;
                        for (int i = 1; i <= loop_value; i++)
                        {
                            find_divs = FindElementsIfExists(By.CssSelector("#main-content > div > div.w-100.center.ph4-l.mw1200.ph3.pt4 > div > main > div > div"));
                            if (find_divs != null)
                            {
                                break;
                            }
                        }
                        foreach (var fr in find_divs)
                        {
                            try
                            {
                                var friends_obj = new Followers();
                                var name = removeIlegalCharaters(fr.FindElement(By.CssSelector("div > div > div > div.flex-auto.mw-100 > div > div.relative.flex-auto.mw-100.mw-none-ns > div > a")).Text.Replace("\r\n", " "));
                                var loc = removeIlegalCharaters(fr.FindElement(By.CssSelector("div > div > div > div.flex-auto.mw-100 > div > div.relative.flex-auto.mw-100.mw-none-ns > div.f6.lh-copy.fw4.silver.nowrap.truncate")).Text.Replace("\r\n", " "));
                                var posts_pic_vids = removeIlegalCharaters(fr.FindElement(By.CssSelector("div > div > div > div.flex-auto.mw-100 > div > div.relative.flex-auto.mw-100.mw-none-ns > div.relative.pd1.f6.fw4.lh-copy.mid-gray.nowrap.truncate")).Text.Replace("\r\n", " "));
                                if (name != null)
                                    friends_obj.name = name;
                                if (loc != null)
                                    friends_obj.loc = loc;
                                if (posts_pic_vids != null)
                                    friends_obj.posts_pic_vids = posts_pic_vids;

                                us.follower_friends.Add(friends_obj);

                            }
                            catch (Exception el)
                            {
                                _log.Error("follower friends", el);
                            }

                        }

                        find_next = FindElementIfExists(By.CssSelector("#main-content > div > div.w-100.center.ph4-l.mw1200.ph3.pt4 > div > main > footer > div.pagination > a.next_page"));
                        if (find_next != null)
                        {
                            find_next.Click();
                            Thread.Sleep(1000);
                        }

                    } while (find_next != null);
                }
                catch (Exception ee)
                {
                    _log.Error("follower friends", ee);
                }
            }
        }

        public string removeIlegalCharaters(string name)
        {
            return Regex.Replace(name, @"[^0-9a-zA-Z ]+", "");
            //string regexSearch = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
            //Regex r = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));
            //return r.Replace(name, "");
        }

        public bool click_again_and_again()
        {
            IWebElement find_view_more;
            try
            {
                find_view_more = FindElementIfExists(By.CssSelector("#view_more_link > a"));

                while (find_view_more != null)
                {
                    find_view_more.Click();
                }
            }
            catch (Exception ex)
            {
                _log.Error("click again and again", ex);
            }
            return false;
        }

        public bool click_again_and_again_common()
        {
            IWebElement find_view_more;
            try
            {
                find_view_more = FindElementIfExists(By.CssSelector("#main-content > div > div.w-100.center.ph4-l.mw1200.ph3.pt4 > div > main > footer > div.pagination > a.next_page"));

                while (find_view_more != null)
                {
                    find_view_more.Click();
                }
            }
            catch (Exception ex)
            {
                _log.Error("click again and again common", ex);
            }
            return false;
        }



        private void takescreenshot(string name, bool isError = false)
        {
            try
            {
                if(isError)
                {
                    var sc = driver.GetScreenshot();
                    if(!Directory.Exists("Error Images"))
                    {
                        Directory.CreateDirectory("Error Images");
                    }
                    sc.SaveAsFile("Error Images\\" + name + ".jpg");
                }
                else
                {
                    //var sc = driver.GetScreenshot();
                    //sc.SaveAsFile(name + ".jpg");
                }
            }
            catch
            {
            }

        }

        private void ShowDriverState()
        {
            consoleprint(driver.Url);
            consoleprint(driver.Title);
            consoleprint(driver.SessionId.ToString());
        }


        private IWebElement FindElementIfExists(By by, int wait_time1 = -1)
        {
            try
            {
                if (wait_time1 == -1)
                    wait_time1 = wait_time;
                IWebElement webElement;
                new WebDriverWait(driver, TimeSpan.FromSeconds(wait_time)).Until(ExpectedConditions.ElementExists(by));
                var webElements = driver.FindElements(by);
                if (webElements.Count >= 1)
                {
                    webElement = webElements.First<IWebElement>();
                }
                else
                {
                    webElement = null;
                }
                return webElement;
            }
            catch
            {
                return null;
            }
        }

        private List<IWebElement> FindElementsIfExists(By by, int wait_time1 = -1)
        {
            try
            {
                if (wait_time1 == -1)
                    wait_time1 = wait_time;
                new WebDriverWait(driver, TimeSpan.FromSeconds(wait_time1)).Until(ExpectedConditions.ElementExists(by));
                var webElements = driver.FindElements(by);
                return webElements.ToList();
            }
            catch
            {
                return null;
            }
        }

        private IWebElement FindElementOnObject(By by, IWebElement el)
        {
            try
            {
                var webElement = el.FindElement(by);
                return webElement;
            }
            catch
            {
                return null;
            }
        }

        private List<IWebElement> FindElementsOnObject(By by, IWebElement el)
        {
            try
            {
                var webElement = el.FindElements(by);
                return webElement.ToList();
            }
            catch
            {
                return null;
            }
        }

        private string cookieString(IWebDriver driver)
        {
            string str = string.Join("; ",
                from c in driver.Manage().Cookies.AllCookies
                select string.Format("{0}={1}", c.Name, c.Value));
            return str;
        }

        public void getImage(string filename, string selector)
        {
            var base64string = driver.ExecuteScript(@"
                var c = document.createElement('canvas');
                var ctx = c.getContext('2d');
                var img = document.getElementsByClassName('fl-picture__img')[0];
                c.height=img.naturalHeight;
                c.width=img.naturalWidth;
                ctx.drawImage(img, 0, 0,img.naturalWidth, img.naturalHeight);
                var base64String = c.toDataURL();
                return base64String;
            ") as string;

            var base64 = base64string.Split(',').Last();
            try
            {
                using (var stream = new MemoryStream(Convert.FromBase64String(base64)))
                {
                    using (var bitmap = new Bitmap(stream))
                    {
                        bitmap.Save(filename + ".png", ImageFormat.Png);
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error("getImage", ex);
                try
                {
                    using (var stream = new MemoryStream(Convert.FromBase64String(base64)))
                    {
                        using (var bitmap = new Bitmap(stream))
                        {
                            bitmap.Save(filename + ".png", ImageFormat.Png);
                        }
                    }
                }
                catch (Exception ex1)
                {
                    takescreenshot("open pictures error in getting picture " + selector, show_images_open_pictures);
                    _log.Error("getImage 2nd time", ex);
                    us.errors_list.Add("Unable to Save FileName: " + filename + " Index: " + selector + " Exception: " + ex1.Message);
                }

            }

        }

        public void data_write()
        {
            filepath = filepath + "\\" + name_user;
            if (!Directory.Exists(filepath))
            {
                Directory.CreateDirectory(filepath);
            }
            string file_ext = "general_info.txt";
            using (StreamWriter outputFile = new StreamWriter(Path.Combine(filepath, file_ext)))
            {
                outputFile.WriteLine("Name\n\t->" + us.name);

                outputFile.WriteLine("Location\n\t->" + us.location);

                outputFile.WriteLine("RelationShip Status\n\t->" + us.relationship_status);

                outputFile.WriteLine("Orientation\n\t->" + us.orientation);

                outputFile.WriteLine("Active\n\t->" + us.active);

                outputFile.WriteLine("Is_looking For\n\t->" + us.is_lookingfor);

                outputFile.WriteLine("About Section is given Below\n");
                int a = 1;
                foreach (var abt in us.about_me)
                {
                    outputFile.WriteLine("(" + a + ")" + abt);
                    a++;
                }


                outputFile.WriteLine("Events Going To Be Held are: ");

                int b = 1;
                foreach (var abt in us.events_going_to)
                {
                    outputFile.WriteLine("(" + b + ")" + abt);
                    b++;
                }

                outputFile.WriteLine("Events May be Going To Be Held are: ");

                int c = 1;
                foreach (var abt in us.events_may_be_going_to)
                {
                    outputFile.WriteLine("(" + c + ")" + abt);
                    c++;
                }


            }

            string file_ext1 = "Activities.txt";
            using (StreamWriter outputFile = new StreamWriter(Path.Combine(filepath, file_ext1)))
            {
                outputFile.WriteLine("Activities are given below\n");
                int a = 1;
                foreach (var act in us.activites_list)
                {
                    outputFile.WriteLine("(" + a + ")" + act);
                    a++;
                }
            }


            var filepath_for_friends = filepath + "\\" + "Friends Data";
            if (!Directory.Exists(filepath_for_friends))
                Directory.CreateDirectory(filepath_for_friends);
            string friend = "Friends.txt";
            using (StreamWriter outputFile = new StreamWriter(Path.Combine(filepath_for_friends, friend)))
            {
                outputFile.WriteLine("Friends Lists are given below\n");
                int a = 1;
                foreach (var act in us.list_friends)
                {
                    outputFile.WriteLine("(" + a + ") " + " Name :" + act.name);
                    outputFile.WriteLine("    " + "Location:" + act.loc);
                    outputFile.WriteLine("    " + "Data :" + act.posts_pic_vids);
                    a++;
                }
            }
            string m_friend = "Mutual_Friends.txt";
            using (StreamWriter outputFile = new StreamWriter(Path.Combine(filepath_for_friends, m_friend)))
            {
                outputFile.WriteLine("Mutual Friends Lists are given below\n");
                int a = 1;
                foreach (var act in us.mutual_friends)
                {
                    outputFile.WriteLine("(" + a + ") " + " Name :" + act.name);
                    outputFile.WriteLine("    " + "Location:" + act.loc);
                    outputFile.WriteLine("    " + "Data :" + act.posts_pic_vids);
                    a++;
                }
            }
            string fo_friend = "Following_Friends.txt";
            using (StreamWriter outputFile = new StreamWriter(Path.Combine(filepath_for_friends, fo_friend)))
            {
                outputFile.WriteLine("Following Friends Lists are given below\n");
                int a = 1;
                foreach (var act in us.following_friends)
                {
                    outputFile.WriteLine("(" + a + ") " + " Name :" + act.name);
                    outputFile.WriteLine("    " + "Location:" + act.loc);
                    outputFile.WriteLine("    " + "Data :" + act.posts_pic_vids);
                    a++;
                }
            }
            string f_friend = "Followers_Friends.txt";
            using (StreamWriter outputFile = new StreamWriter(Path.Combine(filepath_for_friends, f_friend)))
            {
                outputFile.WriteLine("Follwers Friends Lists are given below\n");
                int a = 1;
                foreach (var act in us.follower_friends)
                {
                    outputFile.WriteLine("(" + a + ") " + " Name :" + act.name);
                    outputFile.WriteLine("    " + "Location:" + act.loc);
                    outputFile.WriteLine("    " + "Data :" + act.posts_pic_vids);

                    a++;
                }
            }



            string file_ext2 = "Groups.txt";
            using (StreamWriter outputFile = new StreamWriter(Path.Combine(filepath, file_ext2)))
            {
                outputFile.WriteLine("Groups Joins are given below\n");
                int a = 1;
                foreach (var act in us.groups)
                {
                    outputFile.WriteLine("(" + a + ")" + act);
                    a++;
                }

                outputFile.WriteLine("Groups Leading are given below\n");
                int c = 1;
                foreach (var act in us.group_leads)
                {
                    outputFile.WriteLine("(" + c + ")" + act);
                    a++;
                }

            }

            string file_ext3 = "Wall.txt";
            using (StreamWriter outputFile = new StreamWriter(Path.Combine(filepath, file_ext3)))
            {
                outputFile.WriteLine("Wall Messages are given below\n");

                int a = 1;
                foreach (var act in us.wall_list)
                {
                    outputFile.WriteLine("(" + a + ")" + act.wall_message);
                    outputFile.WriteLine("\tGiven By: " + act.wall_message_given_by);
                    a++;
                }

            }



            var filepath_for_posts = filepath + "\\" + "Posts";
            if (!Directory.Exists(filepath_for_posts))
                Directory.CreateDirectory(filepath_for_posts);

            us.posts_list.ForEach((pst) =>
            {
                if (pst.posts_counter.Length >= 40)
                {
                    pst.posts_counter = pst.posts_counter.Substring(0, 40);
                }
                var f1 = File.OpenWrite(filepath_for_posts + "\\" + pst.posts_counter + ".txt");
                var s1 = new StreamWriter(f1);
                s1.WriteLine("Post Title: " + pst.title);
                s1.WriteLine("Date: " + pst.time);
                s1.WriteLine("Description: " + pst.description);
                s1.WriteLine("\n");
                s1.WriteLine("Comments on this post are:\n");
                int i = 1;
                pst.comments.ForEach((com) =>
                {
                    s1.WriteLine(i + "  Given By : \t  " + com.comment_given_by);
                    s1.WriteLine("\n");
                    s1.WriteLine("Date : \t " + com.comment_date + "\n");
                    s1.WriteLine("Comment is : \t" + com.comment_description);
                    i++;
                });
                s1.WriteLine("");

                s1.Flush();
                s1.Close();
                f1.Close();
            });

            if (!Directory.Exists(imgpath))
                Directory.CreateDirectory(imgpath);
            var filepath_for_comments = imgpath + "\\" + "Comments";
            if (!Directory.Exists(filepath_for_comments))
                Directory.CreateDirectory(filepath_for_comments);

            us.imgs_list.ForEach((img) =>
            {
                var f1 = File.OpenWrite(filepath_for_comments + "\\" + img.img + ".txt");
                var s1 = new StreamWriter(f1);
                s1.WriteLine("\t\t\t\t" + img.alt_atr + "\n");
                img.comments.ForEach((comment) =>
                {
                    s1.WriteLine("Given By: " + comment.comment_given_by);
                    s1.WriteLine("Date: " + comment.comment_date);
                    s1.WriteLine("Description: " + comment.comment_description);
                    s1.WriteLine("\n");
                });
                s1.Flush();
                s1.Close();
                f1.Close();
            });

            string log_file = "log.txt";
            if (!Directory.Exists(imgpath))
                Directory.CreateDirectory(imgpath);
            using (StreamWriter outputFile = new StreamWriter(Path.Combine(imgpath, log_file)))
            {
                outputFile.WriteLine("Error Messages are given below\n");

                int a = 1;
                foreach (var act in us.errors_list)
                {
                    outputFile.WriteLine("Error is :" + act);
                    //outputFile.WriteLine("\tGiven By: " + act.wall_message_given_by);
                    a++;
                }

            }

            var filepath_for_conversation = filepath + "\\" + "Conversations";
            if (!Directory.Exists(filepath_for_conversation))
                Directory.CreateDirectory(filepath_for_conversation);
            us.conv_list.ForEach((con) =>
            {
                var f1 = File.OpenWrite(filepath_for_conversation + "\\" + con.title + ".txt");
                var s1 = new StreamWriter(f1);
                s1.WriteLine("Title = :\t\t" + con.title);
                con.conversations.ForEach((conversation) =>
                {
                    s1.WriteLine("Sender: " + conversation.sender_name);
                    s1.WriteLine("\n");
                    s1.WriteLine("Message: " + conversation.message);

                    s1.WriteLine("\n");
                });
                s1.Flush();
                s1.Close();
                f1.Close();
            });
            string file_extt = filepath + "\\" + "data.js";
            if(!Directory.Exists(filepath))
            {
                Directory.CreateDirectory(filepath);
            }
            try
            {
                var user = getdataforjson();

                var jsonToWrite = JsonConvert.SerializeObject(user, Formatting.None);

                using (var writer = new StreamWriter(file_extt))
                {
                    writer.Write("data = '" + jsonToWrite.Replace("'", "&quot;") + "'");
                }
                var indexfilepath = filepath + "\\index.html";
                var s1 = new StreamWriter(indexfilepath);
                var s = new StreamReader("index.html");
                s1.Write(s.ReadToEnd());
                s1.Flush();
                s.Close();
                s1.Close();
            }
            catch (Exception ex)
            {
                _log.Error("data_write for json", ex);
                throw ex;

            }

        }

        private User getdataforjson()
        {
            var user = new User
            {
                name = HttpUtility.HtmlEncode(us.name),
                location = HttpUtility.HtmlEncode(us.location),
                relationship_status = HttpUtility.HtmlEncode(us.relationship_status),
                about_me = new List<string>(),
                active = HttpUtility.HtmlEncode(us.active),
                is_lookingfor = HttpUtility.HtmlEncode(us.is_lookingfor),
                orientation = HttpUtility.HtmlEncode(us.orientation),
                activites_list = new List<string>(),
                groups = new List<string>(),
                wall_list = new List<Wall>(),
                img_count = us.imgs_list.Count(),
                post_count = us.posts_list.Count(),
                imgs_list = new List<Images_class>(),
                posts_list = new List<Posts>(),
                errors_list = new List<string>(),
                conv_list = new List<Conversation_Class>(),
                list_friends = new List<List_Friends>(),
                following_friends = new List<Following>(),
                follower_friends = new List<Followers>(),
                mutual_friends = new List<Mutual_Friends>(),
                events_may_be_going_to = new List<string>(),
                events_going_to = new List<string>(),
                group_leads = new List<string>(),

            };
            us.activites_list.ForEach((act) =>
            {
                user.activites_list.Add(HttpUtility.HtmlEncode(act));
            });
            us.about_me.ForEach((act) =>
            {
                user.about_me.Add(HttpUtility.HtmlEncode(act));
            });
            us.groups.ForEach((act) =>
            {
                user.groups.Add(HttpUtility.HtmlEncode(act));
            });
            us.group_leads.ForEach((act) =>
            {
                user.group_leads.Add(HttpUtility.HtmlEncode(act));
            });
            us.events_going_to.ForEach((act) =>
            {
                user.events_going_to.Add(HttpUtility.HtmlEncode(act));
            });
            us.events_may_be_going_to.ForEach((act) =>
            {
                user.events_may_be_going_to.Add(HttpUtility.HtmlEncode(act));
            });
            us.wall_list.ForEach((act) =>
            {
                user.wall_list.Add(act);
            });
            us.imgs_list.ForEach((act) =>
            {
                user.imgs_list.Add(act);
            });
            us.posts_list.ForEach((act) =>
            {
                user.posts_list.Add(act);
            });
            us.conv_list.ForEach((act) =>
            {
                user.conv_list.Add(act);
            });
            us.mutual_friends.ForEach((act) =>
            {
                user.mutual_friends.Add(act);
            });
            us.list_friends.ForEach((act) =>
            {
                user.list_friends.Add(act);
            });
            us.follower_friends.ForEach((act) =>
            {
                user.follower_friends.Add(act);
            });
            us.following_friends.ForEach((act) =>
            {
                user.following_friends.Add(act);
            });
            return user;
        }

        #endregion
        
        #region Destructor
        ~Locate_m()
        {
            if (driver != null)
                driver.Quit();
        }
        #endregion Destructor


        #region classes

        public class User
        {
            public User()
            {
                pics_list = new List<string>();
                activites_list = new List<string>();
                wall_list = new List<Wall>();
                imgs_list = new List<Images_class>();
                about_me = new List<string>();
                groups = new List<string>();
                posts_list = new List<Posts>();
                errors_list = new List<string>();
                conv_list = new List<Conversation_Class>();
                list_friends = new List<List_Friends>();
                mutual_friends = new List<Mutual_Friends>();
                following_friends = new List<Following>();
                follower_friends = new List<Followers>();
                events_may_be_going_to = new List<string>();
                events_going_to = new List<string>();
                group_leads = new List<String>();

            }
            public string name { get; set; }
            public string location { get; set; }
            public string relationship_status { get; set; }
            public string orientation { get; set; }
            public string active { get; set; }
            public string is_lookingfor { get; set; }
            public List<string> about_me { get; set; }
            public List<string> pics_list { get; set; }
            public List<Wall> wall_list { get; set; }
            public List<string> activites_list { get; set; }
            public List<string> groups { get; set; }

            public List<Images_class> imgs_list { get; set; }

            public List<Posts> posts_list { get; set; }

            public List<string> errors_list { get; set; }

            public List<Conversation_Class> conv_list { get; set; }

            public int img_count, post_count;

            public List<string> events_going_to { get; set; }
            public List<string> events_may_be_going_to { get; set; }

            public List<string> group_leads { get; set; }
            public List<List_Friends> list_friends { get; set; }
            public List<Mutual_Friends> mutual_friends { get; set; }
            public List<Following> following_friends { get; set; }
            public List<Followers> follower_friends { get; set; }
        }

        public class Posts
        {
            public Posts()
            {
                comments = new List<Comments>();
            }
            public string comment_url;
            public List<Comments> comments { get; set; }
            public string title;
            public string time;
            public string description;
            public string posts_counter;



        }
        public class Comments
        {
            public string comment_given_by;
            public string comment_description;
            public string comment_date;

            public Comments()
            {
                comment_given_by = comment_description = comment_date = "";
            }
        }

        public class Wall
        {
            public string wall_message_given_by;
            public string wall_message;

            public Wall()
            {
                wall_message_given_by = wall_message = "";
            }
        }
        public class Images_class
        {
            public Images_class()
            {
                comments = new List<Comments>();
            }
            public string img;
            public string img_path;
            public string alt_atr;
            public List<Comments> comments { get; set; }
        }

        public class Conversation_Class
        {
            public Conversation_Class()
            {
                conversations = new List<Conversation_fields>();
            }
            public string conversation_url;
            public List<Conversation_fields> conversations { get; set; }
            public string title;

        }
        public class Conversation_fields
        {
            public string sender_name;
            public string message;

        }

        public class List_Friends
        {
            public string name;
            public string loc;
            public string posts_pic_vids;
        }
        public class Mutual_Friends
        {
            public string name;
            public string loc;
            public string posts_pic_vids;
        }
        public class Following
        {
            public string name;
            public string loc;
            public string posts_pic_vids;
        }
        public class Followers
        {
            public string name;
            public string loc;
            public string posts_pic_vids;
        }

        public class Logger
        {
            private List<string> errorList;
            public Logger(List<string> errList)
            {
                errorList = errList;
            }
            public void Error(string msg, Exception ex)
            {
                errorList.Add(msg + "  " + ex.Message + "  " + ex.StackTrace);
            }
        }
        #endregion
    }
}
