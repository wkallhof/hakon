REN = {
    NODE_WIDTH: 10,
    MOUSE_DISTANCE: 15,
    BACKGROUND_COLOR: "rgb(33,33,33)",
    LINK_COLOR: "rgba(255,242,126, .8)",
    FONT_COLOR: "white",
    FONT_STYLE: "12px Roboto",
    FONT_WEIGHT: "300",
    NODE_COLOR: (alpha) => `rgba(255,242,126,${alpha})`
}

class GraphRenderer {
    constructor($canvas) {
        this._canvas = $canvas.get(0);
        this._$canvas = $canvas;
        this._ctx = this._canvas.getContext("2d");
        this._ps = null;
        this._currentNodeFocus = null;
    }

        /*
        * Init method called by the particle system
        * @param system : Particle System passed in
        */
    init(particleSystem) {
        this._ps = particleSystem

        // handle window resize  
        $(window).resize(this.onResize.bind(this));
        // handle mouse move
        this._$canvas.mousemove(this.onMouseMove.bind(this));
        // call resize initially
        this.onResize();
    }

    /*
    * Handle the mousemove event from the canvas
    */    
    onMouseMove(e) {
        var offset = this._$canvas.offset();
        var mousePosition = arbor.Point(e.pageX - offset.left, e.pageY - offset.top)
        
        // find the nearest node to the mouse
        var nearest = this._ps.nearest(mousePosition);

        // if it doesn't exist or its not within 20 pixels, clear
        // current item        
        if (!nearest || !nearest.node || nearest.distance > REN.MOUSE_DISTANCE) {
            this._currentNodeFocus = null;
            return;
        }

        // set the nearest node to the current node        
        this._currentNodeFocus = nearest.node;
    }

    /*
    * Handles when the window is resized. Sets the canvas
    * widths for re-rendering
    */    
    onResize() {
        this._canvas.width = window.innerWidth;
        this._canvas.height = window.innerHeight;
        this._ps.screenSize(this._canvas.width, this._canvas.height)
    }

    /*
    * Loop Draw method called for each animation frame.
    * Called by the particle system
    */    
    redraw() {
        // clear
        this._ctx.fillStyle = REN.BACKGROUND_COLOR;
        this._ctx.fillRect(0, 0, this._canvas.width, this._canvas.height)
        
        // draw 
        this._ps.eachNode(this.drawNode.bind(this));       
        this._ps.eachEdge(this.drawEdge.bind(this));
        this.drawName(this._currentNodeFocus);
    }

    /*
    * Handles drawing each edge in the graph
    */ 
    drawEdge(edge, pt1, pt2) {
        let fromNodeAct = this.getActivationAlpha(edge.source)
        let toNodeAct = this.getActivationAlpha(edge.target);

        let gradient = this._ctx.createLinearGradient(pt1.x, pt1.y, pt2.x, pt2.y);
        gradient.addColorStop(0, REN.NODE_COLOR(fromNodeAct));
        gradient.addColorStop(1, REN.NODE_COLOR(toNodeAct));

        //this._ctx.strokeStyle = REN.LINK_COLOR;
        this._ctx.strokeStyle = gradient;
        this._ctx.lineWidth = 1;
        this._ctx.beginPath();
        this._ctx.moveTo(pt1.x, pt1.y);
        this._ctx.lineTo(pt2.x, pt2.y);
        this._ctx.stroke();

        // draw arrow head        
        var endRadians = Math.atan((pt2.y-pt1.y)/(pt2.x-pt1.x));
        endRadians+=( (pt2.x > pt1.x) ? 90 :- 90 ) * Math.PI / 180;
        this.drawArrowHead(pt2.x, pt2.y, endRadians);
    }

    /*
    * Handles drawing each node in the graph
    */    
    drawNode(node, pt) {
        var activation = this.getActivationAlpha(node);
        // draw circle on image
        this._ctx.lineWidth = 2;
        this._ctx.beginPath();
        this._ctx.arc(pt.x, pt.y, activation * 10, 0, 2*Math.PI);
        // this._ctx.strokeStyle = REN.NODE_COLOR(activation);
        // this._ctx.stroke();
        this._ctx.shadowColor = REN.NODE_COLOR(activation * 20);
        this._ctx.shadowBlur = activation * 20;
        this._ctx.fillStyle = REN.NODE_COLOR(activation);
        this._ctx.fill();
        this._ctx.shadowBlur = 0;
    }

    /*
    * Draw the name for the node closest to the user's
    * mouse
    */    
    drawName(node) {
        if (!node) return;

        var w = REN.NODE_WIDTH;        
        var pt = this._ps.toScreen(node.p);

        let text = `${node.data.label} (${node.data.activationValue.toFixed(2)})`;
        
        this._ctx.fillStyle = REN.FONT_COLOR;
        this._ctx.font = REN.FONT_STYLE;
        this._ctx.fontWeight = REN.FONT_WEIGHT;
        this._ctx.fillText(text, pt.x - (w / 2) - 10, pt.y + w + 20 );
    }

    /*
    * Handles drawing the arrowhead for an edge
    */    
    drawArrowHead(x, y, radians) {
        this._ctx.lineWidth = 1;
        this._ctx.save();
        this._ctx.beginPath();
        this._ctx.translate(x,y);
        this._ctx.rotate(radians);
        this._ctx.moveTo(0,0);
        this._ctx.lineTo(5,10);
        this._ctx.lineTo(-5,10);
        this._ctx.closePath();
        this._ctx.restore();
        this._ctx.stroke();
    }

    getActivationAlpha(node) {
        return Math.max(node.data.activationValue / 100, 0.1);
    }
}