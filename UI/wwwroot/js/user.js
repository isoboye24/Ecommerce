var dataTable;

$(document).ready(function () {
    loadDataTable();
})

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": { url: '/admin/user/getall'},
        "columns": [
            { "data": 'name', "width": "10%" },
            { "data": 'email', "width": "25%" },
            { "data": 'phoneNumber', "width": "15%" },
            { "data": 'company.name', "width": "20%" },
            { "data": 'role', "width": "15%" },
            {
                data: 'id',
                "render": function (data) {
                    return `<div class="w-75 btn-group" role="group">
                     <a href="/admin/user/upsert?id=${data}" class="btn btn-primary mx-2"> <i class="bi bi-pencil-square"></i> Edit</a>               
                     
                    </div>`
                },
                "width": "15%"
            }
        ]
    });
}
