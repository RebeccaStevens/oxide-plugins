using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;
using Oxide.Game.Rust.Cui;

namespace Oxide.Plugins;

public partial class UiBuilderLibrary
{
    /// <summary>
    /// A wrapper around a CUI element to help ensure valid properties.
    /// </summary>
    public class SafeCuiElement
    {
        /// <summary>
        /// The actual CUI element.
        /// </summary>
        private readonly CuiElement cuiElement;

        /// <summary>
        /// The bounds of this element.
        /// </summary>
        private Bounds? position;

        public string Name
        {
            get => cuiElement.Name;
            set => cuiElement.Name = value;
        }

        public string Parent
        {
            get => cuiElement.Parent;
            set => cuiElement.Parent = value;
        }

        // ReSharper disable once RedundantNullableFlowAttribute
        [MaybeNull]
        public float FadeOut
        {
            get => cuiElement.FadeOut;
            set => cuiElement.FadeOut = value;
        }

        // ReSharper disable once RedundantNullableFlowAttribute
        [MaybeNull]
        public bool Update
        {
            get => cuiElement.Update;
            set => cuiElement.Update = value;
        }

        public bool? ActiveSelf
        {
            get => cuiElement.ActiveSelf;
            set => cuiElement.ActiveSelf = value;
        }

        /// <summary>
        /// The settings to use when encoding an element.
        /// </summary>
        private static JsonSerializerSettings? _jsonSettings;

        /// <summary>
        /// Create a new safe CUI element.
        /// </summary>
        /// <param name="name">The name of this element.</param>
        /// <param name="parent">The name of the parent of this element.</param>
        public SafeCuiElement(string name, string parent)
        {
            cuiElement = new CuiElement
            {
                Name = name,
                Parent = parent
            };

            _jsonSettings ??= new JsonSerializerSettings
            {
#if DEBUG
                Formatting = Formatting.Indented,
#else
                Formatting = Formatting.None,
#endif
                DefaultValueHandling = DefaultValueHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = CuiElementContractResolver.Instance,
            };
        }

        /// <summary>
        /// Get the underlying CUI element.
        /// </summary>
        public CuiElement GetUnsafeCuiElement()
        {
            return cuiElement;
        }

        /// <summary>
        /// Set the position of this element.
        /// </summary>
        /// <param name="position">The position to set this element to.</param>
        public void SetPosition(Bounds position)
        {
            this.position = position;
        }

        /// <summary>
        /// Add a component to this element.
        /// </summary>
        /// <param name="component">The component to add.</param>
        public void AddComponent(ICuiComponent component)
        {
            Debug.Assert(!component.GetType().IsInstanceOfType(typeof(CuiRectTransformComponent)),
                "CuiRectTransformComponents should be added with SetPosition.");
            cuiElement.Components.Add(component);
        }

        /// <summary>
        /// Encode this element to a JSON string.
        /// </summary>
        public string Encode(ElementState state)
        {
            Debug.AssertNotNull(_jsonSettings);
            // Name and Parent must be set or DestroyUi must be set.
            Panic.If((string.IsNullOrEmpty(cuiElement.Name) || string.IsNullOrEmpty(cuiElement.Parent)) &&
                     string.IsNullOrEmpty(cuiElement.DestroyUi));
            cuiElement.DestroyUi ??= cuiElement.Name;

            // null if no position or no size.
            var rectTransform = position?.CreateCuiRectTransformComponent(state);

            if (rectTransform == null)
            {
                // Destroy the ui if it has no size.
                // TODO: If the ui is not open, don't do anything
                if (position != null)
                    return JsonConvert.SerializeObject(new CuiElement() { DestroyUi = cuiElement.DestroyUi },
                        _jsonSettings);
            }
            else
                cuiElement.Components.Add(rectTransform);

            return JsonConvert.SerializeObject(cuiElement, _jsonSettings);
        }
    }
}
