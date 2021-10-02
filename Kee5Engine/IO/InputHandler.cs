using System;
using System.Collections.Generic;
using System.Text;

using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Input;

namespace Kee5Engine.IO
{

    public enum ControllerKeys
    {
        A = 0,
        B = 1,
        X = 2,
        Y = 3,
        L = 4,
        R = 5,
        SELECT = 6,
        START = 7,
        LEFT_ANALOG = 8,
        RIGHT_ANALOG = 9,
        UP = 10,
        RIGHT = 11,
        DOWN = 12,
        LEFT = 13
    }

    public enum ControllerAngle
    {
        LEFT,
        RIGHT,
        UP,
        DOWN
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

            //Console.WriteLine($"{IsLeftStickAngle(ControllerAngle.UP)} {IsLeftStickAngle(ControllerAngle.DOWN)} {IsLeftStickAngle(ControllerAngle.LEFT)} {IsLeftStickAngle(ControllerAngle.RIGHT)}");
            //Console.WriteLine($"({jState.GetAxis(0)}, {jState.GetAxis(1)})");
        }

        public bool IsKeyDown(Keys key)
        {
            return state.IsKeyDown(key);
        }

        public bool IsButtonDown(ControllerKeys key)
        {
            return jState.IsButtonDown((int)key);
        }

        public bool IsKeyPressed(Keys key)
        {
            return state.IsKeyDown(key) && !prevstate.IsKeyDown(key);
        }

        public bool IsButtonPressed(ControllerKeys key)
        {
            return jState.IsButtonDown((int)key) && !prevjstate.IsButtonDown((int)key);
        }

        public bool IsKeyReleased(Keys key)
        {
            return state.IsKeyReleased(key);
        }

        public bool IsButtonReleased(ControllerKeys key)
        {
            return !jState.IsButtonDown((int)key) && prevjstate.IsButtonDown((int)key);
        }

        public bool IsLeftStickAngle(ControllerAngle angle)
        {
            if (jState.GetAxis(0) > 0.2f)
            {
                if (jState.GetAxis(1) > jState.GetAxis(0))
                {
                    return angle == ControllerAngle.DOWN;
                }
                return angle == ControllerAngle.RIGHT;
            }
            else if (jState.GetAxis(0) < -0.2f)
            {
                if (jState.GetAxis(1) < jState.GetAxis(0))
                {
                    return angle == ControllerAngle.UP;
                }
                return angle == ControllerAngle.LEFT;
            }
            else if (jState.GetAxis(1) > 0.2f)
            {
                return angle == ControllerAngle.DOWN;
            }
            else if (jState.GetAxis(1) < -0.2f)
            {
                return angle == ControllerAngle.UP;
            }
            return false;
        }

        public bool IsAnyKeyDown()
        {
            return state.IsAnyKeyDown;
        }
    }
}
