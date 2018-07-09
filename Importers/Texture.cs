using SFML.Graphics;

namespace Tiler.Importers
{
	public class TextureImporter : Importer<Texture>
	{
		public override bool CanLoadExtension(string extension)
		{
			switch (extension)
			{
			case ".png":
			case ".bmp":
			case ".jpg":
			case ".tga":
				return true;
			}

			return false;
		}

		public override Texture Load(string filePath)
		{
			return new Texture(filePath);
		}
	}
}
