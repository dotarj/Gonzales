// Copyright (c) Arjen Post. See License.txt in the project root for license information. Credits go to Marc Gravell 
// for the original idea, which found here https://code.google.com/p/fast-member/, and some parts of the code.

using System;
using System.Dynamic;

namespace Gonzales
{
    /// <summary>
    /// Provides methods for accessing an objects members by name.
    /// </summary>
    public abstract class ObjectAccessor
    {
        /// <summary>
        /// Creates an accessor for accessing an objects members by name.
        /// </summary>
        /// <param name="obj">The object to create the accessor for.</param>
        /// <returns>The accessor for the specified object.</returns>
        public static ObjectAccessor Create(object obj)
        {
            return Create(obj, TypeAccessorOptions.None);
        }

        /// <summary>
        /// Creates an accessor for accessing an objects members by name.
        /// </summary>
        /// <param name="obj">The object to create the accessor for.</param>
        /// <param name="options">The options for the accessor.</param>
        /// <returns>The accessor for the specified object.</returns>
        public static ObjectAccessor Create(object obj, TypeAccessorOptions options)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            var dynamicMetaObjectProvider = obj as IDynamicMetaObjectProvider;

            if (dynamicMetaObjectProvider != null)
            {
                return new DynamicObjectAccessor(dynamicMetaObjectProvider);
            }
            
            return new StaticObjectAccessor(obj, options);
        }

        /// <summary>
        /// Gets or sets the member value.
        /// </summary>
        /// <param name="name">The member name.</param>
        /// <returns>The member value.</returns>
        public abstract object this[string name] { get; set; }

        /// <summary>
        /// Sets the member value.
        /// </summary>
        /// <param name="name">The member name.</param>
        /// <param name="value">The new member value.</param>
        public abstract void SetValue(string name, object value);

        /// <summary>
        /// Returns the member value.
        /// </summary>
        /// <param name="name">The member name.</param>
        /// <returns>The member value.</returns>
        public abstract object GetValue(string name);

        /// <summary>
        /// Sets the member value.
        /// </summary>
        /// <param name="name">The member name.</param>
        /// <param name="value">The new member value.</param>
        /// <returns>True if successful; otherwise, false.</returns>
        public abstract bool TrySetValue(string name, object value);

        /// <summary>
        /// Returns the member value.
        /// </summary>
        /// <param name="name">The member name.</param>
        /// <param name="value">The member value.</param>
        /// <returns>True if successful; otherwise, false.</returns>
        public abstract bool TryGetValue(string name, out object value);

        sealed class DynamicObjectAccessor : ObjectAccessor
        {
            private readonly IDynamicMetaObjectProvider obj;

            internal DynamicObjectAccessor(IDynamicMetaObjectProvider obj)
            {
                this.obj = obj;
            }

            public override object this[string name]
            {
                get { return CallSiteCache.GetValue(name, obj); }
                set { CallSiteCache.SetValue(name, obj, value); }
            }

            public override void SetValue(string name, object value)
            {
                CallSiteCache.SetValue(name, obj, value);
            }

            public override object GetValue(string name)
            {
                return CallSiteCache.GetValue(name, obj);
            }

            public override bool TrySetValue(string name, object value)
            {
                CallSiteCache.SetValue(name, obj, value);

                return true;
            }

            public override bool TryGetValue(string name, out object value)
            {
                value = CallSiteCache.GetValue(name, obj);

                return true;
            }
        }

        sealed class StaticObjectAccessor : ObjectAccessor
        {
            private readonly object obj;
            private readonly TypeAccessor typeAccessor;
            private readonly bool disableInputValidation;

            internal StaticObjectAccessor(object obj, TypeAccessorOptions options)
            {
                this.obj = obj;

                typeAccessor = TypeAccessor.Create(obj.GetType(), options);

                disableInputValidation = options.HasFlag(TypeAccessorOptions.DisableArgumentValidation);
            }

            public override object this[string name]
            {
                get { return typeAccessor[obj, name]; }
                set { typeAccessor[obj, name] = value; }
            }

            public override void SetValue(string name, object value)
            {
                typeAccessor.SetValue(obj, name, value);
            }

            public override object GetValue(string name)
            {
                return typeAccessor.GetValue(obj, name);
            }

            public override bool TrySetValue(string name, object value)
            {
                return typeAccessor.TrySetValue(obj, name, value);
            }

            public override bool TryGetValue(string name, out object value)
            {
                return typeAccessor.TryGetValue(obj, name, out value);
            }
        }
    }
}