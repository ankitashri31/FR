var WebsitePath;
var DashboardTable;

$(document).ready(function () {

    WebsitePath = $("#hdnWebsitePath").val();
    
    $("#txtSearchCloseDate").datepicker({
        changeMonth: true,
        changeYear: true,
        dateFormat: "dd/mm/yy",
        min: true,
        maxDate: new Date()
    });
   

    $("#txtCreatedActiveTicket").datepicker({
        changeMonth: true,
        changeYear: true,
        dateFormat: "dd/mm/yy",
        min: true,
        maxDate: new Date()
    });
   

    BindTicketData();
    BindClosedTicketData();

    $("#SearchText").keypress(function (e) {
        var keyCode = e.which;

        /* 
        48-57 - (0-9)Numbers
        65-90 - (A-Z)
        97-122 - (a-z)
        8 - (backspace)
        32 - (space)
        */
        // Not allow special 
        if (!((keyCode >= 48 && keyCode <= 57)
            || (keyCode >= 65 && keyCode <= 90)
            || (keyCode >= 97 && keyCode <= 122))
            && keyCode != 8 && keyCode != 32) {
            e.preventDefault();
        }

        BindTicketData();
    });


    $("#SearchTextForClosed").keypress(function (e) {
        var keyCode = e.which;

        /* 
        48-57 - (0-9)Numbers
        65-90 - (A-Z)
        97-122 - (a-z)
        8 - (backspace)
        32 - (space)
        */
        // Not allow special 
        if (!((keyCode >= 48 && keyCode <= 57)
            || (keyCode >= 65 && keyCode <= 90)
            || (keyCode >= 97 && keyCode <= 122))
            && keyCode != 8 && keyCode != 32) {
            e.preventDefault();
        }

        BindClosedTicketData();
    });

    $("#Dashboard").addClass('active');
})


function RedirectToHwleAddTicket() {
    window.location.href = WebsitePath + "TicketUser/HWLEAddTicket";
}

function RedirectToAddTicket() {
    window.location.href = WebsitePath + "TicketUser/AddTicket";
}

function BindTicketData() {

    var ChannelId = $("#txtActiveChannel").val();
    var WaitingOnId = $("#txtWaitingOn").val();
    var CreatedActiveTicketOn = $("#txtCreatedActiveTicket").val();
    var SearchActiveTicket = $("#SearchActiveTicket").val();

    var SearchModel = {
        ChannelId: ChannelId,
        OrganisationId: WaitingOnId,
        CreatedOn: CreatedActiveTicketOn,
        TextSearch: SearchActiveTicket
    }
    console.log(SearchModel);

    var OrganisationId = $("#LoggedInOrganisationId").val();


    DashboardTable = $('#TableActiveTicket').DataTable({
        "processing": true,
        "serverSide": false,
        "JQueryUI": true,
        "filter": true,
        "orderMulti": false,
        "pagingType": "full_numbers",
        "searching": false,
        "paging": true,
        "ordering": true,
        "info": true,
        "destroy": true,
        "bFilter": false,
        "pageLength": 25,
        "stateSave": false,

        "ajax": {
            "url": "GetActiveTicketBySearch",
            "type": "POST",
            "datatype": "json",
            "data": { ChannelId: ChannelId, Organisationid: WaitingOnId, CreatedOn: CreatedActiveTicketOn, TextSearch: SearchActiveTicket},
        },

        "columns": [

            {
                "data": "id", "name": "id",
                fnCreatedCell: function (nTd, sData, oData, iRow, iCol) {
                    $(nTd).html("<a href='" + WebsitePath + "ticketuser/TicketDetail?id=" + oData.encryptedTicketId + "'>" + oData.id + "</a>");
                }
            },
            //{
            //    "data": "id", "name": "id", "autoWidth": true
            //},
            { "data": "name", "name": "name", "autoWidth": true },
            { "data": "channel", "name": "channel", "autoWidth": true },
            { "data": "waitingOn", "name": "waitingOn", "autoWidth": true },
            { "data": "addedBy", "name": "addedBy", "autoWidth": true },
            { "data": "addedOn", "name": "addedOn", "autoWidth": true },
            {

                "render": function (data, type, full, meta) {
                    if (OrganisationId == '3' || OrganisationId == '1') {

                        return '<a href="#" onclick="OpenCloseTicketPopup(' + full.id + ');" class="list-icons-item item-close" data-toggle="dropdown" aria-expanded="false" title="Close Ticket"><i class="far fa-window-close"></i></a>';

                    }
                    else {
                        return "NA";
                    }
                },
            }
        ],
       
        "language": {
            "emptyTable": "No data available"
        },
        columnDefs: [{ type: 'date', 'targets': [6] }],
        order: [[6, 'desc']], 
       // "aaSorting": [[6, "desc"]],
        "dom": '<"top"f>rt<"bottom"ilp><"clear">'
    });

    //-------------Show Hide Action column------------
    if ($("#LoggedInOrganisationId").val() == "3" || $("#LoggedInOrganisationId").val() == "1") {
    
        DashboardTable.column(6).visible(true);
    }
    else {
        DashboardTable.column(6).visible(false); 
    }
}

