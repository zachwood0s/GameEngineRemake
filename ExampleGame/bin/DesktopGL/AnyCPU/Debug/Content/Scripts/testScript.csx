#r "../../ECS.dll"
#r "../../EngineCore.dll"

using System;
using ECS.Entities;
using EngineCore.Components;
using Microsoft.Xna.Framework;
using EngineCore.Systems.Global.InputManager;


var Update = new Action<Entity>(e =>
{
    e.UpdateComponents<Transform2DComponent, BasicTextureComponent>((comp, tex) =>
    {
        comp.Position += InputManager.GetAxesAsVector("horizontal", "vertical");
    });
});


var Update2 = new Action<Entity>(e =>
{
    e.UpdateComponent<Transform2DComponent>(comp =>
    {
        
        comp.Position = new Vector2(comp.Position.X + 5, comp.Position.Y);
    });
});