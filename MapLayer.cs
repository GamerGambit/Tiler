using System;
using System.Collections.Generic;

using SFML.Graphics;
using SFML.System;
using TiledSharp;

namespace Tiler
{
    public class MapLayer : Drawable
    {
        private bool dirty = true;
        private List<Tuple<VertexArray, RenderStates>> Meshes = new List<Tuple<VertexArray, RenderStates>>();
        private List<TmxLayerTile> Tiles = new List<TmxLayerTile>();

        private void Rebuild()
        {
            Meshes.Clear();

            foreach (var tile in Tiles)
            {
                var tileset = TileSet.GetTileSetForTile(tile);
                var entry = Meshes.Find(t => t.Item2.Texture == tileset.Texture);

                if (entry == null)
                {
                    entry = new Tuple<VertexArray, RenderStates>(new VertexArray(PrimitiveType.Quads), new RenderStates(BlendMode.Alpha, Transform.Identity, tileset.Texture, null));
                    Meshes.Add(entry);
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

        public void AddTile(TmxLayerTile tile)
        {
            Tiles.Add(tile);
            dirty = true;
        }

        public void RemoveTile(TmxLayerTile tile)
        {
            Tiles.Remove(tile);
            dirty = true;
        }

        public void RemoveTileByPosition(Vector2f pos)
        {
            var tile = Tiles.Find(t => t.X == pos.X && t.Y == pos.Y);

            if (tile == null)
                return;

            Tiles.Remove(tile);
            dirty = true;
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            if (dirty)
            {
                Rebuild();
                dirty = false;
            }

            foreach (var tex in Meshes)
            {
                target.Draw(tex.Item1, tex.Item2);
            }
        }
    }
}
