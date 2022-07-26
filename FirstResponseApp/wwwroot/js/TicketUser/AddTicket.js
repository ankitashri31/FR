var WebsitePath;

var IsError = 0;
$(document).ready(function () {

    

    WebsitePath = $("#hdnWebsitePath").val();
    
    //$("#TicketNotes").summernote({
    //    height: 100,                 // set editor height  
    //    minHeight: null,             // set minimum height of editor  
    //    maxHeight: null,             // set maximum height of editor  
    //    focus: true,
    //    toolbar: [
    //        //[groupname, [button list]]

    //        ['style', ['bold', 'italic', 'underline', 'clear']],
    //        ['color', ['color']],
    //        // ['para', ['ul', 'ol', 'paragraph']],
    //        // ['view', ['codeview']],
    //    ]
    //});

    $('#FileUpload').bind('change', function () {
      
        var allowedFiles = ["doc", "docx", "pdf", "jpg", "jpeg", "png", "xls", "xlsx", "mp4", "avi"];
        $("#spnFileTypesFormat").hide();
        $("#errorSize").hide();
        var myFile = $('#FileUpload').prop('files');
        
        if (myFile[0].name != "") {
            var extension = myFile[0].name.substr((myFile[0].name.lastIndexOf('.') + 1));
            var allowedFile = allowedFiles.indexOf(extension.toLowerCase());
            
            if (allowedFile<0) {
                $("#spnFileTypesFormat").show();
                $("#errorSize").hide();
                IsError = 1;
            } else {
                IsError = 0;
                $("#spnFileTypesFormat").hide();
            }
        }
      
        if (myFile[0].name != "") {
            //5242880 3,15,91,292 
            if (myFile[0].size > 27053265) /*32296141*/
            {
                IsError = 1;
                $("#errorSize").show();
                $("#spnFileTypesFormat").hide();
            }
        }
        else {
            IsError = 0;
            $("#errorSize").hide();
        }

    });

    $("#FormAddTicket").on("submit", function (e) {
      
        if (IsError == 1) {
            return false;
        }
        else
        {
            //return true;
            if ($(this).valid()) {
                $("#loaderContainer").css("display", "block");
                return true;
            }
        }
    })

   
});



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

function ClearFileUpload() {

    $("#FileUpload").val('');
    $("#errorSize").hide();
    $("#spnFileTypesFormat").hide();
    IsError = 0;


}

function AllowNumeric(evt) {

    evt = (evt) ? evt : window.event;
    var charCode = (evt.which) ? evt.which : evt.keyCode;
    if (charCode > 31 && (charCode < 48 || charCode > 57)) {
        return false;
    }
    return true;
}

function ShowLoader() {
    $("#loaderContainer").css("display", "none");
}