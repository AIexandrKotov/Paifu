using KTXCore;
using KTXCore.Graphics;
using KTXCore.Extensions;
using System.IO;
using System.Drawing;
using System.Reflection;
using System.Collections.Generic;
using System;

namespace Paifu
{
    public class Paifu
    {
        public bool IsLovely { get; set; } = false;
    }

    public class PaifuViewer : Block
    {
        public Paifu Current { get; set; }
        public Random Rng { get; set; } = new Random();

        public static Dictionary<string, Complexive> Sprites { get; private set; } = new Dictionary<string, Complexive>();
        public static Dictionary<string, MediaComplexive> Animations { get; private set; } = new Dictionary<string, MediaComplexive>();

        static Stream GetResource(string s)
        {
            return Assembly.GetExecutingAssembly().GetManifestResourceStream(s);
        }

        static PaifuViewer()
        {
            var ass = Assembly.GetExecutingAssembly();
            foreach (var x in ass.GetManifestResourceNames())
            {
                if (!x.StartsWith("Paifu.Sprites")) continue;
                var s = x.Replace("Paifu.Sprites.", "");
                using (var b = new Bitmap(ass.GetManifestResourceStream(x)))
                    Sprites[s] = Complexive.FromBitmap(b);
            }

            Animations.Add("eyes", new MediaComplexive(new Complexive[] { Sprites["eyes.png"], Sprites["eyes_2.png"], Sprites["eyes_3.png"], Sprites["eyes_2.png"] }));
        }

        public PaifuViewer()
        {
            //var renderContainer = new DynamicContainer(() => 0, () => 0, () => Console.Fi, () => 32);
            ImageRender = new ComplexiveRectangle(null, 0, 0, Alignment.CenterWidth | Alignment.CenterHeight);
            MediaRender = new MediaRectangle(null, 0, 0, Alignment.CenterWidth | Alignment.CenterHeight);

            Current = new Paifu();

            Add(SpaceAction);
            AddRedrawer(() => true, RedrawEyes);
            AddRedrawer(RedrawSkin);
            OnAllRedraw += AllRedraw;
            OnKeyPress += KeyPress;
        }

        public ComplexiveRectangle ImageRender;
        public MediaRectangle MediaRender;

        public void DrawImage(string s)
        {
            ImageRender.Source = Sprites[s];
            ImageRender.Draw();
        }

        public void RedrawHairs()
        {
            DrawImage("hairs.png");
        }

        public void RedrawSkin()
        {
            DrawImage(Current.IsLovely ? "skin_lovely.png" : "skin_usualy.png");
        }

        public bool in_eyes = false;
        public string[] shots = new string[] { "eyes.png", "eyes_2.png", "eyes_3.png", "eyes_2.png", "eyes_3.png" };
        public int shot = 0;

        public TimeSpan lastEyesAnimation;
        public TimeSpan lastShot;
        public void RedrawEyes()
        {
            if (!in_eyes && lastEyesAnimation < BlockTimer && lastShot < BlockTimer) in_eyes = true;
            if (in_eyes)
            {
                RedrawSkin();
                RedrawMouth();
                DrawImage(shots[shot++]);
                if (shot >= shots.Length)
                {
                    in_eyes = false;
                    shot = 0;
                    lastEyesAnimation = BlockTimer.Add(
                        Rng.Chance(0.2) ? new TimeSpan(200 * TimeSpan.TicksPerMillisecond)
                        : new TimeSpan(Rng.Next(1500, 2500) * TimeSpan.TicksPerMillisecond)
                        );
                }
                lastShot = BlockTimer.Add(new TimeSpan(0, 0, 0, 0, 500));
            }
            else DrawImage("eyes.png");
        }

        public void RedrawMouth()
        {
            DrawImage("mouth.png");
        }

        public void AllRedraw()
        {
            RedrawHairs();
        }

        public void SpaceAction()
        {
            Current.IsLovely = !Current.IsLovely;
            Reactions["RedrawSkin"] = true;
            Reactions["RedrawMouth"] = true;
        }

        public void KeyPress(byte key)
        {
            KeypressInline(key, "SpaceAction", Key.Spacebar);
        }
    }
}
