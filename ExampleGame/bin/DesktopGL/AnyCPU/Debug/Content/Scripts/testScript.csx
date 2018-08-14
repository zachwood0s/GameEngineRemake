using System;
using ECS.Entities;
using EngineCore.Components;
using Microsoft.Xna.Framework;
using EngineCore.Systems.Global.InputManager;


var Update = new Action<Entity>(e =>
{
    e.UpdateComponents<Transform2DComponent, BasicTextureComponent>((comp, tex) =>
    {
        comp.Position = Vector2.Add(comp.Position, -InputManager.GetAxesAsVector("horizontal", "vertical"));
        //comp.Position = new Vector2(comp.Position.X + InputManager.GetAxis("horizontal"), comp.Position.Y);
    });
});

var Update2 = new Action<Entity>(e =>
{
    e.UpdateComponent<Transform2DComponent>(comp =>
    {
        comp.Position = new Vector2(comp.Position.X + 5, comp.Position.Y);
    });
});