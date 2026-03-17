const canvas = document.getElementById("whiteboard");
const ctx = canvas.getContext("2d");

let drawing = false;
let startX, startY;
let lastX, lastY;

let history = [];
let redoStack = [];

const tool = document.getElementById("tool");
const colorPicker = document.getElementById("colorPicker");
const brushSizeInput = document.getElementById("brushSize");

const sessionId = window.sessionId || 1;
const apiBaseUrl = window.apiBaseUrl || "https://localhost:7102";
const userToken = window.userToken || "";

const connection = new signalR.HubConnectionBuilder()
    .withUrl(`${apiBaseUrl}/liveboardHub`, {
        accessTokenFactory: () => userToken
    })
    .build();

async function loadWhiteboardHistory() {
    if (!sessionId || !apiBaseUrl) return;
    try {
        const res = await fetch(`${apiBaseUrl}/api/LiveSession/WhiteboardHistory/${sessionId}`, {
            headers: { "Authorization": "Bearer " + (userToken || "") }
        });
        if (!res.ok) return;
        const list = await res.json();
        for (const item of list) {
            if (item.data) {
                try {
                    const parsed = typeof item.data === "string" ? JSON.parse(item.data) : item.data;
                    drawFromEvent(parsed.type || "freehand", typeof item.data === "string" ? item.data : JSON.stringify(parsed), false);
                } catch (err) { }
            }
        }
    } catch (e) { }
}

connection.start().then(() => {
    connection.invoke("JoinSessionGroup", sessionId);
    loadWhiteboardHistory();
});

connection.on("ReceiveDraw", function (data) {
    drawFromEvent(data.type, JSON.stringify(data), false);
});

canvas.addEventListener("mousedown", (e) => {
    drawing = true;
    startX = e.offsetX;
    startY = e.offsetY;
    lastX = startX;
    lastY = startY;

    if (tool.value === "text") {
        const textData = {
            startX,
            startY,
            color: colorPicker.value,
            size: brushSizeInput ? parseInt(brushSizeInput.value, 10) || 3 : 3
        };
        drawFromEvent("text", JSON.stringify(textData), true);
        drawing = false;
    }
});

canvas.addEventListener("mousemove", (e) => {
    if (!drawing) return;

    const currentX = e.offsetX;
    const currentY = e.offsetY;

    if (tool.value === "freehand" || tool.value === "eraser") {
        const eventData = {
            startX: lastX,
            startY: lastY,
            endX: currentX,
            endY: currentY,
            color: tool.value === "eraser" ? "#ffffff" : colorPicker.value,
            size: brushSizeInput ? parseInt(brushSizeInput.value, 10) || 3 : 3
        };

        drawFromEvent(tool.value, JSON.stringify(eventData), true);

        lastX = currentX;
        lastY = currentY;
    }
});

canvas.addEventListener("mouseup", (e) => {
    if (!drawing) return;

    drawing = false;

    const endX = e.offsetX;
    const endY = e.offsetY;

    if (tool.value === "freehand" || tool.value === "eraser" || tool.value === "text") {
        return;
    }

    const eventData = {
        startX,
        startY,
        endX,
        endY,
        color: colorPicker.value,
        size: brushSizeInput ? parseInt(brushSizeInput.value, 10) || 3 : 3
    };

    drawFromEvent(tool.value, JSON.stringify(eventData), true);
});

function drawFromEvent(type, dataJson, sendToServer = true) {
    const data = JSON.parse(dataJson);

    const lineWidth = data.size || 3;
    ctx.lineWidth = lineWidth;

    if (type === "eraser") {
        ctx.strokeStyle = "#ffffff";
        ctx.fillStyle = "#ffffff";
    } else {
        ctx.strokeStyle = data.color || "#000";
        ctx.fillStyle = data.color || "#000";
    }

    ctx.beginPath();

    switch (type) {
        case "dot":
            ctx.fillRect(data.x, data.y, lineWidth, lineWidth);
            break;

        case "freehand":
        case "eraser":
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
            const text = data.text || prompt("Enter text:");
            if (text) {
                ctx.font = `${lineWidth * 4}px Poppins, sans-serif`;
                ctx.fillText(text, data.startX, data.startY);

                if (!data.text) {
                    data.text = text;
                    dataJson = JSON.stringify(data);
                }
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

