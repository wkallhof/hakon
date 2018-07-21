namespace Hakon.Core.Brain.Cortex.ConceptNetwork
{
    public interface IWeighted
    {
        string Id { get; }
        int Weight { get; set; }
    }
}