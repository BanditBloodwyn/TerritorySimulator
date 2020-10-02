using System;
using System.Runtime.CompilerServices;

namespace Core.Configuration
{
    public static class LayerConfiguration
    {
        public static event EventHandler LayersChanged;

        private static bool showEarthTexture = true;

        public static bool ShowEarthTexture
        {
            get => showEarthTexture;
            set
            {
                showEarthTexture = value;
                LayersChanged?.Invoke(null, EventArgs.Empty);
            }
        } 
    }
}
