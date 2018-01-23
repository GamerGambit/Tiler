using System;
using System.Collections.Generic;

using SFML.Graphics;
using SFML.System;
using TiledSharp;

namespace Tiler
{
    public class MapLayer : Drawable
    {
        private void Rebuild()
        {
            Textures = new List<Tuple<VertexArray, RenderStates>>();

            foreach (var tile in Tiles)
            {
                var tileset = TileSet.GetTileSetForTile(tile);
                var entry = Textures.Find(t => t.Item2.Texture == tileset.Texture);

                if (entry == null)
                {
                    entry = new Tuple<VertexArray, RenderStates>(new VertexArray(PrimitiveType.Quads), new RenderStates(BlendMode.Alpha, Transform.Identity, tileset.Texture, null));
                    Textures.Add(entry);
                }

                var spriteIndex = tile.Gid - tileset.FirstGID;
                var mapPos = new Vector2f(tile.X * tileset.TileWidth, tile.Y * tileset.TileHeight);
                var texCoords = new Vector2f((spriteIndex % tileset.Columns) * tileset.TileWidth, (spriteIndex / tileset.Columns) * tileset.TileHeight);

                var vertexArray = entry.Item1;

                // Top Left
                vertexArray.Append(new Vertex()
                {
                    Color = Color.White,
                    Position = mapPos,
                    TexCoords = texCoords
                });

                // Top Right
                vertexArray.Append(new Vertex()
                {
                    Color = Color.White,
                    Position = mapPos + new Vector2f(tileset.TileWidth, 0),
                    TexCoords = texCoords + new Vector2f(tileset.TileWidth, 0)
                });

                // Bottom Right
                vertexArray.Append(new Vertex()
                {
                    Color = Color.White,
                    Position = mapPos + new Vector2f(tileset.TileWidth, tileset.TileHeight),
                    TexCoords = texCoords + new Vector2f(tileset.TileWidth, tileset.TileHeight)
                });

                // Bottom Left
                vertexArray.Append(new Vertex()
                {
                    Color = Color.White,
                    Position = mapPos + new Vector2f(0, tileset.TileHeight),
                    TexCoords = texCoords + new Vector2f(0, tileset.TileHeight)
                });
            }
        }

        public List<Tuple<VertexArray, RenderStates>> Textures { get; private set; }
        public List<TmxLayerTile> Tiles { get; private set; } = new List<TmxLayerTile>();

        public void AddTile(TmxLayerTile tile)
        {
            Tiles.Add(tile);
            Rebuild();
        }

        public void RemoveTile(TmxLayerTile tile)
        {
            Tiles.Remove(tile);
            Rebuild();
        }

        public void RemoveTileByPosition(Vector2f pos)
        {
            var tile = Tiles.Find(t => t.X == pos.X && t.Y == pos.Y);

            if (tile == null)
                return;

            Tiles.Remove(tile);
            Rebuild();
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            foreach (var tex in Textures)
            {
                target.Draw(tex.Item1, tex.Item2);
            }
        }
    }
}
