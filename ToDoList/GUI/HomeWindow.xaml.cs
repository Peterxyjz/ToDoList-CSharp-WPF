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

        public HomeWindow()
        {
            InitializeComponent();
            SelectedDate = DateTime.Today;
            Tab = true;
            DataContext = this;
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

    }


}
