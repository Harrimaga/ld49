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
        public static readonly string[] levels = { "Tut1", "Tut2", "lvl4", "lvl3", "lvl7", "lvl2", "lvl1", "lvl5", "lvl6", "lvl8", "lvl9", "lvl10" };
        public static readonly string[] levelNames = { "Tutorial 1", "Tutorial 2", "Level 1", "Level 2", "Level 3", "Level 4", "Level 5", "Level 6", "Level 7", "Level 8", "Level 9", "Level 10" };
    }
}
