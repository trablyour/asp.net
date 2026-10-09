document.getElementById('send').addEventListener('click', async function () {
    const method = document.getElementById('method').value;
    let url = document.getElementById('url').value;
    const query = document.getElementById('query').value.trim();
    const body = document.getElementById('body').value;

    if (query) {
        url += '?' + query;
    }

    const options = {
        method: method,
        headers: { 'X-Custom-Header': 'test' }
    };

    // GET и DELETE отправляем без тела
    if (method === 'POST' || method === 'PUT') {
        options.body = body;
        options.headers['Content-Type'] = 'text/plain';
    }

    const result = document.getElementById('result');

    try {
        const response = await fetch(url, options);
        const text = await response.text();

        document.getElementById('status').textContent = response.status + ' ' + response.statusText;
        document.getElementById('ctype').textContent = response.headers.get('Content-Type') || '-';

        // если пришел json, выводим красиво
        try {
            result.textContent = JSON.stringify(JSON.parse(text), null, 2);
        } catch {
            result.textContent = text;
        }
    } catch (e) {
        result.textContent = 'Ошибка: ' + e.message;
    }
});
