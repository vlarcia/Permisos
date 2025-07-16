using Entity;
using System;
using System.Collections.Generic;

namespace Entity;

public partial class TbPermiso
{
    public int IdPermiso { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public string? Institucion { get; set; }

    public string Encargado { get; set; } = null!;

    public DateTime FechaVencimiento { get; set; }

    public int DiasGestion { get; set; }

    public string Criticidad { get; set; } = null!;

    public string Tipo { get; set; } = null!;

    public string EstadoPermiso { get; set; } = null!;
    public string? NombreEvidencia { get; set; }
    public string? UrlEvidencia2 { get; set; }
    public string? NombreEvidencia2 { get; set; }
    public string? UrlEvidencia3 { get; set; }
    public string? NombreEvidencia3 { get; set; }
    public string? UrlEvidencia { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime? FechaModificacion { get; set; }
    public int? IdArea { get; set; }

    public virtual TbArea? IdAreaNavigation { get; set; }

    public virtual ICollection<TbAlerta> TbAlerta { get; set; } = new List<TbAlerta>();

    public virtual ICollection<TbPermisoDestinatario> TbPermisoDestinatarios { get; set; } = new List<TbPermisoDestinatario>();
}
