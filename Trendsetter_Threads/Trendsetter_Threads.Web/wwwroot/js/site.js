
const Toast = Swal.mixin({
    toast: true,
    position: "top-end",
    showConfirmButton: false,
    timer: 3000,
    timerProgressBar: true,
    didOpen: (toast) => {
        toast.onmouseenter = Swal.stopTimer;
        toast.onmouseleave = Swal.resumeTimer;
    }
});

window.logout = function () {
    localStorage.removeItem("cart");
    $.ajax({
        url: '/User/Logout', // Replace with your API URL
        method: 'Get',
        success: function (data) {
            toastr.success(data, "Success");
            window.location = "/";
        },
        error: function (xhr, status, error) {
            toastr.error(error, "Error");
        }
    });
}