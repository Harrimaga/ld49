using System;
using System.Collections.Generic;
using System.Text;

using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Kee5Engine.IO
{
    public class InputHandler
    {
        private KeyboardState state, prevstate;
        private MouseState mState, prevmstate;

        public void Update(KeyboardState kstate, MouseState mstate)
        {
            prevstate = state;
            prevmstate = mState;
            state = kstate;
            mState = mstate;
        }

        public bool IsKeyDown(Keys key)
        {
            return state.IsKeyDown(key);
        }

        public bool IsKeyPressed(Keys key)
        {
            return state.IsKeyDown(key) && !prevstate.IsKeyDown(key);
        }

        public bool IsKeyReleased(Keys key)
        {
            return state.IsKeyReleased(key);
        }

        public bool IsAnyKeyDown()
        {
            return state.IsAnyKeyDown;
        }
    }
}
