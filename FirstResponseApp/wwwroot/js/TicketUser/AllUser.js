var WebsitePath;
var DashboardTable;
$(document).ready(function () {
    WebsitePath = $("#hdnWebsitePath").val();
    $('#btnConfirmDeactive').click(function (e) {
        $("#frmDeactive").submit();
    })

    $("#UserManagement").addClass('active');

    $("#SearchDate").datepicker({
        changeMonth: true,
        changeYear: true,
        dateFormat: "dd/mm/yy",
        maxDate: new Date()
    });
    //$("#SearchDate").datepicker().datepicker("setDate", new Date());
    

    $("#Search").keypress(function (e) {
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
    });

    BindUserData();
});

function ResetFilter() {

    $("#IsActive").val('');
    $("#OrganisationId").val('');
    $("#SearchDate").val('');
    $("#Search").val('');

    BindUserData();
}

function OpenConfirmDeactivePopup(Id) {
    $('#hdnUserId').val(Id);
    $('#ConfirmDeActivePopup').modal("show");
}
function OpenConfirmActivePopup(Id) {
    $('#hdnUserId').val(Id);
    $('#ConfirmActivePopup').modal("show");
}

function OpenConfirmResetPopup(Id) {
    $('#hdnUserId').val(Id);
    $('#ConfirmRestPasswordPopup').modal("show");
}

function SubmitResetPopup() {
    $('#ConfirmRestPasswordPopup').modal("hide");
    var UserId = $("#hdnUserId").val();
    var Url = "AllUser";
    if (UserId != "") {
        $("#loaderContainer").css("display", "block");
        $.ajax({
            url: "ResetPassword",
            data: { 'id': UserId, 'url': Url },
            type: 'POST',
            dataType: 'json', // added data type
            success: function (data) {
                $("#loaderContainer").css("display", "none");
                $("#SuccessTextMessage").text("system generated password has been sent on registered email address");
                $('#divSuccessMessages').modal("show");
                $('#hdnUserId').val('');
                    BindUserData();
            }
        });
    }
}

function SubmitActivatePopup() {
    $('#ConfirmActivePopup').modal("hide");
    var UserId = $("#hdnUserId").val();
    var Url = "AllUser";
    if (UserId != "") {
        $("#loaderContainer").css("display", "block");
       
        $.ajax({
            url: "DeactiveUser",
            data: { 'id': UserId, 'url': Url },
            type: 'POST',
            dataType: 'json', // added data type
            success: function (data) {
                $("#loaderContainer").css("display", "none");
                $("#SuccessTextMessage").text("User Has been activated successfully.");
                $('#divSuccessMessages').modal("show");
                $('#hdnUserId').val('');
                    BindUserData();
            }
        });
    }
}
function SubmitDeactivatePopup() {
    $('#ConfirmDeActivePopup').modal("hide");
    var UserId = $("#hdnUserId").val();
    var Url = "AllUser";
    if (UserId != "") {
        $("#loaderContainer").css("display", "block");
        $.ajax({
            url: "DeactiveUser",
            data: { 'id': UserId, 'url': Url },
            type: 'POST',
            dataType: 'json', // added data type
            success: function (data) {
                $("#loaderContainer").css("display", "none");
                $("#SuccessTextMessage").text("User Has been deactivated successfully.");
                $('#divSuccessMessages').modal("show");
                $('#hdnUserId').val('');
                    BindUserData();
            }
        });
    }
}


function BindUserData() {
    var IsActive = $("#IsActive").val();
    var OrganisationId = $("#OrganisationId").val();
    var CreatedOn = $("#SearchDate").val();
    var Search =  $("#Search").val();

   

    DashboardTable = $('#TblUserRecord').DataTable({
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
            "url": "GetUserData",
            "type": "POST",
            "datatype": "json",
            "data": { IsActive: IsActive, Organisationid: OrganisationId, CreatedOn: CreatedOn, TextSearch: Search },
        },

            "columns": [
                {
                    "data": "name", "name": "name",
                    fnCreatedCell: function (nTd, sData, oData, iRow, iCol) {
                        $(nTd).html("<a href='" + WebsitePath +"TicketUser/ViewUser?id=" + oData.id + "'>" + oData.name + "</a>");
                    }
                },
            { "data": "emailAddress", "name": "emailAddress", "autoWidth": true },
            { "data": "emailType", "name": "emailType", "autoWidth": true },
            { "data": "organisation", "name": "organisation", "autoWidth": true },
                { "data": "status", "name": "status", "autoWidth": true },
                { "data": "deactvatedOn", "name": "deactvatedOn", "autoWidth": true },
            { "data": "addedOn", "name": "addedOn", "autoWidth": true },
            {
                "render": function (data, type, full, meta) {
                    
                    var IsActive = $.trim(full.isActive);
                    var UserId = $.trim(full.id);
                    var UserEmailId = $.trim(full.emailAddress);
                    if (IsActive == 'false')
                        return '<div class="list-icons"><div class="dropdown user-dropdown listing-arrow-none"><a href="#" class="list-icons-item" data-toggle="dropdown" aria-expanded="false"><i class="icon-menu9"></i></a><div class="dropdown-menu dropdown-menu-right" x-placement="bottom-end" style="position: absolute; transform: translate3d(-158px, 19px, 0px); top: 0px; left: 0px; will-change: transform;"></a><a href="' + WebsitePath + 'TicketUser/EditUser?id=' + full.id + '" class="dropdown-item show"><i class="fas fa-edit"></i> Edit</a><a onclick="OpenConfirmResetPopup(\'' + UserId + '\')" class="dropdown-item show"><i class="fas fa-key"></i> Change password</a><a  onclick="OpenConfirmActivePopup(\'' + UserId + '\')"  class="dropdown-item show"><i class="fas fa-toggle-on"></i> Activate</a>';
                    else
                        return '<div class="list-icons"><div class="dropdown user-dropdown listing-arrow-none"><a href="#" class="list-icons-item" data-toggle="dropdown" aria-expanded="false"><i class="icon-menu9"></i></a><div class="dropdown-menu dropdown-menu-right" x-placement="bottom-end" style="position: absolute; transform: translate3d(-158px, 19px, 0px); top: 0px; left: 0px; will-change: transform;"></a><a href="' + WebsitePath + 'TicketUser/EditUser?id=' + full.id + '" class="dropdown-item show"><i class="fas fa-edit"></i> Edit</a><a onclick="OpenConfirmResetPopup(\'' + UserId + '\')" class="dropdown-item show"><i class="fas fa-key"></i> Change password</a><a  onclick="OpenConfirmDeactivePopup(\'' + UserId + '\')"  class="dropdown-item show"><i class="fas fa-toggle-off"></i> Deactivate</a>';
                        
                }
            },
        ],
        "language": {
            "emptyTable": "No data available"
        },
        columnDefs: [{ type: 'date', 'targets': [7] }],
        order: [[7, 'desc']], 
       // "aaSorting": [[7, "desc"]],
        "dom": '<"top"f>rt<"bottom"ilp><"clear">'
    });
   
   
}

