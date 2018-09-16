//#r "../../ECS.dll"
//#r "../../EngineCore.dll"

using System;
using ECS.Entities;
using EngineCore.Components;
using EngineCore.Components.UI;
using Microsoft.Xna.Framework;
using EngineCore.Systems.Global.InputManager;


var Update = new Action<Entity>(e =>
{
    e.UpdateComponents<Transform2DComponent, BasicTextureComponent>((comp, tex) =>
    {
        comp.Position += InputManager.GetAxesAsVector("horizontal", "vertical") * 2;
    });
});

var OnClick = new Action<Entity>(e =>
{
    e.UpdateComponent<UITextComponent>(text =>
    {
        text.Text += ";";
    });

    e.UpdateComponent<UITransformComponent>(transform =>
    {
        transform.Position += new Vector2(100, 100);
    });
});

var OnMouseEnter = new Action<Entity>(e =>
{
    e.UpdateComponent<UITextComponent>(text =>
    {
        text.Text += "A";
    });
});
var OnMouseExit = new Action<Entity>(e =>
{
    e.UpdateComponent<UITextComponent>(text =>
    {
        text.Text += "Z";
    });
});