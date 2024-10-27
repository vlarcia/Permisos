using System;
using System.Collections.Generic;

namespace Entity;

public partial class DetalleVisita
{
    public int IdReg { get; set; }

    public int IdVisita { get; set; }

    public int IdActvidad { get; set; }

    public DateTime? Fecha { get; set; }

    public decimal? Avances { get; set; }

    public decimal? Avanceanterior { get; set; }

    public string? Estado { get; set; }

    public string? Estadoanterior { get; set; }

    public string? Comentarios { get; set; }

    public string? Observaciones { get; set; }

    public virtual Actividad IdActvidadNavigation { get; set; } = null!;

    public virtual Visita IdVisitaNavigation { get; set; } = null!;
}
