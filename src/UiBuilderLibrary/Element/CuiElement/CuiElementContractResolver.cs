using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Oxide.Plugins;

public partial class UiBuilderLibrary
{
    /// <summary>
    /// Used to serialize CUI elements.
    /// </summary>
    private class CuiElementContractResolver : DefaultContractResolver
    {
        /// <summary>
        /// The singleton instance of this resolver.
        /// </summary>
        public static readonly CuiElementContractResolver Instance = new();

        /// <inheritdoc/>
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);

            // Always serialize the destroyUi key first.
            if (property.PropertyName == "destroyUi")
                property.Order = int.MinValue;

            // Only serialize non-default anchormin.
            if (property.PropertyName == "anchormin")
            {
                property.ShouldSerialize = instance =>
                {
                    if ((instance?.GetType().GetProperty(property.UnderlyingName))?.GetValue(instance) is not string
                        value)
                        return false;
                    return value != "0 0";
                };
            }

            // Only serialize non-default anchormax.
            else if (property.PropertyName == "anchormax")
            {
                property.ShouldSerialize = instance =>
                {
                    if ((instance?.GetType().GetProperty(property.UnderlyingName))?.GetValue(instance) is not string
                        value)
                        return false;
                    return value != "1 1";
                };
            }

            // Only serialize non-default offsets.
            else if (property.PropertyName == "offsetmin" || property.PropertyName == "offsetmax")
            {
                property.ShouldSerialize = instance =>
                {
                    if ((instance?.GetType().GetProperty(property.UnderlyingName))?.GetValue(instance) is not string
                        value)
                        return false;
                    return value != "0 0";
                };
            }

            // Only serialize non-default pivot.
            else if (property.PropertyName == "pivot")
            {
                property.ShouldSerialize = instance =>
                {
                    if ((instance?.GetType().GetProperty(property.UnderlyingName))?.GetValue(instance) is not string
                        value)
                        return false;
                    return value != "0.5 0.5";
                };
            }

            // Only serialize non-default rotation.
            else if (property.PropertyName == "rotation")
            {
                property.ShouldSerialize = instance =>
                {
                    if ((instance?.GetType().GetProperty(property.UnderlyingName))?.GetValue(instance) is not float
                        value)
                        return false;
                    return value != 0f;
                };
            }

            // Only serialize non-default rotation.
            else if (property.PropertyName == "setTransformIndex")
            {
                property.ShouldSerialize = instance =>
                {
                    if ((instance?.GetType().GetProperty(property.UnderlyingName))?.GetValue(instance) is not int value)
                        return false;
                    return value != -1;
                };
            }

            // Only serialize non-empty enumerables.
            else if (property.PropertyType != typeof(string) &&
                     property.PropertyType.GetInterface(nameof(IEnumerable)) != null)
            {
                property.ShouldSerialize = instance =>
                {
                    if ((instance?.GetType().GetProperty(property.UnderlyingName))?.GetValue(instance) is not
                        IEnumerable<object> enumerable)
                        return false;
                    return enumerable.Any();
                };
            }

            return property;
        }
    }
}
