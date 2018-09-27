//#r "../../ECS.dll"
//#r "../../EngineCore.dll"

using System;
using ECS.Entities;
using EngineCore.Utils;
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
        text.Text += "C";
    });
    if (!e.HasComponent<EaseComponent>())
    {
        var text = e.GetComponent<UITransformComponent>();
        var x = text.Position.X;
        EaseComponent ease = new EaseComponent()
        {
            EasingFunction = Easings.Functions.BackEaseInOut,
            StartValue = x,
            EndValue = 400 - x+200,
            EaseLength = 100,
            SetFunction = (double val) => text.Position = new Vector2((float) val, text.Position.Y)
        };
        e.With(ease);
    }
});

var OnMouseEnter = new Action<Entity>(e =>
{
    e.UpdateComponent<UITextComponent>(text =>
    {
        text.Text += "E";
    });
});
var OnMouseExit = new Action<Entity>(e =>
{
    e.UpdateComponent<UITextComponent>(text =>
    {
        text.Text += "e";
    });
});
var OnMouseDown = new Action<Entity>(e =>
{
    e.UpdateComponent<UITextComponent>(text =>
    {
        text.Text += "D";
    });
});
var OnMouseUp = new Action<Entity>(e =>
{
    e.UpdateComponent<UITextComponent>(text =>
    {
        text.Text += "U";
    });
});