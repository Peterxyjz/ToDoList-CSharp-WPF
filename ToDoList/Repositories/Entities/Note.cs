using System;
using System.Collections.Generic;

namespace Repositories.Entities;

public partial class Note
{
    public int NoteId { get; set; }

    public int ProfileId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime ModifiedDate { get; set; }

    public string? Status { get; set; }

    public DateTime Time { get; set; }

    public virtual Profile Profile { get; set; } = null!;
}
