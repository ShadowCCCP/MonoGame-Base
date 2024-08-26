using AsepriteDotNet.Aseprite;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Aseprite;
using MonoGame_Base.Project.Utility.Basics;
using MonoGame_Base.Project.Utility.Basics.Colliders;
using MonoGame_Base.Project.Utility.Basics.Colliders.Shapes;
using System.Collections.Generic;

namespace MonoGame_Base.Project.Utility.TileMap
{
    public class Map
    {
        public Vector2 scale { get; private set; }

        private Vector2 _mapSize;
        private Vector2 _tileSize;

        private Tilemap _tilemap;

        private List<BoxCollider> mapColliders = new List<BoxCollider>();

        // From this, we get the region info by firstly giving the layerId, then the tileId of that layer...
        private Dictionary<int, Dictionary<int, List<Rectangle>>> colliderTileInfo = new Dictionary<int, Dictionary<int, List<Rectangle>>>();

        public Map(string pMapName)
        {
            scale = new Vector2(4.0f, 4.0f);
            AsepriteFile aseFile = Globals.Content.Load<AsepriteFile>($"Maps/{pMapName}");

            _tilemap = aseFile.CreateTilemap(Globals.GraphicsDevice, frameIndex: 0);

            // Get all the important info about the tilemap...
            TilemapLayer layer = _tilemap.GetLayer(0);
            _mapSize = new Vector2(layer.Width, layer.Height);
            _tileSize = new Vector2(layer.Width / layer.Columns, layer.Height / layer.Rows);
             
            for (int i = 0; i < _tilemap.LayerCount; i++)
            {

                if (_tilemap[i].IsVisible && _tilemap[i].Name.Contains("(Collider)"))
                {
                    SetMapColliders(_tilemap[i], scale);
                }
                else if (_tilemap[i].IsVisible && _tilemap[i].Name.Contains("(PerfCollider)"))
                {
                    colliderTileInfo.Add(i, new Dictionary<int, List<Rectangle>>());

                    SetMapCollidersPerf(i, _tilemap[i], scale);
                }
            }
        }

        private Texture2D GetTextureOfRegion(TextureRegion pRegion)
        {
            // Create a new texture to hold the extracted portion...
            Texture2D texture = new Texture2D(Globals.GraphicsDevice, pRegion.Bounds.Width, pRegion.Bounds.Height);
            // Create a color array to hold the texture data...
            Color[] data = new Color[pRegion.Bounds.Width * pRegion.Bounds.Height];
            // Get the color data of the specified region from the source texture...
            pRegion.Texture.GetData(0, pRegion.Bounds, data, 0, data.Length);

            texture.SetData(data);
            return texture;
        }

        public void ActivateColliders()
        {
            foreach (Collider collider in mapColliders)
            {
                collider.Activate();
            }
        }

        public void DeactivateColliders()
        {
            foreach (Collider collider in mapColliders)
            {
                collider.Deactivate();
            }
        }

        public void Draw()
        {
            int currentLayer = 0;
            // This fixes layering issues when drawing sprites back to forth...
            float layerDepth = 0;

            foreach (TilemapLayer item in _tilemap)
            {
                Vector2 position = Vector2.Zero + item.Offset * scale;
                // Check if layering is needed or not...
                if (item.IsVisible && (item.Name.Contains("(Collider)") || item.Name.Contains("(PerfCollider)")))
                {
                    DrawTiles(item, position, Color.White, scale, layerDepth, currentLayer);
                }
                else if (item.IsVisible)
                {
                    Globals.SpriteBatch.Draw(item, position, Color.White, scale, layerDepth);
                    layerDepth += 0.0001f;
                }

                currentLayer++;
            }
        }

        private void SetMapColliders(TilemapLayer layer, Vector2 scale)
        {
            Vector2 position = new Vector2();
            for (int i = 0; i < layer.Columns; i++)
            {
                for (int j = 0; j < layer.Rows; j++)
                {
                    Tile tile = layer.GetTile(i, j);

                    if (!tile.IsEmpty)
                    {
                        position.X = (float)(i * layer.Tileset.TileWidth) * scale.X;
                        position.Y = (float)(j * layer.Tileset.TileHeight) * scale.Y;

                        Vector2 offset = new Vector2(layer.OffsetX * scale.X, layer.OffsetY * scale.Y);

                        BoxCollider col = new BoxCollider(position + offset, _tileSize * scale, null);
                        mapColliders.Add(col);
                    }
                }
            }
        }

