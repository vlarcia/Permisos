using System;
using System.Collections.Generic;

namespace Entity;

public partial class MaestroFinca
{
    public int IdFinca { get; set; }

    public int? CodFinca { get; set; }

    public string? Descripcion { get; set; }

    public string? Proveedor { get; set; }

    public string? Email { get; set; }

    public string? Supervisor { get; set; }

    public int? Propiedad { get; set; }

    public string? Empresa { get; set; }

    public string? Zona { get; set; }

    public string? Variedad { get; set; }

    public string? Encargado { get; set; }

    public string? Emailsuper { get; set; }

    public DateTime? DatetimeUpdate { get; set; }

    public string? Tecnico { get; set; }

    public decimal? Area { get; set; }

    public string? Telefono { get; set; }

    public int? Grupo { get; set; }

    public virtual ICollection<Actividad> Actividades { get; set; } = new List<Actividad>();

    public virtual ICollection<DetalleVisita> DetalleVisita { get; set; } = new List<DetalleVisita>();

    public virtual ICollection<PlanesTrabajo> PlanesTrabajos { get; set; } = new List<PlanesTrabajo>();

    public virtual ICollection<Revisione> Revisiones { get; set; } = new List<Revisione>();

    public virtual ICollection<Revision> Revision { get; set; } = new List<Revision>();

    public virtual ICollection<Visita> Visita { get; set; } = new List<Visita>();
}
