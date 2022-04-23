using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Bowling.Classes
{
    public enum Condition { Move, Stand }
    class Arrow
    {
        private Vector2 position;
        private Vector2 start_position;
        private float rotation;
        private Texture2D texture;
        private Condition condition;
        private Rectangle destinationRectangle;
        private float speed_of_rotation;
        private Vector2 speed_of_position;

        public float Rotation { get { return rotation; } }

        public Arrow(Vector2 position)
        {
            start_position = position;
            this.position = new Vector2(position.X, position.Y + 15);
            condition = Condition.Move;
            rotation = 0.785f;
            speed_of_rotation = -0.0174f;
            speed_of_position = new Vector2(0.33f, -0.33f);
        }

        public void LoadContent(ContentManager manager)
        {
            texture = manager.Load<Texture2D>("arrow");       
        }

        public void Draw(SpriteBatch brush)
        {
            destinationRectangle = new Rectangle(Convert.ToInt32(position.X), Convert.ToInt32(position.Y), texture.Width, texture.Height);
            Vector2 origin = new Vector2(destinationRectangle.Width / 2, destinationRectangle.Height / 2);

            brush.Draw(texture, destinationRectangle, null, Color.White, rotation, origin, SpriteEffects.None, 1);
        }

        public void Update()
        {
            if (condition == Condition.Stand) return;
            // rotation
            rotation += speed_of_rotation;
            if (rotation >= 0.785f || rotation <= -0.785f) speed_of_rotation *= -1;

            // movement
            position += speed_of_position;
            if (position.X >= start_position.X || position.X <= start_position.X - 30) speed_of_position.X *= -1;
            if (position.Y >= start_position.Y + 15 || position.Y <= start_position.Y - 15) speed_of_position.Y *= -1;

            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                condition = Condition.Stand;
            }
        }
    }
}
