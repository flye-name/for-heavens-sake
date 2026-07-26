using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TowerToHeaven.Core;
using static TowerToHeaven.ProjectileManager;

namespace TowerToHeaven;

public enum ProjectileType : byte
{
    FireballL,
    FireballR,
    FallBomb,
    Explosion
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

            if (rand.Next(100) == 0)
            {
                SpawnProjectile(FHS.ScreenPosition + new Vector2((int)(rand.NextDouble() * FHS.ScreenSize.X), 0), Vector2.Zero, 200, ProjectileType.FallBomb);
            }

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
                            continue;
                        }
                        break;

                    case ProjectileType.FireballR:

                        if (TileCollide(p.Position))
                        {
                            p.TimeLeft -= 200;
                            ParticleSystem.SpawnParticle(p.Position, Vector2.Zero, 5, ParticleType.Strike);
                            continue;
                        }
                        break;

                    case ProjectileType.FallBomb:

                        p.Velocity.Y += 0.1f;
                        if (TileCollide(p.Position))
                        {
                            p.TimeLeft -= 200;
                            ParticleSystem.SpawnParticle(p.Position, Vector2.Zero, 5, ParticleType.Strike);
                            continue;
                        }
                        break;

                    case ProjectileType.Explosion:
                        break;
                }
            }
        }
    }

    public static bool TileCollide(Vector2 position)
    {
        int x = (int)(position.X / FHS.TileSize);
        int y = FHS.GroundLevel - (int)(position.Y / FHS.TileSize);

        if (x < 0 || x >= Tiles.MaxTilesX || y < 0 || y >= Tiles.MaxTilesY)
            return false;

        var tile = Tiles.Grid[x, y];

        Tiles.RemoveTile(x, y);

        if (tile == 0 || tile == TileTypes.CannonL || tile == TileTypes.CannonR)
            return false;

        return true;
    }

    public static void Draw()
	{
		var fireballTex = Assets.Textures.Fireball;
        var bombTex = Assets.Textures.Bomb;
        var explosionTex = Assets.Textures.Placeholder;

        for (int k = 0; k < MaxProjectiles; k++)
        {
            var p = Projectiles[k];
            
            if (p.TimeLeft <= 0)
                continue;

            var progress = 1f - (p.TimeLeft / (float)p.Lifetime);

            switch (p.Type)
            {
                case ProjectileType.FireballL:
                    FHS.SpriteBatch.Draw(fireballTex, p.Position - FHS.ScreenPosition, null, Color.White, 0, fireballTex.Size / 2f, 4, SpriteEffects.None, 0);
                    break;

                case ProjectileType.FireballR:
                    FHS.SpriteBatch.Draw(fireballTex, p.Position - FHS.ScreenPosition, null, Color.White, 0, fireballTex.Size / 2f, 4, SpriteEffects.FlipHorizontally, 0);
                    break;

                case ProjectileType.FallBomb:
                    FHS.SpriteBatch.Draw(bombTex, p.Position - FHS.ScreenPosition, null, Color.White, 0, bombTex.Size / 2f, 4, SpriteEffects.None, 0);
                    break;

                case ProjectileType.Explosion:
                    FHS.SpriteBatch.Draw(explosionTex, p.Position - FHS.ScreenPosition, null, Color.White, 0, explosionTex.Size / 2f, 4, SpriteEffects.None, 0);
                    break;
            }
        }
	}
}