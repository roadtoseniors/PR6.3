using System;
using System.Collections.Generic;

namespace pr14V2;

public partial class Session
{
    public int Id { get; set; }

    public int MovieId { get; set; }

    public int HallId { get; set; }

    public DateTime? StartDateTime { get; set; }

    public decimal? BaseTickerPrice { get; set; }

    public virtual Hall Hall { get; set; } = null!;

    public virtual Movie Movie { get; set; } = null!;

    public virtual ICollection<SessionSeat> SessionSeats { get; set; } = new List<SessionSeat>();

}
