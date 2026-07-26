using TowerToHeaven.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static TowerToHeaven.ParticleSystem;

namespace TowerToHeaven;

public static class TileTypes
{
	public const byte Inactive = 0;
	public const byte Normal = 1;
	public const byte Damaging = 2;
	public const byte Phasing = 3;
	public const byte Bouncy = 4;
	public const byte Sticky = 5;
	public const byte Breakable = 6;

    public const byte CannonL = 128;
    public const byte CannonR = 129;
    public const byte Bomb = 130;

    public const byte Wall = 254;
	public const byte Wall2 = 255;
}

public static class Tiles
{
	public const int MaxTilesX = 30;
	public const int MaxTilesY = 25000;
    public static int CannonTimer;

    public static byte[,] Grid = new byte[MaxTilesX, MaxTilesY];

	public static void PlaceTile(Vector2 pos, byte type = TileTypes.Normal, bool silent = false)
	{
		var y = (int)MathF.Floor(FHS.GroundLevel - pos.Y / FHS.TileSize) + 1;
		var x = (int)(pos.X / FHS.TileSize);

		if (x < 0 || y < 0 || x > MaxTilesX || y > MaxTilesY)
			return;
		
		PlaceTile(x, y, type, silent);
	}

	public static void PlaceTile(int x, int y, byte type = TileTypes.Normal, bool silent = false)
	{
		if (Grid[x, y] >= TileTypes.CannonL)
			return;

        if (Grid[x, y] != type && !silent)
			Assets.Sounds.TilePlace?.Play(1, new Random(FHS.AmbientTimer).Next(100) / 100f * -0.2f, 0);
		
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
		if (Grid[x, y] is > TileTypes.Inactive and < TileTypes.CannonL)
		{
			if (!silent)
				Assets.Sounds.TileBreak?.Play(1, new Random(FHS.AmbientTimer).Next(100) / 100f * -0.2f, 0);

            SpawnParticle(pos, Vector2.Zero, 5, ParticleType.Strike);
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
		var seedShift = DateTime.Now.Microsecond + DateTime.Now.Second + DateTime.Now.Month + DateTime.Now.Day;
		for (int i = 0; i < MaxTilesX; i++)
		{
			for (int j = 0; j < MaxTilesY; j++)
			{
				var rand = new Random(i + j + seedShift);
				var type = (byte)(rand.Next(10) == 0 ? TileTypes.Wall : TileTypes.Wall2);

				seedShift = rand.Next(int.MaxValue);
				if (i < 4 || i > MaxTilesX - 5)
					PlaceTile(i, j, type, true);
				else
                {
                    if (rand.Next(100) == 0)
                        PlaceTile(i, j, TileTypes.Normal, true);

					// didn't get to this
                    /*if (rand.Next(200) == 0)
                        PlaceTile(i, j, TileTypes.Bomb, true);*/
                }

				if (j > 10)
				{
                    if (i == 4 && rand.Next(10) == 0)
                        PlaceTile(i, j, TileTypes.CannonL, true);

                    if (i == MaxTilesX - 5 && rand.Next(10) == 0)
                        PlaceTile(i, j, TileTypes.CannonR, true);
                }
            }	
		}
	}

	public static void Update()
    {
		if (!FHS.Frozen)
		{
			CannonTimer++;

			for (int i = 0; i < MaxTilesX; i++)
			{
				for (int j = 0; j < MaxTilesY; j++)
				{
					var y = (FHS.GroundLevel - j) * FHS.TileSize;
					if (y > FHS.ScreenPosition.Y + FHS.ScreenSize.Y * 1.5f || y < FHS.ScreenPosition.Y - FHS.ScreenSize.Y * .5f)
						continue;

					var pos = new Vector2(i, FHS.GroundLevel - j) * FHS.TileSize + new Vector2(FHS.TileSize / 2f);

					switch (Grid[i, j])
					{
						case TileTypes.CannonL:

							if (CannonTimer >= 120)
							{
								Assets.Sounds.FireballSpit?.Play(1, new Random(FHS.AmbientTimer).Next(100) / 100f * -0.2f, 0);
								ProjectileManager.SpawnProjectile(pos, Vector2.UnitX * 10, 200, ProjectileType.FireballL);
							}

							break;

						case TileTypes.CannonR:

							if (CannonTimer >= 120)
							{
								Assets.Sounds.FireballSpit?.Play(1, new Random(FHS.AmbientTimer).Next(100) / 100f * -0.2f, 0);
								ProjectileManager.SpawnProjectile(pos, Vector2.UnitX * -10, 200, ProjectileType.FireballR);
							}

							break;
					}
				}
			}

			if (CannonTimer >= 120)
			{
				CannonTimer = 0;
			}
		}
    }

    public static void Draw()
	{
		for (int i = 0; i < MaxTilesX; i++)
		{
			for (int j = 0; j < MaxTilesY; j++)
			{
				var y = (FHS.GroundLevel - j) * FHS.TileSize;
				if (y > FHS.ScreenPosition.Y + FHS.ScreenSize.Y * 1.5f || y < FHS.ScreenPosition.Y - FHS.ScreenSize.Y * .5f)
					continue;
				
				switch (Grid[i, j])
				{
					case TileTypes.Inactive:
						break;
					
					case TileTypes.Normal:
						FHS.SpriteBatch.Draw(Assets.Textures.BrickBlock, new Vector2(i, FHS.GroundLevel - j) * FHS.TileSize - FHS.ScreenPosition, null, Color.White, 0, Vector2.Zero, 2, SpriteEffects.None, 0);
						break;
					
					case TileTypes.Damaging: 
						FHS.SpriteBatch.Draw(Assets.Textures.Spinner, new Vector2(i, FHS.GroundLevel - j) * FHS.TileSize - FHS.ScreenPosition, null, Color.White, 0, Vector2.Zero, 2, SpriteEffects.None, 0);
						break;
					
					case TileTypes.Phasing:
						FHS.SpriteBatch.Draw(Assets.Textures.Mirror, new Vector2(i, FHS.GroundLevel - j) * FHS.TileSize - FHS.ScreenPosition, null, Color.White * MathF.Sin(i + j + FHS.AmbientTimer * 0.015f), 0, Vector2.Zero, 2, SpriteEffects.None, 0);
                        FHS.SpriteBatch.Draw(Assets.Textures.Mirror, new Vector2(i, FHS.GroundLevel - j) * FHS.TileSize - FHS.ScreenPosition, null, Color.White * 0.1f, 0, Vector2.Zero, 2, SpriteEffects.None, 0);
                        break;
					
					case TileTypes.Bouncy:
						FHS.SpriteBatch.Draw(Assets.Textures.Balloon, new Vector2(i, FHS.GroundLevel - j) * FHS.TileSize - FHS.ScreenPosition, null, Color.White, 0, Vector2.Zero, 2, SpriteEffects.None, 0);
						break;
					
					case TileTypes.Sticky:
						FHS.SpriteBatch.Draw(Assets.Textures.StickyBlock, new Vector2(i, FHS.GroundLevel - j) * FHS.TileSize - FHS.ScreenPosition, null, Color.White, 0, Vector2.Zero, 2, SpriteEffects.None, 0);
						break;

                    case TileTypes.CannonL:
                        FHS.SpriteBatch.Draw(Assets.Textures.Cannon, new Vector2(i, FHS.GroundLevel - j) * FHS.TileSize - FHS.ScreenPosition, null, Color.White, 0, Vector2.Zero, 2, SpriteEffects.None, 0);
                        break;

                    case TileTypes.CannonR:
                        FHS.SpriteBatch.Draw(Assets.Textures.Cannon, new Vector2(i, FHS.GroundLevel - j) * FHS.TileSize - FHS.ScreenPosition, null, Color.White, 0, Vector2.Zero, 2, SpriteEffects.FlipHorizontally, 0);
                        break;

                    case TileTypes.Bomb:
                        FHS.SpriteBatch.Draw(Assets.Textures.Bomb, new Vector2(i, FHS.GroundLevel - j) * FHS.TileSize - FHS.ScreenPosition, null, Color.Red, 0, Vector2.Zero, 2, SpriteEffects.None, 0);
                        break;

                    case TileTypes.Wall:
						FHS.SpriteBatch.Draw(Assets.Textures.WallB, new Vector2(i, FHS.GroundLevel - j) * FHS.TileSize - FHS.ScreenPosition, null, Color.White, 0, Vector2.Zero, 2, SpriteEffects.None, 0);
						break;
					
					case TileTypes.Wall2:
						FHS.SpriteBatch.Draw(Assets.Textures.WallA, new Vector2(i, FHS.GroundLevel - j) * FHS.TileSize - FHS.ScreenPosition, null, Color.White, 0, Vector2.Zero, 2, SpriteEffects.None, 0);
						break;
					
					default:
						var rand = new Random(i * j + FHS.AmbientTimer);
						var offset = new Vector2(rand.Next(-15, 15), rand.Next(-15, 15)) * (Grid[i, j] / 80f);
						FHS.SpriteBatch.Draw(Assets.Textures.Ice, new Vector2(i, FHS.GroundLevel - j) * FHS.TileSize + offset - FHS.ScreenPosition, null, Color.White, 0, Vector2.Zero, 2, SpriteEffects.None, 0);
						break;
				}
			}
		}
	}
}