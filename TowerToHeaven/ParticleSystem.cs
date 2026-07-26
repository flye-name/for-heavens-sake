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
		var texture = Assets.Textures.Atlas;

        for (int k = 0; k < MaxParticles; k++)
        {
            var particle = Particles[k];
            
            if (particle.TimeLeft <= 0)
                continue;

            var progress = 1f - (particle.TimeLeft / (float)particle.Lifetime);
            
            (Vector2 scale, Color color, Rectangle frame) drawData = particle.Type switch
            {
                ParticleType.FootStep => (Vector2.One, Color.White, new Rectangle(122, 34, 16, 16)),
                ParticleType.Hurt => (new Vector2(1 - MathF.Floor(progress * 8) / 8f, MathF.Floor(progress * 8) / 8f) * 2, Color.Red, new Rectangle(18, 34, 16, 14)),
                ParticleType.Strike => (Vector2.One * 2, Color.White, new Rectangle(140, 34, 32, 32)),
                ParticleType.TileBreak => (Vector2.One * 2, Color.White, new Rectangle(36, 34, 16, 16)),
                _ => (Vector2.One, new Color(116, 131, 250), new Rectangle(306, 0, 32, 32))
            };

            FHS.SpriteBatch.Draw(texture, particle.Position - FHS.ScreenPosition, drawData.frame, drawData.color, particle.Rotation, new Vector2(drawData.frame.Width, drawData.frame.Height) / 2f, drawData.scale, SpriteEffects.None, 0);
        }
	}
}