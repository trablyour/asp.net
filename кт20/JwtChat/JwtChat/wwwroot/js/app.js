"use strict";

const TOKEN_KEY = "jwtchat-token";

// ---------- элементы ----------
const $ = id => document.getElementById(id);

const authView = $("authView");
const chatView = $("chatView");
const userbox = $("userbox");

const authForm = $("authForm");
const usernameInput = $("username");
const passwordInput = $("password");
const confirmInput = $("confirm");
const confirmBlock = $("confirmBlock");
const authErrors = $("authErrors");
const authSubmit = $("authSubmit");

const messagesEl = $("messages");
const usersEl = $("users");
const typingEl = $("typing");
const statusEl = $("status");
const sendForm = $("sendForm");
const messageInput = $("messageInput");
const sendBtn = $("sendBtn");

let mode = "login";
let token = null;
let payload = null;
let connection = null;
let expiryTimer = null;

// ---------- работа с токеном ----------

// payload JWT - это base64url JSON, его можно прочитать без ключа (но не подделать)
function decodeToken(jwt) {
    try {
        const part = jwt.split(".")[1].replace(/-/g, "+").replace(/_/g, "/");
        const json = decodeURIComponent(atob(part).split("").map(c =>
            "%" + ("00" + c.charCodeAt(0).toString(16)).slice(-2)).join(""));
        return JSON.parse(json);
    } catch {
        return null;
    }
}

function isExpired(p) {
    return !p || !p.exp || p.exp * 1000 <= Date.now();
}

function saveToken(value) {
    token = value;
    payload = decodeToken(value);
    localStorage.setItem(TOKEN_KEY, value);
}

function clearToken() {
    token = null;
    payload = null;
    localStorage.removeItem(TOKEN_KEY);
}

// запрос к API с заголовком Authorization: Bearer <токен>
async function api(url, options = {}) {
    const headers = { "Content-Type": "application/json", ...(options.headers || {}) };
    if (token) headers["Authorization"] = "Bearer " + token;

    const response = await fetch(url, { ...options, headers });
    let data = null;
    try { data = await response.json(); } catch { }
    return { ok: response.ok, status: response.status, data };
}

// ---------- вход и регистрация ----------

document.querySelectorAll(".tab").forEach(tab => {
    tab.addEventListener("click", () => {
        mode = tab.dataset.tab;
        document.querySelectorAll(".tab").forEach(t => t.classList.toggle("active", t === tab));
        confirmBlock.classList.toggle("hidden", mode !== "register");
        authSubmit.textContent = mode === "register" ? "Зарегистрироваться" : "Войти";
        passwordInput.autocomplete = mode === "register" ? "new-password" : "current-password";
        showErrors([]);
    });
});

function showErrors(list) {
    authErrors.innerHTML = "";
    list.forEach(text => {
        const div = document.createElement("div");
        div.textContent = text;
        authErrors.appendChild(div);
    });
}

// ошибки валидации ASP.NET приходят в виде { errors: { Поле: [тексты] } }
function extractErrors(data) {
    if (!data) return ["Ошибка сервера"];
    if (data.error) return [data.error];
    if (data.errors) return Object.values(data.errors).flat();
    return ["Неизвестная ошибка"];
}

authForm.addEventListener("submit", async e => {
    e.preventDefault();

    const body = {
        username: usernameInput.value.trim(),
        password: passwordInput.value
    };

    if (mode === "register" && body.password !== confirmInput.value) {
        showErrors(["Пароли не совпадают"]);
        return;
    }

    authSubmit.disabled = true;
    const result = await api(`/api/auth/${mode}`, { method: "POST", body: JSON.stringify(body) });
    authSubmit.disabled = false;

    if (!result.ok) {
        showErrors(extractErrors(result.data));
        return;
    }

    saveToken(result.data.token);
    authForm.reset();
    showErrors([]);
    await openChat();
});

$("logoutBtn").addEventListener("click", () => logout());

async function logout(reason) {
    clearToken();
    clearInterval(expiryTimer);

    if (connection) {
        await connection.stop().catch(() => { });
        connection = null;
    }

    chatView.classList.add("hidden");
    userbox.classList.add("hidden");
    authView.classList.remove("hidden");
    messagesEl.innerHTML = "";

    if (reason) showErrors([reason]);
}

// ---------- шапка: пользователь и таймер токена ----------

function showUser() {
    $("userName").textContent = "👤 " + payload.unique_name;
    $("userRole").textContent = payload.role;
    $("userRole").className = "role " + (payload.role === "Admin" ? "admin" : "");
    $("clearBtn").classList.toggle("hidden", payload.role !== "Admin");
    userbox.classList.remove("hidden");

    $("tokenRaw").textContent = token;
    $("tokenPayload").textContent = JSON.stringify(payload, null, 2);

    clearInterval(expiryTimer);
    updateExpiry();
    expiryTimer = setInterval(updateExpiry, 1000);
}

function updateExpiry() {
    const left = Math.max(0, Math.floor(payload.exp - Date.now() / 1000));
    const min = String(Math.floor(left / 60)).padStart(2, "0");
    const sec = String(left % 60).padStart(2, "0");
    $("expiresIn").textContent = `${min}:${sec}`;

    if (left === 0)
        logout("Срок действия токена истек. Войдите заново.");
}

