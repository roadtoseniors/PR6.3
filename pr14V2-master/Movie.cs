using System;
using System.Collections.Generic;

namespace pr14V2;

public partial class Movie
{
    public int Id { get; set; }

    public string? MovieName { get; set; }

    public string? Description { get; set; }

    public double? Rating { get; set; }

    public int? AgeRating { get; set; }

    public DateOnly? ReleaseDate { get; set; }

    public int? Duration { get; set; }

    public string? Image { get; set; }

    public virtual ICollection<MovieGenre> MovieGenres { get; set; } = new List<MovieGenre>();

    public virtual ICollection<Session> Sessions { get; set; } = new List<Session>();
}
