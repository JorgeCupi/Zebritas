using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZebrasLib.Classes;

namespace ZebritasWin8
{
    public class bindingCategory
    {
        public Category category { get; set; }
        public List<Place> lstPlaces { get; set; }
    }
    public class staticClasses
    {
        public static Place selectedPlace { get; set; }
        public static Category selectedCategory { get; set; }
        public static Problem selectedEvent { get; set; }
    }
}
