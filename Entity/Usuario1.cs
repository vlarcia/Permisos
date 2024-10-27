using System;
using System.Collections.Generic;

namespace Entity;

public partial class Usuario1
{
    public int Iduser { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Apellidos { get; set; }

    public string Login { get; set; } = null!;

    public string Passwd { get; set; } = null!;

    public DateTime? Fechaing { get; set; }

    public bool? Activo { get; set; }

    public bool? Admin { get; set; }

    public bool? Ventas { get; set; }

    public bool? Inventarios { get; set; }

    public bool? Cartera { get; set; }

    public bool? Reporte { get; set; }

    public bool? Especial { get; set; }

    public byte[]? Foto { get; set; }

    public int? Codcomedor { get; set; }
}
