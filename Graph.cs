namespace AAI6
{
    internal class Graph(IEnumerable<IValue> values, IEnumerable<IComponent> components)
    {
        public IEnumerable<IValue> Values { get; } = values;
        public IEnumerable<IComponent> Components { get; } = components;
        private readonly IDictionary<IComponent, uint> componentIndices = components
            .Select((c, i) => (c, (uint)i))
            .ToDictionary();
        private readonly Dictionary<IValue, IEnumerable<IComponent>> componentsOfValues = AnalyzeValueComponents(values, components);

        public float Likelyhood(uint[] variants)
        {
            int i = 0;
            float likelyhood = 1;
            foreach (var component in Components)
            {
                likelyhood *= component.Likelyhood(variants[i]);
                i++;
            }
            return likelyhood;
        }

        public uint IndexOf(IComponent component)
        {
            return componentIndices[component];
        }

        public uint[] VariantCounts => Components.Select(c => c.VariantCount).ToArray();

        public IEnumerable<IComponent> GetComponentsWithValue(IValue value) {
            if (componentsOfValues.TryGetValue(value, out var components))
            {
                return components;
            }
            return Enumerable.Empty<IComponent>();
        }

        private static Dictionary<IValue, IEnumerable<IComponent>> AnalyzeValueComponents(IEnumerable<IValue> values, IEnumerable<IComponent> components)
        {
            var result = new Dictionary<IValue, IEnumerable<IComponent>>();
            foreach (var value in values)
            {
                result.Add(value, new HashSet<IComponent>());
            }
            foreach (var component in components)
            {
                foreach (var value in component.ConnectedValues)
                {
                    if (result.TryGetValue(value, out var componentList))
                    {
                        (componentList as ICollection<IComponent>)?.Add(component);
                    }
                }
            }
            return result;
        }
    }
}
