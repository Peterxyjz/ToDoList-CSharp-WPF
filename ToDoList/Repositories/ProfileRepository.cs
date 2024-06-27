using Repositories.Entities;
using Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class ProfileRepository : IProfileRepository
    {
        // ========================
        // == Properties & Fields
        // ========================
        private readonly ToDoListDbContext _dbContext;

        // ========================
        // == Constructors
        // ========================

        public ProfileRepository(ToDoListDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // ========================
        // == Methods
        // ========================

        /// <summary>
        /// Add new profile into the table
        /// </summary>
        /// <param name="profile"></param>
        /// <returns></returns>
        public bool AddProfile(Profile profile)
        {
            _dbContext.Add(profile);
            return _dbContext.SaveChanges() > 0;
        }

        /// <summary>
        /// Delete a profile from the database
        /// </summary>
        /// <param name="profile"></param>
        /// <returns></returns>
        public bool DeleteProfile(Profile profile)
        {
            _dbContext.Remove(profile);
            return _dbContext.SaveChanges() > 0;
        }

        /// <summary>
        /// Get specific profile by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Profile? GetProfileById(int id)
        {
            return _dbContext.Profiles.Find(id);
        }

        /// <summary>
        /// Update the specific profile
        /// </summary>
        /// <param name="profile"></param>
        /// <returns></returns>
        public bool UpdateProfile(Profile profile)
        {
            _dbContext.Update(profile);
            return _dbContext.SaveChanges() > 0;
        }

        /// <summary>
        /// Get all available profiles
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Profile> GetAllProfiles()
        {
            return _dbContext.Profiles.ToList();
        }

        /// <summary>
        /// Searching availabel profiles by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IEnumerable<Profile> GetProfilesByName(string name)
        {
            return _dbContext.Profiles
                .Where(p => p.ProfileName.ToString().Equals(name)).ToList();
        }
    }
}
