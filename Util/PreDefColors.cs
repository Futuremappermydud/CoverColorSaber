using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CoverColorSaber.Util
{
    public static class PreDefColors
    {
        public static Dictionary<string, Color> colors = new Dictionary<string, Color>
        {
            { "black", new Color(0, 0, 0) },
            { "white", new Color(255, 255, 255) }
        };
    }
}
