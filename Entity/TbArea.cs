using System;
using System.Collections.Generic;

namespace Entity;

public partial class TbArea
{
    public int IdArea { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public bool? Activo { get; set; }

    public DateTime? FechaRegistro { get; set; }

    public virtual ICollection<TbDestinatario> TbDestinatarios { get; set; } = new List<TbDestinatario>();

    public virtual ICollection<TbPermiso> TbPermisos { get; set; } = new List<TbPermiso>();

    public virtual ICollection<TbUsuario> TbUsuarios { get; set; } = new List<TbUsuario>();
}
