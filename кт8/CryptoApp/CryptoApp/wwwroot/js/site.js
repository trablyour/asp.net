// показываем поля ключей только для выбранного алгоритма
const algorithm = document.getElementById('algorithm');
const aesKeys = document.getElementById('aes-keys');
const rsaKeys = document.getElementById('rsa-keys');

function toggleKeys() {
    const isRsa = algorithm.value === 'RSA';
    aesKeys.style.display = isRsa ? 'none' : 'block';
    rsaKeys.style.display = isRsa ? 'block' : 'none';
}

algorithm.addEventListener('change', toggleKeys);
toggleKeys();
