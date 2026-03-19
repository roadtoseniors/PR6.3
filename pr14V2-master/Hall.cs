using System;
using System.Collections.Generic;

namespace pr14V2;

public partial class Hall
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public double? HallRating { get; set; }

    public int? RowsCount { get; set; }

    public int? SeatsPerRow { get; set; }

    public virtual ICollection<Session> Sessions { get; set; } = new List<Session>();
}
