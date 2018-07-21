using System;

namespace Hakon.Core.Brain.Cortex.ConceptNetwork
{
    public class Node : IWeighted
    {
        public string Id { get; }
        public string Label { get; set; }
        public int Weight { get; set; }
        public NodeClassification Classification { get; set; }

        public double ActivationValue { get; set; }
        public double OldActivationValue { get; set; }
        public int Age { get; set; }

        public double? InfluenceValue { get; set; }
        public int? InfluenceNumber { get; set; }
        public double InfluenceWeight { get; set; }

        public Node(string label, int weight, NodeClassification classification = NodeClassification.None){
            this.Id = Guid.NewGuid().ToString();
            this.Label = label;
            this.Weight = weight;
            this.Classification = classification;
        }
    }
}