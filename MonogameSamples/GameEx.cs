﻿
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonogameSamples.Engine.Core;
using MonogameSamples.Engine.Core.Common;
using MonogameSamples.Engine.Core.Common.Collections;
using MonogameSamples.Engine.Core.Components;
using MonogameSamples.Engine.Core.Components.ParticleSystemComponent;
using MonogameSamples.Engine.Editor;
using MonogameSamples.Engine.Graphics;
using MonogameSamples.Engine.Graphics.SceneSystem;
using MonogameSamples.Engine.Graphics.Shaders;
using System;
using System.Collections.Generic;

namespace MonogameSamples
{

    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    /// 


    public class GameEx : Game
    {
        protected GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;



        public Dictionary<string, DrawableComponent> drawableComponents = new Dictionary<string, DrawableComponent>();
        public Dictionary<string, UpdateableComponent> updateableComponents = new Dictionary<string, UpdateableComponent>();
        public RenderSystem RenderSystem { get { return renderSystem; } }
        public UpdateSystem UpdateSystem { get { return updateSystem; } }


        public RenderSystem renderSystem;
        public UpdateSystem updateSystem;


        Scene scene;




        public GameEx()
        {
            
            graphics = new GraphicsDeviceManager(this);
            IsMouseVisible = true;
            Content.RootDirectory = "Content";
        }



        protected override void Initialize()
        {

            graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();

            base.Initialize();
        }

        Entity entity;
        Entity entity2;
        Entity entity3;
        Entity entity4;
        Entity particleEntity;
        SpriteFont spriteFont;


        protected override void LoadContent()
        {
            renderSystem = new RenderSystem(GraphicsDevice);
            renderSystem.Initialize();
            updateSystem = new UpdateSystem();
            updateSystem.Initialize();
            drawableComponents.Add("RenderSystem", renderSystem);
            updateableComponents.Add("UpdateSystem", updateSystem);
            scene = new Scene("Scene0", renderSystem);
            updateSystem.AddComponent(new KeyValuePair<string, UpdateableComponent>("GlobalSceneUpdater", scene.sceneUpdater));

            foreach (var dc in drawableComponents.Values)
            {
                dc.LoadContent(Content);
            }

            Matrix test = Matrix.CreateRotationZ(-MathHelper.PiOver4) * Matrix.CreateTranslation(new Vector3(5, -2, 0));
            Matrix test2 = Matrix.CreateRotationZ(-MathHelper.PiOver2) * Matrix.CreateTranslation(new Vector3(-9, 6, 0));


            Matrix localWorld = Matrix.Invert(test) * test2;
            Vector3 pos = test2.Translation;

            Vector3 posLocal = Vector3.Transform(new Vector3(1, 1, 0), test);

            Vector3 posLocal2 = Vector3.Transform(new Vector3(6.41f, -2, 0), Matrix.Invert(test));

            Vector3 ttt = Vector3.Transform(Vector3.Zero, Matrix.CreateTranslation(1, 0, 0));
            spriteBatch = new SpriteBatch(GraphicsDevice);



            spriteFont = Content.Load<SpriteFont>("font");
            Effect effect = Content.Load<Effect>("mainShader");
            Effect effect2 = Content.Load<Effect>("mainShader2");
            Effect particleEffect = Content.Load<Effect>("shaders\\particleShader");

            Texture2D baseTexture = Content.Load<Texture2D>("house1Live");
            Texture2D baseNormal = Content.Load<Texture2D>("house1Normal");
            Texture2D particleTexture = Content.Load<Texture2D>("ParticleSystem2D\\playerParticle");

                
            entity = new Entity(scene);
            entity2 = new Entity(scene);
            entity3 = new Entity();
            entity4 = new Entity();

    
           

            
            Light light = new Light("Light0", null);
            Light light2 = new Light("Light1", null);
            Light light3 = new Light("Light2", null);

          

            light2.LightColor = Color.CornflowerBlue.ToVector3();
            light3.LightColor = Color.Green.ToVector3();

            //We can't add drawable component to Entity which doesn't have Scene.
           entity2.AddDrawableComponent(light);
            entity3.AddDrawableComponent(light2);
           entity4.AddDrawableComponent(light3);

            entity.Transform.Position = new Vector3(250, 0, 0);

            entity2.Transform.SetTransform(new Vector3(250, 0, 0), Vector3.Zero, new Vector3(1, 1, 1));
            entity3.Transform.SetTransform(new Vector3(250, 0, 0), Vector3.Zero, new Vector3(1, 1, 1));
            entity4.Transform.SetTransform(new Vector3(250, 0, 0), Vector3.Zero, new Vector3(1, 1, 1));

            IPipelineStateSetter normalPSSetter = new NormalShaderPipelineStateSetter();
            normalPSSetter.Initialize(effect);

            IPipelineStateSetter particlePSSetter = new ParticleShaderPipelineStateSetter();
            particlePSSetter.Initialize(effect2);

            scene.Shaders.Add(new ShaderReference("NormalShader"), new Pair<Effect, IPipelineStateSetter>(effect, normalPSSetter));
            scene.Shaders.Add(new ShaderReference("ParticleShader"), new Pair<Effect, IPipelineStateSetter>(effect2, particlePSSetter));


            scene.Materials.Add(new MaterialReference("houseMaterial"), new Material(new[] { baseTexture, baseNormal }, new ShaderReference("NormalShader")));
            
            

            Sprite sprite = new Sprite("Sprite0", new MaterialReference("houseMaterial"));


            entity3.AddEntity(entity4);

           // scene2D.AddEntity(entity3);

            entity2.AddEntity(entity3);

            // If you need to skip texture in Material, for example, Normal Map, use this construction:
            // sprite.Material = new Material(baseTexture, effect, m => m.effect.Parameters["NormalMap"].SetValue((Texture2D)null));
            // You MUST set all values in your shader, but some values can be NullPointer (defualt value)

            scene.Materials.Add(new MaterialReference("PSMaterial"),
                new Material(new[] { particleTexture, null }, new ShaderReference("ParticleShader")));

           
            particleEntity = new Entity(scene);
            ParticleSystem2D ps = new ParticleSystem2D("ParticleSystem0", 10f, 1, new MaterialReference("PSMaterial"));

            particleEntity.AddDrawableComponent( ps);

            //sprite.material = new material(basetexture, effect2,
            //    m =>
            //    {
            //        m.effect.parameters["screenwidth"].setvalue((float)graphicsdevice.viewport.width);
            //        m.effect.parameters["screenheight"].setvalue((float)graphicsdevice.viewport.height);
            //        m.effect.parameters["normalmap"].setvalue(m.textures[1]);

            //    });

            



            entity.AddDrawableComponent(sprite);

            //entity4.AddEntity(particleEntity);

        }

