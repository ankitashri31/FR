var WebsitePath;
var TicketDetailTable;
var IsError = 0;
$(document).ready(function () {
 WebsitePath = $("#hdnWebsitePath").val();
 BindDocumentList();
 $("#Dashboard").addClass('active');


    $('#FileUpload').bind('change', function () {

        var allowedFiles = ["doc", "docx", "pdf", "jpg", "jpeg", "png", "xls", "xlsx", "mp4", "avi"];
        $("#spnFileTypesFormat").hide();
        $("#errorSize").hide();
        var myFile = $('#FileUpload').prop('files');

        if (myFile[0].name != "") {
            var extension = myFile[0].name.substr((myFile[0].name.lastIndexOf('.') + 1));
            var allowedFile = allowedFiles.indexOf(extension.toLowerCase());

            if (allowedFile < 0) {
                $("#spnFileTypesFormat").show();
                $("#errorSize").hide();
                IsError = 1;
            } else {
                IsError = 0;
                $("#spnFileTypesFormat").hide();
            }
        }

        if (myFile[0].name != "") {
            //5242880
            if (myFile[0].size > 27053265) /*32296141*/
            {
                IsError = 1;
                $("#errorSize").show();
            }
        }
        else {
            IsError = 0;
            $("#errorSize").hide();
        }

    });

    $("#FormUpdateTicket").on("submit", function (e) {
      
        if (IsError == 1) {
            return false;
        }
        else {
           // return true;
            if ($(this).valid()) {
                $("#loaderContainer").css("display", "block");
                return true;
            }
        }

    })

    //-----------------SHow - Hide-------------------
    $('#btnShowAllDocuments').on('click', function () {
        //$(this).toggleText('Show all documents', 'Hide all documents').next().toggle();
        $("#spnDocumentText").toggleText('Show all documents', 'Hide all documents').next().toggle();
        $("#docIcon").toggleClass("icon-eye-blocked2"); 
    });

})

jQuery.fn.extend({
    toggleText: function (stateOne, stateTwo) {
        return this.each(function () {
            stateTwo = stateTwo || '';
            console.log(stateOne, stateTwo, $(this).text());
            $(this).text() !== stateTwo && stateOne
                ? $(this).text(stateTwo)
                : $(this).text(stateOne);
        });
    }
});

function ClearFileUpload() {

    $("#FileUpload").val('');
    $("#errorSize").hide();
    $("#spnFileTypesFormat").hide();
    IsError = 0;
}

function BindDocumentList() {

    var hdnEncryptedTicketId = $("#hdnEncryptedTicketTId").val();
    DashboardTable = $('#TableDocuments').DataTable({
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
            "url": "GetDocumentsByTicketId?id=" + hdnEncryptedTicketId,
            "type": "GET",
            "datatype": "json",
        },

        "columns": [
            {
                "data": "documentName", "name": "documentName", "autoWidth": true,
                fnCreatedCell: function (nTd, sData, oData, iRow, iCol) {
                 
                    var FileName = $.trim(oData.documentName)
                    if (FileName !=null)
                        $(nTd).html("<a href='" + WebsitePath + "Ticket/TicketUser/DonwloadDocumentById?id=" + oData.id + "&&type=" + oData.tableType+"'>" + FileName + "</a>");
                }
            },
            { "data": "addedBy", "name": "addedBy", "autoWidth": true },
            { "data": "organisation", "name": "organisation", "autoWidth": true },
            { "data": "addedOn", "name": "addedOn", "autoWidth": true },
            {

                "render": function (data, type, full, meta) {
                    if (full.isCreatedByLoggedInuser == 'true') {

                        return '<a href="#" title="Delete Document" onclick="OpenConfirmPopup(' + full.id + ', \'' + full.tableType + '\');" class="list-icons-item item-delete" data-toggle="dropdown" aria-expanded="false"><i class="fas fa-trash-alt"></i></a>';

                    }
                    else {
                        return "NA";
                    }
                },
            }
        ],

        "order": [[0, "desc"]],
        "language": {
            "emptyTable": "No data available"
        },
    });

}

function OpenConfirmPopup(Id, TableType) {
    $("#hdnDocumentId").val(Id);
    $("#hdnTableType").val(TableType);
    $('#ConfirmDeletePopup').modal('show');
}


function SubmitDeleteDocument() {

    var TicketId = $("#hdnDocumentId").val();
    var TableType = $("#hdnTableType").val();

    if (TicketId != "") {
        $("#loaderContainer").css("display", "block");
        $.ajax({
            url: "DeleteDocumentById?id=" + TicketId + "&&TableType=" + TableType,
            type: 'GET',
            dataType: 'json', // added data type
            success: function (data) {
                $("#loaderContainer").css("display", "none");
                $('#ConfirmDeletePopup').modal("hide");
                $('#divSuccessCloseMessages').modal("show");
                $("#SuccessTextCloseMessage").text("Document has been deleted successfully.");

                if (data == "True") {
                    BindDocumentList();
                }
            }
        });
    }
}




function ShowLoader() {
    $("#loaderContainer").css("display", "none");
}

function RedirectToDashbaord() {

    window.location.href = WebsitePath + "TicketUser/Dashboard";
}
function AllowAlphaNumeric(event) {

    var txt = String.fromCharCode(event.which);

    if (!txt.match(/^[a-zA-Z0-9\\b]+$/))//+#-.
    {
        event.preventDefault();
        return false;
    }
}

function AllowNumeric(evt) {

    evt = (evt) ? evt : window.event;
    var charCode = (evt.which) ? evt.which : evt.keyCode;
    if (charCode > 31 && (charCode < 48 || charCode > 57)) {
        return false;
    }
    return true;
}