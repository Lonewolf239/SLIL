﻿using System.Collections.Generic;
using SLIL_v0_1.MazeGenerator.Enum;

namespace SLIL_v0_1.MazeGenerator
{
    public class Room
    {
        public int X { get; set; }

        public int Y { get; set; }

        public List<Direction> Links { get; set; } = new List<Direction>();
    }
}