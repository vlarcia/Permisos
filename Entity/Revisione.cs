using System;
using System.Collections.Generic;

namespace Entity;

public partial class Revisione
{
    public int? IdRevision { get; set; }

    public int? IdFinca { get; set; }

    public int? IdRequisito { get; set; }

    public DateTime? Fecha { get; set; }

    public string? Estado { get; set; } = null!;

    public string? Observaciones { get; set; }

    public string? Comentarios { get; set; }

    public string? Tipo { get; set; }

    public decimal? Cumplimiento { get; set; }

    public virtual MaestroFinca IdFincaNavigation { get; set; } = null!;

    public virtual CheckList IdRequisitoNavigation { get; set; } = null!;
}
