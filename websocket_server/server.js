const WebSocket = require("ws");

const wss = new WebSocket.Server({ port: 3000 });

console.log("WebSocket server running on port 3000");

wss.on("connection", ws => {
    console.log("Client connected");

    ws.on("message", msg => {
        // console.log("received:", msg.toString());
        wss.clients.forEach(client => {
            if (client.readyState === WebSocket.OPEN) {
                client.send(msg.toString());
            }
        });
    });

    ws.send("connected");
});
