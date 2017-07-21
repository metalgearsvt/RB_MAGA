using ff14bot.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MetalBuddy.db
{
    class LocationManager
    {
        private string Name1 = "Name1";
        private string Name2 = "Name2";

        public List<Location> l = new List<Location>();
        JObject o;

        public LocationManager()
        {
            o = JObject.Parse(ZoneID.json);
        }

        public string GetLocationString(int ZoneID)
        {
            String name1 = o.GetValue(ZoneID.ToString()).ToObject<JObject>().GetValue(Name1).ToObject<String>();
            String name2 = o.GetValue(ZoneID.ToString()).ToObject<JObject>().GetValue(Name2).ToObject<String>();
            return $"{name1} - {name2}";
        }
    }
}
