using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MonoRogue {
    public class MouseHandler {
        private MouseState State;
        private MouseState LastState;

        public MouseHandler() {
            State = Mouse.GetState();
        }

        public void Update() {
            LastState = State;
            State = Mouse.GetState();
        }

        public Point Position() {
            return new Point(State.X, State.Y);
        }

        public bool LeftClicked() {
            return State.LeftButton == ButtonState.Pressed && LastState.LeftButton != ButtonState.Pressed;
        }

        public bool RightClicked() {
            return State.RightButton == ButtonState.Pressed && LastState.RightButton != ButtonState.Pressed;
        }

        public bool ButtonClicked() {
            return LeftClicked() || RightClicked();
        }

        // Return the tile that the mouse is over in the worldview
        public Point GetTile(WorldView world) {
            Point p = GetViewTile(world);
            if (p.X == -1) { return p; }
            p.X += world.OffsetX;
            p.Y += world.OffsetY;
            if (!world.InBounds(p)) { return new Point(-1); }
            return p;
        }

        // Return the tile relative to the worldview
        public Point GetViewTile(WorldView world) {
            if (State.X < 0 || State.Y < 0 || State.X >= world.Width * 32 || State.Y >= world.Height * 32) {
                return new Point(-1, -1);
            }
            int x = State.X / 32;
            int y = State.Y / 32;
            return new Point(x, y);
        }
    }
}
