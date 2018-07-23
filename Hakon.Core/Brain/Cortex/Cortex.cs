using System;
using System.Collections.Generic;
using System.Linq;
using Hakon.Core.Brain.Cortex.ConceptNetwork;
using Hakon.Core.Brain.Utilities;
using Hakon.Core.Extensions;

namespace Hakon.Core.Brain.Cortex
{
    public interface ICortex
    {
        void AddEntry(string entry);
        CortexResponse GenerateResponse();
        object GetState();
    }

    public class ConceptNetworkCortex : ICortex
    {
        private Network _network;

        public ConceptNetworkCortex(){
            this._network = new Network();
        }

        public void AddEntry(string entry){
            if(!entry.IsSet())
                throw new Exception("Entry was empty");

            var sentences = Tokenizer.GetSentences(entry);
            //Node prevSentenceNode = null;

            foreach(var sentence in sentences){
                var words = Tokenizer.GetWords(sentence);
                //var currentSentenceNode = this._network.AddNodeOrIncrement(sentence, classification: NodeClassification.Sentence);

                //if(prevSentenceNode != null)
                   //this._network.AddLinkOrIncrement(prevSentenceNode, currentSentenceNode);

                ///set last sentence nodeid

                //this._network.ActivateNode(currentSentenceNode.Id);

                Node prevWordNode = null;
                foreach(var word in words){
                    var currentWordNode = this._network.AddNodeOrIncrement(word, classification: NodeClassification.Word);
                    //this._network.AddLinkOrIncrement(currentSentenceNode, currentWordNode);
                    this._network.ActivateNode(currentWordNode.Id);
                    if(prevWordNode != null)
                        this._network.AddLinkOrIncrement(prevWordNode, currentWordNode);
                    prevWordNode = currentWordNode;
                }
                //prevSentenceNode = currentSentenceNode;
            }
        }

        public CortexResponse GenerateResponse(){
            this._network.Propagate(decay:80);

            var maxWordValue = this._network.GetMaximumActivationValue(NodeClassification.Word);
            var activatedWordNodes = this._network.GetActivatedNodes(NodeClassification.Word, (maxWordValue - 10.0));

            var node = this.RandomElementByWeight(activatedWordNodes, (x) => x.Weight);
            var phraseNodes = new List<Node>() { node };

            phraseNodes = this.Generate(phraseNodes, GenerateDirection.Forward);
            phraseNodes = this.Generate(phraseNodes, GenerateDirection.Backward);

            var sentence = string.Join(" ", phraseNodes.Select(x => x.Label));
            return new CortexResponse() { Success = true, Message = sentence };
        }

        public object GetState(){
            return new
            {
                Nodes = this._network.GetNodes(),
                Links = this._network.GetLinks()
            };
        }

        private List<Node> Generate(List<Node> phraseNodes, GenerateDirection direction){
            var targetNode = direction == GenerateDirection.Backward 
                ? phraseNodes.FirstOrDefault()
                : phraseNodes.LastOrDefault();

            if(targetNode == null)
                return phraseNodes;

            var links = direction == GenerateDirection.Backward 
                ? this._network.GetLinksToNode(targetNode.Id) 
                : this._network.GetLinksFromNode(targetNode.Id);

            var nodeSelection = new List<Node>();
            foreach(var link in links){
                var nextNode = direction == GenerateDirection.Backward
                    ? this._network.GetNodeById(link.From)
                    : this._network.GetNodeById(link.To);

                if(nextNode == null || nextNode.Classification != NodeClassification.Word)
                    continue;

                var activation = Math.Max(nextNode.ActivationValue, 1);
                var count = phraseNodes.Count(x => x.Label.Equals(nextNode.Label));
                if(count * nextNode.Label.Length <= 5 * 3){
                    var rep = 1 + count * count * nextNode.Label.Length;
                    nextNode.InfluenceWeight = (int)(link.Weight * activation / rep);
                    nodeSelection.Add(nextNode);
                }
                //nodeSelection.Add(nextNode);
            }

            if(nodeSelection.Count == 0)
                return phraseNodes;

            var word = this.RandomElementByWeight(nodeSelection, (x) => x.InfluenceWeight);

            if(direction == GenerateDirection.Backward)
                phraseNodes.Insert(0,word);
            else
                phraseNodes.Add(word);

            return this.Generate(phraseNodes, direction);
        }

        private T RandomElementByWeight<T>(IEnumerable<T> sequence, Func<T, double> weightSelector) {
            var totalWeight = sequence.Sum(weightSelector);
            // The weight we are after...
            var itemWeightIndex =  new Random().NextDouble() * totalWeight;
            var currentWeightIndex = 0d;

            foreach(var item in from weightedItem in sequence select new { Value = weightedItem, Weight = weightSelector(weightedItem) }) {
                currentWeightIndex += item.Weight;

                // If we've hit or passed the weight we are after for this item then it's the one we want....
                if(currentWeightIndex >= itemWeightIndex)
                    return item.Value;
            }

            return default(T);
        }

        public enum GenerateDirection{
            Forward,
            Backward
        }
    }
}