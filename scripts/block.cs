using Raylib_cs;
using System.Numerics;

namespace Labirynt_3_Edytor.scripts
{
    internal class Block
    {
        public Vector2 Pos = new(0, 0);
        public readonly int Layer;
        public readonly string? Filter;
        public readonly int Coins;
        public Texture2D Texture;
        public readonly int Index;
        public Block(Image img, Vector2 size, int index, int layer, string filter, int price)
        {
            Coins = price;
            if(BlockSpawn.Coins < price)
            {
                CoinsMenu.BlockIndexes.Add(index);
                return;
            }
            Index = index;
            Layer = layer;
            Filter = filter;
            Raylib.ImageResizeNN(ref img, (int) size.X, (int) size.Y);
            Texture = Raylib.LoadTextureFromImage(img);
        }
    }
}