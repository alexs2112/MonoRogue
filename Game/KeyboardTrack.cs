using System.Linq;
using Microsoft.Xna.Framework.Input;

namespace MonoRogue {
    public class KeyboardTrack {
        // A class to keep track of which keys are pressed and held between frames

        private Keys[] LastKeys { get; set; }
        private Keys[] CurrentKeys { get; set; }
        private long TicksHeld;

        public KeyboardTrack() {
            LastKeys = new Keys[0];
            CurrentKeys = new Keys[0];
        }

        public void Update(Keys[] pressed, System.TimeSpan elapsed) {
            LastKeys = CurrentKeys;
            CurrentKeys = pressed;

            if (LastKeys.Count() > 0 && CurrentKeys.Count() > 0) { 
                if (LastKeys[0] == CurrentKeys[0]) {
                    TicksHeld += elapsed.Ticks;
                    if (TicksHeld >= Constants.TickHoldRate) {
                        TicksHeld = 0;
                        CurrentKeys = new Keys[0];
                    }
                } else { TicksHeld = 0; }
            } else { TicksHeld = 0; }
        }

        public bool KeyJustPressed() {
            return CurrentKeys.Count() > 0 && !CurrentKeys.SequenceEqual(LastKeys);
        }

        public bool KeyJustPressed(Keys key) {
            return CurrentKeys.Contains(key) && !LastKeys.Contains(key);
        }

        // A bunch of movement commands
        public bool MovementNPressed() {
            return
                KeyJustPressed(Keys.Up) ||
                KeyJustPressed(Keys.D8) ||
                KeyJustPressed(Keys.K) ||
                KeyJustPressed(Keys.NumPad8);
        }
        public bool MovementSPressed() {
            return
                KeyJustPressed(Keys.Down) ||
                KeyJustPressed(Keys.D2) ||
                KeyJustPressed(Keys.J) ||
                KeyJustPressed(Keys.NumPad2);
        }
        public bool MovementEPressed() {
            return
                KeyJustPressed(Keys.Right) ||
                KeyJustPressed(Keys.D6) ||
                KeyJustPressed(Keys.L) ||
                KeyJustPressed(Keys.NumPad6);
        }
        public bool MovementWPressed() {
            return
                KeyJustPressed(Keys.Left) ||
                KeyJustPressed(Keys.D4) ||
                KeyJustPressed(Keys.H) ||
                KeyJustPressed(Keys.NumPad4);
        }
        public bool MovementNEPressed() {
            return
                KeyJustPressed(Keys.PageUp) ||
                KeyJustPressed(Keys.D9) ||
                KeyJustPressed(Keys.U) ||
                KeyJustPressed(Keys.NumPad9);
        }
        public bool MovementNWPressed() {
            return
                KeyJustPressed(Keys.Home) ||
                KeyJustPressed(Keys.D7) ||
                KeyJustPressed(Keys.Y) ||
                KeyJustPressed(Keys.NumPad7);
        }
        public bool MovementSEPressed() {
            return
                KeyJustPressed(Keys.PageDown) ||
                KeyJustPressed(Keys.D3) ||
                KeyJustPressed(Keys.N) ||
                KeyJustPressed(Keys.NumPad3);
        }
        public bool MovementSWPressed() {
            return
                KeyJustPressed(Keys.End) ||
                KeyJustPressed(Keys.D1) ||
                KeyJustPressed(Keys.B) ||
                KeyJustPressed(Keys.NumPad1);
        }
        public bool WaitPressed() {
            return
                KeyJustPressed(Keys.OemPeriod) ||
                KeyJustPressed(Keys.D5) ||
                KeyJustPressed(Keys.NumPad5);
        }
    }
}
