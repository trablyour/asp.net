"use strict";

const chatEl = document.getElementById("chat");
const currentUser = chatEl.dataset.user;

const messagesEl = document.getElementById("messages");
const usersEl = document.getElementById("users");
const onlineCountEl = document.getElementById("onlineCount");
const statusEl = document.getElementById("status");
const typingEl = document.getElementById("typing");
const form = document.getElementById("sendForm");
const input = document.getElementById("messageInput");
const sendButton = document.getElementById("sendButton");
const counterEl = document.getElementById("counter");

// подключение к хабу; кука входа отправляется браузером автоматически
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chatHub")
    .withAutomaticReconnect()
    .build();

// ---------- вывод ----------

function formatTime(iso) {
    const date = new Date(iso);
    return date.toLocaleTimeString("ru-RU", { hour: "2-digit", minute: "2-digit" });
}

function scrollDown() {
    messagesEl.scrollTop = messagesEl.scrollHeight;
}

// текст вставляем через textContent, а не innerHTML - защита от XSS
function addMessage(msg) {
    const isMine = msg.userName === currentUser;

    const item = document.createElement("div");
    item.className = "message" + (isMine ? " mine" : "");

    const author = document.createElement("div");
    author.className = "author";
    author.textContent = isMine ? "Вы" : msg.userName;

    const text = document.createElement("div");
    text.className = "text";
    text.textContent = msg.text;

    const time = document.createElement("div");
    time.className = "time";
    time.textContent = formatTime(msg.sentAt);

    item.append(author, text, time);
    messagesEl.appendChild(item);
    scrollDown();
}

function addSystem(text) {
    const item = document.createElement("div");
    item.className = "system";
    item.textContent = text;
    messagesEl.appendChild(item);
    scrollDown();
}

function renderUsers(users) {
    usersEl.innerHTML = "";
    onlineCountEl.textContent = users.length;

    users.forEach(name => {
        const li = document.createElement("li");
        li.textContent = name + (name === currentUser ? " (вы)" : "");
        if (name === currentUser) li.className = "me";
        usersEl.appendChild(li);
    });
}

function setStatus(text, css) {
    statusEl.textContent = text;
    statusEl.className = "status " + (css || "");
}

function setEnabled(enabled) {
    input.disabled = !enabled;
    sendButton.disabled = !enabled;
}

// ---------- события от сервера ----------

connection.on("ReceiveMessage", msg => {
    addMessage(msg);
    // если человек отправил сообщение, он больше не печатает
    if (typingEl.dataset.user === msg.userName) hideTyping();
});

connection.on("UserJoined", name => addSystem(`${name} вошел в чат`));
connection.on("UserLeft", name => addSystem(`${name} вышел из чата`));
connection.on("OnlineUsers", renderUsers);

let typingTimer = null;

function hideTyping() {
    typingEl.textContent = "";
    typingEl.dataset.user = "";
}

connection.on("UserTyping", name => {
    typingEl.textContent = `${name} печатает...`;
    typingEl.dataset.user = name;
    clearTimeout(typingTimer);
    typingTimer = setTimeout(hideTyping, 2500);
});

connection.onreconnecting(() => {
    setStatus("Переподключение...", "warn");
    setEnabled(false);
});

connection.onreconnected(() => {
    setStatus("В сети", "ok");
    setEnabled(true);
});

connection.onclose(() => {
    setStatus("Нет соединения", "error");
    setEnabled(false);
});

// ---------- отправка ----------

form.addEventListener("submit", async e => {
    e.preventDefault();

    const text = input.value.trim();
    if (!text) return;

    try {
        await connection.invoke("SendMessage", text);
        input.value = "";
        counterEl.textContent = "0";
        input.focus();
    } catch (err) {
        addSystem("Ошибка: " + (err.message || err));
    }
});

// сообщаем, что печатаем, не чаще раза в секунду
let lastTypingSent = 0;

input.addEventListener("input", () => {
    counterEl.textContent = input.value.length;

    const now = Date.now();
    if (now - lastTypingSent > 1000 && input.value.length > 0) {
        lastTypingSent = now;
        connection.invoke("Typing").catch(() => { });
    }
});

// ---------- старт ----------

async function start() {
    try {
        await connection.start();
        setStatus("В сети", "ok");

        const history = await connection.invoke("GetHistory");
        if (history.length === 0) {
            addSystem("Сообщений пока нет. Напишите первым!");
        } else {
            history.forEach(addMessage);
            addSystem("— новые сообщения —");
        }

        setEnabled(true);
        input.focus();
    } catch (err) {
        console.error(err);
        setStatus("Ошибка подключения", "error");
    }
}

start();