        protected override void UnloadContent()
        {
            foreach (var c in drawableComponents.Values)

            {
                c.Dispose();
            }

            foreach (var c in updateableComponents.Values)
            {
                c.Dispose();
            }
        }

        protected override void Update(GameTime gameTime)
        {


            this.Window.Title = Mouse.GetState().ScrollWheelValue.ToString();
            this.Window.Title = entity3.Transform.ToGlobalPosition(new Vector3(250, 0, 0)).ToString() + " " + Mouse.GetState().Position.ToString();
            foreach (var uc in updateableComponents.Values)
            {
                uc.Update(gameTime);
            }

            if (Mouse.GetState().LeftButton == ButtonState.Pressed && Keyboard.GetState().IsKeyDown(Keys.LeftControl))
            {
                Matrix world = entity2.Parent == null ? Matrix.Identity : entity2.Parent.GlobalTransform.World;
                entity2.Transform.Position = Vector3.Transform(new Vector3(Mouse.GetState().Position.ToVector2(), 0), Matrix.Invert(world));
            }

            if (Mouse.GetState().LeftButton == ButtonState.Pressed && Keyboard.GetState().IsKeyDown(Keys.LeftAlt))
            {
                
                entity2.GlobalTransform.Position = new Vector3(Mouse.GetState().Position.ToVector2(), 0);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.R))
            {
                entity2.GlobalTransform.Scale += new Vector3(0.02f, 0, 0f);
            }


            if (Keyboard.GetState().IsKeyDown(Keys.Q))
            {
                entity2.GlobalTransform.Rotation += new Vector3(0f, 0f, 0.02f);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.P))
            {
                entity3.GlobalTransform.Rotation += new Vector3(0f, 0f, 0.02f);
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            foreach (var dc in drawableComponents.Values)
            {
                dc.Draw(gameTime);
            }


            spriteBatch.Begin();
            spriteBatch.DrawString(spriteFont, "Hello, World", Vector2.Zero, Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }

}
