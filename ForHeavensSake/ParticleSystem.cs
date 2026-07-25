using ForHeavensSake.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ForHeavensSake;

public class ParticleSystem
{
    public record struct Particle()
    {
        public Vector2 Position;
        public Vector2 Velocity;
        public int TimeLeft;
        public float Rotation;
    }

    public const int MaxParticles = 500;
    public static Particle[] Particles = new Particle[500];

    public static int SpawnParticle(Vector2 position, Vector2 velocity, int lifetime)
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
            TimeLeft = lifetime
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
            particle.Velocity.Y += 0.5f;

            if (particle.TimeLeft % 10 == 0)
            {
                particle.Rotation += MathHelper.PiOver4;
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

            FHS.SpriteBatch.Draw(texture, particle.Position - FHS.ScreenPosition, null, new Color(116, 131, 250), particle.Rotation, texture.Size / 2f, 1f, SpriteEffects.None, 0);
        }
	}
}