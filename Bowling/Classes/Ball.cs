using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Bowling.Classes
{
    public enum State { Waiting, Flying, InGutter }
    class Ball
    {
        private Vector2 position;
        private int radius;
        private Texture2D texture;
        private Vector2 speed;
        private Color color;
        private State state;
        private int gutter_top_y;
        private int gutter_bottom_y;
        private int gutter_height;
        private Arrow arrow;

        public Vector2 Speed { get { return speed; } set { speed = value; } }

        public Ball(Vector2 position, Vector2 speed, Color color, int gutter_top_y, int gutter_bottom_y, int gutter_height)
        {
            this.position = position;
            this.speed = speed;
            this.color = color;
            texture = null;
            this.gutter_top_y = gutter_top_y;
            this.gutter_bottom_y = gutter_bottom_y;
            this.gutter_height = gutter_height;
            state = State.Waiting;
            arrow = new Arrow(new Vector2(60, gutter_top_y + (gutter_bottom_y - gutter_top_y) / 2));
        }

        public void LoadContent(ContentManager manager)
        {
            arrow.LoadContent(manager);
            texture = manager.Load<Texture2D>("ball");
            radius = texture.Width / 2;
        }

        public void Draw(SpriteBatch brush)
        {
            arrow.Draw(brush);
            brush.Draw(texture, position, color);   
        }

        public void Update()
        {
            arrow.Update();
            switch (state)
            {
                case State.Waiting:
                    if (Keyboard.GetState().IsKeyDown(Keys.Space))
                    {
                        state = State.Flying; 
                        AwfulMath();
                    }
                    break;
                case State.Flying:
                    Go(); break;
                case State.InGutter:
                    GoInGutter(); break;
            }
        }

        private void Go()
        { 
            position += speed;
            if (position.Y <= gutter_top_y - gutter_height)
            {
                state = State.InGutter; 
                speed.Y = 0; 
                position.Y = gutter_top_y - gutter_height;
            }
            if (position.Y >= gutter_bottom_y)
            {
                state = State.InGutter;
                speed.Y = 0;
                position.Y = gutter_bottom_y;
            }
        }

        private void GoInGutter()
        {
            position += speed;
        }

        private void AwfulMath()
        {
            speed.X = (float)Math.Sqrt(36 / ((1 + Math.Tan(arrow.Rotation) * (1 + Math.Tan(arrow.Rotation)))));
            speed.Y = (float)(speed.X * Math.Tan(arrow.Rotation));
        }

        public List<Pin> CollidesKeggles(List<Pin> pins)
        {
            List<Pin> collides_keggles = new List<Pin>();

            foreach (Pin Pin in pins)
            {
                if (!Pin.IsVisible) continue;
                if (Math.Sqrt((position.X - Pin.Position.X) * (position.X - Pin.Position.X) + (position.Y - Pin.Position.Y) * (position.Y - Pin.Position.Y)) <= radius + Pin.Size.X / 2)
                {
                    collides_keggles.Add(Pin);
                }
            }
            return collides_keggles;
        }
    }
}
