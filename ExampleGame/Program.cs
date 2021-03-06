﻿using System;
using EngineCore.Systems.Global.EntityBuilderLoader;
using ECS.Components;

namespace ExampleGame
{
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            using (var game = new TestGame())
                game.Run();
        }
    }
}
