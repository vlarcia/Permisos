﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Interfaces
{
    public interface IUtilidadesService
    {
        string GenerarClave();

        string ConvertirSha256(string texto);
    }
}
