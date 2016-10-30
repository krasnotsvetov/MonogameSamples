﻿using Ascension.Engine.Core.Common.EventArguments;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ascension.Engine.Core.Common.Collections
{
    public class StringReferenceCollection<Reference, Value> : IResourceCollection<string, Reference, Value> where Reference : BaseReference<string>, new() where Value : class
    {

        public EventHandler<ResourceCollectionEventArgs<string>> CollectionChanged;

        private Dictionary<Reference, Value> items = new Dictionary<Reference, Value>();

        public StringReferenceCollection()
        {

        }

        public Value this[Reference r]
        {
            get
            {
                return FromReference(r);
            }
            set
            {
                if (items.ContainsKey(r))
                {
                    var lastValue = items[r];
                    items[r] = value;
                    if (!lastValue.Equals(value))
                    {
                        CollectionChanged?.Invoke(this, new ResourceCollectionEventArgs<string>(Operation.Replaced, r, lastValue, value));
                    }
                } else
                {
                    throw new IndexOutOfRangeException("Collection doesn't contain this key");
                }               
            }
        }

        public Value this[string r]
        {
            get
            {
                return FromIdentifier(r);
            }

            set
            {
                Reference _r = new Reference();
                _r.Name = r;
                this[_r] = value;
            }
        }

        public bool Add(Reference r, Value v)
        {
            if (items.ContainsKey(r))
            {
                return false;
            }

            items.Add(r, v);

            CollectionChanged?.Invoke(this, new ResourceCollectionEventArgs<string>(Operation.Added, r, null, v));
            return true;
        }

        public bool Add(string r, Value v)
        {
            Reference _r = new Reference();
            _r.Name = r;
            return Add(_r, v);
        }

        public Value FromIdentifier(string r)
        {
            Reference _r = new Reference();
            _r.Name = r;
            return FromReference(_r);
        }

        public Value FromReference(Reference t)
        {
            if (!items.ContainsKey(t))
            {
                return null;
            }
            return items[t];
        }

        public bool Remove(Reference t)
        {
            if (!items.ContainsKey(t))
            {
                return false;
            }
            var lastValue = items[t];
            bool temp = items.Remove(t);
            CollectionChanged?.Invoke(this, new ResourceCollectionEventArgs<string>(Operation.Removed, t, lastValue, null));
            return temp;
        }

        public bool Remove(string t)
        {
            var _r = new Reference();
            _r.Name = t;
            return Remove(_r);
        }

        public IEnumerator<Value> GetEnumerator()
        {
            return items.Values.GetEnumerator();
        }

       

        IEnumerator IEnumerable.GetEnumerator()
        {
            return items.Values.GetEnumerator();
        }

        public IEnumerable<Reference> References()
        {
            return items.Keys;
        }
    }
}