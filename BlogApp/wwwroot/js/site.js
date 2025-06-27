// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(document).ready(function() {
    $("#btnPostCount").click(function() {
        $.ajax({
            url: '/Post/GetPostCount',
            type: 'GET',
            success: function(data) {
                $("#postCountResult").text("Toplam blog yazısı: " + data.count);
            },
            error: function() {
                $("#postCountResult").text("Hata oluştu!");
            }
        });
    });
});
