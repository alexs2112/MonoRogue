using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MonoRogue {
    public class Font {
        public int Size;
        public int Height;
        public int Width;
        private SpriteFont SpriteFont;
        public SpriteFont Get() { return SpriteFont; }

        private Font(SpriteFont spriteFont, int size) {
            SpriteFont = spriteFont;
            Size = size;

            // The SDS font is monospaced, so each char should be the same size
            (float height, float width) = spriteFont.MeasureString("A");
            Height = (int)System.Math.Ceiling(height);
            Width = (int)System.Math.Ceiling(width);
        }

        // Width here is the max screen width of the string you are splitting
        public List<string> SplitString(string message, int width) {
            List<string> output = new List<string>();
            string current = "";
            foreach (string s in message.Split(' ')) {
                if ((current.Length + s.Length) * Width > width) {
                    output.Add(current);
                    current = s + " ";
                } else {
                    current += s + " ";
                }
            }
            if (current.Length > 0) {
                output.Add(current);
            }

            return output;
        }

        public static Font Size10;
        public static Font Size12;
        public static Font Size14;
        public static Font Size16;
        public static Font Size24;
        public static void LoadFonts(ContentManager content) {
            Size24 = new Font(content.Load<SpriteFont>("Interface/sds24"), 24);
            Size16 = new Font(content.Load<SpriteFont>("Interface/sds16"), 16);
            Size14 = new Font(content.Load<SpriteFont>("Interface/sds14"), 14);
            Size12 = new Font(content.Load<SpriteFont>("Interface/sds12"), 12);
            Size10 = new Font(content.Load<SpriteFont>("Interface/sds10"), 10);
        }
    }
}
