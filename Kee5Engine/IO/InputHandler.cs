using System;
using System.Collections.Generic;
using System.Text;

using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Input;

namespace Kee5Engine.IO
{

    public enum controllerKeys
    {
        A = 0,
        B = 1,
        X = 2,
        Y = 3,
        L = 4,
        R = 5,
        LT = 6,
        RT = 7,
        LEFT_ANALOG = 8,
        RIGHT_ANALOG = 9
    }
    public class InputHandler
    {
        private KeyboardState state, prevstate;
        private MouseState mState, prevmstate;
        private JoystickState jState, prevjstate;

        public void Update(KeyboardState kstate, MouseState mstate, JoystickState jstate)
        {
            prevstate = state;
            prevmstate = mState;
            prevjstate = jState;
            state = kstate;
            mState = mstate;
            jState = jstate;

            for (int i = 0; i < 13; i++)
            {
                if (IsButtonDown(i))
                {
                    Console.WriteLine(i);
                }
            }
        }

        public bool IsKeyDown(Keys key)
        {
            return state.IsKeyDown(key);
        }

        public bool IsButtonDown(int key)
        {
            return jState.IsButtonDown(key);
        }

        public bool IsKeyPressed(Keys key)
        {
            return state.IsKeyDown(key) && !prevstate.IsKeyDown(key);
        }

        public bool IsButtonPressed(int key)
        {
            return jState.IsButtonDown(key) && !prevjstate.IsButtonDown(key);
        }

        public bool IsKeyReleased(Keys key)
        {
            return state.IsKeyReleased(key);
        }

        public bool IsButtonReleased(int key)
        {
            return !jState.IsButtonDown(key) && prevjstate.IsButtonDown(key);
        }

        public bool IsAnyKeyDown()
        {
            return state.IsAnyKeyDown;
        }
    }
}
