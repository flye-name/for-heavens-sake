using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ForHeavensSake;

public static class CommonExtensions
{
	extension(Texture2D texture)
	{
		public Vector2 Size => new Vector2(texture.Width, texture.Height);
	}

	extension(Vector2 vector)
	{
		public float Rotation => (float)Math.Atan2(vector.Y, vector.X);
	}
}