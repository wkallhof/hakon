class ChatController extends Stimulus.Controller {
    initialize() { }

    static get targets() {
        return [ "output", "input", "send" ]
      }

    connect() {
        console.log("Chat controller connected.")
    }

    send(e) {
        e.preventDefault();

        var value = this.inputTarget.value;
        if (!value || value.length <= 0)
            return;
        
        this.addToChatLog("You", value);
        w.get("/api/process?text="+encodeURIComponent(value), this.onProcessResponse.bind(this), this.onRequestFailure.bind(this));
    }

    onProcessResponse(statusCode, response) {
        if (statusCode != 200 || !response.success) {
            //TODO: Display error message
            console.log("Error processing text: "+response.error);
            return;
        }

        console.log(response);
        this.inputTarget.value = "";
        this.addToChatLog("Hakon", response.data);
    }

    addToChatLog(source, message) {
        this.outputTarget.value += `${source}: ${message}\n`;
        this.outputTarget.scrollTop = this.outputTarget.scrollHeight;
    }

    onRequestFailure(data) {
        console.log("Request Failed");
        console.log(data);
    }
}