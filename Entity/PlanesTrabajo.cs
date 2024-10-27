using System;
using System.Collections.Generic;

namespace Entity;

public partial class PlanesTrabajo
{
    public int IdPlan { get; set; }

    public string? Descripcion { get; set; }

    public int? IdFinca { get; set; }

    public DateTime? FechaIni { get; set; }

    public DateTime? FechaFin { get; set; }

    public string? Observaciones { get; set; }

    public decimal? Avance { get; set; }

    public string? Estado { get; set; }

    public virtual ICollection<Actividad> Actividades { get; set; } = new List<Actividad>();

    public virtual MaestroFinca? IdFincaNavigation { get; set; }

    public virtual ICollection<Visita> Visita { get; set; } = new List<Visita>();
}
