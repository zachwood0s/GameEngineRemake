using System;
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
            ComponentPool.RegisterAllComponents();
            var loader = new EntityBuilderLoader(new System.Collections.Generic.Dictionary<string, ECS.Entities.EntityBuilder>());
            loader.RootDirectory = "Content/EntityBuilders";
            loader.Initialize();
           // using (var game = new TestGame())
            //    game.Run();
        }
    }
}
