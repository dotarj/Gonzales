// Copyright (c) Arjen Post. See License.txt in the project root for license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;

namespace Gonzales
{
    /// <summary>
    /// Provides methods for accessing a types members by name.
    /// </summary>
    public abstract class TypeAccessor
    {
        /// <summary>
        /// Creates an accessor for accessing a types members by name.
        /// </summary>
        /// <param name="type">The type to create the accessor for.</param>
        /// <returns>The accessor for the specified type.</returns>
        public static TypeAccessor Create(Type type)
        {
            return Create(type, TypeAccessorOptions.None);
        }

        /// <summary>
        /// Creates an accessor for accessing a types members by name.
        /// </summary>
        /// <param name="type">The type to create the accessor for.</param>
        /// <param name="options">The options for the accessor.</param>
        /// <returns>The accessor for the specified type.</returns>
        public static TypeAccessor Create(Type type, TypeAccessorOptions options)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            if (typeof(IDynamicMetaObjectProvider).IsAssignableFrom(type))
            {
                return DynamicTypeAccessor.CreateNew(options);
            }

            return StaticTypeAccessor.CreateNew(type, options);
        }

        /// <summary>
        /// Gets or sets the member value for a specified object.
        /// </summary>
        /// <param name="obj">The object whose member value will be returned or set.</param>
        /// <param name="name">The member name.</param>
        /// <returns>The member value of the specified object.</returns>
        public abstract object this[object obj, string name] { get; set; }

        /// <summary>
        /// Sets the member value for a specified object.
        /// </summary>
        /// <param name="obj">The object whose member value will be set.</param>
        /// <param name="name">The member name.</param>
        /// <param name="value">The new member value.</param>
        public abstract void SetValue(object obj, string name, object value);

        /// <summary>
        /// Returns the member value of a specified object.
        /// </summary>
        /// <param name="obj">The object whose member value will be returned.</param>
        /// <param name="name">The member name.</param>
        /// <returns>The member value of the specified object.</returns>
        public abstract object GetValue(object obj, string name);

        /// <summary>
        /// Sets the member value for a specified object.
        /// </summary>
        /// <param name="obj">The object whose member value will be set.</param>
        /// <param name="name">The member name.</param>
        /// <param name="value">The new member value.</param>
        /// <returns>True if successful; otherwise, false.</returns>
        public abstract bool TrySetValue(object obj, string name, object value);

