using Microsoft.EntityFrameworkCore;
using Repositories.Entities;
using Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class NoteService
    {
        private readonly INoteRepository _noteRepo;
        public NoteService(INoteRepository noteRepo) {  _noteRepo = noteRepo; }

        public NoteService()
        {
        }

        public IEnumerable<Note> GetAllNotes()
        {
            return _noteRepo.GetAllNotes();
        }

        public void CreateNote(Note x)
        {
            _noteRepo.AddNote(x);   
        }

        Note a = new Note { Title = "aa", Description = "fcihsdvcbi", Time = DateTime.Now };
        public IEnumerable<Note> GetNotCompleteNotes()
        {
            //IEnumerable<Note> notes = _noteRepo.GetAllNotes();
            List<Note> notesNotComplete = new List<Note>();
            //foreach (Note note in notes)
            //{
            //    if (note.Status == "Pending")
            //    {
            //        notesNotComplete.Add(note);
            //    }
            //}
            notesNotComplete.Add(a);
            IEnumerable<Note> notesUpdated = notesNotComplete;
            return notesUpdated;
        }

        public void AddNote(Note note)
        {
            _noteRepo.AddNote(note);
        }
        public void UpdateNote(Note note)
        {
            _noteRepo.UpdateNote(note);
        }

        public void DeleteNoteById(int id)
        {
            Note deleteNote = _noteRepo.GetNoteById(id);
            _noteRepo.DeleteNote(deleteNote);
        }
    }
}
