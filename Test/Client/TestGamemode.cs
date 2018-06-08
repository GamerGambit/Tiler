using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
	class TestGamemode : Tiler.Gamemode
	{
		public override bool IsTeamBased { get; protected set; } = true;
	}
}
