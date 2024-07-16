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
using System.Threading;

namespace GUI
{
    public partial class HomeWindow : Window
    {
        private readonly NoteRepository _noteRepository = new NoteRepository(new ToDoListDbContext());
        private readonly NoteService _noteService;
        private Timer _timer;
        private List<DateTime> _notificationTimes;
        private int Id;
        private bool isShowingNotCompletedNotes = false;


        public HomeWindow()
        {
            InitializeComponent();
            _noteService = new NoteService(_noteRepository);
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            WelcomeLabel.Content = $"{ProfileInfo.UserProfile.ProfileName}";
            RefreshNotes();
        }

        private void CalendarControl_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            DateTime selectedDate = CalendarControl.SelectedDate.GetValueOrDefault();
            NotesDataGrid.ItemsSource = null;
            NotesDataGrid.ItemsSource = _noteService.GetNotesByProfileIdAndTime(ProfileInfo.UserProfile.ProfileId, selectedDate);
        }

        private void AllBtn_Click(object sender, RoutedEventArgs e)
        {
            isShowingNotCompletedNotes = false;
            RefreshNotes();
        }

        private void NotCompletedBtn_Click(object sender, RoutedEventArgs e)
        {
            isShowingNotCompletedNotes = true;
            RefreshNotes();
        }

        private void QuitBtn_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void CreateNoteButton_Click(object sender, RoutedEventArgs e)
        {
            DetailWindow detail = new DetailWindow();
            detail.ShowDialog();
        }

        private void ChageProfileButton_Click(object sender, RoutedEventArgs e)
        {
            ProfileInfo.UserProfile = null;
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

        private void StatusButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var note = button.DataContext as Note;
            if (note != null)
            {
                note.Status = note.Status == "Completed" ? "Pending" : "Completed";
                _noteService.UpdateNote(note);

                if (isShowingNotCompletedNotes)
                {
                    NotesDataGrid.ItemsSource = _noteService.GetNotCompletedNotes(ProfileInfo.UserProfile.ProfileId)
                                                            .OrderByDescending(n => n.ModifiedDate)
                                                            .ToList();
                }
                else
                {
                    RefreshNotes();
                }
            }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string status)
            {
                return status == "Completed";
            }
            return false; // Default to false if not "Completed"
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public void RefreshNotes()
        {
            var notes = isShowingNotCompletedNotes
                        ? _noteService.GetNotCompletedNotes(ProfileInfo.UserProfile.ProfileId)
                                       .OrderByDescending(note => note.ModifiedDate)
                                       .ToList()
                        : _noteService.GetNotesByProfileId(ProfileInfo.UserProfile.ProfileId)
                                      .OrderByDescending(note => note.ModifiedDate)
                                      .ToList();

            NotesDataGrid.ItemsSource = null;
            NotesDataGrid.ItemsSource = notes;
        }

        private void NotesDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.Column is DataGridCheckBoxColumn && e.EditingElement is CheckBox checkBox)
            {
                var note = e.Row.Item as Note;
                if (note != null)
                {
                    string status = checkBox.IsChecked == true ? "Completed" : "Pending";
                    MessageBox.Show($"Note '{note.Title}' has been {status}.", "Status Changed", MessageBoxButton.OK, MessageBoxImage.Information);
                    note.Status = status;
                    _noteService.UpdateNote(note);
                }
            }
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
