﻿using Microsoft.EntityFrameworkCore;
using Repositories.Entities;
using Repositories.Interfaces;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class NoteService : INoteService
    {
        private readonly INoteRepository _noteRepo;
        public NoteService(INoteRepository noteRepo) {  _noteRepo = noteRepo; }

        public IEnumerable<Note> GetAllNotes()
        {
            return _noteRepo.GetAllNotes();
        }

        public void CreateNote(Note x)
        {
            _noteRepo.AddNote(x);   
        }

        public IEnumerable<Note> GetNotCompleteNotes()
        {
            IEnumerable<Note> notes = _noteRepo.GetAllNotes();
            List<Note> notesNotComplete = new List<Note>();
            foreach (Note note in notes)
            {
                if(note.Status == "Pending")
                {
                    notesNotComplete.Add(note);
                }
            }
            IEnumerable<Note> notesUpdated = notesNotComplete;
            return notesUpdated;
        }
    }
}