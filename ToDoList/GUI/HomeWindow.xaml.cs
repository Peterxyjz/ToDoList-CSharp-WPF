﻿using Microsoft.Toolkit.Uwp.Notifications;
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
using System.IO;
using System.Threading;
using System.ComponentModel;

namespace GUI
{
    public partial class HomeWindow : Window
    {
        private readonly NoteRepository _noteRepository = new NoteRepository(new ToDoListDbContext());
        private readonly NoteService _noteService;
        private Timer _timer;
        private List<Note> _notificationTimes;
        private int Id;
        private bool isShowingNotCompletedNotes = false;

        public static bool isShow = false;

        public HomeWindow()
        {
            InitializeComponent();
            _noteService = new NoteService(_noteRepository);
            DataContext = this;

          
            Closing += (sender, e) =>
            {
                e.Cancel = true;
                isShow = false;
                this.Hide();

            };
        }

        private void ShowNotification(Note note)
        {


            var img = Path.GetFullPath(@"thayiu.ico");
            new ToastContentBuilder()

          .AddAppLogoOverride(new Uri(img))
          .AddText(note.Title)
          .AddText(note.Description)
          .Show();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            WelcomeLabel.Content = $"Hello 👋 What have you planned for today, {ProfileInfo.UserProfile.ProfileName}?";
           
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
            MessageBoxResult result = MessageBox.Show("Are you sure you want to exit?", "Confirm Exit", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
        }

        private void CreateNoteButton_Click(object sender, RoutedEventArgs e)
        {
            DetailWindow detail = new DetailWindow();
            detail.ShowDialog();
            if (detail.DialogResult == true)
                RefreshNotes();
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

            DetailWindow detail = new DetailWindow(note);
            if (note != null)
            {
                detail.ShowDialog();
            }
            if (detail.DialogResult == true)
                RefreshNotes();

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

            InitializeNotificationTimes();
            SetupTimer();
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
            _notificationTimes =  _noteService.GetDateTimesByProfileId(ProfileInfo.UserProfile.ProfileId);
            

        }

        private void SetupTimer()
        {
            //OntimedEvent là làm call back để nói gọi đi gọi lại

            _timer = new Timer(OnTimedEvent, null, 0, 1000); // Kiểm tra mỗi giây
        }

        private void OnTimedEvent(object sender)
        {
            DateTime now = DateTime.Now;

            foreach (var note in _notificationTimes.ToList())
            {
                if (now.Date == note.Time.Date && now.Hour == note.Time.Hour && now.Minute == note.Time.Minute && now.Second == note.Time.Second)
                {
                    Dispatcher.BeginInvoke(() => ShowNotification(note));
                    _notificationTimes.Remove(note); // Xóa thời gian đã thông báo
                }
            }
        }
    }


}
