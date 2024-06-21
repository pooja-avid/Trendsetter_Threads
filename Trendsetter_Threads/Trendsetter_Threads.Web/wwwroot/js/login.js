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

$(document).ready(function () {
    $("#signUpform").validate({
        rules: {
            fullname: {
                required: true
            },
            emailaddress: {
                required: true,
                email: true
            },
            password : {
                required: true,
                minlength: 8
                
            },
            termsCheckbox: {
                required: true // Ensure terms checkbox is checked
            }
        },
        messages: {
            fullname: {
                required: 'Please enter username'
            },
            emailaddress: {
                required: 'Please enter email address',
                email: 'Please enter a valid email address'
            },
            password: {
                required: 'Please enter password',
                minlength: 'password should be atleast 8 characters long.',
            },
            termsCheckbox: {
                required: 'Please accept the terms and conditions to proceed.'
            }

        },
        errorPlacement: function (error, element) {
            if (element.parent().hasClass('input-group')) {
                error.insertAfter(element.parent());
            }
            else if (element.parent().hasClass('form-check')) {
                error.insertAfter(element.parent());
            }
            else {
                error.insertAfter(element);
            }
        },
        submitHandler: function (form) {
            SignUpNewUser()
        }
    });


    $("#loginform").validate({
        rules: {
            uname: {
                required: true
            },
           
            password: {
                required: true
            }
        },
        messages: {
            uname: {
                required: 'Please enter username'
            },
          
            password: {
                required: 'Please enter password'
            }

        },
        errorPlacement: function (error, element) {
            if (element.parent().hasClass('input-group')) {
                error.insertAfter(element.parent());
            } else {
                error.insertAfter(element);
            }
        },
        submitHandler: function (form) {
            LoggedInExistingUser()
        }
    });
});  

function SignUpNewUser() {
    var data = {
        Username: $('#uname').val().trim(),
        Email: $('#emailaddress').val().trim(),
        Password: $('#password').val()
    }
	$(".loader").show();
    $.ajax({
        type: "POST",
        url: "/User/Register",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify(data),
        success: function (result) {
			$(".loader").hide();
            if (result.isSuccess) {
                Toast.fire({
                    icon: "success",
                    title: data.message
                });
                window.location.href = '/User/Index';
                
            } else {
                Toast.fire({
                    icon: "error",
                    title: data.message
                });
            }

        },
        error: function (result) {
			$(".loader").hide();
            Toast.fire({
                icon: "error",
                title: result.responseText
            });
        }
    });
}

function LoggedInExistingUser() {
    var data = {
        Username: $("#uname").val().trim(),
        Password: $("#password").val()
    }
	$(".loader").show();
    $.ajax({
        type: "POST",
        url: "/User/Login",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify(data),
        success: function (result) {
            //localStorage.removeItem('cart');
			$(".loader").hide();
            if (result.isSuccess) {
                Toast.fire({
                    icon: "success",
                    title: result.message
                });
                window.location.href = "/Product";
            } else {
                Toast.fire({
                    icon: "error",
                    title: result.message
                });
            }
        },
        error: function (result) {
			$(".loader").hide();
            Toast.fire({
                icon: "error",
                title: result.responseText
            });
        }
    });
}