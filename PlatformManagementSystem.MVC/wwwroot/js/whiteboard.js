const canvas = document.getElementById("whiteboard");
const ctx = canvas.getContext("2d");

let drawing = false;
let startX, startY;

let history = [];
let redoStack = [];

const tool = document.getElementById("tool");
const colorPicker = document.getElementById("colorPicker");

const sessionId = window.sessionId || 1;
const apiBaseUrl = window.apiBaseUrl || "https://localhost:7102";
const userToken = window.userToken || "";

const connection = new signalR.HubConnectionBuilder()
    .withUrl(`${apiBaseUrl}/liveboardHub`, {
        accessTokenFactory: () => userToken
    })
    .build();

connection.start().then(() => {
    connection.invoke("JoinSessionGroup", sessionId);
});

connection.on("ReceiveDraw", function (data) {
    drawFromEvent(data.type, JSON.stringify(data), false);
});

connection.on("ReceiveHistory", function (events) {
    if (events && Array.isArray(events)) {
        events.forEach(function (evt) {
            try {
                if (evt.eventType === "clear") {
                    ctx.clearRect(0, 0, canvas.width, canvas.height);
                    history = [];
                } else if (evt.data) {
                    drawFromEvent(evt.eventType, evt.data, false);
                }
            } catch (err) {
                console.error("Failed to render historical stroke:", err);
            }
        });
    }
});

canvas.addEventListener("mousedown", (e) => {
    drawing = true;
    startX = e.offsetX;
    startY = e.offsetY;
});

canvas.addEventListener("mouseup", (e) => {

    if (!drawing) return;

    drawing = false;

    const endX = e.offsetX;
    const endY = e.offsetY;

    const eventData = {
        startX,
        startY,
        endX,
        endY,
        color: colorPicker.value
    };

    drawFromEvent(tool.value, JSON.stringify(eventData), true);
});

function drawFromEvent(type, dataJson, sendToServer = true) {

    const data = JSON.parse(dataJson);

    ctx.strokeStyle = data.color || "#000";
    ctx.fillStyle = data.color || "#000";
    ctx.beginPath();

    switch (type) {
        case "dot":
            ctx.fillRect(data.x, data.y, 2, 2);
            break;

        case "freehand":
            ctx.moveTo(data.startX, data.startY);
            ctx.lineTo(data.endX, data.endY);
            ctx.stroke();
            break;

        case "rectangle":
            ctx.strokeRect(data.startX, data.startY,
                data.endX - data.startX,
                data.endY - data.startY);
            break;

        case "circle":
            const radius = Math.sqrt(
                Math.pow(data.endX - data.startX, 2) +
                Math.pow(data.endY - data.startY, 2)
            );
            ctx.arc(data.startX, data.startY, radius, 0, 2 * Math.PI);
            ctx.stroke();
            break;

        case "line":
            ctx.moveTo(data.startX, data.startY);
            ctx.lineTo(data.endX, data.endY);
            ctx.stroke();
            break;

        case "arrow":
            ctx.moveTo(data.startX, data.startY);
            ctx.lineTo(data.endX, data.endY);
            ctx.stroke();
            break;

        case "text":
            const text = prompt("Enter text:");
            if (text) {
                ctx.fillText(text, data.startX, data.startY);
            }
            break;

        case "clear":
            ctx.clearRect(0, 0, canvas.width, canvas.height);
            break;
    }

    if (sendToServer) {
        history.push({ type, data: dataJson });
        redoStack = [];

        connection.invoke(
            "Draw",
            sessionId,
            { ...data, type }
        );
    }
}

function clearBoard() {

    ctx.clearRect(0, 0, canvas.width, canvas.height);

    connection.invoke(
        "Draw",
        sessionId,
        { type: "clear" }
    );
}

function redrawAll() {

    ctx.clearRect(0, 0, canvas.width, canvas.height);

    history.forEach(item => {
        drawFromEvent(item.type, item.data, false);
    });
}

function undo() {

    if (history.length === 0) return;

    const last = history.pop();
    redoStack.push(last);

    redrawAll();
}

function redo() {

    if (redoStack.length === 0) return;

    const item = redoStack.pop();
    history.push(item);

    redrawAll();
}

function saveImage() {

    const link = document.createElement("a");
    link.download = "whiteboard.png";
    link.href = canvas.toDataURL("image/png");
    link.click();
}

