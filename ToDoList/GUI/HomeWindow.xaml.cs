using Microsoft.Toolkit.Uwp.Notifications;
using Repositories;
using Repositories.Entities;
using Services;
using System;
using System.Collections.Generic;
using System.Globalization;
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


namespace GUI
{
    public partial class HomeWindow : Window
    {
        private readonly NoteService _noteService = new(new NoteRepository(new ToDoListDbContext()));

        private Timer _timer;
        private List<DateTime> _notificationTimes;
        private int Id;
        public HomeWindow()
        {
            InitializeComponent();
            SelectedDate = DateTime.Today;
            Tab = true;
            DataContext = this;

            InitializeNotificationTimes();
            SetupTimer();
        }

        private void ShowNotification(DateTime time)
        {
            new ToastContentBuilder()
            .AddArgument("action", "viewConversation")
            .AddArgument("conversationId", 9813)
            .AddText("Title")
            .AddText("Description")
            .Show(); // Not seeing the Show() method? Make sure you have v
        }

        private Profile? _loginedAccount;

        public Profile LoginedAccount
        {
            get { return _loginedAccount; }
            set
            {
                _loginedAccount = value;
                if (_loginedAccount != null)
                {
                    WelcomeLabel.Content = $"{_loginedAccount.ProfileName}, what do you have planned for today?";
                }
            }
        }



        public DateTime SelectedDate { get; set; }

        public Boolean Tab { get; set; }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Id = LoginedAccount.ProfileId;
            NotesDataGrid.ItemsSource = _noteService.GetAllNotes(LoginedAccount.ProfileId, SelectedDate);
        }

        private void CalendarControl_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedDate = CalendarControl.SelectedDate.GetValueOrDefault();
            if (Tab)
            {
                NotesDataGrid.ItemsSource = null;
                NotesDataGrid.ItemsSource = _noteService.GetAllNotes(LoginedAccount.ProfileId, SelectedDate);

            }
            else
            {
                NotesDataGrid.ItemsSource = null;
                NotesDataGrid.ItemsSource = _noteService.GetNotCompleteNotes(LoginedAccount.ProfileId, SelectedDate);

            }
        }


        private void AllBtn_Click(object sender, RoutedEventArgs e)
        {
            Tab = true;
            NotesDataGrid.ItemsSource = null;
            NotesDataGrid.ItemsSource = _noteService.GetAllNotes(LoginedAccount.ProfileId, SelectedDate);
            //NotesDataGrid.ItemsSource = _noteService.GetNotesByProfileId(LoginedAccount.ProfileId);
        }
        private void NotCompleteBtn_Click(object sender, RoutedEventArgs e)
        {
            Tab = false;
            NotesDataGrid.ItemsSource = null;
            NotesDataGrid.ItemsSource = _noteService.GetNotCompleteNotes(LoginedAccount.ProfileId, SelectedDate);
            //NotesDataGrid.ItemsSource = _noteService.GetNotCompleteNotes(LoginedAccount.ProfileId);
        }
        private void QuitBtn_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void CreateNoteButton_Click(object sender, RoutedEventArgs e)
        {
            DetailWindow detail = new DetailWindow();
            detail.ShowDialog();
            NotesDataGrid.ItemsSource = null;
            NotesDataGrid.ItemsSource = _noteService.GetAllNotes(LoginedAccount.ProfileId, SelectedDate);
        }

        private void ChageProfileButton_Click(object sender, RoutedEventArgs e)
        {
            ProfileWindow profileWindow = new ProfileWindow();
            profileWindow.Show();
            this.Close();
        }


        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var note = button.DataContext as Note;
            if (note != null)
            {
                DetailWindow detail = new DetailWindow(note);
                detail.ShowDialog();

                NotesDataGrid.ItemsSource = null;
                NotesDataGrid.ItemsSource = _noteService.GetAllNotes(LoginedAccount.ProfileId, SelectedDate);
            }
        }
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var note = button.DataContext as Note;

            if (note != null)
            {
                MessageBoxResult result = MessageBox.Show($"Are you sure you want to delete the note '{note.Title}'?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    _noteService.DeleteNoteById(note.NoteId);

                    RefreshNotes();
                }
            }
        }
        public void RefreshNotes()
        {
            NotesDataGrid.ItemsSource = null;
            NotesDataGrid.ItemsSource = _noteService.GetAllNotes(LoginedAccount.ProfileId, SelectedDate);
        }

        private void InitializeNotificationTimes()
        {
            // Danh sách các thời gian để hiện thông báo
            //_notificationTimes = _noteService.GetNotesByProfileID(Id);
            _notificationTimes = new List<DateTime>
            {
            new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 00, 37, 00), // 12h hôm nay
            new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 00, 38, 00), // 14:30 hôm nay
            // Thêm các thời gian khác vào đây
            };

        }

        private void SetupTimer()
        {
            //OntimedEvent là làm call back để nói gọi đi gọi lại

            _timer = new Timer(OnTimedEvent, null, 0, 1000); // Kiểm tra mỗi giây
        }

        private void OnTimedEvent(object sender)
        {
            DateTime now = DateTime.Now;

            foreach (var time in _notificationTimes.ToList())
            {
                if (now.Hour == time.Hour && now.Minute == time.Minute && now.Second == time.Second)
                {
                    Dispatcher.BeginInvoke(() => ShowNotification(time));
                    _notificationTimes.Remove(time); // Xóa thời gian đã thông báo
                }
            }
        }

    }


}
