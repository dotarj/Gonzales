// Copyright (c) Arjen Post. See License.txt in the project root for license information.

using System;

namespace Gonzales
{
    /// <summary>
    /// Specifies the options used for creating the accessor.
    /// </summary>
    [Flags]
    public enum TypeAccessorOptions
    {
        /// <summary>
        /// Specifies no options.
        /// </summary>
        None = 0,

        /// <summary>
        /// Specifies that input validation (eg. null/type guards) should be bypassed. Setting this 
        /// option optimizes performance.
        /// </summary>
        DisableInputValidation = 1
    }
}