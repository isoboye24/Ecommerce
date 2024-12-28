﻿var dataTable;

$(document).ready(function () {
    loadDataTable();
})

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": { url: '/admin/company/getall'},
        "columns": [
            { "data": 'name', "width": "10%" },
            { "data": 'streetAddress', "width": "25%" },
            { "data": 'city', "width": "13%" },
            { "data": 'state', "width": "13%" },
            { "data": 'postalCode', "width": "12%" },
            { "data": 'phoneNumber', "width": "12%" },
            {
                data: 'companyID',
                "render": function (data) {
                    return `<div class="w-75 btn-group" role="group">
                     <a href="/admin/company/upsert?id=${data}" class="btn btn-primary mx-2"> <i class="bi bi-pencil-square"></i> Edit</a>               
                     <a onClick=Delete('/admin/company/delete/${data}') class="btn btn-danger mx-2"> <i class="bi bi-trash-fill"></i> Delete</a>
                    </div>`
                },
                "width": "15%"
            }
        ]
    });
}

function Delete(url) {
    Swal.fire({
        title: "Are you sure?",
        text: "You won't be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes, delete it!"
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: 'DELETE',
                success: function (data) {
                    dataTable.ajax.reload();
                    toastr.success(data.message);
                }
            })
        }
    });
}