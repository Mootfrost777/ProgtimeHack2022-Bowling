using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Bowling.Classes;
using Bowling.Classes.UI;
using System.Collections.Generic;
using System.Windows.Forms;
using NetLib;

namespace Bowling
{
    public enum GameState
    {
        Menu, Game, Exit, Connect_to_server
    }
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private static int gutter_height;
        private static int gutter_top_y;
        private static int gutter_bottom_y;
        private Texture2D whiteRectangle;
        private Ball ball;

        private int knockedPins;

        private List<Pin> pins = new List<Pin>();

        public static GameState gameState = GameState.Connect_to_server;

        Player player;

        Menu menu;

        public static int Gutter_height { get { return gutter_height; } }
        public static int Gutter_top_y { get { return gutter_top_y; } }
        public static int Gutter_bottom_y { get { return gutter_bottom_y; } }

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = 1500;
            _graphics.PreferredBackBufferHeight = 900;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            gutter_top_y = _graphics.PreferredBackBufferHeight / 2 - 50;
            gutter_bottom_y = _graphics.PreferredBackBufferHeight - 100;
            gutter_height = 20;
            ball = new Ball(new Vector2(10, gutter_top_y + (gutter_bottom_y - gutter_top_y) / 2 - 25), Vector2.Zero, Color.Blue, Gutter_top_y, gutter_bottom_y, gutter_height);
            menu = new Menu();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            Texture2D keggle_texture = Content.Load<Texture2D>("keggle");
            int count = 1;
            int modifier = 2;
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < count; j++)
                {
                    pins.Add(new Pin(keggle_texture, new Vector2(_graphics.PreferredBackBufferWidth - 150 * modifier + i * keggle_texture.Width * modifier, j * keggle_texture.Height * modifier + gutter_top_y - keggle_texture.Height * count / 2 * modifier + keggle_texture.Height * 6 + 30)));
                }
                count += 1;
            }
            ball.LoadContent(Content);
            whiteRectangle = new Texture2D(GraphicsDevice, 1, 1);
            whiteRectangle.SetData(new[] { Color.White });
            
            menu.LoadContent(Content);
        }

        protected override void Update(GameTime gameTime)
        {
            // TODO: Add your update logic here
            switch (gameState)
            {
                case GameState.Exit:
                    Exit();
                    break;
                case GameState.Menu:
                    menu.Update();
                    break;
                case GameState.Game:
                    UpdateGame(gameTime);
                    break;
                case GameState.Connect_to_server:
                    Connect connect = new Connect();
                    if (connect.ShowDialog() == DialogResult.OK)
                    {
                        gameState = GameState.Menu;
                        player = new Player(connect.name);  // Игрок, потом из него подключение к серверу
                        NetLib.NetLib.IP = connect.IP;
                        NetLib.NetLib.port = connect.Port;
                        NetLib.NetLib.Connect();
                        NetLib.NetLib.Send(player.Serialize());
                        System.Console.WriteLine(NetLib.NetLib.Receive());
                    }
                    else { gameState = GameState.Exit; }
                    break;
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.LightYellow);

            // TODO: Add your drawing code here
            _spriteBatch.Begin();
            switch (gameState)
            {
                case GameState.Game:
                    _spriteBatch.Draw(whiteRectangle, new Rectangle(0, gutter_top_y - gutter_height, 10000, gutter_height), Color.Aquamarine);
                    _spriteBatch.Draw(whiteRectangle, new Rectangle(0, gutter_bottom_y, 10000, gutter_height), Color.Aquamarine);
                    for (int i = 0; i < pins.Count; i++)
                    {
                        pins[i].Draw(_spriteBatch);
                    }
                    ball.Draw(_spriteBatch);
                    base.Draw(gameTime);
                    break;
                case GameState.Menu:
                    menu.Draw(_spriteBatch);
                    break;
            }
            
            base.Draw(gameTime);
            _spriteBatch.End();
        }

        private void UpdateGame(GameTime gameTime)
        {
            ball.Update();
            List<Pin> collides_keggles = ball.CollidesKeggles(pins);
            foreach (Pin pin in collides_keggles)
            {
                pin.IsVisible = false;
                knockedPins++;
            }
        }
    }
}