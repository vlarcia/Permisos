using Entity;
using System;
using System.Collections.Generic;

namespace Entity;

public partial class TbDestinatario
{
    public int IdDestinatario { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Correo { get; set; }

    public string? TelefonoWhatsapp { get; set; }

    public bool RecibeAlta { get; set; }

    public bool RecibeMedia { get; set; }

    public bool RecibeBaja { get; set; }

    public bool? Activo { get; set; }
    public DateTime FechaRegistro { get; set; }
    public int? IdArea { get; set; }

    public virtual TbArea? IdAreaNavigation { get; set; }

    public virtual ICollection<TbAlerta> TbAlerta { get; set; } = new List<TbAlerta>();

    public virtual ICollection<TbPermisoDestinatario> TbPermisoDestinatarios { get; set; } = new List<TbPermisoDestinatario>();
}
