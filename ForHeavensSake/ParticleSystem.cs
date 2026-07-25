using ForHeavensSake.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ForHeavensSake;

public class ParticleSystem
{
    public class Particle(Vector2 position, Vector2 velocity, int lifetime)
    {
        public int TimeLeft;
        public int Lifetime = lifetime;
        public Vector2 Velocity = velocity;
        public Vector2 Position = position;
        public float Rotation;
        public float Timer;
    }

    public static List<Particle> Particles
    {
        get;
        internal set;
    } = [];

    public static void Update()
    {
        for (int k = 0; k < Particles.Count; k++)
        {
            var particle = Particles[k];

            particle.TimeLeft++;
            particle.Position += particle.Velocity;
            particle.Velocity.Y += 0.5f;

            if (++particle.Timer >= 10)
            {
                particle.Rotation += MathHelper.PiOver2;
                particle.Timer = 0;
            }
        }

        Particles.RemoveAll(p => p.TimeLeft >= p.Lifetime);
    }
	
	public static void Draw()
	{
		var texture = Assets.Textures.Placeholder;

        for (int k = 0; k < Particles.Count; k++)
        {
            var particle = Particles[k];

            FHS.SpriteBatch.Draw(texture, particle.Position - FHS.ScreenPosition, null, new Color(116, 131, 250), particle.Rotation, texture.Size / 2f, 1f, SpriteEffects.None, 0);
        }
	}
}