using Microsoft.EntityFrameworkCore;
using Repositories.Entities;
using Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class NoteRepository : INoteRepository
    {
        // ========================
        // == Properties & Fields
        // ========================
        private readonly ToDoListDbContext _dbContext;

        // ========================
        // == Constructors
        // ========================
        public NoteRepository(ToDoListDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // ========================
        // == Methods
        // ========================

        /// <summary>
        /// Delete specific note and save changes
        /// </summary>
        /// <param name="note"></param>
        /// <returns></returns>
        public bool DeleteNote(Note note)
        {
            _dbContext.Remove(note);
            return _dbContext.SaveChanges() > 0;
        }

        /// <summary>
        /// Update the note if existing
        /// </summary>
        /// <param name="note"></param>
        /// <returns></returns>
        public bool UpdateNote(Note note)
        {
            _dbContext.Update(note);
            return _dbContext.SaveChanges() > 0;

        }

        /// <summary>
        /// Add new note into the dbContext
        /// </summary>
        /// <param name="note"></param>
        /// <returns></returns>
        public bool AddNote(Note note)
        {
            _dbContext.Add(note);
            return _dbContext.SaveChanges() > 0;
        }

        /// <summary>
        /// Get specific note by id
        /// </summary>
        /// <param name="id">passed in id</param>
        /// <returns></returns>
        public Note? GetNoteById(int id)
        {
            return _dbContext.Notes.Find(id);
        }

        /// <summary>
        /// Eagle loading all your notes
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Note> GetAllNotes()
        {
            return _dbContext.Notes
                .Include(note => note.Profile)
                .ToList();
        }

        /// <summary>
        /// Get a note by profile id
        /// </summary>
        /// <param name="profileId"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public IEnumerable<Note> GetNotesByProfileId(int profileId)
        {
            return _dbContext.Notes
                .Where(note => note.ProfileId == profileId)
                .ToList();
        }

        /// <summary>
        /// Searching note by title, noteId, description
        /// </summary>
        /// <param name="searchValue">note name</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public IEnumerable<Note> SearchingNotes(string searchValue)
        {
            return _dbContext.Notes
                .Where(note => note.NoteId.ToString() == searchValue
                || note.Title.Contains(searchValue)
                || note.Description.Contains(searchValue)).ToList();
        }

        public IEnumerable<Note> GetNotesByProfileIdAndTime(int profileId, DateTime time)
        {
            return _dbContext.Notes
                .Where(note => note.ProfileId == profileId && note.Time.Date == time.Date)
                .ToList();
        }
    }
}

