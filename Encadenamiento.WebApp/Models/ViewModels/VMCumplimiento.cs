﻿using Microsoft.Identity.Client;

namespace Permisos.WebApp.Models.ViewModels
{
    public class VMCumplimiento
    {
        public int? CodFinca { get; set; }
        public string?  NombreFinca { get; set; }
        public decimal? Cumplimiento { get; set; }
        public DateTime? FechaUltimarevision { get; set; }

    }
}
