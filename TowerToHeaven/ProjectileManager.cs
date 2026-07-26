using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TowerToHeaven.Core;
using static TowerToHeaven.ProjectileManager;

namespace TowerToHeaven;

/// list of known issues:
/// collision works(?) horribly
/// yay i got the list down to one thing!

public enum ProjectileType : byte
{
    FireballL,
    FireballR,
    FallBomb
}

public class ProjectileManager
{
    public record struct Projectile()
    {
        public Vector2 Position;
        public Vector2 Velocity;
        public int TimeLeft;
        public int Lifetime;
        public ProjectileType Type;
    }

    public const int MaxProjectiles = 500;
    public static Projectile[] Projectiles = new Projectile[MaxProjectiles];

    public static int SpawnProjectile(Vector2 position, Vector2 velocity, int lifetime, ProjectileType type)
    {
        int index = 0;

        while (Projectiles[index].TimeLeft > 0 && index < MaxProjectiles - 1)
        {
            index++;
        }

        Projectiles[index] = new Projectile()
        {
            Position = position,
            Velocity = velocity,
            TimeLeft = lifetime,
            Lifetime = lifetime,
            Type = type
        };

        return index;
    }
    public static void Update()
    {
        if (!FHS.Frozen)
        {
            var rand = new Random(FHS.AmbientTimer);

            if (rand.Next(250) == 0)
                SpawnProjectile(new Vector2(rand.Next(4, Tiles.MaxTilesX - 4) * FHS.TileSize + FHS.TileSize * 0.5f, FHS.ScreenPosition.Y), Vector2.Zero, 200, ProjectileType.FallBomb);

            for (int k = 0; k < MaxProjectiles; k++)
            {
                ref var p = ref Projectiles[k];

                if (p.TimeLeft < 0)
                    continue;

                p.TimeLeft--;
                p.Position += p.Velocity;

                switch (p.Type)
                {
                    case ProjectileType.FireballL:

                        if (TileCollide(p.Position))
                        {
                            p.TimeLeft -= 200;
                            ParticleSystem.SpawnParticle(p.Position, Vector2.Zero, 5, ParticleType.Strike);
                        }
                        break;

                    case ProjectileType.FireballR:

                        if (TileCollide(p.Position))
                        {
                            p.TimeLeft -= 200;
                            ParticleSystem.SpawnParticle(p.Position, Vector2.Zero, 5, ParticleType.Strike);
                        }
                        break;

                    case ProjectileType.FallBomb:

                        p.Velocity.Y += 0.1f;
                        if (TileCollide(p.Position, true))
                        {
                            p.TimeLeft -= 200;
                            ParticleSystem.SpawnParticle(p.Position, Vector2.Zero, 5, ParticleType.Strike);
                        }
                        break;
                }
            }
        }
    }

    public static bool TileCollide(Vector2 position, bool bomb = false)
    {
        int x = (int)(position.X / FHS.TileSize);
        int y = FHS.GroundLevel - (int)(position.Y / FHS.TileSize);

        if (x < 0 || x >= Tiles.MaxTilesX || y < 0 || y >= Tiles.MaxTilesY)
            return false;

        var tile = Tiles.Grid[x, y];

        if (tile == 0 || tile == TileTypes.CannonL || tile == TileTypes.CannonR)
            return false;

        Tiles.RemoveTile(x, y);

        if (bomb)
        {
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    int nx = x + i;
                    int ny = y + j;

                    if (nx < 0 || nx >= Tiles.MaxTilesX || ny < 0 || ny >= Tiles.MaxTilesY)
                        continue;

                    Tiles.RemoveTile(nx, ny);
                }
            }
        }

        return true;
    }

    public static void Draw()
	{
		var texture = Assets.Textures.Atlas;

        for (int k = 0; k < MaxProjectiles; k++)
        {
            var p = Projectiles[k];
            
            if (p.TimeLeft <= 0)
                continue;

            var progress = 1f - (p.TimeLeft / (float)p.Lifetime);

            switch (p.Type)
            {
                case ProjectileType.FireballL:
                    FHS.SpriteBatch.Draw(texture, p.Position - FHS.ScreenPosition,  new Rectangle(54, 34, 32, 28), Color.White, 0, new Vector2(32, 28) / 2f, 2, SpriteEffects.None, 0);
                    break;

                case ProjectileType.FireballR:
                    FHS.SpriteBatch.Draw(texture, p.Position - FHS.ScreenPosition, new Rectangle(54, 34, 32, 28), Color.White, 0, new Vector2(32, 28) / 2f, 2, SpriteEffects.FlipHorizontally, 0);
                    break;

                case ProjectileType.FallBomb:
                    FHS.SpriteBatch.Draw(texture, p.Position - FHS.ScreenPosition, new Rectangle(0, 34, 16, 28), Color.White, 0, new Vector2(16, 28) / 2f, 2, SpriteEffects.None, 0);
                    break;
            }
        }
	}
}