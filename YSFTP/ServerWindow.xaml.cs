using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace YSFTP
{
    /// <summary>
    /// Interaction logic for ServerWindow.xaml
    /// </summary>
    public partial class ServerWindow : Window
    {
        public ServerWindow()
        {
            InitializeComponent();
            listBoxServers.ItemsSource = MainWindow.ServerList;
            InitializeEvent();
        }

        #region InitializeEvent
        private void InitializeEvent()
        {
            textBoxPort.TextChanged += TextBoxPort_TextChanged;
            listBoxServers.SelectionChanged += ListBoxServers_SelectionChanged;
            buttonDelete.Click += ButtonDelete_Click;
            buttonSave.Click += ButtonSave_Click;
            buttonUpdate.Click += ButtonUpdate_Click;
            buttonClose.Click += ButtonClose_Click;
        }
        #endregion

        #region ButtonClose_Click
        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        #endregion

        #region ButtonUpdate_Click
        private void ButtonUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (listBoxServers.SelectedItems.Count == 0) return;
            var selectedServer = MainWindow.ServerList[listBoxServers.SelectedIndex];
            selectedServer.ServerName = textBoxServerName.Text;
            selectedServer.Host = textBoxHost.Text;
            selectedServer.Port = Convert.ToInt32(textBoxPort.Text);
            selectedServer.UserName = textBoxUserName.Text;
            selectedServer.UserPass = passwordBoxUserPass.Password;
            listBoxServers.Items.Refresh();
        }
        #endregion

        #region ListBoxServers_SelectionChanged
        private void ListBoxServers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listBoxServers.SelectedItems.Count == 0) return;
            var selectedServer = MainWindow.ServerList[listBoxServers.SelectedIndex];
            textBoxServerName.Text = selectedServer.ServerName;
            textBoxHost.Text = selectedServer.Host;
            textBoxPort.Text = selectedServer.Port.ToString();
            textBoxUserName.Text = selectedServer.UserName;
            passwordBoxUserPass.Password = selectedServer.UserPass;
        }
        #endregion

        #region ButtonSave_Click
        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            var newUser = new YSFTPUser
            {
                ServerName = textBoxServerName.Text,
                Host = textBoxHost.Text,
                Port = Convert.ToInt32(textBoxPort.Text),
                UserName = textBoxUserName.Text,
                UserPass = passwordBoxUserPass.Password
            };
            MainWindow.ServerList.Add(newUser);
            ResetInput();
        }
        #endregion

        #region ResetInput
        private void ResetInput()
        {
            textBoxServerName.Clear();
            textBoxHost.Clear();
            textBoxPort.Clear();
            textBoxUserName.Clear();
            passwordBoxUserPass.Clear();
            listBoxServers.Items.Refresh();
        }
        #endregion

        #region ButtonDelete_Click
        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            if (listBoxServers.SelectedItems.Count == 0) return;
            MainWindow.ServerList.RemoveAt(listBoxServers.SelectedIndex);
            ResetInput();
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

    }
}
