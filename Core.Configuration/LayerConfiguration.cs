using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Configuration
{
    public static class LayerConfiguration
    {
        private static bool showEarthTexture = true;

        public static bool ShowEarthTexture
        {
            get => showEarthTexture;
            set => showEarthTexture = value;
        }
    }
}
