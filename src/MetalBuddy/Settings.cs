using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using ff14bot.Helpers;


namespace MetalBuddy
{
    public class Settings
    {
        public Settings()
        {
            LoadSettings();
        }

        public void LoadSettings()
        {
            EnableOverlay = false;
            ShowAetherCurrents = false;
            OnlyForeground = false;
            ShowTargetablePlayers = false;
            OverlayXPos = 0;
            OverlayYPos = 0;
            EnableStats = false;
            AlertObject = "";
            AlertXPos = 150;
            AlertYPos = 120;
        }
        public bool EnableOverlay { get; set; }

        public bool ShowAetherCurrents { get; set; }

        public bool OnlyForeground { get; set; }

        public int OverlayXPos { get; set; }

        public int OverlayYPos { get; set; }

        public bool EnableStats { get; set; }

        public String AlertObject { get; set; }

        public int AlertXPos { get; set; }

        public int AlertYPos { get; set; }

        public bool ShowTargetablePlayers { get; set; }
    }
}
