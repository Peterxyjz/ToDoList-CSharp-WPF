using Repositories.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    public interface INoteRepository
    {
        bool DeleteNote(Note note);
        bool UpdateNote(Note note);
        bool AddNote(Note note);
        Note GetNoteById(int id);
        IEnumerable<Note> GetAllNotes();
        IEnumerable<Note> GetNotesByProfileId(int profileId);
        IEnumerable<Note> SearchingNotes(string searchValue);
        //IEnumerable<Note> GetNotesByStatus(string status);
    }
}
