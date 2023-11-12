using Google.Apis.Drive.v3;
using GoogleDriveAPIExample;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LoginForm
{
    public partial class ProcessForm : Form
    {
        private Watcher wat;
        private string labelText;
        private DriveService service;
        private APIService apiService;
        private System.Windows.Forms.Timer timer;
        public ProcessForm(string text)
        {
            InitializeComponent();

            labelText = text;

            // Tạo label và nút logout trong Form2
            Label label = new Label();
            label.Text = labelText;
            label.Location = new Point(50, 50);
            this.Controls.Add(label);
            Button logoutButton = new Button();
            logoutButton.Text = "Logout";
            logoutButton.Location = new Point(50, 100);
            logoutButton.Click += LogoutButton_Click;
            this.Controls.Add(logoutButton);
            
            apiService = new APIService();
            service = apiService.automatic(labelText);

            wat = new Watcher(apiService, service);

            //timer = new System.Windows.Forms.Timer();
            //timer.Interval = 1 * 60 * 1000; // 5 phút expressed in milliseconds
            //timer.Tick += Timer_Tick;
            listView1.View = View.LargeIcon; // Sử dụng chế độ hiển thị biểu tượng lớn
            listView1.LargeImageList = imageList1; // Sử dụng ImageList để chứa các biểu tượng

            // Đọc tất cả các file và thư mục trong thư mục gốc
            string rootPath = apiService.location; // Thay thế bằng đường dẫn thư mục gốc của bạn
            //LoadDirectory(rootPath);
            IList<Google.Apis.Drive.v3.Data.File> files = apiService.LoadFilesFromRootFolder(service);
            loadFileFromDrive(files);
            //wat.Start();
        }
        public ProcessForm()
        {
            InitializeComponent();

        }
        private void LoadDirectory(string path)
        {
            // Xóa tất cả các mục hiện tại trong ListView
            listView1.Items.Clear();

            try
            {
                // Đọc tất cả các file trong thư mục
                string[] files = Directory.GetFiles(path);
                foreach (string file in files)
                {
                  
                    FileInfo fileInfo = new FileInfo(file);
                    string fileName = fileInfo.Name;
                    if (fileName != "data.txt" && fileName != "Token.txt")
                    {
                        Icon fileIcon = Icon.ExtractAssociatedIcon(file); // Lấy biểu tượng của file

                        // Thêm biểu tượng và tên file vào ListView
                        imageList1.Images.Add(fileIcon);
                        listView1.Items.Add(fileName, imageList1.Images.Count - 1);
                    }
                }

                // Đọc tất cả các thư mục con
                string[] directories = Directory.GetDirectories(path);
                foreach (string directory in directories)
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(directory);
                    string directoryName = directoryInfo.Name;
                    Icon folderIcon = SystemIcons.Warning; // Sử dụng biểu tượng thư mục mặc định

                    // Thêm biểu tượng và tên thư mục vào ListView
                    imageList1.Images.Add(folderIcon);
                    listView1.Items.Add(directoryName, imageList1.Images.Count - 1);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi trong quá trình đọc thư mục: " + ex.Message);
            }
        }
        
        private void ProcessForm_Load(object sender, EventArgs e)
        {
            //timer.Start();
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            //apiService.dowloadAllFilesAndFolders(service, apiService.location);
            //listView1.Clear();
            //LoadDirectory(apiService.location);

        }
        private void LogoutButton_Click(object sender, EventArgs e)
        {
            // Đóng form hiện tại (Form2)
            wat.Stop();
            this.Close();
            
        }
        //Đủ sài
        private void loadFileFromDrive(IList<Google.Apis.Drive.v3.Data.File> files)
        {
            listView1.Clear();
            
            if (files != null && files.Count > 0)
            {
                foreach (var file in files)
                {
                    ListViewItem item = new ListViewItem(file.Name);

                    if (!string.IsNullOrEmpty(file.IconLink))
                    {
                        // Tải biểu tượng từ URL và thêm vào ImageList
                        using (var client = new WebClient())
                        {
                            byte[] iconData = client.DownloadData(file.IconLink);
                            using (var iconStream = new MemoryStream(iconData))
                            {
                                var bitmap = new Bitmap(iconStream);
                                var icon = Icon.FromHandle(bitmap.GetHicon());
                                imageList1.Images.Add(file.Id, icon);
                            }
                        }

                        // Đặt biểu tượng cho ListViewItem
                        item.ImageKey = file.Id;
                    }

                    item.SubItems.Add(file.Id);
                    item.SubItems.Add(file.MimeType);
                    listView1.Items.Add(item);
                }
            }
        }

        //Chưa tối ưu
        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                ListViewItem selectedItem = listView1.SelectedItems[0];
                string fileName = selectedItem.Text;
                string id=apiService.GetFileOrFolderId(service,fileName);
                string fileType = GetFileType(fileName);
                apiService.DownloadFile(id, service,fileType);
            }
        }
        private string GetFileType(string filePath)
        {
            FileInfo fileInfo = new FileInfo(filePath);
            string fileType = fileInfo.Extension;

            // Xóa dấu chấm "." ở đầu extension nếu cần thiết
            if (!string.IsNullOrEmpty(fileType))
            {
                fileType = fileType.TrimStart('.');
            }

            return fileType;
        }

        private void listView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (listView1.SelectedItems.Count > 0)
                {
                    contextMenuStrip1.Show(listView1, e.Location);
                }
            }
        }

        ///Đã tối ưu
        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (listView1.SelectedItems.Count > 0)
            {
                int clientSelectedItem = listView1.SelectedItems.Count;
                while(clientSelectedItem>0)
                {
                    ListViewItem selectedItem = listView1.SelectedItems[0];
                    string id = selectedItem.SubItems[1].Text;
                    string fileName = selectedItem.Text;
                    string filePath = Path.Combine(apiService.location, fileName);
                    // Kiểm tra nếu đối tượng là một thư mục
                    //text/plain
                    //application/vnd.google-apps.folder
                    apiService.DeleteFolderAndContents(service, id);
                            // Xóa item khỏi ListView
                    listView1.Items.Remove(selectedItem);
                    clientSelectedItem--;
                }
            
            }
        }
        //Không sử dụng nữa
        private void listView1_DragDrop(object sender, DragEventArgs e)
        {
            wat.Stop();
            // Lấy danh sách các đường dẫn của các file/thư mục được thả
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            // Xử lý từng đối tượng được thả
            foreach (string file in files)
            {
                // Kiểm tra nếu là một thư mục
                if (Directory.Exists(file))
                {
                    // Lấy tên thư mục từ đường dẫn
                    string folderName = Path.GetFileName(file);
                    // Tạo đường dẫn đích bằng cách kết hợp đường dẫn thư mục đích và tên thư mục
                    string destinationPath = Path.Combine(apiService.location, folderName);

                    // Copy thư mục và nội dung của nó sang thư mục đích
                    Directory.CreateDirectory(destinationPath);
                    CopyDirectory(file, destinationPath);
                   
                    // Tạo một ListViewItem mới với tên thư mục
                    ListViewItem folderItem = new ListViewItem(folderName);
                    
                    // Gắn đường dẫn vào thuộc tính Tag của ListViewItem
                    folderItem.Tag = file;

                    // Thêm ListViewItem vào ListView
                    listView1.Items.Add(folderItem);
                    
                    apiService.createFolder(folderName, service, Path.Combine(apiService.location, folderName), null);
                }
                // Kiểm tra nếu là một file
                else if (File.Exists(file))
                {
                    // Lấy tên file từ đường dẫn
                    string fileName = Path.GetFileName(file);
                    string destinationFilePath = Path.Combine(apiService.location, fileName);

                    // Copy file vào thư mục đích
                    
                    
                    // Tạo một ListViewItem mới với tên file
                    ListViewItem fileItem = new ListViewItem(fileName);

                    // Gắn đường dẫn vào thuộc tính Tag của ListViewItem
                    fileItem.Tag = file;

                    // Thêm ListViewItem vào ListView
                    listView1.Items.Add(fileItem);
                    File.Copy(file, destinationFilePath);
                    
                }
            }
        }
        //Chưa tối ưu , cần chỉnh sửa
        private void listView1_DragDrop2(object sender, DragEventArgs e)
        {

            // Lấy danh sách các đường dẫn của các file/thư mục được thả
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            // Xử lý từng đối tượng được thả
            foreach (string file in files)
            {
                // Kiểm tra nếu là một thư mục
                if (Directory.Exists(file))
                {
                    string folderName = Path.GetFileName(file);

                    // Tạo một ListViewItem mới với tên thư mục
                    ListViewItem folderItem = new ListViewItem(folderName);

                    // Gắn đường dẫn vào thuộc tính Tag của ListViewItem
                    folderItem.Tag = file;

                    // Thêm ListViewItem vào ListView
                    listView1.Items.Add(folderItem);
                    apiService.UploadFolder(service, file, null);
                }
                // Kiểm tra nếu là một file
                else if (File.Exists(file))
                {
                    // Lấy tên file từ đường dẫn
                    string fileName = Path.GetFileName(file);
                    // Tạo một ListViewItem mới với tên file
                    ListViewItem fileItem = new ListViewItem(fileName);

                    // Gắn đường dẫn vào thuộc tính Tag của ListViewItem
                    fileItem.Tag = file;
                    
                    // Thêm ListViewItem vào ListView
                    listView1.Items.Add(fileItem);
                }
                
            }
        }
        public void CopyDirectory(string sourceDir, string destDir)
        {
            // Lấy tất cả các tệp tin trong thư mục nguồn
            string[] files = Directory.GetFiles(sourceDir);
            foreach (string file in files)
            {
                string fileName = Path.GetFileName(file);
                string destFile = Path.Combine(destDir, fileName);
                File.Copy(file, destFile);
            }

            // Lấy tất cả các thư mục con trong thư mục nguồn
            string[] subdirectories = Directory.GetDirectories(sourceDir);
            foreach (string subdir in subdirectories)
            {
                string folderName = new DirectoryInfo(subdir).Name;
                string destSubdir = Path.Combine(destDir, folderName);
                Directory.CreateDirectory(destSubdir);
                CopyDirectory(subdir, destSubdir);
            }
        }
        private void listView1_DragEnter(object sender, DragEventArgs e)
        {
            // Kiểm tra xem có phải là thả file hay không
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // Cho phép thả
                e.Effect = DragDropEffects.Copy;
            }
        }
        //Chưa sử dụng được 
        private void listView1_ItemActivate(object sender, EventArgs e)
        {
            // Kiểm tra xem đã chọn một item folder hay chưa
            if (listView1.SelectedItems.Count > 0)
            {
                ListViewItem selectedItem = listView1.SelectedItems[0];
                string folderPath =Path.Combine(apiService.location, selectedItem.Text);

                // Kiểm tra nếu item là một folder
                if (Directory.Exists(folderPath))
                {
                    // Xóa tất cả các item hiện tại trong ListView
                    listView1.Items.Clear();

                    // Lấy danh sách tất cả các tệp và thư mục con của folder
                    string[] subItems = Directory.GetFileSystemEntries(folderPath);

                    // Xử lý từng tệp và thư mục con
                    foreach (string subItemPath in subItems)
                    {
                        // Lấy tên tệp hoặc thư mục từ đường dẫn
                        string subItemName = Path.GetFileName(subItemPath);

                        // Tạo một ListViewItem mới với tên tệp hoặc thư mục
                        ListViewItem subItem = new ListViewItem(subItemName);

                        // Gắn đường dẫn vào thuộc tính Tag của ListViewItem
                        subItem.Tag = subItemPath;

                        // Thêm ListViewItem vào ListView
                        listView1.Items.Add(subItem);
                    }
                }
            }
        }
        //Chưa sử dụng được
        private void btnBack_Click(object sender, EventArgs e)
        {
            // Kiểm tra xem có item trong ListView hay không
            if (listView1.Items.Count > 0)
            {
                // Xóa tất cả các item hiện tại trong ListView
                listView1.Items.Clear();

                // Tìm ListViewItem đầu tiên trong ListView
                ListViewItem firstItem = null;
                foreach (ListViewItem item in listView1.Items)
                {
                    firstItem = item;
                    break;
                }

                // Kiểm tra nếu tìm thấy ListViewItem đầu tiên
                if (firstItem != null)
                {
                    // Lấy đường dẫn của thư mục cha từ thuộc tính Tag của ListViewItem
                    string currentPath = firstItem.Tag.ToString();
                    string parentPath = Directory.GetParent(currentPath).FullName;

                    // Kiểm tra xem thư mục cha có tồn tại hay không
                    if (Directory.Exists(parentPath))
                    {
                        // Lấy danh sách tất cả các tệp và thư mục trong thư mục cha
                        string[] subItems = Directory.GetFileSystemEntries(parentPath);

                        // Xử lý từng tệp và thư mục
                        foreach (string subItemPath in subItems)
                        {
                            // Lấy tên tệp hoặc thư mục từ đường dẫn
                            string subItemName = Path.GetFileName(subItemPath);

                            // Tạo một ListViewItem mới với tên tệp hoặc thư mục
                            ListViewItem subItem = new ListViewItem(subItemName);

                            // Gắn đường dẫn vào thuộc tính Tag của ListViewItem
                            subItem.Tag = subItemPath;

                            // Thêm ListViewItem vào ListView
                            listView1.Items.Add(subItem);
                        }
                    }
                }
            }
        }


        //Chưa test
        private void moveToTrashCanToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (listView1.SelectedItems.Count > 0)
            {
                int clientSelectedItem = listView1.SelectedItems.Count;
                while (clientSelectedItem > 0)
                {
                    ListViewItem selectedItem = listView1.SelectedItems[0];
                    string id = selectedItem.SubItems[1].Text;
                    string fileName = selectedItem.Text;
                    string filePath = Path.Combine(apiService.location, fileName);
                    // Kiểm tra nếu đối tượng là một thư mục
                    //text/plain
                    //application/vnd.google-apps.folder
                    apiService.MoveFileToTrash(id, service);
                    // Xóa item khỏi ListView
                    listView1.Items.Remove(selectedItem);
                    clientSelectedItem--;
                }

            }
        }

        private void btnOpenTrashFiles_Click(object sender, EventArgs e)
        {
            
            this.listView1.ContextMenuStrip=this.contextMenuStrip2;
            this.RecoverMenu.Click+= new EventHandler(this.recoverToolStripMenuItem);
           
            IList<Google.Apis.Drive.v3.Data.File> files = apiService.GetFilesFromTrash(service);
            loadFileFromDrive(files);
        }
        private void recoverToolStripMenuItem(object sender, EventArgs e)
        {
            
            if (listView1.SelectedItems.Count > 0)
            {
                int clientSelectedItem = listView1.SelectedItems.Count;
                while (clientSelectedItem > 0)
                {
                    ListViewItem selectedItem = listView1.SelectedItems[0];
                    string id = selectedItem.SubItems[1].Text;
                    string fileName = selectedItem.Text;
                    apiService.RestoreFileFromTrash(id, service);
                    listView1.Items.Remove(selectedItem);
                    clientSelectedItem--;
                }

            }
        }

        private void btnHome_Click(object sender, EventArgs e)
        {
            this.listView1.ContextMenuStrip = this.contextMenuStrip1;
            IList<Google.Apis.Drive.v3.Data.File> files = apiService.LoadFilesFromRootFolder(service);
            loadFileFromDrive(files);
        }

        private void deletePermanentToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (listView1.SelectedItems.Count > 0)
            {
                int clientSelectedItem = listView1.SelectedItems.Count;
                while (clientSelectedItem > 0)
                {
                    ListViewItem selectedItem = listView1.SelectedItems[0];
                    string id = selectedItem.SubItems[1].Text;
                    string fileName = selectedItem.Text;
                    apiService.DeleteFolderAndContents(service, id);
                    listView1.Items.Remove(selectedItem);
                    clientSelectedItem--;
                }

            }
        }
    }

}