// ---------- чат ----------

async function openChat() {
    // проверяем токен на сервере
    const me = await api("/api/auth/me");
    if (!me.ok) {
        logout("Токен недействителен, войдите заново");
        return;
    }

    authView.classList.add("hidden");
    chatView.classList.remove("hidden");
    showUser();

    await connect();
}

async function connect() {
    // accessTokenFactory: SignalR добавит токен к подключению (?access_token=...)
    connection = new signalR.HubConnectionBuilder()
        .withUrl("/chatHub", { accessTokenFactory: () => token })
        .withAutomaticReconnect()
        .build();

    connection.on("ReceiveMessage", msg => {
        addMessage(msg);
        if (typingEl.dataset.user === msg.userName) hideTyping();
    });
    connection.on("UserJoined", name => addSystem(`${name} вошел в чат`));
    connection.on("UserLeft", name => addSystem(`${name} вышел из чата`));
    connection.on("OnlineUsers", renderUsers);
    connection.on("UserTyping", showTyping);
    connection.on("ChatCleared", admin => {
        messagesEl.innerHTML = "";
        addSystem(`Администратор ${admin} очистил чат`);
    });

    connection.onreconnecting(() => { setStatus("Переподключение...", "warn"); setEnabled(false); });
    connection.onreconnected(() => { setStatus("В сети", "ok"); setEnabled(true); });
    connection.onclose(() => { setStatus("Нет соединения", "error"); setEnabled(false); });

    try {
        setStatus("Подключение...");
        await connection.start();
        setStatus("В сети", "ok");

        messagesEl.innerHTML = "";
        const history = await connection.invoke("GetHistory");
        if (history.length === 0)
            addSystem("Сообщений пока нет. Напишите первым!");
        else
            history.forEach(addMessage);

        setEnabled(true);
        messageInput.focus();
    } catch (err) {
        console.error(err);
        setStatus("Нет доступа", "error");
        addSystem("Не удалось подключиться к чату: " + (err.message || err));
    }
}

function setStatus(text, css) {
    statusEl.textContent = text;
    statusEl.className = "status " + (css || "");
}

function setEnabled(enabled) {
    messageInput.disabled = !enabled;
    sendBtn.disabled = !enabled;
}

function formatTime(iso) {
    return new Date(iso).toLocaleTimeString("ru-RU", { hour: "2-digit", minute: "2-digit" });
}

function scrollDown() {
    messagesEl.scrollTop = messagesEl.scrollHeight;
}

// textContent вместо innerHTML: HTML в сообщении не выполнится
function addMessage(msg) {
    const mine = payload && msg.userName === payload.unique_name;

    const item = document.createElement("div");
    item.className = "message" + (mine ? " mine" : "");

    const author = document.createElement("div");
    author.className = "author";
    author.textContent = mine ? "Вы" : msg.userName;

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
    $("onlineCount").textContent = users.length;
    users.forEach(name => {
        const li = document.createElement("li");
        const me = payload && name === payload.unique_name;
        li.textContent = name + (me ? " (вы)" : "");
        if (me) li.className = "me";
        usersEl.appendChild(li);
    });
}

let typingTimer = null;

function showTyping(name) {
    typingEl.textContent = `${name} печатает...`;
    typingEl.dataset.user = name;
    clearTimeout(typingTimer);
    typingTimer = setTimeout(hideTyping, 2500);
}

function hideTyping() {
    typingEl.textContent = "";
    typingEl.dataset.user = "";
}

sendForm.addEventListener("submit", async e => {
    e.preventDefault();
    const text = messageInput.value.trim();
    if (!text) return;

    try {
        await connection.invoke("SendMessage", text);
        messageInput.value = "";
        $("counter").textContent = "0";
        messageInput.focus();
    } catch (err) {
        addSystem("Ошибка: " + (err.message || err));
    }
});

let lastTyping = 0;
messageInput.addEventListener("input", () => {
    $("counter").textContent = messageInput.value.length;
    if (Date.now() - lastTyping > 1000 && messageInput.value) {
        lastTyping = Date.now();
        connection?.invoke("Typing").catch(() => { });
    }
});

$("clearBtn").addEventListener("click", async () => {
    if (!confirm("Удалить все сообщения?")) return;
    try {
        await connection.invoke("ClearChat");
    } catch (err) {
        addSystem("Ошибка: " + (err.message || err));
    }
});

// панель с токеном
$("tokenBtn").addEventListener("click", () => $("tokenPanel").classList.toggle("hidden"));

$("meBtn").addEventListener("click", async () => {
    const result = await api("/api/auth/me");
    const box = $("meResult");
    box.classList.remove("hidden");
    box.textContent = `HTTP ${result.status}\n` + JSON.stringify(result.data, null, 2);
});

// ---------- старт ----------

(async function init() {
    const saved = localStorage.getItem(TOKEN_KEY);

    if (saved) {
        const p = decodeToken(saved);
        if (!isExpired(p)) {
            saveToken(saved);
            await openChat();
            return;
        }
        clearToken();
    }

    authView.classList.remove("hidden");
})();
