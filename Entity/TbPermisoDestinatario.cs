using System;
using System.Collections.Generic;

namespace Entity;

public partial class TbPermisoDestinatario
{
    public int IdPermisoDestinatario { get; set; }

    public int IdPermiso { get; set; }

    public int IdDestinatario { get; set; }

    public DateTime? FechaAsignacion { get; set; }

    public bool? Estado { get; set; }

    public virtual TbDestinatario IdDestinatarioNavigation { get; set; } = null!;

    public virtual TbPermiso IdPermisoNavigation { get; set; } = null!;
}
