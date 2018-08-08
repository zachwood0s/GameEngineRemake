using ECS.Attributes;
using ECS.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineCore.Components.UI
{
    [Component]
    class UITextComponent: ICopyableComponent
    {
        public string Text { get; set; }
        public string FontName { get; set; }
        public SpriteFont LoadedFont { get; set; }
        public Color TextColor { get; set; }
        //colors 'n shit

        public IComponent Copy()
        {
            return new UITextComponent()
            {
                Text = Text,
                FontName = FontName,
                LoadedFont = LoadedFont,
                TextColor = TextColor
            };
        }
    }
}
