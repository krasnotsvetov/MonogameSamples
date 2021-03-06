﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ascension.Engine.Core;
using Ascension.Engine.Core.Components;
using Ascension.Engine.Graphics.SceneSystem;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Ascension.Engine.Core.Systems.Content;

namespace Ascension.Engine.Graphics.Filters
{
    public class DefferedLightFilter : Filter
    {

        Effect effect;
        
        public DefferedLightFilter(RenderSystem renderSystem) : base(renderSystem)
        {

        }

        public override void Initialize()
        {
          
            base.Initialize();
        }

        public override void LoadContent()
        {
            base.LoadContent();

            effect = ContentContainer.Instance().GetEffect("Engine\\Shaders\\LightEffect");
        }


        public void Render(Texture2D diffuse, Texture2D normalMap, Texture2D lightMap, ReadOnlyCollection<Light> lights) 
        {

            spriteBatch.Begin(effect: effect); 
            effect.Parameters["ScreenWidth"].SetValue(width);
            effect.Parameters["ScreenHeight"].SetValue(height);
            effect.Parameters["LightCount"].SetValue(lights.Count);
            effect.Parameters["AmbientColor"].SetValue(Color.Black.ToVector3());
            effect.Parameters["NormalMap"].SetValue(normalMap);
            effect.Parameters["LightMap"].SetValue(lightMap);
           

            effect.Parameters["positionLight"].SetValue(lights.Select(t => (t.Parent as Entity).GlobalTransform.Position).ToArray());
            effect.Parameters["colorLight"].SetValue(lights.Select(t => t.LightColor).ToArray());
            effect.Parameters["invRadiusLight"].SetValue(lights.Select(t => t.InvRadius).ToArray());
            spriteBatch.Draw(diffuse, Vector2.Zero, Color.White);
            spriteBatch.End();    
        }
    }
}
