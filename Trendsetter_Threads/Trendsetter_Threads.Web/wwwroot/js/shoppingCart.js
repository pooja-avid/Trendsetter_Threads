

$(document).ready(function () {
    GetCartData()
});
var table;
var cartData;
function GetCartData() {
    cartData = JSON.parse(localStorage.getItem('cart')) || [];
    table = $('#carttable').DataTable({
        data: cartData,
        "bPaginate": false,
        "bFilter": false,
        "bInfo": false,
        "bLengthChange": false,
        columns: [
            { data: null },
            {
                data: null, className: 'td-name', render: function (data, type, row) {
                    var shortdesc = row.description.substring(0, 50) + " ...";
                    return `${row.name}<br><small title="` + row.description +`">${shortdesc}</small>`;
                }
            },
            {
                data: null, className: 'td-number text-right', render: function (data, type, row) {
                    return `<small>$</small>${row.price}`
                }
            },
            {
                data: 'quantity', className: 'td-number', render: function (data, type, row) {
                    return `<div class="input-group input-group-sm">
                          <input type="text" class="form-control text-center quantity mx-1" style="max-width: 50px;" value="${data}" readonly>
                          <div class="btn-group btn-group-sm">
                              <button class="btn btn-round btn-info" onclick="updateQuantity(${row.id}, -1)"> <i class="material-icons">remove</i> </button>
                              <button class="btn btn-round btn-info" onclick="updateQuantity(${row.id}, 1)"> <i class="material-icons">add</i> </button>
                          </div>`;
                }
            },
            {
                data: null, className: 'td-number', render: function (data, type, row) {
                    return `<small>$</small> ${(row.price * row.quantity).toFixed(2)}`;
                }
            },
            {
                data: null, className: 'td-actions', sorting: false, render: function (data, type, row) {
                    return `<button type="button" rel="tooltip" data-placement="left" title="" class="btn btn-link" onclick="removeItem(${row.id})">
                           <i class="material-icons">close</i>
                          </button>`;
                }
            }
        ],
        drawCallback: function (settings) {
            updateTotal();
        },
        rowCallback: function (row, data, index) {
            $('td:eq(0)', row).html(index + 1); // Adding serial number
        }
    });
}

function updateQuantity(productId,num) {
    var rowIdx = table.rows().eq(0).filter(function (rowIdx) {
        return table.row(rowIdx).data().id === productId;
    });
    if (rowIdx.length) {
        var rowData = table.row(rowIdx).data();
        var newQuantity = rowData.quantity + num;
        if (newQuantity > 0 && newQuantity <= rowData.stock) {
            rowData.quantity = newQuantity;
            table.row(rowIdx).data(rowData).draw();
            updateTotal();
            cartData[rowIdx[0]] = rowData;
            updateLocalStorage(cartData);
        } else if (newQuantity == 0) {
            removeItem(rowData.id);
        }
    }
}

function removeItem(productId) {
    var rowIdx = table.rows().eq(0).filter(function (rowIdx) {
        return table.row(rowIdx).data().id === productId;
    });
    if (rowIdx.length) {
        var rowData = table.row(rowIdx).data();
        table.row(rowIdx).remove().draw();
        cartData = cartData.filter(item => item.id !== productId);
        updateTotal();
        updateLocalStorage(cartData);
    }
}

function updateLocalStorage(cartData) {
    localStorage.setItem('cart', JSON.stringify(cartData));
}

function updateTotal() {
    var total = 0;
    $('#carttable').DataTable().rows().every(function (rowIdx, tableLoop, rowLoop) {
        var data = this.data();
        total += data.price * data.quantity; // Ensure field names match your data
    });
    $('#total-price').text(total.toLocaleString('en-US', { style: 'currency', currency: 'USD' }));
}

function PlaceOrder() {
    var cartData = JSON.parse(localStorage.getItem('cart')) || [];
    if (cartData.length > 0) {
        var orderItems = [];
        $(cartData).each(function (rowData, data) {
            var item = {
                ProductId: data.id,
                Quantity: data.quantity
            }
            orderItems.push(item);
        });

        model = {
            OrderItems: orderItems
        };
        $.ajax({
            url: '/Order/CreateOrder',
            method: 'POST',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify(model),
            success: function (data) {
                if (data.isSuccess) {
                    Toast.fire({
                        icon: "success",
                        title: data.message
                    });
                    localStorage.removeItem('cart');
                    window.location = "/Product";

                }
                else {
                    Toast.fire({
                        icon: "Error",
                        title: data.message
                    });
                }
            }, error: function (xhr, res, status) {
                if (xhr.status == 401) {
                    window.location.href = xhr.responseJSON.redirectUrl;
                    return;
                }
            }
        });

    } else {
        toastr.error("Cart is Empty", "Error");
    }
}