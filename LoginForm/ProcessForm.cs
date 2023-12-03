using Google.Apis.Drive.v3;
using GoogleDriveAPIExample;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
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
            logoutButton.Hide();
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
                        string folderPath = Path.Combine(Environment.CurrentDirectory,Path.GetFileName(apiService.location)+ "_save");
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
                Thread.Sleep(100000);
            }
        }
        private void UploadItemsToGoogleDrive()
        {
            string folderPath = Path.Combine(Environment.CurrentDirectory, Path.GetFileName(apiService.location)+"_save");
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
            string owner_name = null;
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
                    string file_owner_name = file.Owners.FirstOrDefault().EmailAddress;
                    file_owner_name = file_owner_name.Replace("@gmail.com", "");
                    item.SubItems.Add(file_owner_name);
                    owner_name = file_owner_name;
                    item.SubItems.Add(file.Name);
                    listView1.Items.Add(item);
                }
            }
            string ownerpath = Path.Combine(Environment.CurrentDirectory, owner_name+".txt");
            if (File.Exists(ownerpath))
            {
                File.Delete(ownerpath);
                WriteToFile(files, ownerpath);
            }
            else
            {
                WriteToFile(files, ownerpath);
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
                string owner = selectedItem.SubItems[3].Text;
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
                if (e.Data.GetDataPresent(typeof(List<ListViewItem>)))
                {
                    List<ListViewItem> draggedItems = (List<ListViewItem>)e.Data.GetData(typeof(List<ListViewItem>));
                    string whereFileAt = null;
                    string dowloadPath = Path.Combine(Environment.CurrentDirectory, "Storage");
                    // Tương tác với tất cả các ListViewItem được chọn
                    foreach (ListViewItem draggedItem in draggedItems)
                    {
                        string owner = draggedItem.SubItems[3].Text;
                        string file_id = draggedItem.SubItems[1].Text;
                        string type = draggedItem.SubItems[2].Text;
                        APIService secondapi = new APIService();
                        DriveService secondService = secondapi.automatic(owner);
                       
                        if (!Directory.Exists(dowloadPath))
                        {
                            Directory.CreateDirectory(dowloadPath);
                        }
                        string filefullName = draggedItem.Text;
                        //string fileType = GetFileType(filefullName);
                        whereFileAt = Path.Combine(dowloadPath, filefullName);
                        secondapi.DownloadFileOrFolder(secondService, file_id, dowloadPath);
                       

                        // Add the dragged item to ListView B
                        //listView1.Items.Add((ListViewItem)draggedItem.Clone());
                    }
                    string parentFolderId = pre_current_ids.ElementAt(pre_current_ids.Count - 1);
                    if (parentFolderId == "root")
                    {
                        parentFolderId = null;
                    }
                    apiService.UploadFolder_ver2(service, dowloadPath, parentFolderId);
                    int size = dowloadPath.Length;
                    if (Directory.Exists(dowloadPath))
                    {
                        if (size == 89)
                        {
                            Directory.Delete(dowloadPath,true);
                        }
                    }

                }
                else
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
                            string parentFolderId = pre_current_ids.ElementAt(pre_current_ids.Count - 1);
                            if (parentFolderId == "root")
                            {
                                parentFolderId = null;
                            }
                            apiService.UploadFolder(service, file, parentFolderId);
                        }
                        // Kiểm tra nếu là một file
                        else if (File.Exists(file))
                        {
                            // Lấy tên file từ đường dẫn
                            string fileName = Path.GetFileName(file);
                            string parentFolderId = pre_current_ids.ElementAt(pre_current_ids.Count - 1);
                            if (parentFolderId == "root")
                            {
                                parentFolderId = null;
                            }
                            // Tạo một ListViewItem mới với tên file
                            string fileId = apiService.UploadFileToGoogleDrive(file, service, parentFolderId);
                            ListViewItem item = new ListViewItem(Path.GetFileName(file));
                            item.Tag = fileId;
                            listView1.Items.Add(item);
                        }

                    }
                }
                btnHome.PerformClick();
            }
            else
            {
                string[] filesOrFolders = (string[])e.Data.GetData(DataFormats.FileDrop);
                string wahtizd = Path.GetFileName(apiService.location);
                string saveFolderName = wahtizd + "_save";
                string saveFolderPath = Path.Combine(Environment.CurrentDirectory, saveFolderName);
                if (!Directory.Exists(saveFolderPath))
                {
                    Directory.CreateDirectory(saveFolderPath);
                }
                foreach (string item in filesOrFolders)
                {
                    if (Directory.Exists(item)) // It's a folder
                    {
                       // string saveFolderName = apiService.userName + "_save";
                        CopyFolder(item,saveFolderPath);
                    }
                    else if (File.Exists(item)) // It's a file
                    {
                        string saveFilePath = Path.Combine(saveFolderPath, Path.GetFileName(item));

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
            if (e.Data.GetDataPresent(typeof(ListViewItem)))
            {
                e.Effect = DragDropEffects.Move;
               
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
            if (pre_current_ids.Last() == "shared")
            {
                loadFileFromDrive(apiService.geShareFiles(service));
            }
            else
            {
                loadFileFromDrive(apiService.LoadFileFromAParent(pre_current_ids.Last().ToString(), service));
            }
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
            pre_current_ids.Add("root");
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
               
                string filePath = null;
                string folderPath = null;
                // Hiển thị hộp thoại và kiểm tra kết quả
                string Type = selectedItem.SubItems[2].Text;
                if (Type == "application/vnd.google-apps.folder")
                {
                    saveFileDialog.Title = "Select Destination Folder";
                    saveFileDialog.FileName = selectedItem.SubItems[4].Text;
                    saveFileDialog.Filter = "Folders|*.folder";
                    saveFileDialog.RestoreDirectory = true;
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        {
                            // Lấy đường dẫn tệp tin đã chọn
                            folderPath = saveFileDialog.FileName;

                            // Tiến hành xử lý lưu tệp tin
                            // ...
                        }
                        string folderId = selectedItem.SubItems[1].Text;
                        string directoryPath = Path.GetDirectoryName(folderPath);
                        apiService.DownloadFolder(service, folderId, Path.Combine(directoryPath, selectedItem.SubItems[4].Text));
                        try
                        {
                            Process.Start(directoryPath);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Không thể mở file: " + ex.Message);
                        }
                    }
                }
                else
                {
                    //string folderId = selectedItem.SubItems[1].Text;
                    //apiService.GetDownloadLink(service, folderId);
                    saveFileDialog.FileName = fileName;
                    // Thiết lập các tùy chọn khác cho hộp thoại
                    saveFileDialog.Filter = "All Files (*.*)|*.*";
                    saveFileDialog.Title = "Save File";
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        // Lấy đường dẫn tệp tin đã chọn
                        filePath = saveFileDialog.FileName;

                        // Tiến hành xử lý lưu tệp tin
                        // ...
                    }
                    string directoryPath = Path.GetDirectoryName(filePath);
                    string id = apiService.GetFileOrFolderId(service, fileName);
                    string fileType = GetFileType(fileName);
                    apiService.DownloadFile(id, service, fileType, directoryPath, fileName);
                }

            }
            if (listView1.SelectedItems.Count > 1)
            {
                List<string> selectedFileIds = new List<string>();


                //SaveFileDialog saveFileDialog = new SaveFileDialog();
                //string today = Convert.ToString(DateTime.Now);
                //string fileName = "Drive-dowload-" + today + ".zip";
                //fileName = fileName.Replace(":", "");
                //fileName = fileName.Replace("/", "");
                //fileName = fileName.Replace(" ", "");
                //saveFileDialog.FileName = fileName;
                //// Thiết lập các tùy chọn khác cho hộp thoại
                //saveFileDialog.Filter = "All Files (*.*)|*.*";
                //saveFileDialog.Title = "Save File";
                //string filePath = null;
                //// Hiển thị hộp thoại và kiểm tra kết quả
                //if (saveFileDialog.ShowDialog() == DialogResult.OK)
                //{
                //    // Lấy đường dẫn tệp tin đã chọn
                //    filePath = saveFileDialog.FileName;

                //    // Tiến hành xử lý lưu tệp tin
                //    // ...
                //}
                var folderBrowserDialog1 = new FolderBrowserDialog();

                // Show the FolderBrowserDialog.
                DialogResult result = folderBrowserDialog1.ShowDialog();
                string filePath = null;
                if (result == DialogResult.OK)
                {
                    filePath = folderBrowserDialog1.SelectedPath;
                   //Do your work here!
    }
                if (filePath != null)
                {
                    foreach (ListViewItem selectedItem in listView1.SelectedItems)
                    {
                        // Lấy giá trị ID từ SubItems
                        string id = selectedItem.SubItems[1].Text;
                        apiService.DownloadFileOrFolder(service, id, filePath);

                    }
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

        private void listView1_ItemDrag(object sender, ItemDragEventArgs e)
        {

            if (listView1.SelectedItems.Count > 0)
            {
                // Tạo một danh sách chứa các mục được chọn
                List<ListViewItem> selectedItems = new List<ListViewItem>();
                foreach (ListViewItem item in listView1.SelectedItems)
                {
                    selectedItems.Add(item);
                }

                // Chuyển danh sách các mục được chọn vào đối tượng DataObject
                DataObject dragData = new DataObject();
                dragData.SetData(typeof(List<ListViewItem>), selectedItems);

                // Bắt đầu thực hiện kéo
                listView1.DoDragDrop(dragData, DragDropEffects.Move);
            }
            //listView1.DoDragDrop(e.Item, DragDropEffects.Move);
        }
        public  void WriteToFile(IList<Google.Apis.Drive.v3.Data.File> files, string filePath)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    foreach (var file in files)
                    {
                        // Ghi thông tin file ID và file name vào mỗi dòng của tệp
                        writer.WriteLine($"{file.Id},{file.Name}");
                    }
                }

                Console.WriteLine($"File information has been written to: {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error writing to file: {ex.Message}");
            }
        }

        private void listView1_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(List<ListViewItem>)))
            {
                e.Effect = DragDropEffects.Move;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            loadFileFromDrive(apiService.geShareFiles(service));
            pre_current_ids.Add("shared");
        }

        private void ProcessForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (Directory.Exists(Path.Combine(Environment.CurrentDirectory, "temp")))
            {
                Directory.Delete(Path.Combine(Environment.CurrentDirectory, "temp"), true);
            }
        }
    }

}
