using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System.Reflection.Metadata;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TowerToHeaven.Core;

public static class Assets
{
	public static Texture2D LoadTexture(string path) => FHS.Instance.Content.Load<Texture2D>("Images/" + path);
	public static SoundEffect LoadSound(string path) => FHS.Instance.Content.Load<SoundEffect>("Sounds/" + path);
	public static Song LoadSong(string path) => FHS.Instance.Content.Load<Song>("Music/" + path);
	public static Effect LoadEffect(string path) => FHS.Instance.Content.Load<Effect>("Effects/Compiled/" + path);

	public static Song? Music;

	public static class Textures
	{
		public static Texture2D? Placeholder;
		public static Texture2D? Noise;
        public static Texture2D? Brick;
        public static Texture2D? BrickBlock;
        public static Texture2D? Crosshair;
        public static Texture2D? Ice;
        public static Texture2D? Smoke;
        public static Texture2D? Cannon;
        public static Texture2D? Fireball;
        public static Texture2D? Strike;
        public static Texture2D? Bomb;
        public static Texture2D? HitHurt;
        public static Texture2D? WallA;
        public static Texture2D? WallB;
        public static Texture2D? Player;

        public static void LoadAssets()
		{
			Placeholder = LoadTexture("Placeholder");
			Noise = LoadTexture("Noise");
            Brick = LoadTexture("Brick");
            BrickBlock = LoadTexture("BrickBlock");
            Crosshair = LoadTexture("Crosshair");
			Ice = LoadTexture("Ice");
            Smoke = LoadTexture("Smoke");
            Cannon = LoadTexture("Cannon");
            Fireball = LoadTexture("Fireball");
            Strike = LoadTexture("Strike");
            Bomb = LoadTexture("Bomb");
            HitHurt = LoadTexture("Blood");
            WallA = LoadTexture("WallA");
            WallB = LoadTexture("WallB");
            Player = LoadTexture("Player");
        }

		public static void Dispose()
		{
			Placeholder?.Dispose();
			Noise?.Dispose();
            Brick?.Dispose();
            BrickBlock?.Dispose();
            Crosshair?.Dispose();
            Ice?.Dispose();
            Smoke?.Dispose();
            Cannon?.Dispose();
            Fireball?.Dispose();
            Strike?.Dispose();
            Bomb?.Dispose();
            HitHurt?.Dispose();
            WallA?.Dispose();
            WallB?.Dispose();
            Player?.Dispose();
        }
    }

    public static class Effects
    {
        public static Effect? CRTBarrelFilter;

        public static void LoadAssets()
        {
            CRTBarrelFilter = LoadEffect("CRT");
        }

        public static void Dispose()
        {
            CRTBarrelFilter?.Dispose();
        }
    }

    public static class Sounds
	{
		public static SFX? TestSound;
		public static SFX? Step;
		public static SFX? MenuSelect;
		public static SFX? Blip;
		public static SFX? Jump;
		public static SFX? Hurt;
		public static SFX? TilePlace;
		public static SFX? TileBreak;
        public static SFX? FireballSpit;

        public static SoundEffect? Fall;
		public static SoundEffectInstance? FallInstance;

		public static void LoadAssets()
		{
			TestSound = new("TestSound", 5);
			Step = new("Step", 10);
			MenuSelect = new("MenuSelect", 7);
			Blip = new("MenuBlip", 10);
			Jump = new("Jump", 3);
			Hurt = new("Hurt", 3);
			TilePlace = new("BlockPlace", 5);
			TileBreak = new("BlockBreak", 5);
            FireballSpit = new("FireballSpit", 1);

            Fall = LoadSound("Fall");
			FallInstance = Fall.CreateInstance();
		}

		public static void Dispose()
		{
			TestSound?.Dispose();
			Step?.Dispose();
			MenuSelect?.Dispose();
			Blip?.Dispose();
			Jump?.Dispose();
			Fall?.Dispose();
			Hurt?.Dispose();
			TilePlace?.Dispose();
			TileBreak?.Dispose();
            FireballSpit?.Dispose();

            FallInstance?.Dispose();
		}
	}
	
	public static void Load()
	{
		Music = LoadSong("StairwayToHeaven");
		
		Textures.LoadAssets();
		Sounds.LoadAssets();
		Effects.LoadAssets();
		
		FHS.FontSystem = new();
		FHS.FontSystem.AddFont(File.ReadAllBytes(@"Content/Fonts/SpaceMono.ttf"));
		
		Saving.Save();
	}

	public static void Unload()
	{
		Music?.Dispose();
		
		Textures.Dispose();
		Sounds.Dispose();
		Effects.Dispose();
		
		FHS.FontSystem.Dispose();
	}
}