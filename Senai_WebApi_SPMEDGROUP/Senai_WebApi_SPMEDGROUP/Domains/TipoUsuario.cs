﻿using System;
using System.Collections.Generic;

namespace Senai_WebApi_SPMEDGROUP.Domains
{
    public partial class TipoUsuario
    {
        public TipoUsuario()
        {
            Usuario = new HashSet<ViewModels>();
        }

        public int Id { get; set; }
        public string Tipo { get; set; }

        public ICollection<ViewModels> Usuario { get; set; }
    }
}
