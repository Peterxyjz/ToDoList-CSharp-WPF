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
        private readonly NoteService _noteService = new(new NoteRepository(new ToDoListDbContext()));

        public HomeWindow()
        {
            InitializeComponent();
        }

        public Profile LoginedAccount { get; set; } = null;


        private void AllBtn_Click(object sender, RoutedEventArgs e)
        {
            NotesDataGrid.ItemsSource = null;
            NotesDataGrid.ItemsSource = _noteService.GetNotesByProfileId(LoginedAccount.ProfileId);
            //var allNotes = _noteService.GetAllNotes();
            //NotesDataGrid.ItemsSource = allNotes;

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
        private void NotCompleteBtn_Click(object sender, RoutedEventArgs e)
        {
            //NotesDataGrid.ItemsSource = null;
            //NotesDataGrid.ItemsSource = _noteService.GetNotCompleteNotes();
            //NotesDataGrid.ItemsSource = _noteService.GetNotCompleteNotes(LoginedAccount.ProfileId);
            IEnumerable<Note> notes = _noteService.GetNotesByProfileId(LoginedAccount.ProfileId);
            List<Note> notesPending = new List<Note>();

            foreach (Note note in notes)
            {
                if (note.Status == "Pending")
                {
                    notesPending.Add(note);
                }
            }
            NotesDataGrid.ItemsSource = null;
            NotesDataGrid.ItemsSource = notesPending;
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
            RefreshNotes();
        }


        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var note = button.DataContext as Note;
            if (note != null)
            {
                DetailWindow detail = new DetailWindow(note);
                detail.ShowDialog();

                RefreshNotes();
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

        private void DetailButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var note = button.DataContext as Note;
            if (note != null)
            {
                DetailWindow detail = new DetailWindow();
                detail.Note = note;
                detail.NoteDetail = "true";
                detail.ShowDialog();

                RefreshNotes();
            }
        }
        public void RefreshNotes()
        {
            NotesDataGrid.ItemsSource = null;
            NotesDataGrid.ItemsSource = _noteService.GetNotesByProfileId(LoginedAccount.ProfileId);
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
