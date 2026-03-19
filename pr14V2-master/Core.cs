using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pr14V2
{
    public class Core
    {
        public static _14prLoobchkinGusenkovContext Context = new _14prLoobchkinGusenkovContext();


        public static User CurrentUser { get; set; }
        public static Session CurrentSession { get; set; }
    }
}
