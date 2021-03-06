﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ascension.Engine.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using Ascension.Engine.Core.Systems;

namespace Ascension.Engine.Graphics
{
    public class UpdateSystem : IUpdateableSystem
    {

        public List<UpdateableComponent> GameComponents
        {
            get
            {
                if (isDirty)
                {
                    _gameComponents.Sort();
                }
                return _gameComponents;
            }
        }

        public bool Enabled { get; set; } = true;


        private List<UpdateableComponent> _gameComponents = new List<UpdateableComponent>();

        private Dictionary<string, UpdateableComponent> gameComponents = new Dictionary<string, UpdateableComponent>();
 

        private bool isDirty = false;

        public UpdateSystem()
        {

        }



        public UpdateSystem(IEnumerable<KeyValuePair<string, UpdateableComponent>> gameComponents)
        {
            gameComponents.ToList().ForEach(pair =>
            {
                if (this.gameComponents.ContainsKey(pair.Key))
                {
                    this.gameComponents.Add(pair.Key, pair.Value);
                    _gameComponents.Add(pair.Value);
                    pair.Value.UpdateOrderChanged += (s, e) => isDirty = true;
                }
                else
                {
                    throw new UpdateSystemException("Double definiton of renderSystem component");
                }
            });
            _gameComponents.Sort();
        }



        public virtual void Initialize()
        {
            foreach (var gameComponent in gameComponents.Values)
            {
                gameComponent.Initialize();
            }
        }

        public void AddComponent(KeyValuePair<string, UpdateableComponent> updateableComponent)
        {
            if (gameComponents.ContainsKey(updateableComponent.Key))
            {
                throw new UpdateSystemException("Double definiton of renderSystem component");
            }
            updateableComponent.Value.Initialize();
            gameComponents.Add(updateableComponent.Key, updateableComponent.Value);
            _gameComponents.Add(updateableComponent.Value);
            updateableComponent.Value.UpdateOrderChanged += (s, e) => isDirty = true;
            isDirty = true;
        }


        public UpdateableComponent RemoveComponent(string drawableComponent)
        {
            if (!gameComponents.ContainsKey(drawableComponent))
            {
                return null;
            }
            var rv = gameComponents[drawableComponent];
            gameComponents.Remove(drawableComponent);
            _gameComponents.Remove(rv);
            return rv;
        }

        public virtual void Update(GameTime gameTime)
        {
            if (!Enabled)
            {
                return;
            }
            if (isDirty)
            {
                isDirty = false;
                _gameComponents.Sort();
            }

            foreach (var gameComponent in _gameComponents)
            {
                gameComponent.Update(gameTime);
            }
        }

        public virtual void Dispose()
        {
            foreach (var gameComponent in _gameComponents)
            {
                gameComponent.Dispose();
            }
        }
    }


    public class UpdateSystemException : Exception
    {
        public UpdateSystemException(string textMessage) : base(textMessage)
        {

        }
    }
}
