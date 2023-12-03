using GoogleDriveAPIExample;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace LoginForm
{
    public partial class Form1 : Form
    {

        private BackgroundWorker internetCheckWorker;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string filePath = "history.txt";

            if (File.Exists(filePath))
            {
                // Đọc tất cả các dòng trong file txt
                string[] lines = File.ReadAllLines(filePath);

                // Tạo label và thêm vào FlowLayoutPanel cho mỗi dòng trong file
                foreach (string line in lines)
                {
                    if (line != "")
                    {
                        Label label = new Label();
                        label.Text = line.ToUpper();
                        label.Margin = new System.Windows.Forms.Padding(3, 10, 3, 0);
                        label.Click += new EventHandler(click_label);
                        Button closeButton = new Button();
                        closeButton.Text = "X";
                        closeButton.Tag = label; // Lưu trữ label tương ứng trong Tag của nút

                        // Thiết lập sự kiện click cho nút "X"
                        closeButton.Click += closeButton_Click;
                        // Thêm label vào FlowLayoutPanel
                        flowLayoutPanel1.Controls.Add(label);
                        flowLayoutPanel1.Controls.Add(closeButton);
                    }
                }
            }
            internetCheckWorker = new BackgroundWorker();
            internetCheckWorker.WorkerSupportsCancellation = true;
            internetCheckWorker.DoWork += InternetCheckWorker_DoWork;

            // Bắt đầu kiểm tra liên tục
            internetCheckWorker.RunWorkerAsync();
            //timer.Start();
        }
        private void InternetCheckWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (!internetCheckWorker.CancellationPending)
            {
                // Kiểm tra kết nối internet
                bool isConnected = CheckInternetConnection();

                // Cập nhật giao diện dựa trên kết quả kiểm tra
                Invoke(new Action(() =>
                {
                    if (isConnected)
                    {
                        labelStatus.Text = "";
                        flowLayoutPanel1.Enabled = true;
                        BtnGoogle.Enabled = true;
                    }
                    else
                    {
                        // Hiển thị thông báo không có kết nối internet
                        labelStatus.Text = "No Internet Connection";
                        labelStatus.ForeColor = Color.FromArgb(255, 0, 0);
                        flowLayoutPanel1.Enabled = false;
                        BtnGoogle.Enabled = false ;
                    }
                }));

                // Ngủ 1 giây trước khi kiểm tra lại
                Thread.Sleep(1000);
            }
        }
        private bool CheckInternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                using (var stream = client.OpenRead("http://www.google.com"))
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
        private void click_label(object sender, EventArgs e)
        {
            Label currentLable = (Label)sender;
            string username = currentLable.Text;

            // Mở form mới (Form2 trong ví dụ này)
            ProcessForm newForm = new ProcessForm(username);
            newForm.Show();
            
        }
        public void closeButton_Click(object sender, EventArgs e)
        {
            Button ClsButton = (Button)sender;
            Label label = (Label)ClsButton.Tag;
            DeleteUserDicrectory(label.Text);
            DeleteUserInHistory(label.Text);
            flowLayoutPanel1.Controls.Remove(ClsButton);
            flowLayoutPanel1.Controls.Remove(label);

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

            APIService apiService = new APIService();
            Watcher wat = new Watcher(apiService, apiService.startService());

            ProcessForm newForm = new ProcessForm(apiService.userName);
            newForm.Show();
            Application.Restart();

        }
        public void DeleteUserInHistory(string valueToDelete)
        {
            try
            {
                // Đọc nội dung tệp tin
                string[] lines = File.ReadAllLines("history.txt");

                // Tạo một danh sách mới để lưu trữ các dòng đã chỉnh sửa
                var updatedLines = new List<string>();

                // Xóa giá trị cần thiết trong danh sách các dòng
                foreach (string line in lines)
                {
                    if (line != valueToDelete)
                    {
                        updatedLines.Add(line);
                    }
                }

                // Ghi lại nội dung đã chỉnh sửa vào tệp tin
                File.WriteAllLines("history.txt", updatedLines);

                Console.WriteLine("Đã xóa giá trị thành công.");
            }
            catch (IOException ex)
            {
                Console.WriteLine("Đã xảy ra lỗi: " + ex.Message);
            }
        }
        public void DeleteUserDicrectory(string value)
        {

            try
            {
                if(value!=""){
                    // Kiểm tra xem thư mục có tồn tại không
                    if (Directory.Exists($"{Environment.CurrentDirectory}\\{value}"))
                    {
                        // Xóa thư mục
                        Directory.Delete($"{Environment.CurrentDirectory}\\{value}", true);
                        Console.WriteLine("Đã xóa thư mục thành công.");
                    }
                    else
                    {
                        Console.WriteLine("Thư mục không tồn tại.");
                    }
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine("Đã xảy ra lỗi: " + ex.Message);
            }
        }

        private void BtnGoogle_Click(object sender, EventArgs e)
        {
            APIService apiService = new APIService();
            Watcher wat = new Watcher(apiService, apiService.startService());

            ProcessForm newForm = new ProcessForm(apiService.userName);
            newForm.Show();
            
        }
    }
}
