using System;

namespace Tiler
{
	public interface IUpdatable
	{
		void Update(TimeSpan deltaTime);
	}
}
