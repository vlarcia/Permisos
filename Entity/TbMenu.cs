using System;
using System.Collections.Generic;

namespace Entity;

public partial class TbMenu
{
    public int IdMenu { get; set; }

    public string? Descripcion { get; set; }

    public int? IdMenuPadre { get; set; }

    public string? Icono { get; set; }

    public string? Controlador { get; set; }

    public string? PaginaAccion { get; set; }

    public bool? EsActivo { get; set; }

    public DateTime? FechaRegistro { get; set; }

    public virtual TbMenu? IdMenuPadreNavigation { get; set; }

    public virtual ICollection<TbMenu> InverseIdMenuPadreNavigation { get; set; } = new List<TbMenu>();
}
