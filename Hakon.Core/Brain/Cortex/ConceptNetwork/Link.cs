using System;

namespace Hakon.Core.Brain.Cortex.ConceptNetwork
{
    public class Link : IWeighted
    {
        public string Id => $"{From}_{To}";
        public readonly string Label;
        public readonly string From;
        public readonly string To;
        public int Weight { get; set; }

        public Link (Node from, Node to, int weight){
            this.From = from.Id;
            this.To = to.Id;
            this.Weight = weight;
            this.Label = $"{from.Label} -> {to.Label}";
        }
    }
}