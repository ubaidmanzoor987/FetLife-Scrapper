using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace fetlife
{
    public partial class fetlife : Form
    {
        Thread backgroudThread;
        public fetlife()
        {
            InitializeComponent();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            showMessage1 = new showMessage(showMessageBox);
        }

        delegate void showMessage(string msg);
        showMessage showMessage1;

        private void BtnDownload_Click(object sender, EventArgs e)
        {
            int count;
            this.mUiContext = new WindowsFormsSynchronizationContext();
            string path = "";
            if (this.txtid.Text == "")
            {
                MessageBox.Show("Enter id");
                return;
            }
            else if (this.txtUserName.Text == "")
            {
                MessageBox.Show("Enter Username");
                return;
            }
            else if (this.txtPassword.Text == "")
            {

                MessageBox.Show("Enter password");
                return;
            }
            else if (this.browse_path.Text == "")
            {
                MessageBox.Show("Please Select path for downloading User Data");
                return;
            }
            else
            {
                this.btnDownload.Enabled = false;
                string username = this.txtUserName.Text;
                string password = this.txtPassword.Text;
                string id = this.txtid.Text;
                status_box.Text = "";
                backgroudThread = new Thread(() =>
                {
                    for(int i=0; i< 3; i++)
                    {
                        try
                        {
                            if (Process_Download(path, username, password, id) == false)
                            {
                                backgroudThread.Abort();
                            }
                            else
                            {
                                break;
                            }

                        }
                        catch (ThreadAbortException ex)
                        {
                            enableButton(true);
                            throw ex;
                        }
                        catch (TimeoutException ex)
                        {
                            if(i!=3)
                                showStatusMessage("Error in network connectivity. Retrying...");
                            else
                            {
                                enableButton(true);
                                showStatusMessage("Error in network connectivity");
                            }

                        }
                        catch (Exception ex)
                        {
                            if (i != 3)
                                showStatusMessage("Error in network connectivity. Retrying...");
                            else
                            {
                                enableButton(true);
                                showStatusMessage("Error in network connectivity");
                            }
                        }
                    }
                });
                backgroudThread.Start();
            }
        }

        public void showMessageBox(string msg)
        {
            MessageBox.Show(msg);
        }

        void showStatusMessage(string msg)
        {
            if (status_box.InvokeRequired)
            {
                status_box.Invoke(new Action<string>(showStatusMessage), msg);
                return;
            }
            status_box.AppendText(msg + "\r\n");
        }

        void enableButton(bool enable)
        {
            if (btnDownload.InvokeRequired)
            {
                btnDownload.Invoke(new Action<bool>(enableButton), enable);
                return;
            }
            btnDownload.Enabled = true;
        }

        private bool Process_Download(string path, string username, string password, string id)
        {
            this.mUiContext = new WindowsFormsSynchronizationContext();
            path = browse_path.Text;
            Locate_m obj = new Locate_m(username, password, id, path);
            try
            {
                showStatusMessage("Logging in.....");
                if (obj.Login() == 2)
                {
                    MessageBox.Show("Incorrect Information Username, password", "Log In Failed");
                    obj.destroy();
                    return false;
                }
                else
                {
                    int id_parsed;
                    bool parsed = int.TryParse(id, out id_parsed);
                    if (parsed)
                    {
                        showStatusMessage("Loading Profile.....");
                        if (obj.goToUserProfile(id) == false)
                        {
                            MessageBox.Show("There is no User on this id " + id);
                            obj.destroy();
                        }
                        else
                        {
                            showStatusMessage("Downloading Data.....");
                            obj.download_user_data();
                            showStatusMessage("Downloading Pictures.....");
                            obj.open_pictures();
                            showStatusMessage("Downloading Wall Messages.....");
                            obj.wall_messages();
                            showStatusMessage("Downloading Activities.....");
                            obj.latest_activities();
                            showStatusMessage("Downloading Posts.....");
                            obj.posts_section();
                            showStatusMessage("Downloading Conversations.....");
                            obj.Conversations();
                            showStatusMessage("Downloading Friends & Mutual Friends.....");
                            obj.friends_user();
                            obj.mutual_friends_data();
                            showStatusMessage("Downloading Following & Followers.....");
                            obj.follower_friends_data();
                            obj.following_friends_data();
                            showStatusMessage("Download  Complete.....");
                            showStatusMessage("Downloading Saving to Files.....");
                            obj.data_write();
                            showStatusMessage("Process complete.....");
                            enableButton(true);
                            obj.destroy();
                        }
                    }
                    else
                    {
                        showStatusMessage("Searching User.......");
                        obj.searchUser(id);
                        showStatusMessage("Selecting User.......");
                        bool a = obj.selectuser();
                        if (a != false)
                        {
                            showStatusMessage("Downloading Data.....");
                            obj.download_user_data();
                            showStatusMessage("Downloading Pictures.....");
                            obj.open_pictures();
                            showStatusMessage("Downloading Wall Messages.....");
                            obj.wall_messages();
                            showStatusMessage("Downloading Activities.....");
                            obj.latest_activities();
                            showStatusMessage("Downloading Posts.....");
                            obj.posts_section();
                            showStatusMessage("Downloading Conversations.....");
                            obj.Conversations();
                            showStatusMessage("Downloading Friends & Mutual Friends.....");
                            obj.friends_user();
                            obj.mutual_friends_data();
                            showStatusMessage("Downloading Following & Followers.....");
                            obj.follower_friends_data();
                            obj.following_friends_data();
                            showStatusMessage("Download  Complete.....");
                            showStatusMessage("Downloading Saving to Files.....");
                            obj.data_write();
                            showStatusMessage("Process complete.....");
                            enableButton(true);
                            obj.destroy();
                        }
                        else
                        {
                            obj.destroy();
                            enableButton(true);
                        }


                    }
                }
            }
            catch (ThreadAbortException ex)
            {
                obj.destroy();
                throw ex;
            }
            return true;
        }

        private void Txtid_TextChanged(object sender, EventArgs e)
        {

        }
        private void Browse_data_Click(object sender, EventArgs e)
        {
            var res = folderBrowserDialog1.ShowDialog();
            browse_path.Text = folderBrowserDialog1.SelectedPath;

        }

        private Button btnDownload;
        private Label label4;

        private void Fetlife_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (backgroudThread != null)
            {
                backgroudThread.Abort();
            }

        }

    }

}