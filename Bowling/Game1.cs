using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Bowling.Classes;
using Bowling.Classes.UI;
using System.Collections.Generic;
using System.Windows.Forms;
using NetLib;
using System.Threading;
using System.Text;
using System;

namespace Bowling
{
    public enum GameState
    {
        Menu, Game, Exit, Connect_to_server, PauseGame, EndGame
    }
    public class Game1 : Game
    {
        private Color[] colors = { Color.White/*, Color.Red, Color.Black, Color.DeepSkyBlue, Color.DeepPink*/ };
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private static int gutter_height;
        private static int gutter_top_y;
        private static int gutter_bottom_y;
        private Texture2D whiteRectangle;
        private Ball ball;
        private Player player1;
        private Player player2;
        private Menu menu;
        private int rowWidth;
        private int rowHeight;
        private int tableMarginTop;
        private int tableMarginLeft;
        private Vector2 ballStartPosition;
        private List<Classes.UI.Label> tableLabels;
        private int intermediateScore;
        private int counter;
        private Texture2D line_texture;
        private Random random;


        private List<Pin> pins = new List<Pin>();

        public static GameState gameState = GameState.Connect_to_server;

        public static int Gutter_height { get { return gutter_height; } }
        public static int Gutter_top_y { get { return gutter_top_y; } }
        public static int Gutter_bottom_y { get { return gutter_bottom_y; } }

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = 1500;
            _graphics.PreferredBackBufferHeight = 1000;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.Title = "Bowling";
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            gutter_top_y = _graphics.PreferredBackBufferHeight / 2 - 50;
            gutter_bottom_y = _graphics.PreferredBackBufferHeight - 100;
            gutter_height = 50;
            ballStartPosition = new Vector2(10, gutter_top_y + (gutter_bottom_y - gutter_top_y) / 2 - 25);
            random = new Random();
            ball = new Ball(ballStartPosition, Vector2.Zero, Color.White, Gutter_top_y, gutter_bottom_y, gutter_height, _graphics.PreferredBackBufferWidth);
            menu = new Menu();
            player2 = new Player();
            rowWidth = 80;
            rowHeight = 50;
            tableMarginTop = 20;
            tableMarginLeft = 20;
            tableLabels = new List<Classes.UI.Label>();
            intermediateScore = 0;
            counter = 0;

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
                    pins.Add(new Pin(keggle_texture, new Vector2(_graphics.PreferredBackBufferWidth - 150 * modifier + i * keggle_texture.Width * modifier, j * keggle_texture.Height * modifier + gutter_top_y - keggle_texture.Height * count / 2 * modifier + keggle_texture.Height * 5 + 40)));
                }
                count += 1;
            }
            ball.LoadContent(Content);
            whiteRectangle = new Texture2D(GraphicsDevice, 1, 1);
            whiteRectangle.SetData(new[] { Color.White });
            menu.LoadContent(Content);
            line_texture = Content.Load<Texture2D>("line");
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
                        player1 = new Player() { Name = connect.name, Score = new List<int>() };
                        NetLib.NetLib.IP = connect.IP;
                        NetLib.NetLib.port = connect.Port;
                        NetLib.NetLib.Connect();
                        SendPlayerData(player1);
                        player2.Deserialize(NetLib.NetLib.Receive());
                        Thread thread = new Thread(() =>
                        {
                            while (true)
                            {
                                string msg = NetLib.NetLib.Receive();
                                if (msg == "11")
                                {
                                    ResetAll();
                                }
                                else if (msg == "12")
                                {
                                    Exit();
                                }
                                else
                                {
                                    player2.Deserialize(msg);
                                }
                            }
                        });
                        thread.Start();
                    }
                    else { gameState = GameState.Exit; }
                    break;
                case GameState.EndGame:
                    counter++;
                    if (counter >= 300)
                    {
                        gameState = GameState.Menu;
                        counter = 0;
                        NetLib.NetLib.Send("11");
                    }
                    break;
                case GameState.PauseGame:
                    if (player2.Score.Count == 21 || (player2.Score.Count == 20 && player2.Score[18] + player2.Score[19] != 10)) gameState = GameState.EndGame;
                    if (tableLabels.Count >= 77) gameState = GameState.EndGame;
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
                    DrawGrid();
                    _spriteBatch.Draw(whiteRectangle, new Rectangle(0, gutter_top_y - gutter_height, 10000, gutter_height), Color.Gray);
                    _spriteBatch.Draw(whiteRectangle, new Rectangle(0, gutter_bottom_y, 10000, gutter_height), Color.Gray);
                    _spriteBatch.Draw(line_texture, new Rectangle(0, gutter_top_y, _graphics.PreferredBackBufferWidth, gutter_bottom_y - gutter_top_y), Color.White);
                    for (int i = 0; i < pins.Count; i++)
                    {
                        pins[i].Draw(_spriteBatch);
                    }
                    ball.Draw(_spriteBatch);
                    foreach (Classes.UI.Label lbl in tableLabels)
                    {
                        lbl.Draw(_spriteBatch);
                    }
                    break;
                case GameState.Menu:
                    menu.Draw(_spriteBatch);
                    break;
                case GameState.EndGame:
                    DrawGrid();
                    foreach (Classes.UI.Label lbl in tableLabels)
                    {
                        lbl.Draw(_spriteBatch);
                    }
                    break;
                case GameState.PauseGame:
                    DrawGrid();
                    foreach (Classes.UI.Label lbl in tableLabels)
                    {
                        lbl.Draw(_spriteBatch);
                    }
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
                intermediateScore++;
            }
            if (ball.State == State.Gone)
            {
                player1.Score.Add(intermediateScore);
                intermediateScore = 0;
                ball = new Ball(ballStartPosition, Vector2.Zero, Color.White, Gutter_top_y, gutter_bottom_y, gutter_height, _graphics.PreferredBackBufferWidth);
                ball.LoadContent(Content);
                if (player1.Score.Count <= 20 && player1.Score.Count % 2 == 0)
                {
                    NewMove();
                }
                else if (player1.Score.Count == 20)
                {
                    if (player1.Score[18] + player1.Score[19] == 10) NewMove();
                }
                if (player1.Score.Count == 21 || (player1.Score.Count == 20 && player1.Score[18] + player1.Score[19] != 10))
                {
                    gameState = GameState.PauseGame;
                    if (player2.Score.Count == 21 || (player2.Score.Count == 20 && player2.Score[18] + player2.Score[19] != 10)) gameState = GameState.EndGame;
                }
                SendPlayerData(player1);
            }
        }

        private void DrawGrid()
        {
            tableLabels.Clear();
            _spriteBatch.Draw(whiteRectangle, new Rectangle(tableMarginLeft, tableMarginTop, rowWidth * 13, rowHeight * 7), Color.RoyalBlue);
            _spriteBatch.Draw(whiteRectangle, new Rectangle(tableMarginLeft + rowWidth, tableMarginTop + rowHeight, rowWidth * 12, rowHeight * 6),
                Color.SkyBlue);
            int currentRow = 0;
            int currentColumn = 0;
            for (int i = 0; i < 6; i++)
            {
                _spriteBatch.Draw(whiteRectangle, new Rectangle((i == 0 || i % 2 == 1) ? tableMarginLeft : tableMarginLeft + rowWidth,
                    currentRow + tableMarginTop, (i == 0 || i % 2 == 1) ? 13 * rowWidth : 12 * rowWidth, 2), Color.Black);
                currentRow += (i != 0 && i % 2 == 0) ? 2 * rowHeight : rowHeight;
            }
            for (int i = 0; i < 13; i++)
            {
                _spriteBatch.Draw(whiteRectangle, new Rectangle(currentColumn + tableMarginLeft, tableMarginTop, 2, currentRow - rowHeight),
                    Color.Black);
                currentColumn += (i == 10) ? 2 * rowWidth : rowWidth;
            }
            for (int i = 1; i < 11; i++)
            {
                int shift = (i == 10) ? tableMarginLeft - 5 + rowWidth * i + rowWidth : tableMarginLeft - 5 + rowWidth * i + rowWidth / 2;
                Classes.UI.Label lbl = new Classes.UI.Label(i.ToString(), new Vector2(shift, tableMarginTop + 2), Color.White);
                lbl.LoadContent(Content);
                tableLabels.Add(lbl);
            }
            Classes.UI.Label label = new Classes.UI.Label("TTL", new Vector2(tableMarginLeft + currentColumn - 2 * rowWidth + 10, tableMarginTop + 5), Color.MonoGameOrange);
            label.LoadContent(Content);
            tableLabels.Add(label);
            for (int i = 0; i < player1.Score.Count; i++)
            {
                string score = (player1.Score[i] == 0) ? "-" : player1.Score[i].ToString();
                if (i != 0 && i % 2 == 1) score = (player1.Score[i] + player1.Score[i - 1] == 10) ? "/" : score;
                Classes.UI.Label lbl = new Classes.UI.Label(score, new Vector2(tableMarginLeft + rowWidth + i * (rowWidth / 2) + 10, tableMarginTop + rowHeight + 5),
                    Color.White);
                lbl.LoadContent(Content);
                tableLabels.Add(lbl);
            }
            for (int i = 0; i < player1.Score.Count / 2; i++)
            {
                int score = player1.Score[i * 2] + player1.Score[i * 2 + 1];
                Classes.UI.Label lbl = new Classes.UI.Label(score.ToString(), new Vector2(tableMarginLeft + rowWidth + i * rowWidth + 30, tableMarginTop + 2 * rowHeight + 25),
                    Color.White);
                lbl.FontName = "gameFont2";
                lbl.LoadContent(Content);
                tableLabels.Add(lbl);
            }
            int countRows = 1;
            for (int i = 0; i < 4; i++)
            {
                int score = 0;
                if (i % 2 == 0) score = 0;
                else if (i == 1) score = Sum(player1.Score);
                else score = Sum(player2.Score);
                Classes.UI.Label lbl = new Classes.UI.Label(score.ToString(), new Vector2(tableMarginLeft + rowWidth * 12 + rowWidth / 2 - 15, tableMarginTop + countRows + rowHeight + ((i % 2 == 1) ? 15 : 5)),
                    Color.White);
                if (i % 2 == 1) lbl.FontName = "gameFont2";
                lbl.LoadContent(Content);
                tableLabels.Add(lbl);
                countRows += (i != 0 && i % 2 == 1) ? 2 * rowHeight : rowHeight;
            }
            for (int i = 0; i < player2.Score.Count; i++)
            {
                string score = (player2.Score[i] == 0) ? "-" : player2.Score[i].ToString();
                if (i != 0 && i % 2 == 1) score = (player2.Score[i] + player2.Score[i - 1] == 10) ? "/" : score;
                Classes.UI.Label lbl = new Classes.UI.Label(score, new Vector2(tableMarginLeft + rowWidth + i * (rowWidth / 2) + 10, tableMarginTop + rowHeight * 4 + 5),
                    Color.White);
                lbl.LoadContent(Content);
                tableLabels.Add(lbl);
            }
            for (int i = 0; i < player2.Score.Count / 2; i++)
            {
                int score = player2.Score[i * 2] + player2.Score[i * 2 + 1];
                Classes.UI.Label lbl = new Classes.UI.Label(score.ToString(), new Vector2(tableMarginLeft + rowWidth + i * rowWidth + 30, tableMarginTop + 5 * rowHeight + 25),
                    Color.White);
                lbl.FontName = "gameFont2";
                lbl.LoadContent(Content);
                tableLabels.Add(lbl);
            }
            Classes.UI.Label label_p1_name = new Classes.UI.Label(player1.Name[0].ToString(), new Vector2(tableMarginLeft + 5, tableMarginTop + rowHeight),
                Color.White);
            label_p1_name.LoadContent(Content);
            tableLabels.Add(label_p1_name);

            Classes.UI.Label label_p2_name = new Classes.UI.Label(player2.Name[0].ToString(), new Vector2(tableMarginLeft + 5, tableMarginTop + 4 * rowHeight),
                Color.White);
            label_p2_name.LoadContent(Content);
            tableLabels.Add(label_p2_name);
        }

        private void NewMove()
        {
            pins.Clear();
            LoadContent();
        }

        private int Sum(List<int> score)
        {
            int sum = 0;
            for (int i = 0; i < score.Count; i++)
            {
                sum += score[i];
            }
            return sum;
        }

        private void SendPlayerData(Player player)
        {
            NetLib.NetLib.Send(player.Serialize());
        }

        private void ResetAll()
        {
            ball = new Ball(ballStartPosition, Vector2.Zero, Color.White, Gutter_top_y, gutter_bottom_y, gutter_height, _graphics.PreferredBackBufferWidth);
            menu = new Menu();
            player1.Score.Clear();
            player2.Score.Clear();
            rowWidth = 80;
            rowHeight = 50;
            tableMarginTop = 20;
            tableMarginLeft = 20;
            tableLabels = new List<Classes.UI.Label>();
            intermediateScore = 0;
            counter = 0;
            gameState = GameState.Menu;
            pins.Clear();
            LoadContent();
        }
    }
}