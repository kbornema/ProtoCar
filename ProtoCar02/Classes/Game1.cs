﻿using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using SharpDX.Toolkit.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtoCar
{
    class Game1 : Game
    {
        //TODO: dont make everything public static... but for prototyping its the easiest way
        public static GraphicsDeviceManager gManager;

        public static SpriteBatch spriteBatch;

        public static KeyboardManager keyboardManager;
        public static KeyboardState keyboardState;

        public static MouseManager mouseManager;
        public static MouseState mouseState;

        public static int width = 800;
        public static int height = 600;

        public static BlendState alphaBlend;
        public static BlendState opaqueBlend;

        public static bool active = false;

        public static Texture2D stoneTexture;
        public static Texture2D hud;
        public static SpriteFont font;

        public static Model skydome;

        //assign this variable, to start with a different gameState
        EGameState currentState = EGameState.Sandbox;
        EGameState prevState;

        IGameState gameState;

        public Game1()
        {
            gManager = new GraphicsDeviceManager(this);
            keyboardManager = new KeyboardManager(this);
            mouseManager = new MouseManager(this);

            Content.RootDirectory = "Content";

            gManager.PreferredBackBufferWidth = width;
            gManager.PreferredBackBufferHeight = height;

        }

        private void gainedFocus(object sender, EventArgs e)
        {
            active = true;
        }

        private void lostFocus(object sender, EventArgs e)
        {
            active = false;
        }

        protected override void Initialize()
        {
            Window.Title = "ProtoCar";
            Window.Deactivated += lostFocus;
            Window.Activated += gainedFocus;

            spriteBatch = new SpriteBatch(GraphicsDevice);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            font = Content.Load<SpriteFont>("Arial16");
            stoneTexture = Content.Load<Texture2D>("Stone");
            hud = Content.Load<Texture2D>("hud");
            skydome = Content.Load<Model>("skydome");

          //  BasicEffect.EnableDefaultLighting(skydome, true);

            alphaBlend = BlendState.New(GraphicsDevice, SharpDX.Direct3D11.BlendOption.SourceAlpha,         //sourceBlend
                                                  SharpDX.Direct3D11.BlendOption.InverseSourceAlpha,         //destinationBlend
                                                  SharpDX.Direct3D11.BlendOperation.Add,              //blendoperation
                                                  SharpDX.Direct3D11.BlendOption.SourceAlpha,    //source alphaBlend
                                                  SharpDX.Direct3D11.BlendOption.InverseSourceAlpha,         //destination alpha blend
                                                  SharpDX.Direct3D11.BlendOperation.Add,              //alphablend operation
                                                  SharpDX.Direct3D11.ColorWriteMaskFlags.All, -1       //rendertarget mask
                                                  );

            var blendStateDesc = SharpDX.Direct3D11.BlendStateDescription.Default();
            opaqueBlend = BlendState.New(GraphicsDevice, "Opaque", blendStateDesc);


            handleNewGameState();

        }

        protected override void Update(GameTime gameTime)
        {
            keyboardState = keyboardManager.GetState();
            mouseState = mouseManager.GetState();


            if (keyboardState.IsKeyDown(Keys.Escape))
                Exit();

            currentState = gameState.update(gameTime);

            if (currentState != prevState)
                handleNewGameState();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {

            gameState.draw(gameTime);

            base.Draw(gameTime);
        }

        private void handleNewGameState()
        {
            //gameState.unloadContent();

            switch (currentState)
            {
                case EGameState.InGame:
                    gameState = new InGame();
                    break;

                case EGameState.Sandbox:
                    gameState = new Sandbox();
                    break;

                default:
                    Exit();
                    break;
            }
            
            gameState.init();
            gameState.loadContent(Content);

            prevState = currentState;
        }



    }
}