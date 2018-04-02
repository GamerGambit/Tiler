using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiler {
	public static class Utils {
		public static float Clamp(float Value, float Min, float Max) {
			return Math.Max(Math.Min(Value, Max), Min);
		}
	}
}