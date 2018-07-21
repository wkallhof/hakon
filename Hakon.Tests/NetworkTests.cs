using System;
using System.Linq;
using Hakon.Core.Brain.Cortex.ConceptNetwork;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hakon.Tests
{
    [TestClass]
    public class NetworkTests
    {
        private TestHarness _t;

        [TestInitialize]
        public void Init(){
            this._t = new TestHarness();
        }

        [TestMethod]
        public void Add_node_returns_correct_node()
        {
            var result = this._t.Network.AddNodeOrIncrement("Chuck Norris");
            Assert.AreEqual("Chuck Norris", result.Label);
            Assert.AreEqual(1, result.Weight);
        }

        [TestMethod]
        public void Node_increment_works_for_matching_node(){
            this._t.Network.AddNodeOrIncrement("Chuck Norris");
            var result = this._t.Network.AddNodeOrIncrement("Chuck Norris");

            Assert.AreEqual(2, result.Weight);
        }

        [TestMethod]
        public void Add_or_increment_supports_custom_weight(){
            var result = this._t.Network.AddNodeOrIncrement("Chuck Norris", 5);

            Assert.AreEqual(5, result.Weight);
        }

        [TestMethod]
        public void Decrement_should_decrement(){
            var node = this._t.Network.AddNodeOrIncrement("Chuck Norris", 5);

            this._t.Network.DecrementNode(node.Id);

            Assert.AreEqual(4, node.Weight);
        }

        [TestMethod]
        public void Decrement_should_remove_if_0(){
            var node = this._t.Network.AddNodeOrIncrement("Chuck Norris");
            this._t.Network.DecrementNode(node.Id);
            var node2 = this._t.Network.GetNodeById(node.Id);

            Assert.IsNull(node2);
        }

        [TestMethod]
        public void Decrement_should_handle_missing_node_without_exception(){
            this._t.Network.DecrementNode("Doesn't Exist");
        }

        [TestMethod]
        public void Remove_node_should_remove_node(){
            Assert.AreEqual(4, this._t.Network.GetNodes().Count);

            var firstNode = this._t.Network.GetNodes().FirstOrDefault();
            this._t.Network.RemoveNode(firstNode.Id);

            Assert.AreEqual(3, this._t.Network.GetNodes().Count);
        }

        [TestMethod]
        public void Remove_node_should_remove_links(){
            Assert.IsTrue(this._t.Network.GetLinks().Any(x => x.To.Equals(this._t.Node2.Id) || x.From.Equals(this._t.Node2.Id)));

            this._t.Network.RemoveNode(this._t.Node2.Id);

            Assert.IsFalse(this._t.Network.GetLinks().Any(x => x.To.Equals(this._t.Node2.Id) || x.From.Equals(this._t.Node2.Id)));
        }

        [TestMethod]
        public void Add_link_should_link_nodes(){
            var result = this._t.Network.AddLinkOrIncrement(this._t.Node1, this._t.Node2);

            Assert.AreEqual(1, result.Weight);
            Assert.AreEqual(this._t.Node1.Id, result.From);
            Assert.AreEqual(this._t.Node2.Id, result.To);
        }

        [TestMethod]
        public void Add_link_should_increment_weight_of_existing_link(){
            var result = this._t.Network.AddLinkOrIncrement(this._t.Node1, this._t.Node2);
            result = this._t.Network.AddLinkOrIncrement(this._t.Node1, this._t.Node2);

            Assert.AreEqual(2, result.Weight);
        }

        [TestMethod]
        public void Add_link_should_support_passing_weight(){
            var result = this._t.Network.AddLinkOrIncrement(this._t.Node1, this._t.Node2, 5);

            Assert.AreEqual(5, result.Weight);
        }

        [TestMethod]
        public void Decrement_link_should_decrement(){
            var result = this._t.Network.AddLinkOrIncrement(this._t.Node1, this._t.Node2, 5);

            this._t.Network.DecrementLink(this._t.Node1.Id, this._t.Node2.Id);
            Assert.AreEqual(4, result.Weight);
        }

        [TestMethod]
        public void Decrement_link_should_remove_link_if_weight_becomes_0(){
            this._t.Network.DecrementLink(this._t.Node2.Id, this._t.Node3.Id);

            var link = this._t.Network.GetLinkByFromAndTo(this._t.Node2.Id, this._t.Node3.Id);
            Assert.IsNull(link);
            Assert.AreEqual(1, this._t.Network.GetLinks().Count);
        }

        [TestMethod]
        public void Remove_link_removes_link() {
            this._t.Network.RemoveLink(this._t.Node2.Id, this._t.Node3.Id);
            Assert.AreEqual(1, this._t.Network.GetLinks().Count);
        }

        [TestMethod]
        public void Calling_get_node_with_valid_id_gets_correct_node() {
            var node = this._t.Network.GetNodeById(this._t.Node2.Id);
            Assert.AreEqual(this._t.Node2.Id, node.Id);
        }

        [TestMethod]
        public void Calling_get_node_returns_null_for_invalid_node(){
            var node = this._t.Network.GetNodeById(Guid.NewGuid().ToString());
            Assert.IsNull(node);
        }

        [TestMethod]
        public void Calling_get_link_with_valid_from_to_returns_correct_link(){
            var link = this._t.Network.GetLinkByFromAndTo(this._t.Node2.Id, this._t.Node3.Id);
            Assert.AreEqual(this._t.Node2.Id, link.From);
            Assert.AreEqual(this._t.Node3.Id, link.To);
        }

        [TestMethod]
        public void Calling_get_link_with_invalid_from_to_returns_null(){
            var link = this._t.Network.GetLinkByFromAndTo(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            Assert.IsNull(link);
        }

        [TestMethod]
        public void Calling_get_links_from_node_gets_correct_links(){
            var links = this._t.Network.GetLinksFromNode(this._t.Node2.Id);
            var link = links.FirstOrDefault();

            Assert.AreEqual(1, links.Count());
            Assert.AreEqual(this._t.Node2.Id, link.From);
            Assert.AreEqual(this._t.Node3.Id, link.To);
        }

        [TestMethod]
        public void Calling_get_links_from_node_gets_correct_links_multiple(){
            var newNode = this._t.Network.AddNodeOrIncrement("Node 5");
            this._t.Network.AddLinkOrIncrement(this._t.Node2, newNode);
            var links = this._t.Network.GetLinksFromNode(this._t.Node2.Id);

            Assert.AreEqual(2, links.Count());
        }

        [TestMethod]
        public void Calling_get_links_from_node_returns_empty_for_node_without_from_links(){
            var links = this._t.Network.GetLinksFromNode(this._t.Node1.Id);

            Assert.AreEqual(0, links.Count());
        }

        [TestMethod]
        public void Calling_get_links_to_node_gets_correct_links(){
            var links = this._t.Network.GetLinksToNode(this._t.Node3.Id);
            var link = links.FirstOrDefault();

            Assert.AreEqual(1, links.Count());
            Assert.AreEqual(this._t.Node2.Id, link.From);
            Assert.AreEqual(this._t.Node3.Id, link.To);
        }

        [TestMethod]
        public void Calling_get_links_to_node_gets_correct_links_multiple(){
            var newNode = this._t.Network.AddNodeOrIncrement("Node 5");
            this._t.Network.AddLinkOrIncrement(newNode, this._t.Node3);
            var links = this._t.Network.GetLinksToNode(this._t.Node3.Id);

            Assert.AreEqual(2, links.Count());
        }

        [TestMethod]
        public void Calling_get_links_to_node_returns_empty_for_node_without_to_links(){
            var links = this._t.Network.GetLinksFromNode(this._t.Node1.Id);

            Assert.AreEqual(0, links.Count());
        }

        [TestMethod]
        public void Calling_activate_on_node_sets_default_activation_value(){
            this._t.Network.ActivateNode(this._t.Node1.Id);
            Assert.AreEqual(Network.DEFAULT_ACTIVATION, this._t.Node1.ActivationValue);
        }

        [TestMethod]
        public void Calling_activate_on_already_activated_node_should_cap_value(){
            this._t.Network.ActivateNode(this._t.Node1.Id);
            this._t.Network.ActivateNode(this._t.Node1.Id);
            Assert.AreEqual(Network.DEFAULT_ACTIVATION, this._t.Node1.ActivationValue);
        }

        [TestMethod]
        public void Default_activation_value_is_0(){
            Assert.AreEqual(0, this._t.Node1.ActivationValue);
        }

        [TestMethod]
        public void Propagate_accurately_captures_old_activation_value(){
            this._t.Network.ActivateNode(this._t.Node2.Id);
            this._t.Network.Propagate();

            Assert.AreEqual(0, this._t.Network.GetNodeById(this._t.Node1.Id).OldActivationValue);
            Assert.AreEqual(Network.DEFAULT_ACTIVATION, this._t.Node2.OldActivationValue);
        }

        [TestMethod]
        public void Get_max_activation_value_returns_0_with_no_active_node(){
            var val = this._t.Network.GetMaximumActivationValue();
            Assert.AreEqual(0, val);
        }

        [TestMethod]
        public void Get_max_activation_value_returns_actual_max_value(){
            this._t.Node1.ActivationValue = 75;
            this._t.Node2.ActivationValue = 70;
            this._t.Node3.ActivationValue = 50;

            var val = this._t.Network.GetMaximumActivationValue();
            Assert.AreEqual(75, val);
        }

        [TestMethod]
        public void Get_max_activation_value_returns_correctly_with_filter(){
            var node = this._t.Network.AddNodeOrIncrement("Test");
            node.ActivationValue = 25;
            node.Classification = NodeClassification.Sentence;

            this._t.Node1.ActivationValue = 75;
            this._t.Node2.ActivationValue = 70;
            this._t.Node3.ActivationValue = 50;

            var val = this._t.Network.GetMaximumActivationValue(NodeClassification.Sentence);
            Assert.AreEqual(25, val);
        }

        [TestMethod]
        public void Get_activated_nodes_returns_0_with_no_active_nodes(){
            var nodes = this._t.Network.GetActivatedNodes();
            Assert.AreEqual(0, nodes.Count());
        }

        [TestMethod]
        public void Get_activated_nodes_returns_correct_nodes_without_filter(){
            this._t.Node1.ActivationValue = Network.DEFAULT_THRESHOLD+1;

            var nodes = this._t.Network.GetActivatedNodes();
            Assert.AreEqual(1, nodes.Count());
        }

        [TestMethod]
        public void Get_activated_nodes_returns_correct_nodes_without_filter_multiple(){
            this._t.Node1.ActivationValue = Network.DEFAULT_THRESHOLD+1;
            this._t.Node2.ActivationValue = Network.DEFAULT_THRESHOLD+1;

            var nodes = this._t.Network.GetActivatedNodes();
            Assert.AreEqual(2, nodes.Count());
        }

        [TestMethod]
        public void Get_activate_nodes_supports_filtering(){
            var node = this._t.Network.AddNodeOrIncrement("Node 5");
            node.ActivationValue = Network.DEFAULT_THRESHOLD + 1;
            node.Classification = NodeClassification.Sentence;

            var nodes = this._t.Network.GetActivatedNodes(NodeClassification.Sentence);
            Assert.AreEqual(1, nodes.Count());
            Assert.AreEqual(node.Id, nodes.First().Id);
        }

        [TestMethod]
        public void Get_activated_nodes_supports_threshold(){
            this._t.Node1.ActivationValue = 50;
            this._t.Node2.ActivationValue = 70;

            var nodes = this._t.Network.GetActivatedNodes(threshold:60);
            Assert.AreEqual(1, nodes.Count());
            Assert.AreEqual(this._t.Node2.Id, nodes.First().Id);
        }

        [TestMethod]
        public void Propagate_should_decay_node_without_affrent_links(){
            this._t.Network.ActivateNode(this._t.Node2.Id);
            this._t.Network.Propagate();

            Assert.IsTrue(this._t.Node2.ActivationValue < Network.DEFAULT_ACTIVATION);
        }

        [TestMethod]
        public void Propagate_should_activate_linked_node(){
            var node5 = this._t.Network.AddNodeOrIncrement("Node 5");
            this._t.Network.AddLinkOrIncrement(this._t.Node1, node5);

            this._t.Network.ActivateNode(this._t.Node1.Id);
            this._t.Network.Propagate();

            Assert.IsTrue(node5.ActivationValue > 0);
        }

        [TestMethod]
        public void Propagate_should_take_decay_into_account(){
            this._t.Network.ActivateNode(this._t.Node2.Id);
            this._t.Network.Propagate(decay:200);

            var activeNodes = this._t.Network.GetActivatedNodes();

            Assert.AreEqual(0, activeNodes.Count());
        }

        [TestMethod]
        public void Propage_should_take_memoryPef_into_account(){
            this._t.Network.ActivateNode(this._t.Node2.Id);
            this._t.Network.Propagate(memoryPerf: int.MaxValue);
            Assert.AreEqual(60, this._t.Node2.ActivationValue);
        }


        public class TestHarness{
            public Network Network { get; set; } = new Network();

            public Node Node1 { get; set; }
            public Node Node2 { get; set; }
            public Node Node3 { get; set; }
            public Node Node4 { get; set; }

            public Link Link1 { get; set; }
            public Link Link2 { get; set; }

            public TestHarness(){
                this.Node1 = Network.AddNodeOrIncrement("Node 1", 2);
                this.Node2 = Network.AddNodeOrIncrement("Node 2");
                this.Node3 = Network.AddNodeOrIncrement("Node 3");
                this.Node4 = Network.AddNodeOrIncrement("Node 4");

                this.Link1 = Network.AddLinkOrIncrement(this.Node2, this.Node3);
                this.Link2 = Network.AddLinkOrIncrement(this.Node3, this.Node4);
            }
        }

    }
}
