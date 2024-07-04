using Repositories.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface INoteService
    {
        public IEnumerable<Note> GetAllNote();
        public void CreateNote(Note x);
    }
}
