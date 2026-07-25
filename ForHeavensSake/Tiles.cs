using ForHeavensSake.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static ForHeavensSake.ParticleSystem;

namespace ForHeavensSake;

public enum TileType : byte
{
	Inactive,
	Normal,
	SpikeSides,
	
}

public static class Tiles
{
	public const int MaxTilesX = 30;
	public const int MaxTilesY = 12000;
	
	public static byte[,] Grid = new byte[MaxTilesX, MaxTilesY];

	public static void PlaceTile(Vector2 pos, byte type = 1)
	{
		var y = (int)MathF.Floor(FHS.GroundLevel - pos.Y / FHS.TileSize) + 1;
		var x = (int)(pos.X / FHS.TileSize);

		if (x < 0 || y < 0 || x > MaxTilesX || y > MaxTilesY)
			return;
		
		if (Grid[x, y] != type)
			Assets.Sounds.TilePlace.Play(1, new Random(FHS.AmbientTimer).Next(100) / 100f * -0.2f, 0);
		
		Grid[x, y] = type;
	}

	public static void RemoveTile(Vector2 pos)
	{
		var y = (int)MathF.Floor(FHS.GroundLevel - pos.Y / FHS.TileSize) + 1;
		var x = (int)(pos.X / FHS.TileSize);

		if (x < 0 || y < 0 || x > MaxTilesX || y > MaxTilesY)
			return;

		if (Grid[x, y] > 0)
		{
			Assets.Sounds.TileBreak.Play(1, new Random(FHS.AmbientTimer).Next(100) / 100f * -0.2f, 0);

            Particles.Add(new Particle(pos, new Vector2(1, -1) * 5, 100));
            Particles.Add(new Particle(pos, new Vector2(-1, 1) * 5, 100));
            Particles.Add(new Particle(pos, new Vector2(-1, -1) * 5, 100));
            Particles.Add(new Particle(pos, new Vector2(1, 1) * 5, 100));
        }
		
		Grid[x, y] = 0;
    }

	public static void Draw()
	{
		for (int i = 0; i < MaxTilesX; i++)
		{
			for (int j = 0; j < MaxTilesY; j++)
			{
				if (Grid[i, j] > 0)
				{
					FHS.SpriteBatch.Draw(Assets.Textures.Placeholder, new Vector2(i, FHS.GroundLevel - j) * FHS.TileSize - FHS.ScreenPosition, null, Color.White, 0, Vector2.Zero, 2, SpriteEffects.None, 0);
				}
			}
		}
	}
}