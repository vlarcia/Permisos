using System;
using System.Collections.Generic;

namespace Entity;

public partial class TipoInforme
{
    public int IdTipoInforme { get; set; }

    public string? Descripcion { get; set; }

    public bool? EsActivo { get; set; }

    public DateTime? FechaRegistro { get; set; }
}