        private void SetMapCollidersPerf(int layerId, TilemapLayer layer, Vector2 scale)
        {
            Vector2 position = new Vector2();
            for (int i = 0; i < layer.Columns; i++)
            {
                for (int j = 0; j < layer.Rows; j++)
                {
                    Tile tile = layer.GetTile(i, j);

                    if (!tile.IsEmpty)
                    {
                        int tileID = tile.TilesetTileID;

                        if (!colliderTileInfo[layerId].ContainsKey(tileID))
                        {
                            Tileset tileset = layer.Tileset;
                            TextureRegion textureRegion = tileset.GetTile(tileID);

                            Texture2D texture = GetTextureOfRegion(textureRegion);

                            List<Rectangle> filledRegions = Basics.Sprites.Sprite.GetMergedBoundingBoxes(texture);
                            colliderTileInfo[layerId].Add(tileID, filledRegions);
                        }

                        foreach (Rectangle region in colliderTileInfo[layerId][tileID])
                        {
                            position.X = (float)(i * layer.Tileset.TileWidth) * scale.X + region.X * scale.X;
                            position.Y = (float)(j * layer.Tileset.TileHeight) * scale.Y + region.Y * scale.Y;

                            Vector2 offset = new Vector2(layer.OffsetX * scale.X, layer.OffsetY * scale.Y);

                            BoxCollider col = new BoxCollider(position + offset, new Vector2(region.Width * scale.X, region.Height * scale.Y), null);
                            mapColliders.Add(col);
                        }
                    }
                }
            }
        }

        public Vector2 GetMapSize()
        {
            return _mapSize * scale;
        }

        public Vector2 GetTileSize()
        {
            return _tileSize * scale;
        }

        public Vector2 GetGroundOffset()
        {
            return _tilemap.GetLayer(1).Offset * scale;
        }

        // The methods below are the same as the spriteBatch draw calls...
        // My addition: Clamping layerDepth and adding colliders to the collision layer...
        private void DrawTiles(TilemapLayer pLayer, Vector2 pPosition, Color pColor, Vector2 pScale, float pLayerDepth, int pCurrentLayer)
        {
            Vector2 position2 = pPosition;
            for (int i = 0; i < pLayer.Columns; i++)
            {
                for (int j = 0; j < pLayer.Rows; j++)
                {
                    Tile tile = pLayer.GetTile(i, j);
                    if (!tile.IsEmpty)
                    {
                        position2.X = pPosition.X + (float)(i * pLayer.Tileset.TileWidth) * pScale.X;
                        position2.Y = pPosition.Y + (float)(j * pLayer.Tileset.TileHeight) * pScale.Y;

                        float layerDepth = MathHelper.Clamp(position2.Y / GetMapSize().Y, pLayerDepth, 1);

                        Color color2 = pColor * pLayer.Transparency;
                        SpriteEffects effects = DetermineFlipEffectForTile(tile.FlipHorizontally, tile.FlipVertically, tile.FlipDiagonally);
                        float rotation = (tile.FlipDiagonally ? MathHelper.ToRadians(90f) : 0f);
                        TextureRegion region = pLayer.Tileset[tile.TilesetTileID];
                        Globals.SpriteBatch.Draw(region, position2, color2, rotation, Vector2.Zero, pScale, effects, layerDepth);
                    }
                }
            }
        }

        private static SpriteEffects DetermineFlipEffectForTile(bool flipHorizontally, bool flipVertically, bool flipDiagonally)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (!flipDiagonally)
            {
                if (flipHorizontally)
                {
                    spriteEffects |= SpriteEffects.FlipHorizontally;
                }

                if (flipVertically)
                {
                    spriteEffects |= SpriteEffects.FlipVertically;
                }
            }
            else
            {
                SpriteEffects spriteEffects2 = (flipHorizontally ? (flipVertically ? SpriteEffects.FlipHorizontally : SpriteEffects.None) : ((!flipVertically) ? SpriteEffects.FlipVertically : (SpriteEffects.FlipHorizontally | SpriteEffects.FlipVertically)));
                spriteEffects = spriteEffects2;
            }

            return spriteEffects;
        }
    }
}