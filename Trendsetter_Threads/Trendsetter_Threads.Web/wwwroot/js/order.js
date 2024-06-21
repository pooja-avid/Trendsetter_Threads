$(document).ready(function () {
    GetOrderDetails()
});
var carttable;
function GetOrderDetails() {
    $.ajax({
        url: '/Order/GetOrdersDetails',
        method: 'GET',
        success: function (data) {
            carttable = $('#Ordertable').DataTable({
                data: data.data,
                "bPaginate": true,
                "bLengthChange": false,
                "bFilter": false,
                "bInfo": false,
                columns: [
                    { data: 'id' },
                    {
                        data:'totalPrice'
                    },
                    {
                        data: 'status'
                    },
                    {
                        data: null, sorting: false, className: "td-actions", render: function (data, type, row) {
                          
                            return '<button class="btn btn-info btn-just-icon btn-sm view-order" onclick=ViewOrderDetails(this)><i class="material-icons">visibility</i></button>';
                        }
                    }
                ],
            });
        },
        error: function (xhr, res, status) {
            if (xhr.status == 401) {
                window.location.href = xhr.responseJSON.redirectUrl;
                return;
            }
        }
    });
}

function ViewOrderDetails(data) {
    var row = $(data).closest('tr');
    var data = carttable.row(row).data();
    $("#orderId").text(data.id);

    $("#orderDate").text(DateFormatter_MMDDYYYY(data.createdAt));
    var order = {
        Id: data.id
    }

    $.ajax({
        url: '/Order/GetOrderDetails',
        method: 'POST',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify(order),
        success: function (data) {
            if (data.isSuccess) {

                Orderview(data.data);
                $('#orderDetailsModal').modal('show');
                updateTotal();
            } else {
                toastr.error(data.message, "Error");
            }
        },
        error: function (xhr, res, status) {
            if (xhr.status == 401) {
                window.location.href = xhr.responseJSON.redirectUrl;
                return;
            }
            toastr.error(xhr.message, "Error");
        }
    });
}

function Orderview(orderData) {
    $('#orderItemsTable').DataTable().clear().destroy();
    var orderItemTable = $('#orderItemsTable').DataTable({
        data: orderData,
        "bPaginate": false,
        "bLengthChange": false,
        "bFilter": false,
        "bInfo": false,
        columns: [
            {
                data: null, render: function (data) {
                    return data.product.name;
                }
            },
            { data: 'quantity' },
            { data: 'price' },
            {
                data: null, render: function (data) {
                    var total = data.price * data.quantity;
                    return '$' + total.toFixed(2);
                }
            }
        ]
    });
}

function DateFormatter_MMDDYYYY (value) {
    if (value) {
        var formattedDate = moment(value).format('MM/DD/YYYY  HH:mm');
        return formattedDate;
    }
    else {
        return "-";
    }
}

function updateTotal() {
    var total = 0;
    $('#orderItemsTable').DataTable().rows().every(function (rowIdx, tableLoop, rowLoop) {
        var data = this.data();
        total += data.price * data.quantity; // Ensure field names match your data
    });
    $('#grandTotal').text(total.toLocaleString('en-US', { style: 'currency', currency: 'USD' }));
}