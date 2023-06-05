using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Serialization;

namespace YSFTP
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        #region Data
        public static List<YSFTPUser> ServerList = new List<YSFTPUser>();
        const string FILENAME = "Server.xml";
        YSFTPUser selectedUser = null;
        YSFTPClient ftp = null;

        public string Status
        {
            set
            {
                textBlockStatus.Text = value;
                LOG = "Status: " + value;
            }
        }
        public string LOG
        {
            set
            {
                var stringBuilder = new StringBuilder(textBlockLog.Text);
                stringBuilder.AppendLine(value);
                textBlockLog.Text = stringBuilder.ToString();
            }
        }
        #endregion

        #region ctor
        public MainWindow()
        {
            InitializeComponent();
            InitializeEvent();
        }
        #endregion

        #region Dummy Data
        private void DummyData()
        {
            if (MainWindow.ServerList.Count > 0) return;

            MainWindow.ServerList.Add(new YSFTPUser
            {
                ServerName = "127.0.0.1",
                Host = "127.0.0.1",
                Port = 21
            });
            MainWindow.ServerList.Add(new YSFTPUser
            {
                ServerName = "yusufsezer.com.tr",
                Host = "ftp.yusufsezer.com",
                Port = 21,
                UserName = "yusufsezer",
                UserPass = "yusufsezer"
            });
            MainWindow.ServerList.Add(new YSFTPUser
            {
                ServerName = "yusufsezer.com-1",
                Host = "yusufsezer.com",
                Port = 21,
                UserName = "yusufsezer",
                UserPass = "yusufsezer"
            });
            MainWindow.ServerList.Add(new YSFTPUser
            {
                ServerName = "yusufsezer.com.tr",
                Host = "yusufsezer.com.tr",
                Port = 20,
                UserName = "yusufsezer",
                UserPass = "yusufsezer"
            });
        }
        #endregion

        #region InitializeEvent
        private void InitializeEvent()
        {
            buttonServerList.Click += ButtonServerList_Click;
            buttonAbout.Click += ButtonAbout_Click; ;
            buttonServer.Click += ButtonServer_Click;
            buttonConnect.Click += ButtonConnect_Click;
            buttonRefresh.Click += ButtonRefresh_Click;
            buttonDelete.Click += ButtonDelete_Click;
            listViewDirectoriesFiles.KeyDown += ListViewDirectoriesFiles_KeyDown;
            listViewDirectoriesFiles.MouseDoubleClick += ListViewDirectoriesFiles_MouseDoubleClick;
            textBoxPort.TextChanged += TextBoxPort_TextChanged;
            this.Loaded += MainWindow_Loaded;
        }

        #endregion

        #region ListViewDirectoriesFiles_MouseDoubleClick
        private void ListViewDirectoriesFiles_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ShowFiles();
        }
        #endregion

        #region ListViewDirectoriesFiles_KeyDown
        private void ListViewDirectoriesFiles_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ShowFiles();
            }
        }
        #endregion

        #region ShowFiles
        private void ShowFiles()
        {
            var selectedItem = listViewDirectoriesFiles.SelectedItem as YSFTPItem;
            if (selectedItem != null && selectedItem.FileType == ItemType.Directory)
            {
                ListDirectory(selectedItem.FullPath);
            }
        }
        #endregion

        #region ButtonDelete_Click
        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = listViewDirectoriesFiles.SelectedItem as YSFTPItem;
            if (selectedItem != null)
            {
                ftp.Delete(selectedItem.FullPath);
                LOG = ftp.Response.StatusDescription;
                buttonRefresh.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            }
        }
        #endregion

        #region ButtonRefresh_Click
        private void ButtonRefresh_Click(object sender, RoutedEventArgs e)
        {
            listViewDirectoriesFiles.ItemsSource = ftp.ListDirectory();
        }
        #endregion

        #region ButtonConnect_Click
        private void ButtonConnect_Click(object sender, RoutedEventArgs e)
        {
            if (!SetConnection()) return;
            if (!SetFTP()) return;

            try
            {
                Status = "Connecting...";
                stackPanelConnection.IsEnabled = true;
                stackPanelConnection.Opacity = 1;
                imageConnect.Source = (BitmapImage)Application.Current.Resources["bitmapImageConnect"];
                ListDirectory("/");
                Status = "Connected";
                listViewDirectoriesFiles.IsEnabled = true;
            }
            catch (Exception ex)
            {
                Status = ex.Message;
                DisableBar();
            }
            finally
            {
                selectedUser = null;
            }
        }
        #endregion

        #region SetConnection
        private bool SetConnection()
        {
            if (ftp != null)
            {
                DisableBar();
                ftp = null;
                return false;
            }
            return true;
        }
        #endregion

        #region SetFTP
        private bool SetFTP()
        {
            bool result = string.IsNullOrWhiteSpace(textBoxHost.Text) ||
                string.IsNullOrWhiteSpace(textBoxPort.Text) ||
                string.IsNullOrWhiteSpace(textBoxUserName.Text) ||
                string.IsNullOrWhiteSpace(passwordBoxUserPass.Password);

            if (selectedUser != null)
            {
                ftp = new YSFTPClient(selectedUser);
                return true;
            }

            if (!result)
            {
                ftp = new YSFTPClient(textBoxHost.Text, Convert.ToInt32(textBoxPort.Text), textBoxUserName.Text, passwordBoxUserPass.Password);
                return true;
            }
            else if (!string.IsNullOrWhiteSpace(textBoxHost.Text))
            {
                ftp = new YSFTPClient(textBoxHost.Text);
                return true;
            }
            return false;
        }
        #endregion

        #region DisableBar
        private void DisableBar()
        {
            Status = "Disconnected";
            imageConnect.Source = (BitmapImage)Application.Current.Resources["bitmapImageDisconnect"];
            stackPanelConnection.IsEnabled = false;
            stackPanelConnection.Opacity = 0.7;
            listViewDirectoriesFiles.ItemsSource = null;
        }
        #endregion

        #region ListDirectory
        private void ListDirectory(string path)
        {
            listViewDirectoriesFiles.ItemsSource = ftp.ListDirectory(path);
        }
        #endregion

        #region Sort GridView
        private GridViewColumnHeader lastHeaderClicked = null;
        private ListSortDirection lastDirection = ListSortDirection.Ascending;

        private void GridViewHeader_Click(object sender, RoutedEventArgs e)
        {
            if (!(e.OriginalSource is GridViewColumnHeader columnHeader)) return;
            var sortDirection = ListSortDirection.Ascending;
            if (columnHeader == lastHeaderClicked && lastDirection == ListSortDirection.Ascending)
                sortDirection = ListSortDirection.Descending;
            sort(columnHeader, sortDirection);
            lastHeaderClicked = columnHeader; lastDirection = sortDirection;
        }

        private void sort(GridViewColumnHeader columnHeader, ListSortDirection sortDirection)
        {
            var path = (columnHeader.Column.DisplayMemberBinding as Binding)?.Path.Path;
            path = path ?? columnHeader.Column.Header as string;
            var dataView = CollectionViewSource.GetDefaultView(listViewDirectoriesFiles.ItemsSource);
            dataView.SortDescriptions.Clear();
            var sortDescription = new SortDescription(path, sortDirection);
            dataView.SortDescriptions.Add(sortDescription);
            dataView.Refresh();
        }

        #endregion

        #region MainWindow_Loaded
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            LoadServerData(FILENAME);
#if DEBUG
            DummyData();
