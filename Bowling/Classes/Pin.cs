using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Bowling.Classes
{
    class Pin
    {
        private Texture2D texture;
        private Vector2 position;
        private bool isVisible;
        //private Rectangle rectangle;
        private Vector2 size;

        //private Rectangle boundingBox;

        //public Rectangle BoundingBox { get { return boundingBox; } }
        public Vector2 Size { get { return size; } }
        public Vector2 Position { get { return position; } }

        public bool IsVisible
        {
            get { return isVisible; }
            set { isVisible = value; }
        }

        public Pin(Texture2D texture, Vector2 position)
        {
            this.texture = texture;
            this.position = position;
            isVisible = true;

            size = new Vector2(texture.Width, texture.Height);
            //rectangle = new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (isVisible)
            {
                spriteBatch.Draw(texture, position, Color.White);
            }
        }
    }
}