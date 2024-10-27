using System;
using System.Collections.Generic;

namespace Entity;

public partial class Visita
{
    public int IdVisita { get; set; }

    public int IdFinca { get; set; }

    public int IdPlan { get; set; }

    public string? Observaciones { get; set; }

    public int? Zafra { get; set; }

    public DateTime? Fecha { get; set; }

    public string? AndroidId { get; set; }

    public int? SentTo { get; set; }

    public string? Responsable { get; set; }

    public string? Mandador { get; set; }

    public string? Latitud { get; set; }

    public string? Longitud { get; set; }

    public virtual ICollection<DetalleVisita> DetalleVisita { get; set; } = new List<DetalleVisita>();

    public virtual MaestroFinca IdFincaNavigation { get; set; } = null!;

    public virtual PlanesTrabajo IdPlanNavigation { get; set; } = null!;
}
