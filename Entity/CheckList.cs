using System;
using System.Collections.Generic;

namespace Entity;

public partial class CheckList
{
    public int IdRequisito { get; set; }

    public string? Descripcion { get; set; }

    public string? Ambito { get; set; }

    public string? Norma { get; set; }

    public string? Bonsucro { get; set; }

    public string? Observaciones { get; set; }

    public virtual ICollection<Revisione> Revisiones { get; set; } = new List<Revisione>();
}
