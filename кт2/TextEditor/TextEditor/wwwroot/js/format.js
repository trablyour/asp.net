const area = document.getElementById('Text');
const preview = document.getElementById('preview');

// оборачивает выделенный текст в тег
function wrapSelection(tag) {
    const start = area.selectionStart;
    const end = area.selectionEnd;

    if (start === end) {
        alert('Сначала выделите текст');
        return;
    }

    const selected = area.value.substring(start, end);
    const result = '<' + tag + '>' + selected + '</' + tag + '>';

    area.setRangeText(result, start, end, 'end');
    area.focus();
    updatePreview();
}

function updatePreview() {
    preview.innerHTML = area.value;
}

document.querySelectorAll('.toolbar button').forEach(function (btn) {
    btn.addEventListener('click', function () {
        wrapSelection(btn.dataset.tag);
    });
});

area.addEventListener('input', updatePreview);
