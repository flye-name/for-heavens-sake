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
		
		var hitbox = FHS.Player.Bounds;
		hitbox.Inflate(-4, -4);
		var tileHitbox = new Rectangle(x * FHS.TileSize, (FHS.GroundLevel - y) * FHS.TileSize,  FHS.TileSize, FHS.TileSize);
		if (hitbox.Intersects(tileHitbox))
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
                }

				if (j is > 10 and < MaxTilesY - 10 && (i == 4 || i == MaxTilesX - 5))
				{
					var canPlace = true;
					for (int k = -3; k < 4; k++)
					{
						if (Grid[i, j + k] is TileTypes.CannonL or TileTypes.CannonR)
							canPlace = false;
					}
					if ((i == 4 && Grid[MaxTilesX - 5, j] is TileTypes.CannonL or TileTypes.CannonR) || (i == MaxTilesX - 5 && Grid[4, j] is TileTypes.CannonL or TileTypes.CannonR))
						canPlace = false;
					
					if (!canPlace)
						continue;
					
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
		var texture = Assets.Textures.Atlas;
		for (int i = 0; i < MaxTilesX; i++)
		{
			for (int j = 0; j < MaxTilesY; j++)
			{
				var y = (FHS.GroundLevel - j) * FHS.TileSize;
				if (y > FHS.ScreenPosition.Y + FHS.ScreenSize.Y * 1.5f || y < FHS.ScreenPosition.Y - FHS.ScreenSize.Y * .5f || Grid[i, j] == TileTypes.Inactive)
					continue;

				var rectangle = new Rectangle(0, 0, 32, 32);
				
				switch (Grid[i, j])
				{
					case TileTypes.Normal:
						FHS.SpriteBatch.Draw(texture, new Vector2(i, FHS.GroundLevel - j) * FHS.TileSize - FHS.ScreenPosition, rectangle, Color.White, 0, Vector2.Zero, 2, SpriteEffects.None, 0);
						break;
					
					case TileTypes.Damaging: 
						rectangle.Offset(34, 0);
						FHS.SpriteBatch.Draw(texture, new Vector2(i, FHS.GroundLevel - j) * FHS.TileSize - FHS.ScreenPosition, rectangle, Color.White, 0, Vector2.Zero, 2, SpriteEffects.None, 0);
						break;
					
					case TileTypes.Phasing:
						rectangle.Offset(34 * 2, 0);
						FHS.SpriteBatch.Draw(texture, new Vector2(i, FHS.GroundLevel - j) * FHS.TileSize - FHS.ScreenPosition, rectangle, Color.White * MathF.Sin(i + j + FHS.AmbientTimer * 0.015f), 0, Vector2.Zero, 2, SpriteEffects.None, 0);
                        FHS.SpriteBatch.Draw(texture, new Vector2(i, FHS.GroundLevel - j) * FHS.TileSize - FHS.ScreenPosition, rectangle, Color.White * 0.1f, 0, Vector2.Zero, 2, SpriteEffects.None, 0);
                        break;
					
					case TileTypes.Bouncy:
						rectangle.Offset(34 * 3, 0);
						FHS.SpriteBatch.Draw(texture, new Vector2(i, FHS.GroundLevel - j) * FHS.TileSize - FHS.ScreenPosition, rectangle, Color.White, 0, Vector2.Zero, 2, SpriteEffects.None, 0);
						break;
					
					case TileTypes.Sticky:
						rectangle.Offset(34 * 4, 0);
						FHS.SpriteBatch.Draw(texture, new Vector2(i, FHS.GroundLevel - j) * FHS.TileSize - FHS.ScreenPosition, rectangle, Color.White, 0, Vector2.Zero, 2, SpriteEffects.None, 0);
						break;

                    case TileTypes.CannonL:
						rectangle.Offset(34 * 6, 0);
                        FHS.SpriteBatch.Draw(texture, new Vector2(i, FHS.GroundLevel - j) * FHS.TileSize - FHS.ScreenPosition, rectangle, Color.White, 0, Vector2.Zero, 2, SpriteEffects.None, 0);
                        break;

                    case TileTypes.CannonR:
						rectangle.Offset(34 * 6, 0);
                        FHS.SpriteBatch.Draw(texture, new Vector2(i, FHS.GroundLevel - j) * FHS.TileSize - FHS.ScreenPosition, rectangle, Color.White, 0, Vector2.Zero, 2, SpriteEffects.FlipHorizontally, 0);
                        break;

                    case TileTypes.Bomb:
						rectangle = new Rectangle(0, 34, 16, 28);
                        FHS.SpriteBatch.Draw(texture, new Vector2(i, FHS.GroundLevel - j) * FHS.TileSize - FHS.ScreenPosition, rectangle, Color.Red, 0, Vector2.Zero, 2, SpriteEffects.None, 0);
                        break;

                    case TileTypes.Wall:
						rectangle.Offset(34 * 7, 0);
						FHS.SpriteBatch.Draw(texture, new Vector2(i, FHS.GroundLevel - j) * FHS.TileSize - FHS.ScreenPosition, rectangle, Color.White, 0, Vector2.Zero, 2, SpriteEffects.None, 0);
						break;
					
					case TileTypes.Wall2:
						rectangle.Offset(34 * 8, 0);
						FHS.SpriteBatch.Draw(texture, new Vector2(i, FHS.GroundLevel - j) * FHS.TileSize - FHS.ScreenPosition, rectangle, Color.White, 0, Vector2.Zero, 2, SpriteEffects.None, 0);
						break;
					
					default:
						rectangle.Offset(34 * 5, 0);
						var rand = new Random(i * j + FHS.AmbientTimer);
						var offset = new Vector2(rand.Next(-15, 15), rand.Next(-15, 15)) * (Grid[i, j] / 80f);
						FHS.SpriteBatch.Draw(texture, new Vector2(i, FHS.GroundLevel - j) * FHS.TileSize + offset - FHS.ScreenPosition, rectangle, Color.White, 0, Vector2.Zero, 2, SpriteEffects.None, 0);
						break;
				}
			}
		}
	}
}