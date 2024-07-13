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
    public partial class Home : Window
    {
        private readonly NoteService _noteService = new(new NoteRepository(new ToDoListDbContext()));

        public Home()
        {
            InitializeComponent();
        }

        public Profile LoginedAccount { get; set; } = null;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NotesDataGrid.ItemsSource = null;
            NotesDataGrid.ItemsSource = _noteService.GetAllNotes();
            //NotesDataGrid.ItemsSource = _noteService.GetNotesByProfileId(LoginedAccount.ProfileId);
        }

        

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            NotesDataGrid.ItemsSource = null;
            NotesDataGrid.ItemsSource = _noteService.GetNotCompleteNotes();
            //NotesDataGrid.ItemsSource = _noteService.GetNotCompleteNotes(LoginedAccount.ProfileId);
        }
         
        private void CreateNoteButton_Click(object sender, RoutedEventArgs e)
        {
            Detail detail = new Detail();
            detail.ShowDialog();
            NotesDataGrid.ItemsSource = null;
            NotesDataGrid.ItemsSource = _noteService.GetAllNotes();
        }

        private void QuitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var note = button.DataContext as Note;
            if (note != null)
            {
                Detail detail = new Detail(note);
                detail.ShowDialog();

                NotesDataGrid.ItemsSource = null;
                NotesDataGrid.ItemsSource = _noteService.GetAllNotes();
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
            NotesDataGrid.ItemsSource = _noteService.GetAllNotes();
        }

        

    }
    

}
