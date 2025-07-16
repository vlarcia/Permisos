using System;
using System.Collections.Generic;

namespace Entity;

public partial class TbUsuario
{
    public int IdUsuario { get; set; }

    public string? Nombre { get; set; }

    public string? Correo { get; set; }

    public string? Telefono { get; set; }

    public int? IdRol { get; set; }

    public string? UrlFoto { get; set; }

    public string? NombreFoto { get; set; }

    public string? Clave { get; set; }

    public bool? EsActivo { get; set; }

    public DateTime? FechaRegistro { get; set; }
    public int? IdArea { get; set; }

    public virtual TbRol? IdRolNavigation { get; set; }
    public virtual TbArea? IdAreaNavigation { get; set; }
}
