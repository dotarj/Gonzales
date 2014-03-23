// Copyright (c) Arjen Post. See License.txt in the project root for license information.

using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace Gonzales
{
    internal static class CallSiteCache
    {
        private static readonly ConcurrentDictionary<string, CallSite<Func<CallSite, object, object>>> getters = new ConcurrentDictionary<string, CallSite<Func<CallSite, object, object>>>();
        private static readonly ConcurrentDictionary<string, CallSite<Func<CallSite, object, object, object>>> setters = new ConcurrentDictionary<string, CallSite<Func<CallSite, object, object, object>>>();

        internal static object GetValue(string name, object target)
        {
            var callSite = getters.GetOrAdd(name, _ => CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, name, typeof(CallSiteCache), new CSharpArgumentInfo[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) })));

            return callSite.Target(callSite, target);
        }

        internal static void SetValue(string name, object target, object value)
        {
            var callSite = setters.GetOrAdd(name, _ => CallSite<Func<CallSite, object, object, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, name, typeof(CallSiteCache), new CSharpArgumentInfo[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null), CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null) })));

            callSite.Target(callSite, target, value);
        }
    }
}