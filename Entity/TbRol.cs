using System;
using System.Collections.Generic;

namespace Entity;

public partial class TbRol
{
    public int IdRol { get; set; }

    public string? Descripcion { get; set; }

    public bool? EsActivo { get; set; }

    public DateTime? FechaRegistro { get; set; }

    public virtual ICollection<TbRolMenu> TbRolMenus { get; set; } = new List<TbRolMenu>();

    public virtual ICollection<TbUsuario> TbUsuarios { get; set; } = new List<TbUsuario>();
}
