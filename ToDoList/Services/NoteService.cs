using Microsoft.EntityFrameworkCore;
using Repositories;
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
        public IEnumerable<Note> GetNotes()
        {
            return _noteRepo.GetAllNotes();
        }
        public IEnumerable<Note> GetNotesByProfileId(int profileId)
        {
            return _noteRepo.GetNotesByProfileId(profileId);
        }
        public IEnumerable<Note> GetNotCompletedNotes(int profileId)
        {
            IEnumerable<Note> notes = GetNotesByProfileId(profileId);
            List<Note> notesNotComplete = new List<Note>();
            foreach (Note note in notes)
            {
                if (note.Status == "Pending")
                {
                    notesNotComplete.Add(note);
                }
            }
            return notesNotComplete;
        }

        public IEnumerable<Note> GetNotesByProfileIdAndTime(int profileId, DateTime time)
        {
            IEnumerable<Note> notes = _noteRepo.GetNotesByProfileIdAndTime(profileId, time);
            return notes;
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
