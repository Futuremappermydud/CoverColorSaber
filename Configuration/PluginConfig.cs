using System.Runtime.CompilerServices;
using IPA.Config.Stores;

namespace CoverColorSaber
{
    public class PluginConfig
    {
        public static PluginConfig Instance { get; set; }
        public virtual bool enabled { get; set; } = true; 
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
