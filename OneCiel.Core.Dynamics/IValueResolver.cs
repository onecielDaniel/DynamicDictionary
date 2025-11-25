using System;
using System.Collections.Generic;
using System.Text;

namespace OneCiel.Core.Dynamics
{
    /// <summary>
    /// Interface for resolving and converting object values within DynamicDictionary.
    /// Allows custom type handling and transformation of values.
    /// </summary>
    public interface IValueResolver
    {
        /// <summary>
        /// Determines whether this resolver can handle the specified value.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <returns>True if this resolver can resolve the value; otherwise, false.</returns>
        bool CanResolve(object value);

        /// <summary>
        /// Resolves and transforms the specified value.
        /// </summary>
        /// <param name="value">The value to resolve.</param>
        /// <returns>The resolved/transformed value, or null if the value represents null.</returns>
        object? Resolve(object value);
    }
}