        /// <summary>
        /// Returns the member value of a specified object.
        /// </summary>
        /// <param name="obj">The object whose member value will be returned.</param>
        /// <param name="name">The member name.</param>
        /// <param name="value">The member value of the specified object.</param>
        /// <returns>True if successful; otherwise, false.</returns>
        public abstract bool TryGetValue(object obj, string name, out object value);
    }

    sealed class DynamicTypeAccessor : TypeAccessor
    {
        private static readonly ConcurrentDictionary<TypeAccessorOptions, DynamicTypeAccessor> lookups = new ConcurrentDictionary<TypeAccessorOptions, DynamicTypeAccessor>();

        private readonly bool disableInputValidation;

        private DynamicTypeAccessor(TypeAccessorOptions options)
        {
            disableInputValidation = options.HasFlag(TypeAccessorOptions.DisableInputValidation);
        }

        internal static DynamicTypeAccessor CreateNew(TypeAccessorOptions options)
        {
            return lookups.GetOrAdd(options, _ => new DynamicTypeAccessor(options));
        }

        private void ValidateObject(object obj)
        {
            if (!disableInputValidation)
            {
                if (obj == null)
                {
                    throw new ArgumentNullException("obj");
                }

                if (!typeof(IDynamicMetaObjectProvider).IsAssignableFrom(obj.GetType()))
                {
                    throw new ArgumentException("Object must be a dynamic type.", "obj");
                }
            }
        }

        private bool HasMember(object obj, string name)
        {
            if (!disableInputValidation)
            {
                var memberNames = ((IDynamicMetaObjectProvider)obj).GetMetaObject(Expression.Constant(obj)).GetDynamicMemberNames();

                return memberNames.Contains(name);
            }

            return true;
        }

        public override object this[object obj, string name]
        {
            get
            {
                ValidateObject(obj);

                if (!HasMember(obj, name))
                {
                    throw new ArgumentException("Member not found.", name);
                }

                return CallSiteCache.GetValue(name, obj);
            }
            set
            {
                ValidateObject(obj);

                CallSiteCache.SetValue(name, obj, value);
            }
        }

        public override void SetValue(object obj, string name, object value)
        {
            ValidateObject(obj);

            CallSiteCache.SetValue(name, obj, value);
        }

        public override object GetValue(object obj, string name)
        {
            ValidateObject(obj);

            if (!HasMember(obj, name))
            {
                throw new ArgumentException("Member not found.", name);
            }

            return CallSiteCache.GetValue(name, obj);
        }

        public override bool TrySetValue(object obj, string name, object value)
        {
            ValidateObject(obj);

            CallSiteCache.SetValue(name, obj, value);

            return true;
        }

        public override bool TryGetValue(object obj, string name, out object value)
        {
            ValidateObject(obj);

            if (!HasMember(obj, name))
            {
                value = null;

                return false;
            }

            value = CallSiteCache.GetValue(name, obj);

            return true;
        }
    }

    sealed class StaticTypeAccessor : TypeAccessor
    {
        private delegate bool GetValueDelegate<TType, TPropertyName, TValue, TResult>(object source, string name, out object value);
        private delegate bool SetValueDelegate<TType, TPropertyName, TValue, TResult>(object target, string name, object value);

        private static readonly ConcurrentDictionary<TypeAccessorOptions, ConcurrentDictionary<Type, StaticTypeAccessor>> lookups = new ConcurrentDictionary<TypeAccessorOptions,ConcurrentDictionary<Type,StaticTypeAccessor>>();
        private static readonly MethodInfo stringIndexerMethod = typeof(string).GetMethod("get_Chars");
        private static readonly ConstructorInfo argumentExceptionConstructor = typeof(ArgumentException).GetConstructor(new[] { typeof(string), typeof(string) });

        private static ModuleBuilder moduleBuilder;
        private static int counter;

        private readonly Type type;
        private readonly bool disableInputValidation;

        private GetValueDelegate<object, string, object, bool> getValue;
        private SetValueDelegate<object, string, object, bool> setValue;

        static StaticTypeAccessor()
        {
            var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("FastExpression_dynamic"), AssemblyBuilderAccess.Run);

            moduleBuilder = assemblyBuilder.DefineDynamicModule("module");
        }

        private StaticTypeAccessor(Type type, TypeAccessorOptions options)
        {
            this.type = type;

            disableInputValidation = options.HasFlag(TypeAccessorOptions.DisableInputValidation);

            var typeBuilder = moduleBuilder.DefineType(string.Format("FastExpression_dynamic.{0}_{1}", type.Name, Interlocked.Increment(ref counter)), TypeAttributes.Sealed | TypeAttributes.Public);

            GetGetMethods(type, typeBuilder, options);
            GetSetMethods(type, typeBuilder, options);

            var accessorType = typeBuilder.CreateType();

            getValue = (GetValueDelegate<object, string, object, bool>)Delegate.CreateDelegate(typeof(GetValueDelegate<object, string, object, bool>), accessorType.GetMethod(string.Format("get_{0}_{1}", type.Name, (long)options)));
            setValue = (SetValueDelegate<object, string, object, bool>)Delegate.CreateDelegate(typeof(SetValueDelegate<object, string, object, bool>), accessorType.GetMethod(string.Format("set_{0}_{1}", type.Name, (long)options)));
        }

        internal static StaticTypeAccessor CreateNew(Type type, TypeAccessorOptions options)
        {
            var lookup = lookups.GetOrAdd(options, _ => new ConcurrentDictionary<Type, StaticTypeAccessor>());

            return lookup.GetOrAdd(type, _ => new StaticTypeAccessor(type, options));
        }

        private void ValidateObject(object obj)
        {
            if (!disableInputValidation)
            {
                if (obj == null)
                {
                    throw new ArgumentNullException("obj");
                }

                if (obj.GetType() != type)
                {
                    throw new ArgumentException(string.Format("Object must be of type {0}.", type.Name), "obj");
                }
            }
        }

        public override object this[object obj, string name]
        {
            get
            {
                ValidateObject(obj);

                object value = null;

                if (!getValue(obj, name, out value))
                {
                    throw new ArgumentException("Member not found.", name);
                }

                return value;

            }
            set
            {
                ValidateObject(obj);

                if (!setValue(obj, name, value))
                {
                    throw new ArgumentException("Member not found.", name);
                }
            }
        }

        public override void SetValue(object obj, string name, object value)
        {
            ValidateObject(obj);

            if (!setValue(obj, name, value))
            {
                throw new ArgumentException("Member not found.", name);
            }
        }

        public override object GetValue(object obj, string name)
        {
            ValidateObject(obj);

            object value = null;

            if (!getValue(obj, name, out value))
            {
                throw new ArgumentException("Member not found.", name);
            }

            return value;
        }

        public override bool TrySetValue(object obj, string name, object value)
        {
            ValidateObject(obj);

            return setValue(obj, name, value);
        }

        public override bool TryGetValue(object obj, string name, out object value)
        {
            ValidateObject(obj);

            return getValue(obj, name, out value);
        }

        private static void GetGetMethods(Type type, TypeBuilder typeBuilder, TypeAccessorOptions options)
        {
            var sourceExpression = Expression.Parameter(typeof(object), "source");
            var nameExpression = Expression.Parameter(typeof(string), "name");
            var valueExpression = Expression.Parameter(typeof(object).MakeByRefType(), "value");
            var returnTarget = Expression.Label(typeof(bool));

            var readableProperties = GetReadableMembers(type);
            var expressions = new List<Expression>();

            foreach (var group in readableProperties.GroupBy(property => property.Name[0]))
            {
                var firstCharacter = group.Key;
                var properties = group.ToArray();

                if (properties.Length == 1)
                {
                    expressions.Add(GetGetExpressions(type, sourceExpression, nameExpression, valueExpression, returnTarget, properties[0]));
                }
                else
                {
                    expressions.Add(GetGetExpressions(type, sourceExpression, nameExpression, valueExpression, returnTarget, properties[0]));
                    var getExpressions = properties
                        .Select(property => GetGetExpressions(type, sourceExpression, nameExpression, valueExpression, returnTarget, property));

                    expressions.Add(Expression.IfThen(
                        Expression.Equal(
                            Expression.Call(nameExpression, stringIndexerMethod, Expression.Constant(0, typeof(int))),
                            Expression.Constant(firstCharacter)),
                        Expression.Block(getExpressions)));
                }
            }

            expressions.Add(Expression.Assign(valueExpression, Expression.Constant(null)));
            expressions.Add(Expression.Label(returnTarget, Expression.Constant(false)));

            var methodName = string.Format("get_{0}_{1}", type.Name, (long)options);
            var expression = Expression.Lambda<GetValueDelegate<object, string, object, bool>>(Expression.Block(expressions), sourceExpression, nameExpression, valueExpression);
            var methodBuilder = typeBuilder.DefineMethod(methodName, MethodAttributes.Public | MethodAttributes.Static);

            expression.CompileToMethod(methodBuilder);
        }

        private static MemberInfo[] GetReadableMembers(Type type)
        {
            return type
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(property => property.CanRead && property.GetIndexParameters().Length == 0 && property.GetGetMethod(false) != null)
                .Cast<MemberInfo>()
                .Concat(type.GetFields(BindingFlags.Public | BindingFlags.Instance))
                .ToArray();
        }

        private static ConditionalExpression GetGetExpressions(Type type, ParameterExpression sourceExpression, ParameterExpression nameExpression, ParameterExpression valueExpression, LabelTarget returnTarget, MemberInfo member)
        {
            Expression expression;

            if (member.MemberType == MemberTypes.Field)
            {
                var field = (FieldInfo)member;

                expression = Expression.Field(Expression.Convert(sourceExpression, type), field);
            }
            else
            {
                var property = (PropertyInfo)member;

                expression = Expression.Call(Expression.Convert(sourceExpression, type), property.GetGetMethod(false));
            }

            return Expression.IfThen(
                Expression.Equal(Expression.Constant(member.Name), nameExpression),
                Expression.Block(
                    Expression.Assign(valueExpression, Expression.Convert(expression, typeof(object))),
                    Expression.Return(returnTarget, Expression.Constant(true))));
        }

        private static void GetSetMethods(Type type, TypeBuilder typeBuilder, TypeAccessorOptions options)
        {
            var targetExpression = Expression.Parameter(typeof(object), "target");
            var nameExpression = Expression.Parameter(typeof(string), "name");
            var valueExpression = Expression.Parameter(typeof(object), "value");
            var returnTarget = Expression.Label(typeof(bool));

            var writeableProperties = GetWriteableMembers(type);
            var expressions = new List<Expression>();

            foreach (var group in writeableProperties.GroupBy(property => property.Name[0]))
            {
                var firstCharacter = group.Key;
                var properties = group.ToArray();

                if (properties.Length == 1)
                {
                    expressions.Add(GetSetExpressions(type, targetExpression, nameExpression, valueExpression, returnTarget, properties[0], options));
                }
                else
                {
                    var setExpressions = properties
                        .Select(property => GetSetExpressions(type, targetExpression, nameExpression, valueExpression, returnTarget, property, options));

                    expressions.Add(Expression.IfThen(
                        Expression.Equal(
                            Expression.Call(nameExpression, stringIndexerMethod, Expression.Constant(0, typeof(int))),
                            Expression.Constant(firstCharacter)),
                        Expression.Block(setExpressions)));
                }
            }

            expressions.Add(Expression.Label(returnTarget, Expression.Constant(false)));

            var methodName = string.Format("set_{0}_{1}", type.Name, (long)options);
            var expression = Expression.Lambda<SetValueDelegate<object, string, object, bool>>(Expression.Block(expressions), targetExpression, nameExpression, valueExpression);
            var methodBuilder = typeBuilder.DefineMethod(methodName, MethodAttributes.Public | MethodAttributes.Static);

            expression.CompileToMethod(methodBuilder);
        }

        private static MemberInfo[] GetWriteableMembers(Type type)
        {
            return type
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(property => property.CanWrite && property.GetIndexParameters().Length == 0 && property.GetSetMethod(false) != null)
                .Cast<MemberInfo>()
                .Concat(type.GetFields(BindingFlags.Public | BindingFlags.Instance))
                .ToArray();
        }

        private static ConditionalExpression GetSetExpressions(Type type, ParameterExpression targetExpression, ParameterExpression nameExpression, ParameterExpression valueExpression, LabelTarget returnTarget, MemberInfo member, TypeAccessorOptions options)
        {
            Expression expression;
            Type memberType;

            if (member.MemberType == MemberTypes.Field)
            {
                var field = (FieldInfo)member;

                expression = Expression.Assign(Expression.Field(Expression.Convert(targetExpression, type), field), Expression.Convert(valueExpression, field.FieldType));
                memberType = field.FieldType;
            }
            else
            {
                var property = (PropertyInfo)member;

                expression = Expression.Call(Expression.Convert(targetExpression, type), property.GetSetMethod(false), Expression.Convert(valueExpression, property.PropertyType));
                memberType = property.PropertyType;
            }

            if (options.HasFlag(TypeAccessorOptions.DisableInputValidation))
            {
                return Expression.IfThen(
                    Expression.Equal(Expression.Constant(member.Name), nameExpression),
                        Expression.Block(
                            expression,
                            Expression.Return(returnTarget, Expression.Constant(true))));
            }
            else
            {
                return Expression.IfThen(
                    Expression.Equal(Expression.Constant(member.Name), nameExpression),
                        Expression.Block(
                            Expression.IfThen(
                                Expression.And(Expression.Not(Expression.Equal(valueExpression, Expression.Constant(null))), Expression.Not(Expression.TypeIs(valueExpression, memberType))),
                                Expression.Throw(Expression.New(argumentExceptionConstructor,
                                    Expression.Constant(string.Format("Value must be of type {0}", memberType.Name)),
                                    Expression.Constant("name")))),
                            expression,
                            Expression.Return(returnTarget, Expression.Constant(true))));
            }
        }
    }
}