using Google.Apis.Drive.v3;
using GoogleDriveAPIExample;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LoginForm
{
    public partial class shareForm : Form
    {
        APIService APIService;
        DriveService serivce;
        List<string> listItem;
        public shareForm()
        {
            InitializeComponent();
        }
        public shareForm(List<string> listItem,DriveService serivce, APIService aPIService)
        {
            this.APIService = aPIService;
            this.serivce = serivce;
            this.listItem = listItem;
            InitializeComponent();
            comboBox1.SelectedIndex = 0;
            emailTextBox.Text = "example@gmail.com";
        }

        private void shareForm_Load(object sender, EventArgs e)
        {

        }

        private void emailTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void emailTextBox_MouseClick(object sender, MouseEventArgs e)
        {
            emailTextBox.ForeColor = Color.FromArgb(18,17,16);
        }
        public static bool IsEmailValid(string email)
        {
            // Biểu thức chính quy kiểm tra địa chỉ email
            string pattern = @"^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$";

            // Kiểm tra xem chuỗi có khớp với biểu thức chính quy không
            Regex regex = new Regex(pattern);
            return regex.IsMatch(email);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(emailTextBox.Text))
            {
                // Hiển thị cảnh báo nếu người dùng không nhập
                MessageBox.Show("Please enter an email.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            else if(!IsEmailValid(emailTextBox.Text)){
                MessageBox.Show("email Invalid.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                APIService.ShareMultipleFilesWithUser(this.listItem, this.serivce, emailTextBox.Text, comboBox1.SelectedItem.ToString());
                MessageBox.Show("Share Request Complete!\n Please check your Email", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.Close();
            }
       
        }
    }
}
