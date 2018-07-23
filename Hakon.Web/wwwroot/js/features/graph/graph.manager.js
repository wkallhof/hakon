class GraphManager{
    constructor($canvas) {
        this._ps = null;
        this._renderer = new GraphRenderer($canvas);
        this._paused = false;
        
        this._nodes = [];
        this._links = [];
    }

    /*
    * Handles setup and initialization for the app
    */  
    init() {
        // create the particle system
        // this._ps = arbor.ParticleSystem(2000, 600, 0.5);
        this._ps = arbor.ParticleSystem(500, 500, 0.5);
        this._ps.parameters({ gravity: true });

        // set the particle systems renderer to the same methods
        this._ps.renderer = { init: this._renderer.init.bind(this._renderer), redraw: this._renderer.redraw.bind(this._renderer) };
        this._renderer.init(this._ps);
    }

    update(nodes, links) {
        _.forEach(nodes, this.addOrUpdateNode.bind(this));
        _.forEach(links, this.addOrUpdateLink.bind(this));
    }

    addOrUpdateNode(apiNode) {
        let node = this._ps.getNode(apiNode.id);
        if (node == null)
            node = this._ps.addNode(apiNode.id);
        
        node.data = apiNode;
    }

    addOrUpdateLink(apiLink) {
        let edge = null;
        let edges = this._ps.getEdges(apiLink.from, apiLink.to);
        if (edges != null && edges.length > 0 || edges[0] != null) {
            edge = edges[0];
        }
        else {
            apiLink.length = 0.1;
            edge = this._ps.addEdge(apiLink.from, apiLink.to, apiLink);
        }
            
        return edge;
    }
}
