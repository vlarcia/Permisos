using System;
using System.Collections.Generic;

namespace Entity;

public partial class TbRolMenu
{
    public int IdRolMenu { get; set; }

    public int? IdRol { get; set; }

    public int? IdMenu { get; set; }

    public bool? EsActivo { get; set; }

    public DateTime? FechaRegistro { get; set; }

    public virtual TbRol? IdRolNavigation { get; set; }
}
