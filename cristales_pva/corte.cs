//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace cristales_pva
{
    using System;
    using System.Collections.Generic;
    
    public partial class corte
    {
        public int id { get; set; }
        public string clave { get; set; }
        public string articulo { get; set; }
        public Nullable<int> modulo_id { get; set; }
        public string partida { get; set; }
        public Nullable<double> longitud_corte { get; set; }
        public Nullable<double> tramo_perfil { get; set; }
        public string acabado { get; set; }
        public Nullable<double> cantidad { get; set; }
    }
}