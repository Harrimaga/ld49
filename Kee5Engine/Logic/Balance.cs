using System;
using System.Collections.Generic;
using System.Text;

namespace Kee5Engine
{
    public static class Balance
    {
        // Player Stats
        public const int speed = 600;
        public const int maxSpeed = 1000;
        public const int gravity = 3000;
        public const int baseResistance = 10;
        public const int jumpSpeed = 1100;
        public const int dashSpeed = 3000;
        public const double dashTime = 0.15;

        // Tile stats
        public const double decayTime = 0.5;
        public const float tileGravity = 200;
        public const float iceResistanceFactor = 0.1f;

        // Level order
        public static readonly string[] levels = { "Tut1", "Tut2", "lvl1", "lvl2", "lvl3", "lvl4" };
    }
}
