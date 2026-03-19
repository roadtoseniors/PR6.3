using System;
using System.Collections.Generic;

namespace pr14V2;

public partial class Ticket
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int SeatId { get; set; }

    public DateTime? PurchaseDateTime { get; set; }

    public decimal? FinalPrice { get; set; }

    public string? Status { get; set; }

    public virtual Seat Seat { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
