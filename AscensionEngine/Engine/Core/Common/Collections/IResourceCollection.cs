﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ascension.Engine.Core.Common.Collections
{
    public interface IResourceCollection<ReferenceTypeValue, ReferenceType, Value> : IEnumerable<Value> where ReferenceType : IReference<ReferenceTypeValue> where Value : class
    {
        /// <summary>
        ///  If ResourceCollections contains element False will be return
        /// </summary>
        bool Add(ReferenceTypeValue t, Value r);

        /// <summary>
        ///  If ResourceCollections contains element False will be return
        /// </summary>
        bool Add(ReferenceType t, Value r);

        bool Remove(ReferenceTypeValue t);

        bool Remove(ReferenceType t);

        IEnumerable<ReferenceType> References();

        Value FromIdentifier(ReferenceTypeValue t);
        Value FromReference(ReferenceType t);


        Value this[ReferenceType t]
        {
            get;
            set;
        }


        Value this[ReferenceTypeValue t]
        {
            get;
            set;
        }



    }
}