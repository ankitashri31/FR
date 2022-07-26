
var DashboardTable;
$(document).ready(function () {


    // OpenConfirmPopup('2');
    BindApplicationdata();

    $("#SearchDate").datepicker({
        changeMonth: true,
        changeYear: true
    });
    $("#txtSearchCloseDate").datepicker({
        changeMonth: true,
        changeYear: true
    });


    //function OpenConfirmPopup() {
    //    $("#MyDeletePopup").dialog({
    //            buttons: {
    //                "Delete all items": function () {
    //                    $(this).dialog("close");
    //                },
    //                Cancel: function () {
    //                    $(this).dialog("close");
    //                }
    //            }
    //        });
    //}

});



function OpenConfirmPopup(id) {
    $('#MyDeletePopup').modal('show');
    //$("#MyDeletePopup").dialog({
    //buttons: {
    //    "Delete all items": function () {
    //        $(this).dialog("close");
    //    },
    //    Cancel: function () {
    //        $(this).dialog("close");
    //    }
    //}
    // });
}

function BindApplicationdata() {
    var FilterValue = "";
    $('#TableVoiData').DataTable({
        "processing": true,
        "serverSide": false,
        "JQueryUI": true,
        "filter": true,
        "orderMulti": false,
        "pagingType": "full_numbers",
        "searching": false,
        "paging": false,
        "ordering": false,
        "info": true,
        "destroy": true,
        "bFilter": false,
        "pageLength": 25,
        "stateSave": false,
        "ajax": {
            "url": "https://localhost:44372/Dumy/GetApplicationData",
            "type": "POST",
            "datatype": "json",
            "data": {
                "FilterSearch": FilterValue,
            }
        },

        "columns": [
            { "data": "id", "name": "id", "autoWidth": true },
            { "data": "name", "name": "name", "autoWidth": true },
            { "data": "channel", "name": "channel", "autoWidth": true },
            { "data": "waitingOn", "name": "waitingOn", "autoWidth": true },
            { "data": "addedBy", "name": "addedBy", "autoWidth": true },
            { "data": "addedOn", "name": "addedOn", "autoWidth": true },
            {
                "render": function (data, type, full, meta) {
                    return '<div class="list-icons"><div class="dropdown"><a href="#" class="list-icons-item" data-toggle="dropdown" aria-expanded="false"><i class="icon-menu9"></i></a><div class="dropdown-menu dropdown-menu-right" x-placement="bottom-end" style="position: absolute; transform: translate3d(-158px, 19px, 0px); top: 0px; left: 0px; will-change: transform;"></a><a href="/TicketDetail" class="dropdown-item show"><i class="icon-eye"></i> View</a><a class="dropdown-item delete openPopup" onclick="OpenConfirmPopup(78)"  ><i class="icon-trash mr-1"></i> Delete</a>';
                }
            },

        ],
        //"fnDrawCallback": function (oSettings) {

        //},
        //"order": [[0, "desc"]],
        "language": {
            "emptyTable": "No data available"
        },
        "createdRow": function (row, data, dataIndex) {
            if (data.status == "Expired") {
                $('td:nth-child(5)', row).css("background-color", "#" + data.colorTheme);
            }

        }

    });
}

