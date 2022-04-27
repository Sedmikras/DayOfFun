$(document).ready(function () {
    var message = document.getElementById("successMessage");
    if (message.textContent) {
        toastr.success(message.textContent);
    }
    message = document.getElementById("errorMessage");
    if (message.textContent) {
        toastr.error(message.textContent);
    }
});