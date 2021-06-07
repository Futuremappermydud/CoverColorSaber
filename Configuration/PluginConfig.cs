namespace CoverColorSaber.Configuration
{
    public class PluginConfig
    {
        public static PluginConfig Instance { get; set; }
        public virtual bool Enabled { get; set; } = true; 
    }
}
