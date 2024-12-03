using System;
using System.Collections.Generic;

namespace Entity;

// Esta tabla surgió por necesidad y no se consideró en el diseño original
//  evitar confundor con el detalle de la revision, se hizo para contener
//  las fotos y control de envío de correos.
public partial class Revision
{
    public int IdReg { get; set; }
    public int IdFinca { get; set; }    
    public DateTime? Fecha { get; set; }    
    public string? Observaciones { get; set; }        
    public decimal? Cumplimiento { get; set; }
    public string? Nombrefoto1 { get; set; }
    public string? Nombrefoto2 { get; set; }
    public string? Urlfoto1 { get; set; }
    public string? Urlfoto2 { get; set; }
    public int? SentTo { get; set; }
    public bool? Sincronizado { get; set; }
    public bool? Aplicado { get; set; }
    public string? Tipo { get; set; }
    public virtual MaestroFinca IdFincaNavigation { get; set; } = null!;
}
