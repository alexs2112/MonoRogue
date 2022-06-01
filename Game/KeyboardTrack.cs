using System.Linq;
using Microsoft.Xna.Framework.Input;


namespace MonoRogue {
    public class KeyboardTrack {
        // A class to keep track of which keys are pressed and held between frames

        private Keys[] LastKeys { get; set; }
        private Keys[] Keys { get; set; }
        private long TicksHeld;

        public KeyboardTrack() {
            LastKeys = new Keys[0];
            Keys = new Keys[0];
        }

        public void Update(Keys[] pressed, System.TimeSpan elapsed) {
            LastKeys = Keys;
            Keys = pressed;

            if (LastKeys.Count() > 0 && Keys.Count() > 0) { 
                if (LastKeys[0] == Keys[0]) {
                    TicksHeld += elapsed.Ticks;
                    if (TicksHeld >= Constants.TickHoldRate) {
                        TicksHeld = 0;
                        Keys = new Keys[0];
                    }
                } else { TicksHeld = 0; }
            } else { TicksHeld = 0; }
        }

        public bool KeyJustPressed() {
            return !Keys.SequenceEqual(LastKeys);
        }

        public bool KeyJustPressed(Keys key) {
            return Keys.Contains(key) && !LastKeys.Contains(key);
        }
    }
}
