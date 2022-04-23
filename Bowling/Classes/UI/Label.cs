using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Bowling.Classes.UI
{
    class Label
    {
        private SpriteFont spriteFont;
        private Color color_default;
        public Vector2 Position { get; set; }
        public string Text { get; set; }
        public Color Color { get; set; }
        public string fontName = "gameFont";

        public string FontName { set { fontName = value; } }

        public Label()
        {
            Position = new Vector2(100, 100);
            Text = "Label";
            Color = Color.White;
            color_default = Color;
        }

        public Label(string Text, Vector2 Position, Color Color)
        {
            this.Text = Text;
            this.Position = Position;
            this.Color = Color;
            color_default = Color;
        }

        public void ResetColor()
        {
            Color = color_default;
        }


        public void LoadContent(ContentManager Content)
        {
            spriteFont = Content.Load<SpriteFont>(fontName);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(spriteFont, Text, Position, Color);
        }
    }
}