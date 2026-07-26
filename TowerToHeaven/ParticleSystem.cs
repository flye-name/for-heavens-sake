using TowerToHeaven.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TowerToHeaven;

public enum ParticleType : byte
{
    TileBreak,
    FootStep,
    Strike,
    Hurt
}

public class ParticleSystem
{
    public record struct Particle()
    {
        public Vector2 Position;
        public Vector2 Velocity;
        public int TimeLeft;
        public int Lifetime;
        public float Rotation;
        public ParticleType Type;
    }

    public const int MaxParticles = 500;
    public static Particle[] Particles = new Particle[500];

    public static int SpawnParticle(Vector2 position, Vector2 velocity, int lifetime, ParticleType type)
    {
        int index = 0;

        while (Particles[index].TimeLeft > 0 && index < MaxParticles - 1)
        {
            index++;
        }

        Particles[index] = new Particle()
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
        for (int k = 0; k < MaxParticles; k++)
        {
            ref var particle = ref Particles[k];
            
            if (particle.TimeLeft < 0)
                continue;

            particle.TimeLeft--;
            particle.Position += particle.Velocity;

            switch (particle.Type)
            {
                case ParticleType.TileBreak:
                    particle.Velocity.Y += 0.5f;
    
                    if (particle.TimeLeft % 10 == 0)
                    {
                        particle.Rotation += MathHelper.PiOver4;
                    }
                    break;
                
                case ParticleType.FootStep:
                    particle.Velocity *= 0.97f;
                    break;
                
                case ParticleType.Hurt:
                    particle.Velocity.Y += 0.1f;

                    if (particle.TimeLeft % 10 == 0)
                    {
                        particle.Rotation = particle.Velocity.Rotation;
                    }
                    break;
            }
        }
    }
	
	public static void Draw()
	{
		var texture = Assets.Textures.Placeholder;

        for (int k = 0; k < MaxParticles; k++)
        {
            var particle = Particles[k];
            
            if (particle.TimeLeft <= 0)
                continue;

            var progress = 1f - (particle.TimeLeft / (float)particle.Lifetime);
            
            (Vector2 scale, Color color, Texture2D? tex) drawData = particle.Type switch
            {
                ParticleType.FootStep => (Vector2.One, Color.White, Assets.Textures.Smoke),
                ParticleType.Hurt => (new Vector2(1 - MathF.Floor(progress * 8) / 8f, MathF.Floor(progress * 8) / 8f) * 2, Color.Red, Assets.Textures.HitHurt),
                ParticleType.Strike => (Vector2.One * 2, Color.White, Assets.Textures.Strike),
                ParticleType.TileBreak => (Vector2.One * 2, Color.White, Assets.Textures.Brick),
                _ => (Vector2.One, new Color(116, 131, 250), texture)
            };

            FHS.SpriteBatch.Draw(drawData.tex, particle.Position - FHS.ScreenPosition, null, drawData.color, particle.Rotation, drawData.tex.Size / 2f, drawData.scale, SpriteEffects.None, 0);
        }
	}
}