using System;
using System.Collections.Generic;

namespace pr14V2;

public partial class Seat
{
    public int Id { get; set; }

    public int SessionId { get; set; }

    public int? RowNumber { get; set; }

    public int? SeatNumber { get; set; }

    public virtual ICollection<SessionSeat> SessionSeats { get; set; } = new List<SessionSeat>();

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
