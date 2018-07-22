class App {
    
    constructor() {
        this._app = Stimulus.Application.start();
        this._app.register("chat", ChatController);
        this._app.register("graph", GraphController);
    }
};

w.ready(() => new App());
