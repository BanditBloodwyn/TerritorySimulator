using System;

namespace Core.Configuration
{
    public static class LayerConfiguration
    {
        public static event EventHandler LayersChanged;

        private static bool showEarthTexture;
        private static bool showCloudTexture;

        public static bool ShowEarthTexture
        {
            get => showEarthTexture;
            set
            {
                showEarthTexture = value;
                LayersChanged?.Invoke(null, EventArgs.Empty);
            }
        }

        public static bool ShowCloudTexture
        {
            get => showCloudTexture;
            set
            {
                showCloudTexture = value;
                LayersChanged?.Invoke(null, EventArgs.Empty);
            }
        }
    }
}
