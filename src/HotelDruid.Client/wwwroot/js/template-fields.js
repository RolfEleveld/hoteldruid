window.hotelDruidTemplate = {
  insertAtCursor: function (element, text) {
    if (!element) return text;
    const start = element.selectionStart ?? element.value.length;
    const end = element.selectionEnd ?? start;
    const next = element.value.slice(0, start) + text + element.value.slice(end);
    element.value = next;
    const caret = start + text.length;
    element.selectionStart = element.selectionEnd = caret;
    element.focus();
    return next;
  }
};
