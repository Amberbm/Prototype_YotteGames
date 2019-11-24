using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using System;
using System.Collections.Generic;

namespace Prototype_YotteGames
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D  paddle;
        List<int> position ;
        int paddleSpeed = 3;
        int totalPaddles = 2;
        int iD=0;
        KeyboardState currentKeyboardState;
        SpriteFont font;
        int currentTime;

        IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = "IBoEtHAuwYAhmrUQhwyK3M5Uj0YSulczkipm2Cj9",
            BasePath = "https://prototype-yottegames.firebaseio.com/"

        };

        IFirebaseClient client;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        static void Main()
        {
            using (var game = new Game1())
                game.Run();
        }
        
      
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            
            spriteBatch = new SpriteBatch(GraphicsDevice);
            client = new FireSharp.FirebaseClient(config);

            
            currentKeyboardState = Keyboard.GetState();


            paddle = Content.Load<Texture2D>("rodeSpeler");
            font = Content.Load<SpriteFont>("SpelFont");
            
            AssignPaddle();



        }
        
        protected void HandleInput()
        {
            
            KeyboardState previousKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();

            if (currentKeyboardState.IsKeyDown(Keys.Up))
            {
                position[iD] -= paddleSpeed;

                SetData();
            }

            if (currentKeyboardState.IsKeyDown(Keys.Down))
            {
                position[iD] += paddleSpeed;

                SetData();
            }

            if (currentKeyboardState.IsKeyDown(Keys.Escape))
            {
                Exit();
            }
        }
        
        

        protected void SetData()
        {
            SetResponse setPosition = client.Set("position/" + iD , position[iD]);
        }

        protected void GetSetTime()
        {
            currentTime = (int)DateTime.Now.TimeOfDay.TotalSeconds;
            SetResponse setTime = client.Set("time/"+iD, currentTime);
        }
        
        protected void GetData()
        {
            position = client.Get("position").ResultAs<List<int>>();
            
        }

        protected void AssignPaddle()
        {
            List<int> lastOnline = client.Get("time/").ResultAs<List<int>>();
            currentTime = (int)DateTime.Now.TimeOfDay.TotalSeconds;
            for (int i = 1; i <=  totalPaddles; i++)
            {
                currentTime = (int)DateTime.Now.TimeOfDay.TotalSeconds;
                if (currentTime< lastOnline[i]  )
                {
                    currentTime += 86370;
                }
                if (lastOnline[i] + 30 < currentTime)
                {
                    iD = i;
                    SetResponse setPosition = client.Set("time/" + iD, currentTime);
                    position = client.Get("position").ResultAs<List<int>>();
                    break;
                }
            }
        }

        protected override void Update(GameTime gameTime)
        {
            
           // GetData();
            if (iD == 0)
                AssignPaddle();
            else
            {
                HandleInput();
                GetSetTime();
            }
           
        }
        

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();
            for (int i = 1; i < position.Count; i++)
                spriteBatch.Draw(paddle, new Vector2(i * 200, position[i]), Color.AliceBlue);
            spriteBatch.DrawString(font, iD.ToString(),  Vector2.Zero, Color.Purple);
            if (iD == 0)
                spriteBatch.DrawString(font, "No free paddles available", new Vector2(300, 300),Color.Purple);
            spriteBatch.End();
            
        }
    }
}
