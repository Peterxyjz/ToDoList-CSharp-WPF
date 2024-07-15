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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
namespace GUI
{
    public partial class HomeWindow : Window
    {
        private readonly NoteRepository _noteRepository = new NoteRepository(new ToDoListDbContext());
        private readonly NoteService _noteService;
        public HomeWindow()
        {
            InitializeComponent();
            _noteService = new NoteService(_noteRepository);
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshNotes();
        }

        private void CalendarControl_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            DateTime selectedDate = CalendarControl.SelectedDate.GetValueOrDefault();
            NotesDataGrid.ItemsSource = null;
            NotesDataGrid.ItemsSource = _noteService.GetNotesByProfileIdAndTime(LoginedAccount.ProfileId, selectedDate);
        }


        private void AllBtn_Click(object sender, RoutedEventArgs e)
        {
            var notes = _noteService.GetNotesByProfileId(LoginedAccount.ProfileId)
                                     .OrderByDescending(note => note.ModifiedDate)
                                     .ToList();

            NotesDataGrid.ItemsSource = null;
            NotesDataGrid.ItemsSource = notes;
        }

        private void NotCompletedBtn_Click(object sender, RoutedEventArgs e)
        {
            var notes = _noteService.GetNotCompletedNotes(LoginedAccount.ProfileId)
                                     .OrderByDescending(note => note.ModifiedDate)
                                     .ToList();

            NotesDataGrid.ItemsSource = null;
            NotesDataGrid.ItemsSource = notes;
        }
        private void QuitBtn_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void CreateNoteButton_Click(object sender, RoutedEventArgs e)
        {
            DetailWindow detail = new DetailWindow();
            detail.LoginedAccount = LoginedAccount;
            detail.ShowDialog();
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
                if (note.Status == "Completed")
                {
                    note.Status = "Pending";
                }
                else
                {
                    note.Status = "Completed";
                }
                _noteService.UpdateNote(note);
                RefreshNotes();
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
            var notes = _noteService.GetNotesByProfileId(LoginedAccount.ProfileId)
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
    }


}
