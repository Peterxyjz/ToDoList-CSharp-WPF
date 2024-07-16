using Repositories;
using Repositories.Entities;
using Services;
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

namespace GUI
{
    public partial class DetailWindow : Window
    {
        private readonly NoteRepository _noteRepository = new NoteRepository(new ToDoListDbContext());
        private readonly NoteService _noteService;
        private Note _noteToUpdate;
        public string NoteDetail { get; set; } = null;
        public Note Note { get; set; } = null;
        public DetailWindow()
        {
            InitializeComponent();
            _noteService = new NoteService(_noteRepository);
        }
        public DetailWindow(Note note) : this()
        {
            _noteToUpdate = note;
            ModifiedLable.Content = "Last Modifide: " + _noteToUpdate.ModifiedDate;
            TitleTextBox.Text = _noteToUpdate.Title;
            DescriptionTextBox.Text = _noteToUpdate.Description;
            ReminderDateTimePicker.Value = _noteToUpdate.Time;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TitleTextBox.Text) ||
                string.IsNullOrWhiteSpace(DescriptionTextBox.Text) ||
                (_noteToUpdate == null && ReminderDateTimePicker.Value == null))
            {
                MessageBox.Show("Please enter a title, description, and reminder date/time.", "Incomplete Information", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }


            if (_noteToUpdate != null && _noteToUpdate.ModifiedDate == null)
            {
                MessageBox.Show("Modified Date cannot be empty.", "Incomplete Information", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_noteToUpdate != null)
            {
                _noteToUpdate.Title = TitleTextBox.Text;
                _noteToUpdate.Description = DescriptionTextBox.Text;
                _noteToUpdate.ModifiedDate = DateTime.Now;
                _noteToUpdate.Time = (DateTime)ReminderDateTimePicker.Value;
                _noteService.UpdateNote(_noteToUpdate);
            }
            else
            {
                Note newNote = new Note();
                //newNote.NoteId = GenerateNewId();

                newNote.Title = TitleTextBox.Text;
                newNote.ProfileId = ProfileInfo.UserProfile.ProfileId;
                newNote.Description = DescriptionTextBox.Text;
                newNote.ModifiedDate = DateTime.Now;
                newNote.Status = "Pending";
                newNote.Time = (DateTime)ReminderDateTimePicker.Value;

                _noteService.AddNote(newNote);
            }

            this.Close();
            var mainWindow = Application.Current.Windows.OfType<HomeWindow>().FirstOrDefault();
            if (mainWindow != null)
            {
                mainWindow.RefreshNotes();
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private int currentId = 0;
        public int GenerateNewId()
        {
            int newId;
            List<Note> allNote = (List<Note>)_noteService.GetNotes();
            List<int> existedId = new List<int>();
            foreach (Note note in allNote)
            {
                existedId.Add(note.NoteId);
            }

            do
            {
                newId = ++currentId;
            } while (existedId.Contains(newId));

            existedId.Add(newId);

            return newId;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            if (NoteDetail == "true")
            {
                _noteToUpdate = Note;
                ModifiedLable.Content = "Last Modifide: " + Note.ModifiedDate;
                TitleTextBox.Text = _noteToUpdate.Title;
                DescriptionTextBox.Text = _noteToUpdate.Description;
                ReminderDateTimePicker.Value = _noteToUpdate.Time;
                TitleTextBox.IsEnabled = false;
                DescriptionTextBox.IsEnabled = false;
                ReminderDateTimePicker.IsEnabled = false;
                SaveButton.IsEnabled = false;
            }
        }
    }
}
