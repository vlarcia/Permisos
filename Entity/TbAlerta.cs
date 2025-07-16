using System;
using System.Collections.Generic;

namespace Entity;

public partial class TbAlerta
{
    public int IdAlerta { get; set; }

    public int IdPermiso { get; set; }

    public int IdDestinatario { get; set; }

    public DateTime FechaEnvio { get; set; }

    public string MedioEnvio { get; set; } = null!;

    public string? Resultado { get; set; }

    public string? Mensaje { get; set; }

    public virtual TbDestinatario IdDestinatarioNavigation { get; set; } = null!;

    public virtual TbPermiso IdPermisoNavigation { get; set; } = null!;
}
