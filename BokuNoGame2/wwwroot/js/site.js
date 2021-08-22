// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function logout() {
    fetch("/api/Account/Logout", {
        method: "POST"
    }).then(response => {
        if (response.status == 200) {
            window.localStorage.removeItem("userInfo");
            window.location.replace("/");
        }
    });
}
