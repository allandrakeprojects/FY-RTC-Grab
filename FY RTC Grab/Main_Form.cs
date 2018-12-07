using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FY_RTC_Grab
{
    public partial class Main_Form : Form
    {
        private JObject __jo;
        private bool m_aeroEnabled;
        private bool __isClose;
        private int __secho;
        private int __display_length = 5000;
        private int __result_count_json;
        private int __total_page;
        private int __i = 0;
        private bool __isLogin = false;
        private bool __isStart = false;
        private bool __isBreak = false;
        private string __player_last_username = "";
        private string __playerlist_cn;
        private string __playerlist_ea;
        private string __playerlist_qq;
        private string __player_id;
        private string __player_ldd;
        private string __start_time;
        private string __end_time;
        private string __brand_code = "FY";
        private int __count = 0;

        // Deposit
        private JObject __jo_deposit;
        private bool __isBreak_deposit = false;
        private bool __isInsert_deposit = false;
        private int __secho_deposit;
        private int __total_page_deposit;
        private int __result_count_json_deposit;
        private int __count_deposit = 0;
        private string __player_ldd_deposit;
        private string __player_id_deposit;
        private bool __detectInsert_deposit = false;

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
            
            timer_landing.Start();
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
        private void label_player_last_registered_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
        private void panel_landing_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
        private void pictureBox_landing_MouseDown(object sender, MouseEventArgs e)
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
            DialogResult dr = MessageBox.Show("Exit the program?", "FY RTC Grab", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
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
                DialogResult dr = MessageBox.Show("Exit the program?", "FY RTC Grab", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
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
        
        [DllImport("winmm.dll")]
        public static extern int waveOutGetVolume(IntPtr h, out uint dwVolume);
        
        // Mute Sounds
        [DllImport("winmm.dll")]
        public static extern int waveOutSetVolume(IntPtr h, uint dwVolume);

        // Form Load
        private void Main_Form_Load(object sender, EventArgs e)
        {
            int NewVolume = ((ushort.MaxValue / 10) * 100);
            uint NewVolumeAllChannels = (((uint)NewVolume & 0x0000ffff) | ((uint)NewVolume << 16));
            waveOutSetVolume(IntPtr.Zero, NewVolumeAllChannels);

            webBrowser.Navigate("http://cs.ying168.bet/account/login");
        }

        // WebBrowser
        private async void webBrowser_DocumentCompletedAsync(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (webBrowser.ReadyState == WebBrowserReadyState.Complete)
            {
                if (e.Url == webBrowser.Url)
                {
                    try
                    {
                        if (webBrowser.Url.ToString().Equals("http://cs.ying168.bet/account/login"))
                        {
                            if (__isStart)
                            {
                                string datetime = DateTime.Now.ToString("dd MMM HH:mm:ss");
                                SendEmail("<html><body>IP: 192.168.10.252<br/>Location: Robisons Summit Office<br/>Date and Time: [" + datetime + "]<br/>Message: <b>The application have been logout, please re-login again.</b></body></html>");
                                SendEmail2("<html><body>IP: 192.168.10.252<br/>Location: Robisons Summit Office<br/>Date and Time: [" + datetime + "]<br/>Message: <b>The application have been logout, please re-login again.</b></body></html>");
                            }
                            
                            __isLogin = false;
                            __isStart = false;
                            timer.Stop();
                            label_player_last_registered.Text = "-";
                            webBrowser.Document.Window.ScrollTo(0, webBrowser.Document.Body.ScrollRectangle.Height);
                            webBrowser.Document.GetElementById("csname").SetAttribute("value", "fyrtcgrab");
                            webBrowser.Document.GetElementById("cspwd").SetAttribute("value", "rg123@@@");
                            webBrowser.Document.GetElementById("la").Enabled = false;
                            webBrowser.Visible = true;
                            label_brand.Visible = false;
                            pictureBox_loader.Visible = false;
                            label_player_last_registered.Visible = false;
                            webBrowser.WebBrowserShortcutsEnabled = true;
                        }

                        if (webBrowser.Url.ToString().Equals("http://cs.ying168.bet/player/list") || webBrowser.Url.ToString().Equals("http://cs.ying168.bet/site/index") || webBrowser.Url.ToString().Equals("http://cs.ying168.bet/player/online") || webBrowser.Url.ToString().Equals("http://cs.ying168.bet/message/platform"))
                        {
                            __isLogin = true;
                            
                            if (!__isStart)
                            {
                                __isStart = true;
                                webBrowser.Visible = false;
                                label_brand.Visible = true;
                                pictureBox_loader.Visible = true;
                                label_player_last_registered.Visible = true;
                                webBrowser.WebBrowserShortcutsEnabled = false;
                                ___PlayerLastRegistered();
                                await ___GetPlayerListsRequest();
                                ___GetPlayerListsRequest_Deposit();
                            }
                        }
                    }
                    catch (Exception err)
                    {
                        string datetime = DateTime.Now.ToString("dd MMM HH:mm:ss");
                        SendEmail("<html><body>IP: 192.168.10.252<br/>Location: Robisons Summit Office<br/>Date and Time: [" + datetime + "]<br/>Message: <b>There's a problem to the server, please re-open the application.</b></body></html>");
                        SendEmail2("<html><body>IP: 192.168.10.252<br/>Location: Robisons Summit Office<br/>Date and Time: [" + datetime + "]<br/>Message: <b>There's a problem to the server, please re-open the application.</b></body></html>");

                        __isClose = false;
                        Environment.Exit(0);
                    }
                }
            }
        }

        // ------ Functions
        private async Task ___GetPlayerListsRequest()
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

                await ___PlayerListAsync();
            }
            catch (Exception err)
            {
                if (__isLogin)
                {
                    await ___GetPlayerListsRequest();
                }
            }
        }

        private async Task ___PlayerListAsync()
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

                        player_info.Add(username + "*|*" + name + "*|*" + date_register + " " + date_time_register + "*|*" + __player_ldd + "*|*" + __playerlist_cn + "*|*" + __playerlist_ea + "*|*" + agent + "*|*" + __playerlist_qq);
                    }
                    else
                    {
                        // send to api
                        if (player_info.Count != 0)
                        {
                            player_info.Reverse();
                            string player_info_get = String.Join(",", player_info);
                            string[] values = player_info_get.Split(',');
                            foreach (string value in values)
                            {
                                Application.DoEvents();
                                string[] values_inner = value.Split(new string[] { "*|*" }, StringSplitOptions.None);
                                int count = 0;
                                string _username = "";
                                string _name = "";
                                string _date_register = "";
                                string _date_deposit = "";
                                string _cn = "";
                                string _email = "";
                                string _agent = "";
                                string _qq = "";

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
                                        if (value_inner != "-")
                                        {
                                            _date_deposit = value_inner;
                                        }
                                        else
                                        {
                                            _date_deposit = "";
                                        }
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
                                    // QQ
                                    else if (count == 8)
                                    {
                                        _qq = value_inner;
                                    }
                                }

                                // ----- Insert Data
                                //using (StreamWriter file = new StreamWriter(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\rtcgrab_fy.txt", true, Encoding.UTF8))
                                //{
                                //    file.WriteLine(_username + "*|*" + _name + "*|*" + _date_register + "*|*" + _date_deposit + "*|*" + _cn + "*|*" + _email + "*|*" + _agent + "*|*" + _qq + "*|*" + __brand_code);
                                //}
                                using (StreamWriter file = new StreamWriter(Path.GetTempPath() + @"\rtcgrab_fy.txt", true, Encoding.UTF8))
                                {
                                    file.WriteLine(_username + "*|*" + _name + "*|*" + _date_register + "*|*" + _date_deposit + "*|*" + _cn + "*|*" + _email + "*|*" + _agent + "*|*" + _qq + "*|*" + __brand_code);
                                }

                                Thread t = new Thread(delegate () { ___InsertData(_username, _name, _date_register, _date_deposit, _cn, _email, _agent, _qq, __brand_code); });
                                t.Start();

                                __count = 0;

                                __playerlist_cn = "";
                                __playerlist_ea = "";
                                __playerlist_qq = "";
                            }
                        }

                        if (!String.IsNullOrEmpty(__player_last_username.Trim()))
                        {
                            ___SavePlayerLastRegistered(__player_last_username);

                            Invoke(new Action(() =>
                            {
                                label_player_last_registered.Text = "Last Registered: " + Properties.Settings.Default.______last_registered_player;
                            }));
                        }

                        player_info.Clear();
                        timer.Start();
                        __isBreak = true;
                        break;
                    }
                }
            }
        }

        private async void timer_TickAsync(object sender, EventArgs e)
        {
            timer.Stop();
            await ___GetPlayerListsRequest();

            if (__isInsert_deposit)
            {
                __isInsert_deposit = false;
                ___GetPlayerListsRequest_Deposit();
            }
        }

        private void ___InsertData(string username, string name, string date_register, string date_deposit, string contact, string email, string agent, string qq, string brand_code)
        {
            try
            {
                string password = username.ToLower() + date_register + "youdieidie";
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
                        ["qq"] = qq,
                        ["wc"] = "",
                        ["brand_code"] = brand_code,
                        ["token"] = token
                    };

                    var response = wb.UploadValues("http://zeus.ssimakati.com:8080/API/sendRTC", "POST", data);
                    string responseInString = Encoding.UTF8.GetString(response);
                }
            }
            catch (Exception err)
            {
                if (__isLogin)
                {
                    __count++;
                    if (__count == 5)
                    {
                        string datetime = DateTime.Now.ToString("dd MMM HH:mm:ss");
                        SendEmail("<html><body>IP: 192.168.10.252<br/>Location: Robisons Summit Office<br/>Date and Time: [" + datetime + "]<br/>Message: <b>There's a problem to the server, please re-open the application.</b></body></html>");
                        SendEmail2("<html><body>IP: 192.168.10.252<br/>Location: Robisons Summit Office<br/>Date and Time: [" + datetime + "]<br/>Message: <b>There's a problem to the server, please re-open the application.</b></body></html>");

                        __isClose = false;
                        Environment.Exit(0);
                    }
                    else
                    {
                        ____InsertData2(username, name, date_register, date_deposit, contact, email, agent, qq, brand_code);
                    }
                }
            }
        }

        private void ____InsertData2(string username, string name, string date_register, string date_deposit, string contact, string email, string agent, string qq, string brand_code)
        {
            try
            {
                string password = username.ToLower() + date_register + "youdieidie";
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
                        ["qq"] = qq,
                        ["wc"] = "",
                        ["brand_code"] = brand_code,
                        ["token"] = token
                    };

                    var response = wb.UploadValues("http://zeus2.ssitex.com:8080/API/sendRTC", "POST", data);
                    string responseInString = Encoding.UTF8.GetString(response);
                }
            }
            catch (Exception err)
            {
                if (__isLogin)
                {
                    __count++;
                    if (__count == 5)
                    {
                        string datetime = DateTime.Now.ToString("dd MMM HH:mm:ss");
                        SendEmail("<html><body>IP: 192.168.10.252<br/>Location: Robisons Summit Office<br/>Date and Time: [" + datetime + "]<br/>Message: <b>There's a problem to the server, please re-open the application.</b></body></html>");
                        SendEmail2("<html><body>IP: 192.168.10.252<br/>Location: Robisons Summit Office<br/>Date and Time: [" + datetime + "]<br/>Message: <b>There's a problem to the server, please re-open the application.</b></body></html>");

                        __isClose = false;
                        Environment.Exit(0);
                    }
                    else
                    {
                        ___InsertData(username, name, date_register, date_deposit, contact, email, agent, qq, brand_code);
                    }
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
                int qq = 0;
                bool cn_detect = false;
                bool ea_detect = false;
                bool qq_detect = false;
                bool cn_ = false;
                bool ea_ = false;
                bool qq_ = false;

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
                    else if (item.Contains("QQ"))
                    {
                        qq = i_label;
                        qq_detect = true;
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

                        if (!qq_detect)
                        {
                            qq_ = true;
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

                if (qq_)
                {
                    qq--;
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
                    else if (i_span == qq)
                    {
                        __playerlist_qq = ItemMatch.Groups[1].Value;
                    }
                }
            }
            catch (Exception err)
            {
                if (__isLogin)
                {
                    await ___PlayerListContactNumberEmailAsync(__player_id);
                }
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
                if (__isLogin)
                {
                    await ___PlayerListLastDeposit(__player_id);
                }
            }
        }

        private void ___PlayerLastRegistered()
        {
            if (Properties.Settings.Default.______last_registered_player == "" && Properties.Settings.Default.______last_registered_player_deposit == "")
            {
                ___GetLastRegisteredPlayer();
            }

            label_player_last_registered.Text = "Last Registered: " + Properties.Settings.Default.______last_registered_player;
        }

        private void ___SavePlayerLastRegistered(string username)
        {
            Properties.Settings.Default.______last_registered_player = username;
            Properties.Settings.Default.Save();
        }

        private void timer_landing_Tick(object sender, EventArgs e)
        {
            panel_landing.Visible = false;
            timer_landing.Stop();
        }

        // Deposit
        private async void ___GetPlayerListsRequest_Deposit()
        {
            __isBreak_deposit = false;

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
                    {"data[0][value]", __secho_deposit++.ToString()},
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
                    __total_page_deposit += Convert.ToInt32(Math.Floor(result_total_records)) + 1;
                }
                else
                {
                    __total_page_deposit += Convert.ToInt32(Math.Floor(result_total_records));
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
                    {"data[0][value]", __secho_deposit++.ToString()},
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

                __jo_deposit = JObject.Parse(deserializeObject.ToString());
                JToken count = __jo_deposit.SelectToken("$.aaData");
                __result_count_json_deposit = count.Count();

                ___PlayerListAsync_Deposit();
            }
            catch (Exception err)
            {
                if (__isLogin)
                {
                    ___GetPlayerListsRequest_Deposit();
                }
            }
        }

        private async void ___PlayerListAsync_Deposit()
        {
            List<string> player_info = new List<string>();
            string path = Path.GetTempPath() + @"\rtcgrab_fy_deposit.txt";

            for (int i = 0; i < __total_page_deposit; i++)
            {
                if (__isBreak_deposit)
                {
                    break;
                }

                for (int ii = 0; ii < __result_count_json_deposit; ii++)
                {
                    if (!File.Exists(path))
                    {
                        using (StreamWriter file = new StreamWriter(path, true, Encoding.UTF8))
                        {
                            file.WriteLine("test123*|*");
                            file.Close();
                        }
                    }

                    JToken username__id = __jo_deposit.SelectToken("$.aaData[" + ii + "][0]").ToString();
                    string username = Regex.Match(username__id.ToString(), "username=\\\"(.*?)\\\"").Groups[1].Value;

                    if (username == Properties.Settings.Default.______last_registered_player)
                    {
                        __detectInsert_deposit = true;
                    }

                    __player_id_deposit = Regex.Match(username__id.ToString(), "player=\\\"(.*?)\\\"").Groups[1].Value;

                    bool isInsert = false;

                    if (__detectInsert_deposit)
                    {
                        using (StreamReader sr = File.OpenText(path))
                        {
                            string s = String.Empty;
                            while ((s = sr.ReadLine()) != null)
                            {
                                Application.DoEvents();

                                if (s == username)
                                {
                                    isInsert = true;
                                    break;
                                }
                                else
                                {
                                    isInsert = false;
                                }
                            }
                            sr.Close();
                        }

                        if (!isInsert)
                        {
                            await ___PlayerListLastDeposit_Deposit(__player_id_deposit);
                        }
                    }

                    if (username != Properties.Settings.Default.______last_registered_player_deposit)
                    {
                        if (__detectInsert_deposit)
                        {
                            if (!isInsert)
                            {
                                if (__player_ldd_deposit != "-")
                                {
                                    player_info.Add(username + "*|*" + __player_ldd_deposit);

                                    using (StreamWriter file = new StreamWriter(path, true, Encoding.UTF8))
                                    {
                                        file.WriteLine(username);
                                        file.Close();
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (player_info.Count != 0)
                        {
                            player_info.Reverse();
                            string player_info_get = String.Join(",", player_info);
                            string[] values = player_info_get.Split(',');
                            foreach (string value in values)
                            {
                                Application.DoEvents();
                                string[] values_inner = value.Split(new string[] { "*|*" }, StringSplitOptions.None);
                                int count = 0;
                                string _username = "";
                                string _date_deposit = "";

                                foreach (string value_inner in values_inner)
                                {
                                    count++;

                                    // Username
                                    if (count == 1)
                                    {
                                        _username = value_inner;
                                    }
                                    // Last Deposit Date
                                    else if (count == 2)
                                    {
                                        if (value_inner != "-")
                                        {
                                            _date_deposit = value_inner;
                                        }
                                        else
                                        {
                                            _date_deposit = "";
                                        }
                                    }
                                }

                                Thread t = new Thread(delegate () { ___InsertData_Deposit(_username, _date_deposit, __brand_code); });
                                t.Start();

                                __count_deposit = 0;
                            }

                        }

                        player_info.Clear();
                        __isBreak_deposit = true;
                        __detectInsert_deposit = false;
                        break;
                    }
                }
            }

            ___DepositLastRegistered();
            __isInsert_deposit = true;
        }

        private async Task ___PlayerListLastDeposit_Deposit(string id)
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
                __player_ldd_deposit = last_deposit.ToString();
            }
            catch (Exception err)
            {
                if (__isLogin)
                {
                    await ___PlayerListLastDeposit_Deposit(__player_id_deposit);
                }
            }
        }

        private void ___InsertData_Deposit(string username, string last_deposit_date, string brand)
        {
            try
            {
                string password = username.ToLower() + last_deposit_date + "youdieidie";
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
                        ["date_deposit"] = last_deposit_date,
                        ["brand_code"] = brand,
                        ["token"] = token
                    };

                    var response = wb.UploadValues("http://zeus.ssimakati.com:8080/API/sendRTCdep", "POST", data);
                }
            }
            catch (Exception err)
            {
                if (__isLogin)
                {
                    __count_deposit++;
                    if (__count_deposit == 5)
                    {
                        string datetime = DateTime.Now.ToString("dd MMM HH:mm:ss");
                        SendEmail("<html><body>IP: 192.168.10.252<br/>Location: Robisons Summit Office<br/>Date and Time: [" + datetime + "]<br/>Message: <b>There's a problem to the server, please re-open the application.</b></body></html>");
                        SendEmail2("<html><body>IP: 192.168.10.252<br/>Location: Robisons Summit Office<br/>Date and Time: [" + datetime + "]<br/>Message: <b>There's a problem to the server, please re-open the application.</b></body></html>");

                        __isClose = false;
                        Environment.Exit(0);
                    }
                    else
                    {
                        ___InsertData2_Deposit(username, last_deposit_date, brand);
                    }
                }
            }
        }

        private void ___InsertData2_Deposit(string username, string last_deposit_date, string brand)
        {
            try
            {
                string password = username.ToLower() + last_deposit_date + "youdieidie";
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
                        ["date_deposit"] = last_deposit_date,
                        ["brand_code"] = brand,
                        ["token"] = token
                    };

                    var response = wb.UploadValues("http://zeus2.ssitex.com:8080/API/sendRTCdep", "POST", data);
                }
            }
            catch (Exception err)
            {
                if (__isLogin)
                {
                    __count_deposit++;
                    if (__count_deposit == 5)
                    {
                        string datetime = DateTime.Now.ToString("dd MMM HH:mm:ss");
                        SendEmail("<html><body>IP: 192.168.10.252<br/>Location: Robisons Summit Office<br/>Date and Time: [" + datetime + "]<br/>Message: <b>There's a problem to the server, please re-open the application.</b></body></html>");
                        SendEmail2("<html><body>IP: 192.168.10.252<br/>Location: Robisons Summit Office<br/>Date and Time: [" + datetime + "]<br/>Message: <b>There's a problem to the server, please re-open the application.</b></body></html>");

                        __isClose = false;
                        Environment.Exit(0);
                    }
                    else
                    {
                        ___InsertData_Deposit(username, last_deposit_date, brand);
                    }
                }
            }
        }

        private void ___DepositLastRegistered()
        {
            string path = Path.GetTempPath() + @"\rtcgrab_fy_deposit.txt";
            if (label_player_last_registered.Text != "-" && label_player_last_registered.Text.Trim() != "")
            {
                if (Properties.Settings.Default.______detect_deposit == "")
                {
                    DateTime today = DateTime.Now;
                    DateTime date = today.AddDays(1);
                    Properties.Settings.Default.______detect_deposit = date.ToString("yyyy-MM-dd 23");
                    Properties.Settings.Default.Save();
                }
                else
                {
                    DateTime today = DateTime.Now;
                    if (Properties.Settings.Default.______detect_deposit == today.ToString("yyyy-MM-dd HH"))
                    {
                        Properties.Settings.Default.______detect_deposit = "";
                        Properties.Settings.Default.______last_registered_player_deposit = label_player_last_registered.Text.Replace("Last Registered: ", "");
                        Properties.Settings.Default.Save();

                        if (File.Exists(path))
                        {
                            File.Delete(path);
                        }
                    }
                    else
                    {
                        string start_datetime = today.ToString("yyyy-MM-dd HH");
                        DateTime start = DateTime.ParseExact(start_datetime, "yyyy-MM-dd HH", CultureInfo.InvariantCulture);

                        string end_datetime = Properties.Settings.Default.______detect_deposit;
                        DateTime end = DateTime.ParseExact(end_datetime, "yyyy-MM-dd HH", CultureInfo.InvariantCulture);

                        if (start > end)
                        {
                            Properties.Settings.Default.______detect_deposit = "";
                            Properties.Settings.Default.______last_registered_player_deposit = label_player_last_registered.Text.Replace("Last Registered: ", "");
                            Properties.Settings.Default.Save();

                            if (File.Exists(path))
                            {
                                File.Delete(path);
                            }
                        }
                    }
                }
            }
        }

        private void SendEmail(string get_message)
        {
            try
            {
                int port = 587;
                string host = "smtp.gmail.com";
                string username = "drake@18tech.com";
                string password = "Wcfajmeojnapa1";
                string mailFrom = "noreply@mail.com";
                string mailTo = "it@18tech.com";
                string mailTitle = "FY RTC Grab";
                string mailMessage = get_message;

                using (SmtpClient client = new SmtpClient())
                {
                    MailAddress from = new MailAddress(mailFrom);
                    MailMessage message = new MailMessage
                    {
                        From = from
                    };
                    message.To.Add(mailTo);
                    message.Subject = mailTitle;
                    message.Body = mailMessage;
                    message.IsBodyHtml = true;
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.UseDefaultCredentials = false;
                    client.Host = host;
                    client.Port = port;
                    client.EnableSsl = true;
                    client.Credentials = new NetworkCredential
                    {
                        UserName = username,
                        Password = password
                    };
                    client.Send(message);
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.ToString());
            }
        }

        private void SendEmail2(string get_message)
        {
            try
            {
                int port = 587;
                string host = "smtp.gmail.com";
                string username = "drake@18tech.com";
                string password = "Wcfajmeojnapa1";
                string mailFrom = "noreply@mail.com";
                string mailTo = "allandulay69@gmail.com";
                string mailTitle = "FY RTC Grab";
                string mailMessage = get_message;

                using (SmtpClient client = new SmtpClient())
                {
                    MailAddress from = new MailAddress(mailFrom);
                    MailMessage message = new MailMessage
                    {
                        From = from
                    };
                    message.To.Add(mailTo);
                    message.Subject = mailTitle;
                    message.Body = mailMessage;
                    message.IsBodyHtml = true;
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.UseDefaultCredentials = false;
                    client.Host = host;
                    client.Port = port;
                    client.EnableSsl = true;
                    client.Credentials = new NetworkCredential
                    {
                        UserName = username,
                        Password = password
                    };
                    client.Send(message);
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.ToString());
            }
        }
        
        private void ___GetLastRegisteredPlayer()
        {
            try
            {
                string password = __brand_code + "youdieidie";
                byte[] encodedPassword = new UTF8Encoding().GetBytes(password);
                byte[] hash = ((HashAlgorithm)CryptoConfig.CreateFromName("MD5")).ComputeHash(encodedPassword);
                string token = BitConverter.ToString(hash)
                   .Replace("-", string.Empty)
                   .ToLower();

                using (var wb = new WebClient())
                {
                    var data = new NameValueCollection
                    {
                        ["brand_code"] = __brand_code,
                        ["token"] = token
                    };

                    var result = wb.UploadValues("http://zeus.ssimakati.com:8080/API/lastRTCrecord", "POST", data);
                    string responsebody = Encoding.UTF8.GetString(result);
                    var deserializeObject = JsonConvert.DeserializeObject(responsebody);
                    JObject jo = JObject.Parse(deserializeObject.ToString());
                    JToken plr = jo.SelectToken("$.msg");

                    Properties.Settings.Default.______last_registered_player = plr.ToString();                    
                    Properties.Settings.Default.______last_registered_player_deposit = plr.ToString();
                    Properties.Settings.Default.Save();
                }
            }
            catch (Exception err)
            {
                if (__isLogin)
                {
                    __count++;
                    if (__count == 5)
                    {
                        string datetime = DateTime.Now.ToString("dd MMM HH:mm:ss");
                        SendEmail("<html><body>IP: 192.168.10.252<br/>Location: Robisons Summit Office<br/>Date and Time: [" + datetime + "]<br/>Message: <b>There's a problem to the server, please re-open the application.</b></body></html>");
                        SendEmail2("<html><body>IP: 192.168.10.252<br/>Location: Robisons Summit Office<br/>Date and Time: [" + datetime + "]<br/>Message: <b>There's a problem to the server, please re-open the application.</b></body></html>");

                        __isClose = false;
                        Environment.Exit(0);
                    }
                    else
                    {
                        ___GetLastRegisteredPlayer2();
                    }
                }
            }
        }

        private void ___GetLastRegisteredPlayer2()
        {
            try
            {
                string password = __brand_code + "youdieidie";
                byte[] encodedPassword = new UTF8Encoding().GetBytes(password);
                byte[] hash = ((HashAlgorithm)CryptoConfig.CreateFromName("MD5")).ComputeHash(encodedPassword);
                string token = BitConverter.ToString(hash)
                   .Replace("-", string.Empty)
                   .ToLower();

                using (var wb = new WebClient())
                {
                    var data = new NameValueCollection
                    {
                        ["brand_code"] = __brand_code,
                        ["token"] = token
                    };

                    var result = wb.UploadValues("http://zeus2.ssitex.com:8080/API/lastRTCrecord", "POST", data);
                    string responsebody = Encoding.UTF8.GetString(result);
                    var deserializeObject = JsonConvert.DeserializeObject(responsebody);
                    JObject jo = JObject.Parse(deserializeObject.ToString());
                    JToken plr = jo.SelectToken("$.msg");

                    Properties.Settings.Default.______last_registered_player = plr.ToString();
                    Properties.Settings.Default.______last_registered_player_deposit = plr.ToString();
                    Properties.Settings.Default.Save();
                }
            }
            catch (Exception err)
            {
                if (__isLogin)
                {
                    __count++;
                    if (__count == 5)
                    {
                        string datetime = DateTime.Now.ToString("dd MMM HH:mm:ss");
                        SendEmail("<html><body>IP: 192.168.10.252<br/>Location: Robisons Summit Office<br/>Date and Time: [" + datetime + "]<br/>Message: <b>There's a problem to the server, please re-open the application.</b></body></html>");
                        SendEmail2("<html><body>IP: 192.168.10.252<br/>Location: Robisons Summit Office<br/>Date and Time: [" + datetime + "]<br/>Message: <b>There's a problem to the server, please re-open the application.</b></body></html>");

                        __isClose = false;
                        Environment.Exit(0);
                    }
                    else
                    {
                        ___GetLastRegisteredPlayer();
                    }
                }
            }
        }
    }
}
