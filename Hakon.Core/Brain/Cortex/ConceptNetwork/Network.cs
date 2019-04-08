using System;
using System.Collections.Generic;
using System.Linq;

namespace Hakon.Core.Brain.Cortex.ConceptNetwork
{
    public class Network
    {
        public const int DEFAULT_ACTIVATION = 100;
        public const int DEFAULT_DECAY = 40;
        public const int DEFAULT_MEMORY_PERF = 100;
        public const int DEFAULT_THRESHOLD = 90;
        public const int NORMAL_NUMBER_COMING_LINKS = 2;

        private List<Node> _nodes { get; set; } = new List<Node>();
        private List<Link> _links { get; set; } = new List<Link>();

        /*------------------------------------*
         *           NODE METHODS             *
         *----------------------------------- */

        public List<Node> GetNodes(){
            return this._nodes.ToList();
        }

        public Node AddNodeOrIncrement(string label, int weight = 1, NodeClassification classification = NodeClassification.None){
            var newNode = new Node(label, weight, classification);

            var existingItem = this._nodes.SingleOrDefault(x => x.Label.Equals(newNode.Label) && x.Classification.Equals(newNode.Classification));
            if(existingItem != null){
                existingItem.Weight += newNode.Weight;
                return existingItem;
            }
            this._nodes.Add(newNode);
            return newNode;
        }

        public void DecrementNode(string id){
            var node = this.GetNodeById(id);
            if(node == null)
                return;

            node.Weight--;

            if(node.Weight <= 0)
                this.RemoveNode(node.Id);
        }

        public void RemoveNode(string id){
            this._links.RemoveAll(x => x.From.Equals(id) || x.To.Equals(id));
            this._nodes.RemoveAll(x => x.Id.Equals(id));
        }

        public Node GetNodeById(string id){
            return this._nodes.SingleOrDefault(x => x.Id.Equals(id));
        }

        public Node GetNodeByLabel(string label, NodeClassification classification = NodeClassification.None){
            return this._nodes.SingleOrDefault(x => x.Label.Equals(label) && x.Classification.Equals(classification));
        }

        public IEnumerable<Link> GetLinksFromNode(string id){
            return this._links.Where(x => x.From.Equals(id));
        }

        public IEnumerable<Link> GetLinksToNode(string id){
            return this._links.Where(x => x.To.Equals(id));
        }

        public void ActivateNode(string id){
            var node = this.GetNodeById(id);
            if(node == null)
                return;

            node.ActivationValue = DEFAULT_ACTIVATION;
        }

        /*------------------------------------*
         *           LINK METHODS             *
         *----------------------------------- */

        public List<Link> GetLinks(){
            return this._links.ToList();
        }

        public List<string> GetLinksOutput(){
            return this._links.Select(x => {
                var fromNode = this.GetNodeById(x.From);
                var toNode = this.GetNodeById(x.To);
                return $"{fromNode.Label}({fromNode.Weight * fromNode.ActivationValue}) -> {toNode.Label}({toNode.Weight * toNode.ActivationValue})";
            }).ToList();
        }

        public Link AddLinkOrIncrement(Node from, Node to, int weight = 1){
            var newLink = new Link(from, to, weight);
            var existingItem = this._links.SingleOrDefault(x => x.Id.Equals(newLink.Id));
            if(existingItem != null){
                existingItem.Weight += newLink.Weight;
                return existingItem;
            }
            this._links.Add(newLink);
            return newLink;
        }

        public void DecrementLink(string from, string to){
            var link = this.GetLinkByFromAndTo(from, to);
            if(link == null)
                return;

            link.Weight--;

            if(link.Weight <= 0)
                this._links.Remove(link);
        }

        public void RemoveLink(string from, string to){
            this._links.RemoveAll(x => x.From.Equals(from) && x.To.Equals(to));
        }

        public Link GetLinkByFromAndTo(string from, string to){
            return this._links.SingleOrDefault(x => x.From.Equals(from) && x.To.Equals(to));
        }

        /*------------------------------------*
         *           HELPER METHODS           *
         *----------------------------------- */

         public double GetMaximumActivationValue(NodeClassification? filter = null){
            var orderedList = this._nodes.OrderByDescending(x => x.ActivationValue).AsEnumerable();

            if(filter.HasValue)
                orderedList = orderedList.Where(x => x.Classification.Equals(filter));

            var topNode = orderedList.FirstOrDefault();
            return topNode != null ? topNode.ActivationValue : 0;
        }

        public IEnumerable<Node> GetActivatedNodes(NodeClassification? filter = null, double threshold = DEFAULT_THRESHOLD){
            var filteredList = this._nodes.Where(x => x.ActivationValue > threshold);

            if(filter.HasValue)
                filteredList = filteredList.Where(x => x.Classification.Equals(filter));

            return filteredList;
        }

        public void Propagate(int decay = DEFAULT_DECAY, int memoryPerf = DEFAULT_MEMORY_PERF){

            // update node states
            this._nodes.ForEach(x =>
            {
                x.Age++;
                x.OldActivationValue = x.ActivationValue;

                x.InfluenceNumber = null;
                x.InfluenceValue = null;
                x.InfluenceWeight = x.Weight;
            });

            this._nodes.ForEach(UpdateInfluence);
            this._nodes.ForEach(x => UpdateActivation(x, decay, memoryPerf));
        }

        private void UpdateInfluence(Node node){
            var fromLinks = this.GetLinksFromNode(node.Id);

            foreach(var link in fromLinks){
                var toNode = this.GetNodeById(link.To);

                if(!toNode.InfluenceValue.HasValue)
                    toNode.InfluenceValue = 0;

                if(!toNode.InfluenceNumber.HasValue)
                    toNode.InfluenceNumber = 0;

                toNode.InfluenceValue += (0.5 + node.OldActivationValue * link.Weight);
                toNode.InfluenceNumber++;
            }
        }

        public void UpdateActivation(Node node, int decay, int memoryPerf){
            var minusAge = 200 / (1 + Math.Exp(-node.Age / memoryPerf)) - 100;
            double newActivationValue;

            if(!node.InfluenceValue.HasValue ){
                newActivationValue = node.OldActivationValue - decay * node.OldActivationValue / 100 - minusAge;
            }
            else{
                var influence = node.InfluenceValue.Value;
                influence /= Math.Log(NORMAL_NUMBER_COMING_LINKS + node.InfluenceNumber.Value) / Math.Log(NORMAL_NUMBER_COMING_LINKS);
                newActivationValue = node.OldActivationValue - decay; //* node.OldActivationValue / 100 + influence - minusAge;
            }

            newActivationValue = Math.Max(newActivationValue, 0);
            newActivationValue = Math.Min(newActivationValue, 100);

            node.ActivationValue = newActivationValue;
        }
    }
}