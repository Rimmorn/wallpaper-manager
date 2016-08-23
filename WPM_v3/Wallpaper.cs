using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.IO;

namespace WPM_v3
{
    class Wallpaper
    {

        public string Wallpaper_filename { get; set; }
        public string Tags { get; set; }
        /*public string Tag2 { get; set; }
        public string Tag3 { get; set; }
        public string Tag4 { get; set; }
        public string Tag5 { get; set; }*/


        public string ToCSV()
        {
            return String.Format("{0};{1};",//{2};{3};{4};{5};{6}",
                                  Wallpaper_filename, Tags); //, Tag2, Tag3, Tag4, Tag5);
        }

        public static Wallpaper FromCSV(string txt)
        {
            Wallpaper o = new Wallpaper();

            string[] blocks = txt.Split(';');
            o.Wallpaper_filename = String.IsNullOrEmpty(blocks[0]) ? "" : blocks[0];
            o.Tags = String.IsNullOrEmpty(blocks[1]) ? "" : blocks[1];

            return o;
        }
    }
}