function OpenCloseTicketPopup(Id) {
  
    $("#hdnTicketId").val(Id);
    $('#ConfirmCloseTicketPopup').modal('show');

}


function ResetActiveFilter() {
    $("#txtActiveChannel").val('');
    $("#txtWaitingOn").val('');
    $("#txtCreatedActiveTicket").val('');
    $("#SearchActiveTicket").val('');

    BindTicketData();
}

function ResetClosedFilter() {
    $("#txtClosedChannel").val('');
    $("#txtClosedWaitingOn").val('');
    $("#txtSearchCloseDate").val('');
    $("#txtClosedSearch").val('');
    BindClosedTicketData();
}


function AllowAlphaNumeric(event) {

    var txt = String.fromCharCode(event.which);

    if (!txt.match(/^[a-zA-Z0-9\ \b']+$/))//+#-.
    {
        event.preventDefault();
        return false;
    }
}


function SubmitCloseTicketPopup() {

    var TicketId = $("#hdnTicketId").val();
    if (TicketId!= "") {
        $("#loaderContainer").css("display", "block");
        $.ajax({
            url: "CloseTicketById?id=" + TicketId,
            type: 'GET',
            dataType: 'json', // added data type
            success: function (data) {
                $("#loaderContainer").css("display", "none");
                $('#ConfirmCloseTicketPopup').modal("hide");
                $('#divSuccessCloseMessages').modal("show");
                $("#SuccessTextCloseMessage").text("Ticket has been closed successfully");

                if (data == "True") {
                    BindTicketData();
                    BindClosedTicketData();
                }
            }
        });
    }
}


function BindClosedTicketData() {

    var ClosedChannelId = $("#txtClosedChannel").val();
    var ClosedWaitingOnId = $("#txtClosedWaitingOn").val();
    var CreatedClosedTicketOn = $("#txtSearchCloseDate").val();
    var SearchClosedTicket = $("#txtClosedSearch").val();

    DashboardTable = $('#TableClosedTickets').DataTable({
        "processing": true,
        "serverSide": false,
        "JQueryUI": true,
        "filter": true,
        "orderMulti": false,
        "pagingType": "full_numbers",
        "searching": false,
        "paging": true,
        "ordering": true,
        "info": true,
        "destroy": true,
        "bFilter": false,
        "pageLength": 25,
        "stateSave": false,

        "ajax": {
            "url": "GetClosedTicketData",
            "type": "GET",
            "datatype": "json",
            "data": { ChannelId: ClosedChannelId, Organisationid: ClosedWaitingOnId, CreatedOn: CreatedClosedTicketOn, TextSearch: SearchClosedTicket },
        },

        "columns": [

            {
                "data": "id", "name": "id",
                fnCreatedCell: function (nTd, sData, oData, iRow, iCol) {
                    $(nTd).html("<a href='" + WebsitePath + "ticketuser/TicketDetail?id=" + oData.encryptedTicketId + "'>" + oData.id + "</a>");
                }
            },
            { "data": "name", "name": "name", "autoWidth": true },
            { "data": "channel", "name": "channel", "autoWidth": true },
            { "data": "closedBy", "name": "closedBy", "autoWidth": true },
            { "data": "closedOn", "name": "closedOn", "autoWidth": true },
            { "data": "addedBy", "name": "addedBy", "autoWidth": true },
            { "data": "addedOn", "name": "addedOn", "autoWidth": true },
        ],

        "order": [[0, "desc"]],
        "language": {
            "emptyTable": "No data available"
        },
        columnDefs: [{ type: 'date', 'targets': [4] }],
        order: [[4, 'desc']], 
        //"aaSorting": [[3, "desc"]],
        "dom": '<"top"f>rt<"bottom"ilp><"clear">'
    });

}