#endif
        }
        #endregion

        #region LoadServerData
        private void LoadServerData(string fileName)
        {
            try
            {
                using (var fileStream = new FileStream(fileName, FileMode.Open))
                {
                    var xmlSerializer = new XmlSerializer(MainWindow.ServerList.GetType(), new XmlRootAttribute()
                    {
                        ElementName = "Servers"
                    });
                    List<YSFTPUser> readList = (List<YSFTPUser>)xmlSerializer.Deserialize(fileStream);
                    ServerList.AddRange(readList);
                }
            }
            catch (Exception ex)
            {
                // Log - file is corrupted
                LOG = "Error: " + ex.Message;
            }
        }
        #endregion

        #region CreateContextMenu
        private void CreateContextMenu()
        {
            buttonServerList.ContextMenu.Items.Clear();
            foreach (YSFTPUser currentUser in ServerList)
            {
                var newMenuItem = new MenuItem();
                newMenuItem.Tag = currentUser;
                newMenuItem.Click += NewMenuItem_Click;
                buttonServerList.ContextMenu.Items.Add(newMenuItem);
            }
        }
        #endregion

        #region NewMenuItem_Click
        private void NewMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var clickedItem = (sender as MenuItem);
            selectedUser = (clickedItem.Tag as YSFTPUser);
            textBoxHost.Text = selectedUser.Host;
            textBoxUserName.Text = selectedUser.UserName;
            passwordBoxUserPass.Password = selectedUser.UserPass;
            textBoxPort.Text = selectedUser.Port.ToString();
            buttonConnect.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        }
        #endregion

        #region ButtonServerList_Click
        private void ButtonServerList_Click(object sender, RoutedEventArgs e)
        {
            var currentButton = sender as Button;
            CreateContextMenu();
            if (currentButton.ContextMenu != null && currentButton.ContextMenu.Items.Count > 0)
            {
                currentButton.ContextMenu.IsOpen = true;
                currentButton.ContextMenu.PlacementTarget = currentButton;
                currentButton.ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
            }
        }
        #endregion

        #region TextBoxPort_TextChanged
        private void TextBoxPort_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            textBox.Text = new String(textBox.Text.Where(c => Char.IsDigit(c)).ToArray());
            textBox.SelectionStart = textBox.Text.Length;
        }
        #endregion

        #region ButtonServer_Click
        private void ButtonServer_Click(object sender, RoutedEventArgs e)
        {
            new ServerWindow() { Owner = this }.ShowDialog();
        }
        #endregion

        #region ButtonAbout_Click
        private void ButtonAbout_Click(object sender, RoutedEventArgs e)
        {
            new AboutWindow() { Owner = this }.ShowDialog();
        }
        #endregion

        #region SaveData
        private void SaveData(string fileName)
        {
            try
            {
                using (var fileStream = new FileStream(fileName, FileMode.Create))
                {
                    var xmlSerializer = new XmlSerializer(MainWindow.ServerList.GetType(), new XmlRootAttribute()
                    {
                        ElementName = "Servers"
                    });
                    xmlSerializer.Serialize(fileStream, MainWindow.ServerList);
                }
            }
            catch
            {
                // Log - Something went wrong
            }
        }
        #endregion

        #region OnClosing
        protected override void OnClosing(CancelEventArgs e)
        {
            SaveData(FILENAME);
            base.OnClosing(e);
        }
        #endregion

    }
}