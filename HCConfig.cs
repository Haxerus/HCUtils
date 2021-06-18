using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace HCUtils
{
    class HCConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;

        [Label("Hide Death Messages and Markers")]
        [DefaultValue(true)]
        public bool hideDeaths { get; set; }

        [Label("Hide Offscreen Player Names")]
        [DefaultValue(true)]
        public bool hidePlayerNames { get; set; }
    }
}
