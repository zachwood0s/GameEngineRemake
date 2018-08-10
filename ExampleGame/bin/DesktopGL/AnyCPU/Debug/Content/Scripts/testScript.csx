using System;

var Update = new Action<Entity>(e =>
{
    e.UpdateComponent<Transform2DComponent>(comp =>
    {
        comp.Position = new Vector2(comp.Position.X + 1, comp.Position.Y);
    });
});