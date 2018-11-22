using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FY_RTC_Grab
{
    public partial class Main_Form : Form
    {
        private bool m_aeroEnabled;
        private bool __isClose;
        private int __secho;
        private int __display_length = 5000;
        private int __result_count_json;
        private int __total_page;
        private int __i = 0;
        private JObject __jo;
        private bool __isStart = false;
        private bool __isBreak = false;
        private string __player_last_username = "";
        private string __playerlist_cn;
        private string __playerlist_ea;
        private string __player_id;
        private string __player_ldd;
        private string __start_time;
        private string __end_time;
        private string __brand_code = "FY";
        private int __count = 0;

        // Drag Header to Move
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        // ----- Drag Header to Move
        
        // Form Shadow
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
        (
            int nLeftRect,
            int nTopRect,
            int nRightRect,
            int nBottomRect,
            int nWidthEllipse,
            int nHeightEllipse
        );
        [DllImport("dwmapi.dll")]
        public static extern int DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS pMarInset);
        [DllImport("dwmapi.dll")]
        public static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);
        [DllImport("dwmapi.dll")]
        public static extern int DwmIsCompositionEnabled(ref int pfEnabled);
        private const int CS_DROPSHADOW = 0x00020000;
        private const int WM_NCPAINT = 0x0085;
        private const int WM_ACTIVATEAPP = 0x001C;
        private const int WM_NCHITTEST = 0x84;
        private const int HTCLIENT = 0x1;
        private const int HTCAPTION = 0x2;
        private const int WS_MINIMIZEBOX = 0x20000;
        private const int CS_DBLCLKS = 0x8;
        public struct MARGINS
        {
            public int leftWidth;
            public int rightWidth;
            public int topHeight;
            public int bottomHeight;
        }
        protected override CreateParams CreateParams
        {
            get
            {
                m_aeroEnabled = CheckAeroEnabled();

                CreateParams cp = base.CreateParams;
                if (!m_aeroEnabled)
                    cp.ClassStyle |= CS_DROPSHADOW;

                cp.Style |= WS_MINIMIZEBOX;
                cp.ClassStyle |= CS_DBLCLKS;
                return cp;
            }
        }
        private bool CheckAeroEnabled()
        {
            if (Environment.OSVersion.Version.Major >= 6)
            {
                int enabled = 0;
                DwmIsCompositionEnabled(ref enabled);
                return (enabled == 1) ? true : false;
            }
            return false;
        }
        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_NCPAINT:
                    if (m_aeroEnabled)
                    {
                        var v = 2;
                        DwmSetWindowAttribute(Handle, 2, ref v, 4);
                        MARGINS margins = new MARGINS()
                        {
                            bottomHeight = 1,
                            leftWidth = 0,
                            rightWidth = 0,
                            topHeight = 0
                        };
                        DwmExtendFrameIntoClientArea(Handle, ref margins);

                    }
                    break;
                default:
                    break;
            }
            base.WndProc(ref m);

            if (m.Msg == WM_NCHITTEST && (int)m.Result == HTCLIENT)
                m.Result = (IntPtr)HTCAPTION;
        }
        // ----- Form Shadow
        
        public Main_Form()
        {
            InitializeComponent();
        }

        // Drag to Move
        private void panel_header_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
        private void label_title_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
        private void panel2_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
        private void pictureBox_loader_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
        private void label_brand_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
        private void label_status_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
        private void label_player_last_registered_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
        // ----- Drag to Move

        // Click Close
        private void pictureBox_close_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("Exit the program?", "FY", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dr == DialogResult.Yes)
            {
                __isClose = true;
                Environment.Exit(0);
            }
        }

        // Click Minimize
        private void pictureBox_minimize_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        // Form Closing
        private void Main_Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!__isClose)
            {
                DialogResult dr = MessageBox.Show("Exit the program?", "FY", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dr == DialogResult.No)
                {
                    e.Cancel = true;
                }
                else
                {
                    Environment.Exit(0);
                }
            }

            Environment.Exit(0);
        }

        // Form Load
        private void Main_Form_Load(object sender, EventArgs e)
        {
            webBrowser.Navigate("http://cs.ying168.bet/account/login");
        }

        // WebBrowser
        private void webBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (webBrowser.ReadyState == WebBrowserReadyState.Complete)
            {
                if (e.Url == webBrowser.Url)
                {
                    try
                    {
                        if (webBrowser.Url.ToString().Equals("http://cs.ying168.bet/account/login"))
                        {
                            System.Media.SoundPlayer player = new System.Media.SoundPlayer(Properties.Resources.rtc_grab);
                            bool isPlaying = false;
                            if (__isStart)
                            {
                                player.PlayLooping();
                                isPlaying = true;
                            }
                            
                            __isStart = false;
                            timer.Stop();
                            label_status.Text = "-";
                            label_player_last_registered.Text = "-";
                            webBrowser.Document.Window.ScrollTo(0, webBrowser.Document.Body.ScrollRectangle.Height);
                            webBrowser.Document.GetElementById("csname").SetAttribute("value", "fyrain");
                            webBrowser.Document.GetElementById("cspwd").SetAttribute("value", "djrain123@@@");
                            webBrowser.Document.GetElementById("la").Enabled = false;
                            webBrowser.Visible = true;
                            label_brand.Visible = false;
                            pictureBox_loader.Visible = false;
                            label_status.Visible = false;
                            label_player_last_registered.Visible = false;
                            webBrowser.WebBrowserShortcutsEnabled = true;

                            if (isPlaying)
                            {
                                DialogResult dr = MessageBox.Show("You've been logout please login again.", "FY", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                if (dr == DialogResult.OK)
                                {
                                    player.Stop();
                                }
                            }
                        }

                        if (webBrowser.Url.ToString().Equals("http://cs.ying168.bet/player/list") || webBrowser.Url.ToString().Equals("http://cs.ying168.bet/site/index") || webBrowser.Url.ToString().Equals("http://cs.ying168.bet/player/online") || webBrowser.Url.ToString().Equals("http://cs.ying168.bet/message/platform"))
                        {
                            if (!__isStart)
                            {
                                __isStart = true;
                                webBrowser.Visible = false;
                                label_brand.Visible = true;
                                pictureBox_loader.Visible = true;
                                label_status.Visible = true;
                                label_player_last_registered.Visible = true;
                                label_status.Text = "...";
                                ___PlayerLastRegistered();
                                ___GetPlayerListsRequest();
                                webBrowser.WebBrowserShortcutsEnabled = false;
                            }
                        }
                    }
                    catch (Exception err)
                    {
                        System.Media.SoundPlayer player = new System.Media.SoundPlayer(Properties.Resources.rtc_grab);
                        player.PlayLooping();

                        DialogResult dr = MessageBox.Show("There's a problem to the server. Please call IT Support, thank you!", "FY", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        if (dr == DialogResult.OK)
                        {
                            player.Stop();
                        }

                        __isClose = false;
                        Environment.Exit(0);
                    }
                }
            }
        }
        
        private void timer_Tick(object sender, EventArgs e)
        {
            label_status.Font = new Font("Microsoft Sans Serif", 12, FontStyle.Bold);
            label_status.Location = new Point(1, 70);
            DateTime start = DateTime.ParseExact(__start_time, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            DateTime end = DateTime.ParseExact(__end_time, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);

            TimeSpan difference = end - start;
            int hrs = difference.Hours;
            int mins = difference.Minutes;
            int secs = difference.Seconds;

            TimeSpan spinTime = new TimeSpan(hrs, mins, secs);

            TimeSpan delta = DateTime.Now - start;
            TimeSpan timeRemaining = spinTime - delta;

            if (timeRemaining.Minutes != 0)
            {
                if (timeRemaining.Seconds == 0)
                {
                    label_status.Text = timeRemaining.Minutes + ":" + timeRemaining.Seconds + "0";
                }
                else
                {
                    if (timeRemaining.Seconds.ToString().Length == 1)
                    {
                        label_status.Text = timeRemaining.Minutes + ":0" + timeRemaining.Seconds;
                    }
                    else
                    {
                        label_status.Text = timeRemaining.Minutes + ":" + timeRemaining.Seconds;
                    }
                }
                
                label_status.Visible = true;
            }
            else
            {
                if (label_status.Text == "1")
                {
                    timer.Stop();
                    label_status.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Regular);
                    label_status.Location = new Point(0, 65);
                    label_status.Text = "...";
                    ___GetPlayerListsRequest();
                }
                else
                {
                    label_status.Text = timeRemaining.Seconds.ToString();
                    label_status.Visible = true;
                }
            }

            if (label_status.Text.Contains("-"))
            {
                System.Media.SoundPlayer player = new System.Media.SoundPlayer(Properties.Resources.rtc_grab);
                player.PlayLooping();

                DialogResult dr = MessageBox.Show("There's a problem to the server. Please call IT Support, thank you!", "FY", MessageBoxButtons.OK, MessageBoxIcon.Information);
                if (dr == DialogResult.OK)
                {
                    player.Stop();
                }

                timer.Stop();
                __isClose = false;
                Environment.Exit(0);
            }
        }
        
        // ------ Functions
        private async void ___GetPlayerListsRequest()
        {
            __isBreak = false;
            
            try
            {
                var cookie = Cookie.GetCookieInternal(webBrowser.Url, false);
                WebClient wc = new WebClient();

                wc.Headers.Add("Cookie", cookie);
                wc.Encoding = Encoding.UTF8;
                wc.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

                var reqparm_gettotal = new NameValueCollection
                {
                    {"s_btype", ""},
                    {"skip", "0"},
                    {"groupid", "0"},
                    {"s_type", "1"},
                    {"s_status_search", ""},
                    {"s_keyword", ""},
                    {"s_playercurrency", "ALL"},
                    {"s_phone", "on"},
                    {"s_email", "on"},
                    {"data[0][name]", "sEcho"},
                    {"data[0][value]", __secho++.ToString()},
                    {"data[1][name]", "iColumns"},
                    {"data[1][value]", "13"},
                    {"data[2][name]", "sColumns"},
                    {"data[2][value]", ""},
                    {"data[3][name]", "iDisplayStart"},
                    {"data[3][value]", "0"},
                    {"data[4][name]", "iDisplayLength"},
                    {"data[4][value]", "1"}
                };

                byte[] result_gettotal = await wc.UploadValuesTaskAsync("http://cs.ying168.bet/player/listAjax1", "POST", reqparm_gettotal);
                string responsebody_gettotatal = Encoding.UTF8.GetString(result_gettotal);

                var deserializeObject_gettotal = JsonConvert.DeserializeObject(responsebody_gettotatal);
                JObject jo_gettotal = JObject.Parse(deserializeObject_gettotal.ToString());
                JToken jt_gettotal = jo_gettotal.SelectToken("$.iTotalRecords");
                double get_total_records_fy = 0;
                get_total_records_fy = double.Parse(jt_gettotal.ToString());
                
                double result_total_records = get_total_records_fy / __display_length;

                if (result_total_records.ToString().Contains("."))
                {
                    __total_page += Convert.ToInt32(Math.Floor(result_total_records)) + 1;
                }
                else
                {
                    __total_page += Convert.ToInt32(Math.Floor(result_total_records));
                }                

                var reqparm = new NameValueCollection
                {
                    {"s_btype", ""},
                    {"skip", "0"},
                    {"groupid", "0"},
                    {"s_type", "1"},
                    {"s_status_search", ""},
                    {"s_keyword", ""},
                    {"s_playercurrency", "ALL"},
                    {"data[0][name]", "sEcho"},
                    {"data[0][value]", __secho++.ToString()},
                    {"data[1][name]", "iColumns"},
                    {"data[1][value]", "13"},
                    {"data[2][name]", "sColumns"},
                    {"data[2][value]", ""},
                    {"data[3][name]", "iDisplayStart"},
                    {"data[3][value]", "0"},
                    {"data[4][name]", "iDisplayLength"},
                    {"data[4][value]", __display_length.ToString()}
                };
                
                byte[] result = await wc.UploadValuesTaskAsync("http://cs.ying168.bet/player/listAjax1", "POST", reqparm);
                string responsebody = Encoding.UTF8.GetString(result);
                var deserializeObject = JsonConvert.DeserializeObject(responsebody);

                __jo = JObject.Parse(deserializeObject.ToString());
                JToken count = __jo.SelectToken("$.aaData");
                __result_count_json = count.Count();
                
                ___PlayerListAsync();
            }
            catch (Exception err)
            {
                ___GetPlayerListsRequest();
            }
        }

        private async void ___PlayerListAsync()
        {
            List<string> player_info = new List<string>();

            for (int i = 0; i < __total_page; i++)
            {
                if (__isBreak)
                {
                    break;
                }

                for (int ii = 0; ii < __result_count_json; ii++)
                {
                    Application.DoEvents();

                    JToken username__id = __jo.SelectToken("$.aaData[" + ii + "][0]").ToString();
                    string username = Regex.Match(username__id.ToString(), "username=\\\"(.*?)\\\"").Groups[1].Value;
                    __player_id = Regex.Match(username__id.ToString(), "player=\\\"(.*?)\\\"").Groups[1].Value;
                    JToken name = __jo.SelectToken("$.aaData[" + ii + "][2]").ToString().Replace("\"", "");
                    JToken agent = __jo.SelectToken("$.aaData[" + ii + "][3]").ToString().Replace("\"", "");
                    JToken date_register__register_domain = __jo.SelectToken("$.aaData[" + ii + "][12]");
                    string date_register = date_register__register_domain.ToString().Substring(0, 10);
                    string date_time_register = date_register__register_domain.ToString().Substring(14, 8);
                    
                    if (username != Properties.Settings.Default.______last_registered_player)
                    {
                        if (ii == 0)
                        {
                            __player_last_username = username;
                        }
                        
                        await ___PlayerListContactNumberEmailAsync(__player_id);
                        await ___PlayerListLastDeposit(__player_id);
                        
                        player_info.Add(username + "*|*" + name + "*|*" + date_register + " " + date_time_register + "*|*" + __player_ldd + "*|*" + __playerlist_cn + "*|*" + __playerlist_ea + "*|*" + agent);
                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(__player_last_username.Trim()))
                        {
                            ___SavePlayerLastRegistered(__player_last_username);
                            label_player_last_registered.Text = "Last Registered: " + Properties.Settings.Default.______last_registered_player;
                        }

                        // send to api
                        if (player_info.Count != 0)
                        {
                            player_info.Reverse();
                            string player_info_get = String.Join(",", player_info);
                            string[] values = player_info_get.Split(',');
                            foreach (string value in values)
                            {
                                string[] values_inner = value.Split(new string[] { "*|*" }, StringSplitOptions.None);
                                int count = 0;
                                string _username = "";
                                string _name = "";
                                string _date_register = "";
                                string _date_deposit = "";
                                string _cn = "";
                                string _email = "";
                                string _agent = "";

                                foreach (string value_inner in values_inner)
                                {
                                    count++;

                                    // Username
                                    if (count == 1)
                                    {
                                        _username = value_inner;
                                    }
                                    // Name
                                    else if (count == 2)
                                    {
                                        _name = value_inner;
                                    }
                                    // Register Date
                                    else if (count == 3)
                                    {
                                        _date_register = value_inner;
                                    }
                                    // Last Deposit Date
                                    else if (count == 4)
                                    {
                                        _date_deposit = value_inner;
                                    }
                                    // Contact Number
                                    else if (count == 5)
                                    {
                                        _cn = value_inner;
                                    }
                                    // Email
                                    else if (count == 6)
                                    {
                                        _email = value_inner;
                                    }
                                    // Agent
                                    else if (count == 7)
                                    {
                                        _agent = value_inner;
                                    }
                                }

                                // ----- Insert Data
                                using (StreamWriter file = new StreamWriter(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\test_fy.txt", true, Encoding.UTF8))
                                {
                                    file.WriteLine(_username + "*|*" + _name + "*|*" + _date_register + "*|*" + _date_deposit + "*|*" + _cn + "*|*" + _email + "*|*" + _agent + "*|*" + __brand_code);
                                }
                                using (StreamWriter file = new StreamWriter(Path.GetTempPath() + @"\test_fy.txt", true, Encoding.UTF8))
                                {
                                    file.WriteLine(_username + "*|*" + _name + "*|*" + _date_register + "*|*" + _date_deposit + "*|*" + _cn + "*|*" + _email + "*|*" + _agent + "*|*" + __brand_code);
                                }
                                ___InsertData(_username, _name, _date_register, _date_deposit, _cn, _email, _agent, __brand_code);
                                __count = 0;
                            }
                            
                            player_info.Clear();
                        }

                        timer.Start();
                        __start_time = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                        __end_time = DateTime.Now.AddSeconds(302).ToString("dd/MM/yyyy HH:mm:ss");

                        __isBreak = true;
                        break;
                    }
                }
            }
        }

        private void ___InsertData(string username, string name, string date_register, string date_deposit, string contact, string email, string agent, string brand_code)
        {
            try
            {
                string password = username + date_register + "youdieidie";
                byte[] encodedPassword = new UTF8Encoding().GetBytes(password);
                byte[] hash = ((HashAlgorithm)CryptoConfig.CreateFromName("MD5")).ComputeHash(encodedPassword);
                string token = BitConverter.ToString(hash)
                   .Replace("-", string.Empty)
                   .ToLower();
                
                using (var wb = new WebClient())
                {
                    var data = new NameValueCollection
                    {
                        ["username"] = username,
                        ["name"] = name,
                        ["date_register"] = date_register,
                        ["date_deposit"] = date_deposit,
                        ["contact"] = contact,
                        ["email"] = email,
                        ["agent"] = agent,
                        ["brand_code"] = brand_code,
                        ["token"] = token
                    };

                    var response = wb.UploadValues("http://zeus.ssitex.com:8080/API/sendRTC", "POST", data);
                    string responseInString = Encoding.UTF8.GetString(response);
                }
            }
            catch (Exception err)
            {
                __count++;
                if (__count == 5)
                {
                    System.Media.SoundPlayer player = new System.Media.SoundPlayer(Properties.Resources.rtc_grab);
                    player.PlayLooping();

                    DialogResult dr = MessageBox.Show("There's a problem to the server. Please call IT Support, thank you!", "FY", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    if (dr == DialogResult.OK)
                    {
                        player.Stop();
                    }

                    __isClose = false;
                    Environment.Exit(0);
                }
                else
                {
                    ____InsertData2(username, name, date_register, date_deposit, contact, email, agent, brand_code);
                }
            }
        }

        private void ____InsertData2(string username, string name, string date_register, string date_deposit, string contact, string email, string agent, string brand_code)
        {
            try
            {
                string password = username + date_register + "youdieidie";
                byte[] encodedPassword = new UTF8Encoding().GetBytes(password);
                byte[] hash = ((HashAlgorithm)CryptoConfig.CreateFromName("MD5")).ComputeHash(encodedPassword);
                string token = BitConverter.ToString(hash)
                   .Replace("-", string.Empty)
                   .ToLower();

                using (var wb = new WebClient())
                {
                    var data = new NameValueCollection
                    {
                        ["username"] = username,
                        ["name"] = name,
                        ["date_register"] = date_register,
                        ["date_deposit"] = date_deposit,
                        ["contact"] = contact,
                        ["email"] = email,
                        ["agent"] = agent,
                        ["brand_code"] = brand_code,
                        ["token"] = token
                    };

                    var response = wb.UploadValues("http://zeus2.ssitex.com:8080/API/sendRTC", "POST", data);
                    string responseInString = Encoding.UTF8.GetString(response);
                }
            }
            catch (Exception err)
            {
                __count++;
                if (__count == 5)
                {
                    System.Media.SoundPlayer player = new System.Media.SoundPlayer(Properties.Resources.rtc_grab);
                    player.PlayLooping();

                    DialogResult dr = MessageBox.Show("There's a problem to the server. Please call IT Support, thank you!", "FY", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    if (dr == DialogResult.OK)
                    {
                        player.Stop();
                    }

                    __isClose = false;
                    Environment.Exit(0);
                }
                else
                {
                    ___InsertData(username, name, date_register, date_deposit, contact, email, agent, brand_code);
                }
            }
        }
        
        private async Task ___PlayerListContactNumberEmailAsync(string id)
        {
            try
            {
                var cookie = Cookie.GetCookieInternal(webBrowser.Url, false);
                WebClient wc = new WebClient();

                wc.Headers.Add("Cookie", cookie);
                wc.Encoding = Encoding.UTF8;
                wc.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                string result_gettotal_responsebody = await wc.DownloadStringTaskAsync("http://cs.ying168.bet/player/playerDetailBox?id=" + id);

                int i_label = 0;
                int cn = 0;
                int ea = 0;
                bool cn_detect = false;
                bool ea_detect = false;
                bool cn_ = false;
                bool ea_ = false;

                Regex ItemRegex_label = new Regex("<label class=\"control-label\">(.*?)</label>", RegexOptions.Compiled);
                foreach (Match ItemMatch in ItemRegex_label.Matches(result_gettotal_responsebody))
                {
                    string item = ItemMatch.Groups[1].Value;
                    i_label++;
                    
                    if (item.Contains("Cellphone No") || item.Contains("手机号"))
                    {
                        cn = i_label;
                        cn_detect = true;
                    }
                    else if (item.Contains("E-mail Address") || item.Contains("邮箱"))
                    {
                        ea = i_label;
                        ea_detect = true;
                    }
                    else if (item.Contains("Agent No") || item.Contains("代理编号"))
                    {
                        if (!cn_detect)
                        {
                            cn_ = true;
                        }

                        if (!ea_detect)
                        {
                            ea_ = true;
                        }
                    }
                }

                if (cn_)
                {
                    cn--;
                }

                if (ea_)
                {
                    ea--;
                }

                int i_span = 0;

                Regex ItemRegex_span = new Regex("<span class=\"text\">(.*?)</span>", RegexOptions.Compiled);
                foreach (Match ItemMatch in ItemRegex_span.Matches(result_gettotal_responsebody))
                {
                    i_span++;
                    string item = ItemMatch.Groups[1].Value;

                    if (i_span == cn)
                    {
                        __playerlist_cn = ItemMatch.Groups[1].Value;
                    }
                    else if (i_span == ea)
                    {
                        __playerlist_ea = ItemMatch.Groups[1].Value;
                    }
                }
            }
            catch (Exception err)
            {
                await ___PlayerListContactNumberEmailAsync(__player_id);
            }
        }

        private async Task ___PlayerListLastDeposit(string id)
        {
            try
            {
                var cookie = Cookie.GetCookieInternal(webBrowser.Url, false);
                WebClient wc = new WebClient();
                wc.Headers.Add("Cookie", cookie);
                wc.Encoding = Encoding.UTF8;
                wc.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                string current_date = DateTime.Now.ToString("yyyy-MM-dd");
                string result_gettotal_responsebody = await wc.DownloadStringTaskAsync("http://cs.ying168.bet/player/playerDeposit?uid=" + id + "&start=2016-06-15&end=");
                var deserializeObject = JsonConvert.DeserializeObject(result_gettotal_responsebody);
                JObject jo_fy_last_deposit = JObject.Parse(deserializeObject.ToString());
                JToken last_deposit = jo_fy_last_deposit.SelectToken("$.last");
                __player_ldd = last_deposit.ToString();
            }
            catch (Exception err)
            {
                await ___PlayerListLastDeposit(__player_id);
            }
        }
        
        private void ___PlayerLastRegistered()
        {
            // handle last registered player
            if (Properties.Settings.Default.______last_registered_player == "")
            {
                //MessageBox.Show("ghghg");
                Properties.Settings.Default.______last_registered_player = "wxs520";
                Properties.Settings.Default.Save();
                // handle request
            }
            
            //Properties.Settings.Default.______last_registered_player = "yuhf4f999";
            //Properties.Settings.Default.Save();

            label_player_last_registered.Text = "Last Registered: " + Properties.Settings.Default.______last_registered_player;
            // todo
        }

        private void ___SavePlayerLastRegistered(string username)
        {
            Properties.Settings.Default.______last_registered_player = username;
            Properties.Settings.Default.Save();
        }
    }
}
