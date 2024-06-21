
$(function () {
    fetchProductData();
    $("#productForm").validate({
        rules: {
            productname: {
                required: true
            },

            description: {
                required: true
            },
            productprice: {
                required: true
            },

            stock: {
                required: true
            }
        },
        messages: {
            productname: {
                required: 'Please enter username'
            },

            description: {
                required: 'Please enter password'
            },
            productprice: {
                required: 'Please enter username'
            },

            stock: {
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
            SaveProductData()
        }
    });
});

function fetchProductData() {
    $.ajax({
        url: '/Product/GetProductsList',
        method: 'GET',
        success: function (data) {
            if (data.data.length > 0) {
                var productData = data.data;
                var productdiv = "";
                $.each(productData, function (key, val) {

                    var shortdesc = val.description.substring(0, 80) + " ...";

                    productdiv += ` <div class="col-xl-4 col-sm-6 mb-xl-0 mb-4 mt-4">
                        <div class="card">
                            <div class="card-header p-3 pt-2">
                                <div class="icon icon-lg icon-shape bg-gradient-dark shadow-dark text-center border-radius-xl mt-n4 position-absolute">
                                    <i class="material-icons opacity-10">weekend</i>
                                </div>
                                <div class="text-end pt-1">
                                    <p class="text-sm mb-0 text-capitalize">`+ val.name + `</p>
                                    <h5 class="mb-0">Price($) : `+ val.price + `</h5>
                                    <h5 class="mb-0">Stock : `+ val.stock + `</h5>
                                </div>
                            </div>
                            <hr class="dark horizontal my-0">
                            <div class="card-footer p-3">
                             <p class="mb-0" title="`+ val.description + `"><span class="text-success text-sm font-weight-bolder"></span>` + shortdesc + `</p>
                             <div class="ms-auto text-end cls-admin-field">
                                 <a class="btn btn-link text-danger text-gradient px-3 mb-0" onclick="DeleteProduct(`+ val.id + `)"><i class="material-icons text-sm me-2">delete</i>Delete</a>
                                 <a class="btn btn-link text-dark px-3 mb-0" onclick='OpenPopupModalForEdit(`+ val.id + `,"` + val.description + `","` + val.name + `",` + val.price + `,` + val.stock + `)'><i class="material-icons text-sm me-2">edit</i>Edit</a>
                            </div>
                             <div class="ms-auto text-end cls-user-field">
                                 <a class="btn btn-link text-info text-gradient px-3 mb-0" onclick='AddToCart(`+ val.id + `,"` + val.description + `","` + val.name + `",` + val.price + `,` + val.stock + `)'><i class="material-icons text-info text-gradient">shopping_cart</i>Add to cart</a>
                            </div>
                            </div>
                        </div>
                    </div>`
                });

                $("#productList").html(productdiv);
                var checkUser = $("#hdnAdmin").val();
                if (checkUser != 1) {
                    $(".cls-admin-field").addClass("d-none");
                    $(".cls-user-field").removeClass("d-none");
                } else {
                    $(".cls-admin-field").removeClass("d-none");
                    $(".cls-user-field").addClass("d-none");
                }
            } else {
                var productdiv = "";
                productdiv += `<div class="alert alert-secondary alert-dismissible text-white" role="alert">
                    <span class="text-sm">No product is available.</span>
                   
                </div>`

                $("#productList").html(productdiv);
            }
        },
        error: function (xhr, res, status) {
            if (xhr.status == 401) {
                window.location.href = xhr.responseJSON.redirectUrl;
                return;
            }
        }
    });
}

function SaveProductData() {
    var product = {
        Name: $("#productname").val(),
        Description: $("#description").val(),
        Price: $("#productprice").val(),
        Stock: $("#stock").val()
    }

    $.ajax({
        url: '/Product/CreateProduct', // Replace with your API URL
        method: 'POST',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify(product),
        success: function (data) {
            if (data.isSuccess) {
                CloseModalpopUp();
                Swal.fire(
                    'Saved!',
                    data.message,
                    'success'
                );

                fetchProductData();
            } else {
                Swal.fire(
                    'Error!',
                    data.message,
                    'Error'
                );
            }
        },
        error: function (xhr, res, status) {
            if (xhr.status == 401) {
                window.location.href = xhr.responseJSON.redirectUrl;
                return;
            }
        }
    });
}

function DeleteProduct(productId) {
    Swal.fire({
        title: "Are you sure you want to delete?",
        showCancelButton: true,
        confirmButtonText: "Delete",
    }).then((result) => {
        if (result.isConfirmed) {
            var product = {
                Id: productId
            }

            $.ajax({
                url: '/Product/DeleteProductById',
                method: 'POST',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: JSON.stringify(product),
                success: function (data) {
                    if (data.isSuccess) {
                        Swal.fire(
                            'Deleted!',
                            'Your file has been deleted.',
                            'success'
                        );
                        fetchProductData();
                    }
                    else {
                        Swal.fire(
                            'Error!',
                            data.message,
                            'Error'
                        );
                    }
                },
                error: function (xhr, res, status) {
                    if (xhr.status == 401) {
                        window.location.href = xhr.responseJSON.redirectUrl;
                        return;
                    }
                }
            });
        } 
    });

}

function OpenPopupModalForEdit(productId, productdescription, productname, price, stock) {

    $('#newproductModal').modal('show');
    $('#productname').val(productname);
    $('#description').val(productdescription);
    $('#productprice').val(price);
    $('#stock').val(stock);
    $('#hdnProductId').val(productId);
}

function CloseModalpopUp() {
    $('#productname').val('');
    $('#description').val('');
    $('#productprice').val('');
    $('#stock').val('');
    $('#newproductModal').modal('hide');
}

function AddToCart(productId, productdescription, productname, price, stock) {

    var product = {
        name: productname,
        description: productdescription,
        price: price,
        quantity: 1,
        stock: stock,
        id: productId
    };

    if (product.stock > 0) {
        var cart = JSON.parse(localStorage.getItem('cart')) || [];
        var found = false;

        for (var i = 0; i < cart.length; i++) {
            if (cart[i].id === product.id) {
                cart[i].quantity += 1;
                if (cart[i].quantity > product.stock) {
                    toastr.error("Maximum quantity of " + product.name + " has been added to your cart!", "Error");
                    return "";
                    break;
                }
                found = true;
                break;
            }
        }

        if (!found) {
            cart.push(product);
        }

        localStorage.setItem('cart', JSON.stringify(cart));
        $("#cartItemCount").text(cart.length);
        /* toastr.success(product.name + " has been added to your cart!", "Success");*/
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
        Toast.fire({
            icon: "success",
            title: product.name + " has been added to your cart!"
        });
    } else {
        Swal.fire({
            title: 'Error!',
            text: 'Product is out of stock.',
            icon: 'error',
            confirmButtonText: 'Close'
        })
    }
}