using Repositories.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    public interface IProfileRepository
    {
        bool DeleteProfile(Profile profile);
        bool UpdateProfile(Profile profile);
        bool AddProfile(Profile profile);
        Profile? GetProfileById(int id);
        IEnumerable<Profile> GetAllProfiles();
        IEnumerable<Profile> GetProfilesByName(string name);
    }
}
