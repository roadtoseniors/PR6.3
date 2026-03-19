using System;
using System.Collections.Generic;

namespace pr14V2;

public partial class MovieGenre
{
    public int MovieId { get; set; }

    public int GenreId { get; set; }

    public int Id { get; set; }

    public virtual Genre Genre { get; set; } = null!;

    public virtual Movie Movie { get; set; } = null!;
}
