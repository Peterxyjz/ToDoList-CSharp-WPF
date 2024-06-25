using System;
using System.Collections.Generic;

namespace Repositories.Entities;

public partial class Profile
{
    public int ProfileId { get; set; }

    public string ProfileName { get; set; } = null!;

    public virtual ICollection<Note> Notes { get; set; } = new List<Note>();
}
