namespace CoverColorSaber.Configuration
{
    public class PluginConfig
    {
        public static PluginConfig Instance { get; set; }
        public virtual bool Enabled { get; set; } = true; 
        public virtual void OnReload()
        {
        }
        public virtual void Changed()
        {
        }
        public virtual void CopyFrom(PluginConfig other)
        {
        }
    }
}
