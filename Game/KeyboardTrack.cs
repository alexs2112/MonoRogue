using System.Linq;
using Microsoft.Xna.Framework.Input;

namespace MonoRogue {
    public class KeyboardTrack {
        // A class to keep track of which keys are pressed and held between frames

        private Keys[] LastKeys { get; set; }
        private Keys[] Keys { get; set; }

        public KeyboardTrack() {
            LastKeys = new Keys[0];
            Keys = new Keys[0];
        }

        public void Update(Keys[] pressed) {
            LastKeys = Keys;
            Keys = pressed;
        }

        public bool KeyJustPressed() {
            return !Keys.SequenceEqual(LastKeys);
        }

        public bool KeyJustPressed(Keys key) {
            return Keys.Contains(key) && !LastKeys.Contains(key);
        }
    }
}
