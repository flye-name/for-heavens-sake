using TowerToHeaven.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static TowerToHeaven.ParticleSystem;

namespace TowerToHeaven;

public static class Tiles
{
	public const int MaxTilesX = 30;
	public const int MaxTilesY = 25000;
	
	public static byte[,] Grid = new byte[MaxTilesX, MaxTilesY];

	public static void PlaceTile(Vector2 pos, byte type = 1, bool silent = false)
	{
		var y = (int)MathF.Floor(FHS.GroundLevel - pos.Y / FHS.TileSize) + 1;
		var x = (int)(pos.X / FHS.TileSize);

		if (x < 0 || y < 0 || x > MaxTilesX || y > MaxTilesY)
			return;
		
		PlaceTile(x, y, type, silent);
	}

	public static void PlaceTile(int x, int y, byte type = 1, bool silent = false)
	{
		if (Grid[x, y] != type && !silent)
			Assets.Sounds.TilePlace.Play(1, new Random(FHS.AmbientTimer).Next(100) / 100f * -0.2f, 0);
		
		Grid[x, y] = type;
	}

	public static void RemoveTile(Vector2 pos, bool silent = false)
	{
		var y = (int)MathF.Floor(FHS.GroundLevel - pos.Y / FHS.TileSize) + 1;
		var x = (int)(pos.X / FHS.TileSize);

		if (x < 0 || y < 0 || x > MaxTilesX || y > MaxTilesY)
			return;

		RemoveTile(x, y, silent);
	}
	

	public static void RemoveTile(int x, int y, bool silent = false)
	{
		if (x < 0 || y < 0 || x > MaxTilesX || y > MaxTilesY)
			return;

		var pos = new Vector2(x, FHS.GroundLevel - y) * FHS.TileSize + new Vector2(FHS.TileSize / 2f);
		if (Grid[x, y] is > 0 and < 254)
		{
			if (!silent)
				Assets.Sounds.TileBreak.Play(1, new Random(FHS.AmbientTimer).Next(100) / 100f * -0.2f, 0);

			SpawnParticle(pos, new Vector2(1, -1) * 5, 100, ParticleType.TileBreak);
			SpawnParticle(pos, new Vector2(-1, 1) * 5, 100, ParticleType.TileBreak);
			SpawnParticle(pos, new Vector2(-1, -1) * 5, 100, ParticleType.TileBreak);
			SpawnParticle(pos, new Vector2(1, 1) * 5, 100, ParticleType.TileBreak);
		
			Grid[x, y] = 0;
		}
	}
	
	public static void RemoveAllTiles()
	{
		for (int i = 0; i < MaxTilesX; i++)
		{
			for (int j = 0; j < MaxTilesY; j++)
			{
				RemoveTile(i, j, true);
			}
		}
	}

	public static void Init()
	{
		var seedShift = 0;
		for (int i = 0; i < 3; i++)
		{
			for (int j = 0; j < MaxTilesY; j++)
			{
				var rand = new Random(i + j + seedShift);
				var type = (byte)(rand.Next(10) == 0 ? 254 : 255);

				seedShift = rand.Next(int.MaxValue);
				
				PlaceTile(i, j, type, true);
				rand = new Random(i + j + MaxTilesX - 1 + seedShift);
				type = (byte)(rand.Next(10) == 0 ? 254 : 255);
				PlaceTile(MaxTilesX - 1 - i, j, type, true);
				
				seedShift = rand.Next(int.MaxValue);
			}	
		}
	}

	public static void Draw()
	{
		for (int i = 0; i < MaxTilesX; i++)
		{
			for (int j = 0; j < MaxTilesY; j++)
			{
				switch (Grid[i, j])
				{
					case 0:
						break;
					
					case 1:
						FHS.SpriteBatch.Draw(Assets.Textures.Placeholder, new Vector2(i, FHS.GroundLevel - j) * FHS.TileSize - FHS.ScreenPosition, null, Color.White, 0, Vector2.Zero, 2, SpriteEffects.None, 0);
						break;
					
					case 2: 
						FHS.SpriteBatch.Draw(Assets.Textures.Placeholder, new Vector2(i, FHS.GroundLevel - j) * FHS.TileSize - FHS.ScreenPosition, null, Color.Red, 0, Vector2.Zero, 2, SpriteEffects.None, 0);
						FHS.SpriteBatch.Draw(Assets.Textures.Placeholder, new Vector2(i, FHS.GroundLevel - j) * FHS.TileSize - FHS.ScreenPosition, null, Color.White, 0, Vector2.Zero,  new Vector2(2.1f, 0.2f), SpriteEffects.None, 0);
						break;
					
					case 254:
						FHS.SpriteBatch.Draw(Assets.Textures.Placeholder, new Vector2(i, FHS.GroundLevel - j) * FHS.TileSize - FHS.ScreenPosition, null, Color.Lime, 0, Vector2.Zero, 2, SpriteEffects.None, 0);
						break;
					
					case 255:
						FHS.SpriteBatch.Draw(Assets.Textures.Placeholder, new Vector2(i, FHS.GroundLevel - j) * FHS.TileSize - FHS.ScreenPosition, null, Color.Gold, 0, Vector2.Zero, 2, SpriteEffects.None, 0);
						break;
					
					default:
						var rand = new Random(i * j + FHS.AmbientTimer);
						var offset = new Vector2(rand.Next(-15, 15), rand.Next(-15, 15)) * (Grid[i, j] / 120f);
						FHS.SpriteBatch.Draw(Assets.Textures.Placeholder, new Vector2(i, FHS.GroundLevel - j) * FHS.TileSize + offset - FHS.ScreenPosition, null, Color.Turquoise, 0, Vector2.Zero, 2, SpriteEffects.None, 0);
						break;
				}
			}
		}
	}
}