// Copyright (c) Arjen Post. See License.txt in the project root for license information. Credits go to Marc Gravell 
// for the original idea, which found here https://code.google.com/p/fast-member/, and some parts of the code.

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
        /// Specifies that argument validation (eg. null/type guards) should be bypassed. Setting this 
        /// option optimizes performance.
        /// </summary>
        DisableArgumentValidation = 1
    }
}