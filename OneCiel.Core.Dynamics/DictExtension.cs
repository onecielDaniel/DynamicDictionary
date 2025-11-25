using System;
using System.Collections.Generic;
using System.Text;

namespace OneCiel.Core.Dynamics
{
    /// <summary>
    /// Provides extension methods for converting objects to dictionaries.
    /// </summary>
    public static class DictExtension
    {
        /// <summary>
        /// Converts the public properties of the specified object to a dictionary with property names as keys and their values as values.
        /// </summary>
        /// <typeparam name="T">The type of the object to convert.</typeparam>
        /// <param name="request">The object instance to convert.</param>
        /// <returns>
        /// <![CDATA[
        /// A <see cref="Dictionary{string, object}"/> containing the property names and values of the object.
        /// ]]>
        /// </returns>
        public static Dictionary<string, object> ToDict<T>(this T request)
        {
            var profileDict = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            var properties = typeof(T).GetProperties();

            foreach (var prop in properties)
            {
                var value = prop.GetValue(request);
                if (value != null)
                {
                    profileDict[prop.Name] = value;
                }
            }
            return profileDict;
        }
    }
}
