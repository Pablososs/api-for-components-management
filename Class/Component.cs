using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace DAPI.Class
{
    public class Component
    {
        public int ID { get; set; }
        public string? Device { get; set; }
        public string? Task { get; set; }
        public DateTime DateEntry { get; set; }
        public string? ProductItem { get; set; }
        public string? Note { get; set; }
        public string? PathImage { get; set; }

    }
}
