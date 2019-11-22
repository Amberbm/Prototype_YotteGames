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
        int[] position= new int[3];
        int paddleSpeed = 9;
        int iD=0;
        KeyboardState currentKeyboardState;
        SpriteFont font;
        int players = 2;

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


            Random random = new Random();
            Color color = new Color(random.Next(0, 250), random.Next(0, 250), random.Next(0, 250));

            currentKeyboardState = Keyboard.GetState();

            paddle = Content.Load<Texture2D>("rodeSpeler");
            font = Content.Load<SpriteFont>("SpelFont");

            // List<String> players = client.Get("ID").ResultAs<List<String>>();

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
                if (previousKeyboardState.IsKeyUp(Keys.Escape))
                    Leave();

        }
        
        protected void Leave()
        {
            SetResponse offline = client.Set("ID/" + iD+"/online",false);
            Exit();
        }

        protected void SetData()
        {
            SetResponse setPosition = client.Set("ID/" + iD + "/position", position[iD]);

        }
        
        protected void GetData()
        {
            for (int i = 1; i <= players; i++)
            {
                position[i] = client.Get("ID/" + i + "/position").ResultAs<List<int>>()[0];
            }
            
        }

        protected void AssignPaddle()
        {
            for (int i = 1; i <= players; i++)
            {
                List<Boolean> on = client.Get("ID/" + i + "/online").ResultAs<List<Boolean>>();
                if (!on[0])
                {
                    iD = i;
                    position[iD] = client.Get("ID/" + iD + "/position").ResultAs<List<int>>()[0];
                    SetResponse setonline = client.Set("ID/" + iD + "/online", true);
                    break;
                }
            }
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
           
            if (iD == 0)
                AssignPaddle();
             else
                 HandleInput();

            //GetData();
           
        }
        

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();
            for (int i = 1; i <= players; i++)
                spriteBatch.Draw(paddle, new Vector2(i * 200, client.Get("ID/" + i + "/position").ResultAs<List<int>>()[0]), Color.AliceBlue);
            if (iD == 0)
                spriteBatch.DrawString(font, "No free paddles available", new Vector2(300, 300),Color.Purple);
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
