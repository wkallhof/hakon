class GraphController extends Stimulus.Controller {

    static get targets() {
        return [ "canvas"]
    }
    
    initialize() {
        
    }

    connect() {
        console.log("Graph controller connected.")

        this._graphManager = new GraphManager($(this.canvasTarget));
        this._graphManager.init();

        this.update();
    }

    update() {
        w.get("/api/network", this.onNetworkResponse.bind(this), this.onRequestFailure.bind(this));
    }

    onNetworkResponse(statusCode, response) {
        if (statusCode != 200 || !response.success) {
            //TODO: Display error message
            console.log("Error getting network: "+response.error);
            return;
        }

        this._graphManager.update(response.data.nodes, response.data.links);
    }

    onRequestFailure(data) {
        console.log("Network Request Failed");
        console.log(data);
    }
}