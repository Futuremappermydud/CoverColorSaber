using IPA.Config.Stores.Attributes;
using IPA.Config.Stores.Converters;
using System.Collections.Generic;
using UnityEngine;

namespace CoverColorSaber.Configuration
{
    public class PluginConfig
    {
        public static PluginConfig Instance { get; set; }
        public virtual bool Enabled { get; set; } = true;
		[UseConverter(typeof(DictionaryConverter<List<ColorCacheParent>, ListConverter<ColorCacheParent>>))]
        public virtual Dictionary<string, List<ColorCacheParent>> SchemeCache { get; set; } = new Dictionary<string, List<ColorCacheParent>>();
		[UseConverter(typeof(DictionaryConverter<List<ColorSerialized>, ListConverter<ColorSerialized>>))]
		public virtual Dictionary<string, List<ColorSerialized>> PaletteCache { get; set; } = new Dictionary<string, List<ColorSerialized>>();

	}

    public class ColorSerialized
    {
		public ColorSerialized()
		{
		}

		public ColorSerialized(Color c)
		{
			this.r = c.r;
			this.g = c.g;
			this.b = c.b;
		}

		public virtual float r { get; set; }
		public virtual float g { get; set; }
		public virtual float b { get; set; }
	}

	public class ColorCacheParent
	{
		public ColorCacheParent()
		{
		}

		public ColorCacheParent(string type, ColorSerialized color)
		{
			this.type = type;
			this.color = color;
		}

		public virtual string type { get; set; }
		public virtual ColorSerialized color { get; set; }
	}
}
