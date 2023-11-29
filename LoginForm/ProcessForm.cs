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
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace LoginForm
{
    public partial class ProcessForm : Form
    {
        private Watcher wat;
        private string labelText;
        private DriveService service;
        private APIService apiService;
        private System.Windows.Forms.Timer timer;
        static string folderMasterId = null;
        private BackgroundWorker internetCheckWorker;
        private List<string> currentFolderPath = new List<string>();
        private List<string> pre_current_ids = new List<string>();

        public ProcessForm(string text)
        {
            InitializeComponent();

            labelText = text;

            // Tạo label và nút logout trong Form2
            Label label = new Label();
            label.Text = labelText;
            label.Location = new Point(50, 50);
            this.Controls.Add(label);
            System.Windows.Forms.Button logoutButton = new System.Windows.Forms.Button();
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
            IList<Google.Apis.Drive.v3.Data.File> files = apiService.LoadFilesFromRootFolder(service, "root");
            loadFileFromDrive(files);
            pre_current_ids.Add("root");
            btnBack.Enabled = false;
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
            cmbFileTyle.SelectedItem = "All";
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
                        // Hiển thị thông báo có kết nối internet
                        labelStatus.Text = "Online";
                        labelStatus.ForeColor = Color.FromArgb(0, 255, 0);
                        string folderPath = Path.Combine(Environment.CurrentDirectory, "save");
                        UploadItemsToGoogleDrive();
                        btnHome.PerformClick();
                    }
                    else
                    {
                        // Hiển thị thông báo không có kết nối internet
                        labelStatus.Text = "Offline";
                        labelStatus.ForeColor = Color.FromArgb(255, 0, 0);
                    }
                }));

                // Ngủ 1 giây trước khi kiểm tra lại
                Thread.Sleep(10000);
            }
        }
        private void UploadItemsToGoogleDrive()
        {
            string folderPath = Path.Combine(Environment.CurrentDirectory, "save");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            // Get all files inside the folder
            string[] files = Directory.GetFiles(folderPath);
            string[] folders = Directory.GetDirectories(folderPath);

            foreach (string filePath in files)
            {
                // Upload each file to Google Drive
                apiService.uploadFile(service, filePath, null);
            }
            foreach (string fPath in folders)
            {
                apiService.UploadFolder(service, fPath, null);
            }
            DeleteFolderContents(folderPath);
        }
        private void DeleteFolderContents(string folderPath)
        {
            // Delete all files inside the folder
            string[] files = Directory.GetFiles(folderPath);
            foreach (string filePath in files)
            {
                File.Delete(filePath);
            }

            // Recursively delete subfolders and their contents
            string[] subfolders = Directory.GetDirectories(folderPath);
            foreach (string subfolder in subfolders)
            {
                DeleteFolderContents(subfolder);
            }

            // Delete the empty folder itself
            Directory.Delete(folderPath);
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

        private void Timer_Tick(object sender, EventArgs e)
        {
            //apiService.dowloadAllFilesAndFolders(service, apiService.location);
            //listView1.Clear();
            //LoadDirectory(apiService.location);

        }
        private void btnSearch_Click(object sender, EventArgs e)
        {
            IList<Google.Apis.Drive.v3.Data.File> files = apiService.SearchFile(service, new Model.SearchFileParams { FileName = this.txtSearchFileName.Text, FileType = this.cmbFileTyle.Text });
            loadFileFromDrive(files);
        }

        private void cmbFileTyle_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void cmbOrderFiled_SelectedIndexChanged(object sender, EventArgs e)
        {
            IList<Google.Apis.Drive.v3.Data.File> files = apiService.SearchFile(service, new Model.SearchFileParams { FileName = this.txtSearchFileName.Text, FileType = this.cmbFileTyle.Text, SortBy = this.cmbOrderFiled.Text, SortType = cmbOrderType.Text });
            loadFileFromDrive(files);
        }

        private void cmbOrderType_SelectedIndexChanged(object sender, EventArgs e)
        {
            IList<Google.Apis.Drive.v3.Data.File> files = apiService.SearchFile(service, new Model.SearchFileParams { FileName = this.txtSearchFileName.Text, FileType = this.cmbFileTyle.Text, SortBy = this.cmbOrderFiled.Text, SortType = cmbOrderType.Text });
            loadFileFromDrive(files);
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
                //application / vnd.google - apps.folder
                string Type = selectedItem.SubItems[2].Text;
                if (Type == "application/vnd.google-apps.folder")
                {
                    string selectedFolderId = selectedItem.SubItems[1].Text; // Lấy Id của item (folder)
                    loadFileFromDrive(apiService.LoadFileFromAParent(selectedFolderId, service));
                    pre_current_ids.Add(selectedFolderId);
                    btnBack.Enabled = true;
                }
                else
                {
                    string fileName = selectedItem.Text;
                    string id = apiService.GetFileOrFolderId(service, fileName);
                    string fileType = GetFileType(fileName);
                    apiService.DownloadFile(id, service, fileType);
                }

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
                while (clientSelectedItem > 0)
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
            bool isConnected = CheckInternetConnection();
            if (isConnected)
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
                        string folderid = folderMasterId;
                        // Tạo một ListViewItem mới với tên file
                        string fileId = apiService.UploadFileToGoogleDrive(file, service, folderid);
                        ListViewItem item = new ListViewItem(Path.GetFileName(file));
                        item.Tag = fileId;
                        listView1.Items.Add(item);
                    }

                }
                btnHome.PerformClick();
            }
            else
            {
                string[] filesOrFolders = (string[])e.Data.GetData(DataFormats.FileDrop);

                foreach (string item in filesOrFolders)
                {
                    if (Directory.Exists(item)) // It's a folder
                    {
                        CopyFolder(item, Path.Combine(Environment.CurrentDirectory, "save"));
                    }
                    else if (File.Exists(item)) // It's a file
                    {
                        string saveFolderPath = Path.Combine(Environment.CurrentDirectory, "save");
                        string saveFilePath = Path.Combine(saveFolderPath, Path.GetFileName(item));

                        if (!Directory.Exists(saveFolderPath))
                        {
                            Directory.CreateDirectory(saveFolderPath);
                        }

                        File.Copy(item, saveFilePath);
                        UpdateListViewItem(Path.GetFileName(item), null);
                    }
                }
            }
        }
        private void CopyFolder(string sourceFolder, string destinationFolder)
        {
            if (!Directory.Exists(destinationFolder))
            {
                Directory.CreateDirectory(destinationFolder);
            }
            string sourceName = Path.GetFileName(sourceFolder);
            string newdestiantion = Path.Combine(destinationFolder, sourceName);
            if (!Directory.Exists(newdestiantion))
            {
                Directory.CreateDirectory(newdestiantion);
            }
            string[] files = Directory.GetFiles(sourceFolder);
            foreach (string file in files)
            {
                string destFile = Path.Combine(newdestiantion, Path.GetFileName(file));
                File.Copy(file, destFile);
            }

            string[] subfolders = Directory.GetDirectories(sourceFolder);
            foreach (string subfolder in subfolders)
            {
                string destSubfolder = Path.Combine(newdestiantion, Path.GetFileName(subfolder));
                CopyFolder(subfolder, destSubfolder);
            }
        }
        private void UpdateListViewItem(string fileName, string fileId)
        {
            ListViewItem item = new ListViewItem(fileName);
            item.Tag = fileId; // Lưu fileId vào thuộc tính Tag của ListViewItem
            listView1.Items.Add(item);
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
        //private void listView1_ItemActivate(object sender, EventArgs e)
        //{
        //    //// Kiểm tra xem đã chọn một item folder hay chưa
        //    //if (listView1.SelectedItems.Count > 0)
        //    //{
        //    //    ListViewItem selectedItem = listView1.SelectedItems[0];
        //    //    string folderPath =Path.Combine(apiService.location, selectedItem.Text);

        //    //    // Kiểm tra nếu item là một folder
        //    //    if (Directory.Exists(folderPath))
        //    //    {
        //    //        // Xóa tất cả các item hiện tại trong ListView
        //    //        listView1.Items.Clear();

        //    //        // Lấy danh sách tất cả các tệp và thư mục con của folder
        //    //        string[] subItems = Directory.GetFileSystemEntries(folderPath);

        //    //        // Xử lý từng tệp và thư mục con
        //    //        foreach (string subItemPath in subItems)
        //    //        {
        //    //            // Lấy tên tệp hoặc thư mục từ đường dẫn
        //    //            string subItemName = Path.GetFileName(subItemPath);

        //    //            // Tạo một ListViewItem mới với tên tệp hoặc thư mục
        //    //            ListViewItem subItem = new ListViewItem(subItemName);

        //    //            // Gắn đường dẫn vào thuộc tính Tag của ListViewItem
        //    //            subItem.Tag = subItemPath;

        //    //            // Thêm ListViewItem vào ListView
        //    //            listView1.Items.Add(subItem);
        //    //        }
        //    //    }
        //    //}
        //    //application / vnd.google - apps.folder
        //    ListViewItem selectedItem = listView1.SelectedItems[0];
        //    string text = selectedItem.SubItems[2].Text;
        //    if (text == null)
        //    {

        //    }
        //}
        //Chưa sử dụng được
        private void btnBack_Click(object sender, EventArgs e)
        {

            pre_current_ids.RemoveAt(pre_current_ids.Count - 1);
            loadFileFromDrive(apiService.LoadFileFromAParent(pre_current_ids.Last().ToString(), service));
            if (pre_current_ids.Count == 1)
            {
                btnBack.Enabled = false;
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

            this.listView1.ContextMenuStrip = this.contextMenuStrip2;
            this.RecoverMenu.Click += new EventHandler(this.recoverToolStripMenuItem);

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
            IList<Google.Apis.Drive.v3.Data.File> files = apiService.LoadFilesFromRootFolder(service, null);
            pre_current_ids = new List<string>();
            btnBack.Enabled = false;
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

        private void dowloadToToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 1)
            {
                ListViewItem selectedItem = listView1.SelectedItems[0];
                string fileName = selectedItem.Text;
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.FileName = fileName;
                // Thiết lập các tùy chọn khác cho hộp thoại
                saveFileDialog.Filter = "All Files (*.*)|*.*";
                saveFileDialog.Title = "Save File";
                string filePath = null;
                // Hiển thị hộp thoại và kiểm tra kết quả
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Lấy đường dẫn tệp tin đã chọn
                    filePath = saveFileDialog.FileName;

                    // Tiến hành xử lý lưu tệp tin
                    // ...
                }
                if (filePath != null)
                {
                    string directoryPath = Path.GetDirectoryName(filePath);
                    string id = apiService.GetFileOrFolderId(service, fileName);
                    string fileType = GetFileType(fileName);
                    apiService.DownloadFile(id, service, fileType, directoryPath, fileName);
                }
            }
            if (listView1.SelectedItems.Count > 1)
            {
                List<string> selectedFileIds = new List<string>();

                // Duyệt qua các mục đã chọn trong ListView
                foreach (ListViewItem selectedItem in listView1.SelectedItems)
                {
                    // Lấy giá trị ID từ SubItems
                    string id = selectedItem.SubItems[1].Text;

                    // Thêm ID vào danh sách
                    selectedFileIds.Add(id);

                }
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                string today = Convert.ToString(DateTime.Now);
                string fileName = "Drive-dowload-" + today + ".zip";
                fileName = fileName.Replace(":", "");
                fileName = fileName.Replace("/", "");
                fileName = fileName.Replace(" ", "");
                saveFileDialog.FileName = fileName;
                // Thiết lập các tùy chọn khác cho hộp thoại
                saveFileDialog.Filter = "All Files (*.*)|*.*";
                saveFileDialog.Title = "Save File";
                string filePath = null;
                // Hiển thị hộp thoại và kiểm tra kết quả
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Lấy đường dẫn tệp tin đã chọn
                    filePath = saveFileDialog.FileName;

                    // Tiến hành xử lý lưu tệp tin
                    // ...
                }
                if (filePath != null)
                {
                    string directoryPath = Path.GetDirectoryName(filePath);
                    apiService.CompressAndDownloadFiles(selectedFileIds, service, directoryPath, fileName);
                }
            }
        }

        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void ProcessForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Dừng kiểm tra khi form đóng
            internetCheckWorker.CancelAsync();
        }
    }

}
