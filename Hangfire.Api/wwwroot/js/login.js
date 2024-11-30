function redirectHangfire() {
    window.location.href = '/hangfire';
}

$("#login-button").click(function (event) {
    event.preventDefault();

    // Prepare the data
    const username = $('#username').val();
    const password = $('#password').val();

    $.ajax({
        url: '/login', // Your API endpoint
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({ username, password }),
        success: function (response) {
            $('form').fadeOut(500);
            $('.wrapper').addClass('form-success');
            // Redirect to Hangfire dashboard
            setTimeout(redirectHangfire, 3000);
        },
        error: function (xhr) {
            // Handle errors
            const errorMessage = xhr.responseJSON?.title || "Please check your username and password.";
            Swal.fire({
                icon: "error",
                title: "Oops... An error occurred.",
                text: errorMessage,
            });
        }
    });
});