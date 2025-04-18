using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Threading;
using System.Windows.Markup;
using Newtonsoft.Json.Linq;
using System.Windows.Media;
using Microsoft.Win32;
using System.IO;
using System.Windows.Media.Imaging;
using System.Resources;
using messenger_lib;
using messenger_gui.Controls;

namespace messenger_gui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        ObservableCollection<Message> Messages { get; set; } = new ObservableCollection<Message>();
        List<Chat> Chats = new List<Chat>();

        List<Message> MessagesCache = new List<Message>();

        public Messenger messenger { get; set; }

        int SelectedChatID = -1;

        bool MsgListWasInBottom = false;

        bool PopupTransparency = false;
        Theme WindowTheme;

        bool IsEditingMessage;
        int EditingMessageID;

        bool Authorized = false;

        public MainWindow()
        {
            InitializeComponent();

            WindowTheme = new Theme(Application.Current.Resources, 8);

            MessengerStartup();
        }

        private async void MessengerStartup()
        {
            ShowPopup(LoginPanel, false, "Авторизация");

            ConnectionFailedPanel.Visibility = Visibility.Collapsed;

            ChatInfoPanel.Visibility = Visibility.Collapsed;
            MessageInput.IsReadOnly = true;
            SendButton.IsEnabled = false;

            messenger = new Messenger();
            messenger.SessionOpen();
            //messengerApi = new MessengerApi();
            //messengerApi.InitConnection();
            //try
            //{
            //    await messenger.SessionOpen();
            //}
            //catch (Exception ex)
            //{
            //    ShowError(ex.Message);
            //}

            MessageList.ItemsSource = Messages;

            Thread UpdateThread = new Thread(UpdateLoop);
            UpdateThread.Start();
        }

        private void ShowPopup(UIElement popupData, bool isTransparent, string windowName)
        {
            SetVisibility(PopupWindow, true);
            PopupTransparency = isTransparent;
            UpdatePopupBackground(isTransparent);
            PopupTitle.Text = windowName;

            foreach (UIElement elem in PopupsGrid.Children)
            {
                SetVisibility(elem, false);
            }
            SetVisibility(popupData, true);

            PopupCloseButton.Visibility = isTransparent ? Visibility.Visible : Visibility.Collapsed;
        }

        private void ClosePopup()
        {
            SetVisibility(PopupWindow, false);
        }

        private void SetVisibility(UIElement Element, bool Visible)
        {
            Element.Visibility = Visible ? Visibility.Visible : Visibility.Collapsed;
        }

        private void UpdatePopupBackground(bool IsTransparent)
        {
            System.Windows.Media.Color newColor;
            if (IsTransparent)
            {
                newColor = WindowTheme.MakeColor(30);
                newColor.A = 120;
            }
            else
            {
                newColor = WindowTheme.MakeColor(64);
            }

            PopupWindow.Background = new SolidColorBrush(newColor);
        }

        private async void UpdateChatList()
        {
            try
            {
                Chats = await messenger.GetAllChats();
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }

            Chats = Chats.OrderBy(o => o.LastMsgTime).Reverse().ToList();

            ChatList.ItemsSource = Chats;
        }


        private bool MessageCacheContains(Message message)
        {
            foreach (Message msg in MessagesCache)
            {
                if (msg.localmessageid == message.localmessageid)
                {
                    return true;
                }
            }
            return false;
        }

        private bool MessageListContains(Message message, List<Message> list)
        {
            foreach (Message msg in list)
            {
                if (msg.localmessageid == message.localmessageid)
                {
                    return true;
                }
            }
            return false;
        }

        private async void UpdateMessagesList()
        {
            if (SelectedChatID == -1)
            {
                MessageInput.IsReadOnly = true;
                SendButton.IsEnabled = false;
                MessagesCache.Clear();
                Messages.Clear();
                return;
            }

            MessageInput.IsReadOnly = false;
            SendButton.IsEnabled = true;

            List<Message> NewMessages = new List<Message>();

            try
            {
                NewMessages = await messenger.GetChatMessages(SelectedChatID);
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }

            NewMessages = NewMessages.OrderBy(o => o.localmessageid).ToList();

            int i = 0;

            List<Message> CacheCopy = [.. MessagesCache];
            foreach (Message message in CacheCopy)
            {
                if (!MessageListContains(message, NewMessages))
                {
                    MessagesCache.Remove(message);
                    Messages.Remove(message);
                }
                i++;
            }
            i = 0;
            foreach (Message message in NewMessages)
            {
                if (!MessageCacheContains(message))
                {
                    MessagesCache.Insert(i,message);
                    Messages.Insert(i,message);
                }
                else
                {
                    if (Messages[i].text != message.text)
                    {
                        Messages[i] = new Message(message);
                    }
                }
                i++;
            }

            if (Messages.Count > 0)
            {
                if (CacheCopy.Count == 0 || CacheCopy.Last().localmessageid != Messages.Last().localmessageid)
                {
                    UpdateChatList();
                }
            }

            UpdateScroll();
        }

        private void ShowError(string ErrorMessage)
        {
            MessageBox.Show(ErrorMessage, "Ошибка");
        }
        public async void UpdateLoop()
        {
            while (true)
            {
                Thread.Sleep(500);
                try
                {
                    if (Authorized)
                    {
                        Dispatcher.Invoke((Delegate)(() =>
                        {
                            UpdateMessagesList();
                            UpdateChatList();
                            //UpdateChatInfo();
                        }));
                    }
                }
                catch (Exception ex)
                {
                    ShowError(ex.Message);
                }
            }
        }

        private async void SendMessage()
        {
            string message = StringFromRichTextBox(MessageInput);
            if (message.EndsWith("\r\n"))
            {
                message = message.Substring(0, message.Length - 2);
            }
            Message msg = new Message();
            msg.text = message;
            msg.IsYourMessage = true;
            MessageInput.Document.Blocks.Clear();
            try
            {
                await messenger.PushMessage(message, SelectedChatID);
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
            UpdateMessagesList();
            
        }

        string StringFromRichTextBox(RichTextBox rtb)
        {
            TextRange textRange = new TextRange(
                // TextPointer to the start of content in the RichTextBox.
                rtb.Document.ContentStart,
                // TextPointer to the end of content in the RichTextBox.
                rtb.Document.ContentEnd
            );

            // The Text property on a TextRange object returns a string
            // representing the plain text content of the TextRange.
            return textRange.Text;
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            SendMessage();
        }

        private void MessageInput_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (IsEnterPressed(e) && MessageInput.IsReadOnly == false)
            {
                SendMessage();
                e.Handled = true;
            }
        }

        private static bool IsEnterPressed(KeyEventArgs e)
        {
            return e.Key == Key.Enter && Keyboard.Modifiers != ModifierKeys.Shift;
        }

        private void MessageList_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateScroll();
        }

        private void UpdateScroll()
        {
            if (MsgListWasInBottom)
            {
                FindScrollViewer(MessageList).ScrollToBottom();
            }
        }

        private static ScrollViewer FindScrollViewer(DependencyObject root)
        {
            var queue = new Queue<DependencyObject>(new[] { root });

            do
            {
                var item = queue.Dequeue();

                if (item is ScrollViewer)
                    return (ScrollViewer)item;

                for (var i = 0; i < VisualTreeHelper.GetChildrenCount(item); i++)
                    queue.Enqueue(VisualTreeHelper.GetChild(item, i));
            } while (queue.Count > 0);

            return null;
        }

        private void MessageList_Loaded(object sender, RoutedEventArgs e)
        {
            var listBox = (ListBox)sender;

            var scrollViewer = FindScrollViewer(listBox);

            if (scrollViewer != null)
            {
                scrollViewer.ScrollChanged += (o, args) =>
                {
                    MsgListWasInBottom = scrollViewer.ScrollableHeight <= args.VerticalOffset;
                };
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private async void ShowErrorText(TextBlock text, string message)
        {
            text.Text = message;

            bool needToStartTask = text.Visibility != Visibility.Visible;

            text.Visibility = Visibility.Visible;
            text.Opacity = 1;
            text.Tag = 100;

            if (!needToStartTask)
            {
                return;
            }

            await Task.Run(() =>
            {
                int i = 0;
                Dispatcher.Invoke(() =>
                {
                    i = (int)text.Tag;
                });
                while (i > 0)
                {
                    Dispatcher.Invoke(() =>
                    {
                        text.Tag = (int)text.Tag - 1;
                        i = (int)text.Tag;
                        if (i < 60)
                        {
                            text.Opacity = Convert.ToDouble(i)/60d;
                        }
                    });
                    Thread.Sleep(1);
                }
                Dispatcher.Invoke(() =>
                {
                    text.Visibility = Visibility.Hidden;
                });
            });
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = LoginUsernameText.Text;
            string password = LoginPasswordText.Password;

            try
            {
                LoginButton.IsEnabled = false;
                //LoginErrorMsg.Text = "Загрузка...";
                //LoginErrorMsg.Visibility = Visibility.Visible;
                bool result = await messenger.Authorization(username, password);
                if (!result)
                {
                    ShowPopup(Login2FAPanel, false, "");
                    LoginButton.IsEnabled = true;
                    return;
                }
            }
            catch (Exception ex)
            {
                ShowErrorText(LoginErrorMsg, ex.Message);
                LoginButton.IsEnabled = true;
                return;
            }
            LoginButton.IsEnabled = true;
            UpdateUserInfo(username);
            ClosePopup();
            UpdateChatList();
            Authorized = true;
        }

        private async void Login2FAButton_Click(object sender, RoutedEventArgs e)
        {
            string code = Login2FACodeText.Text;

            try
            {
                Login2FAButton.IsEnabled = false;
                Login2FAErrorMsg.Text = "Загрузка...";
                Login2FAErrorMsg.Visibility = Visibility.Visible;
                await messenger.Authorization2FA(code);
            }
            catch (Exception ex)
            {
                Login2FAButton.IsEnabled = true;
                ShowErrorText(Login2FAErrorMsg, ex.Message);
                return;
            }
            Login2FAButton.IsEnabled = true;
            UpdateUserInfo(LoginUsernameText.Text);
            ClosePopup();
            UpdateChatList();
            Authorized = true;
        }

        private void UpdateUserInfo(string username, Brush avatar = null)
        {
            Nickname.Text = messenger.nickname;
            SettingsNickname.Text = messenger.nickname;
            Username.Text = username;
            SettingsUsername.Text = username;
            if (avatar == null)
            {
                return;
            }
            AvatarIcon.Fill = avatar;
        }

        private void LoginRegButton_Click(object sender, RoutedEventArgs e)
        {
            ShowPopup(RegPanel, false, "Регистрация");
        }

        private void RegLoginButton_Click(object sender, RoutedEventArgs e)
        {
            ShowPopup(LoginPanel, false, "Авторизация");
        }

        private async void RegButton_Click(object sender, RoutedEventArgs e)
        {
            string username = RegUsernameText.Text;
            string nick = RegNickText.Text;
            string password = RegPasswordText.Password;
            string passwordConfirm = RegPasswordConfirmText.Password;

            if (password != passwordConfirm)
            {
                ShowErrorText(RegErrorMsg, "Пароли не совпадают");
                return;
            }

            try
            {
                RegButton.IsEnabled = false;
                await messenger.Registration(username, nick, password);

            }
            catch (Exception ex)
            {
                RegButton.IsEnabled = true;
                ShowErrorText(RegErrorMsg, ex.Message);
                return;
            }
            RegButton.IsEnabled = true;
            UpdateUserInfo(username);
            ClosePopup();
            UpdateChatList();
            Authorized = true;
        }

        private void CreateChatButton_Click(object sender, RoutedEventArgs e)
        {
            ShowPopup(NewChatPanel, true, "Создание чата");
        }

        private void UpdateChatsButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateChatList();
        }

        private async void NewChatButton_Click(object sender, RoutedEventArgs e)
        {
            string name = NewChatNameText.Text;
            string desc = NewChatDescText.Text;
            string seconname = NewChatSeconsUserText.Text;
            try
            {
                await messenger.CreateNewChat(name, desc, seconname);
            }
            catch (Exception ex)
            {
                ShowErrorText(NewChatErrorMsg, ex.Message);
            }

            ClosePopup();
            UpdateChatList();
        }

        private void ChatList_MouseClick(object sender, MouseButtonEventArgs e)
        {
            Chat SelectedChat = (Chat)((ListBox)sender).SelectedItem;

            if (SelectedChat == null)
            {
                return;
            }

            SelectedChatID = SelectedChat.chatid;

            MessagesCache.Clear();
            Messages.Clear();
            
            // Scroll to last message
            MsgListWasInBottom = true;

            UpdateMessagesList();
            UpdateChatInfo();
        }

        private async void UpdateChatInfo()
        {
            if (SelectedChatID == -1)
            {
                ChatInfoPanel.Visibility = Visibility.Collapsed;
                return;
            }

            ChatInfoObj chat;
            try
            {
                chat = await messenger.GetChatInfo(SelectedChatID);
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
                return;
            }
            ChatInfoPanel.Visibility = Visibility.Visible;
            ChatInfoName.Text = chat.Title;
            ChatInfoDesc.Text = chat.Description;
            ChatUsersList.ItemsSource = chat.Users;
        }

        private async void SubmitSettings(object sender, RoutedEventArgs e)
        {
            string username = SettingsUsername.Text;
            string nickname = SettingsNickname.Text;
            Brush avatar = SettingsAvatar.Fill;

            messenger.nickname = nickname;

            UpdateUserInfo(username, avatar);

            try
            {
                await messenger.SetUserInfo(nickname);
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }

            if (FontComboBox.SelectedItem == null)
            {
                return;
            }

            string font = FontComboBox.SelectedItem.ToString();
            FontFamily = new FontFamily(font);
            
        }

        private void ColorThemeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            WindowTheme.UpdateColorScheme(Convert.ToInt16(ColorThemeSlider.Value*36), Application.Current.Resources);
            UpdatePopupBackground(PopupTransparency);
        }

        private void SaturationThemeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            WindowTheme.saturation = Convert.ToDouble(SaturationThemeSlider.Value / 10);
            WindowTheme.UpdateColorScheme(Convert.ToInt16(ColorThemeSlider.Value * 36), Application.Current.Resources);
            UpdatePopupBackground(PopupTransparency);
        }

        private void PopupCloseButton_Click(object sender, RoutedEventArgs e)
        {
            ClosePopup();
        }

        private void OpenSettings(object sender, RoutedEventArgs e)
        {
            ShowPopup(SettingsPanel, true, "Настройки");
        }

        private void SelectAvatar(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.DefaultExt = ".png";
            dialog.Filter = "(.png)|*.png";
            bool? result = dialog.ShowDialog();

            if (result == true && File.Exists(dialog.FileName))
            {
                ImageBrush newBrush = new ImageBrush(new BitmapImage(new Uri(dialog.FileName)));
                newBrush.Stretch = Stretch.UniformToFill;
                //AvatarIcon.Fill = newBrush;
                SettingsAvatar.Fill = newBrush;
            }
        }

        private void EditMessage(object sender, RoutedEventArgs e)
        {
            if (IsEditingMessage == true)
            {
                MessageBox.Show("Нельзя редактировать несколько сообщений");
                return;
            }

            MenuItem menuItem = (MenuItem)sender;
            ContextMenu menu = (ContextMenu)menuItem.Parent;
            Message msg = (Message)menuItem.DataContext;
            Border border = (Border)menu.PlacementTarget;
            StackPanel stackPanel = (StackPanel)border.Child;
            TextBox msgText = (TextBox)stackPanel.Children[1];

            msgText.IsReadOnly = false;
            msgText.Background = WindowTheme.GetThemeColor(Application.Current.Resources, 1);

            IsEditingMessage = true;
            EditingMessageID = msg.localmessageid;
        }

        private async void DeleteMessage(object sender, RoutedEventArgs e)
        {
            Message msg = (Message)((MenuItem)sender).DataContext;
            try
            {
                await messenger.DeleteMessage(SelectedChatID, msg.localmessageid);
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }

            UpdateMessagesList();
        }

        private async void ChatClear(object sender, RoutedEventArgs e)
        {
            Chat chat = (Chat)((MenuItem)sender).DataContext;
            try
            {
                await messenger.ClearChat(chat.chatid);
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
            UpdateChatList();
            if (SelectedChatID == chat.chatid)
            {
                Messages.Clear();
                MessagesCache.Clear();
            }
        }
        private void ChatEdit(object sender, RoutedEventArgs e)
        {
            Chat chat = (Chat)((MenuItem)sender).DataContext;
            MessageBox.Show($"ChatEdit {chat.chatid}\nimplement later");
        }
        private async void ChatDelete(object sender, RoutedEventArgs e)
        {
            Chat chat = (Chat)((MenuItem)sender).DataContext;
            try
            {
                await messenger.DeleteChat(chat.chatid);
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
            SelectedChatID = -1;
            UpdateChatList();
            UpdateChatInfo();
            UpdateMessagesList();
        }

        private async void MessageText_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            TextBox msgTextBox = (TextBox)sender;
            if (IsEnterPressed(e) && msgTextBox.IsReadOnly == false && IsEditingMessage)
            {
                string newText = msgTextBox.Text;
                msgTextBox.IsReadOnly = true;
                msgTextBox.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
                IsEditingMessage = false;
                e.Handled = true;

                try
                {
                    await messenger.EditMessage(SelectedChatID, EditingMessageID, newText);
                }
                catch (Exception ex)
                {
                    ShowError(ex.Message);
                }

                UpdateMessagesList();
            }
        }

        private async void ChatInfoApplyButton_Click(object sender, RoutedEventArgs e)
        {
            string title = ChatInfoName.Text;
            string desc = ChatInfoDesc.Text;
            try
            {
                await messenger.SetChatInfo(SelectedChatID, title, desc);
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
            UpdateChatList();
        }

        private async void ChatInfoAddUserButton_Click(object sender, RoutedEventArgs e)
        {
            string nick = ChatInfoAddUserText.Text;
            try
            {
                await messenger.AddChatUser(SelectedChatID, nick);
                ChatInfoAddUserText.Text = "";
            }
            catch (Exception ex)
            {
                ShowErrorText(ChatInfoAddUserError, ex.Message);
            }
            UpdateChatInfo();
        }

        private async void ChatUserListRemoveButton_Click(object sender, RoutedEventArgs e)
        {
            string nick = (string)((IconButton)sender).DataContext;
            try
            {
                await messenger.RemoveChatUser(SelectedChatID, nick);
            }
            catch (Exception ex)
            {
                ShowErrorText(ChatInfoAddUserError, ex.Message);
            }
            UpdateChatInfo();
        }
    }
}