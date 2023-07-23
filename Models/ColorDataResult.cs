using ColorThief;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoverColorSaber.Models
{
	public class ColorDataResult
	{
		public ColorScheme Scheme;
		public List<QuantizedColor> Colors;

		public ColorDataResult()
		{
		}

		public ColorDataResult(ColorScheme scheme, List<QuantizedColor> colors)
		{
			Scheme = scheme;
			Colors = colors;
		}
	}
}
