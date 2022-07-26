var varSummerText = "";

$(document).ready(function () {
    //$(".summerNote").summernote({
    //    height: 100,                 // set editor height  
    //    minHeight: null,             // set minimum height of editor  
    //    maxHeight: null,             // set maximum height of editor  
    //    focus: true,
    //    toolbar: [
    //        [groupname, [button list]]

    //        ['style', ['bold', 'italic', 'underline', 'clear']],
    //        ['color', ['color']],
    //         ['para', ['ul', 'ol', 'paragraph']],
    //         ['view', ['codeview']],
    //    ]
    //});

    $(".summerNote").summernote({
        toolbar: [
            ['style', ['bold', 'italic', 'underline', 'clear']]
        ],
        placeholder: 'Leave a comment ...',
        callbacks: {
            onKeydown: function (e) {
                var t = e.currentTarget.innerText;
                if (t.trim().length >= 1000) {
                    //delete keys, arrow keys, copy, cut, select all
                    if (e.keyCode != 8 && !(e.keyCode >= 37 && e.keyCode <= 40) && e.keyCode != 46 && !(e.keyCode == 88 && e.ctrlKey) && !(e.keyCode == 67 && e.ctrlKey) && !(e.keyCode == 65 && e.ctrlKey))
                        e.preventDefault();
                }
            },
            onKeyup: function (e) {
                var t = e.currentTarget.innerText;
                $('#maxContentPost').text(1000 - t.trim().length);
            },
            //onPaste: function (e) {
            //    var t = e.currentTarget.innerText;
            //    var bufferText = ((e.originalEvent || e).clipboardData || window.clipboardData).getData('Text');
            //    e.preventDefault();
            //    var maxPaste = bufferText.length;
            //    if (t.length + bufferText.length > 1000) {
            //        maxPaste = 1000 - t.length;
            //    }
            //    if (maxPaste > 0) {
            //        document.execCommand('insertText', false, bufferText.substring(0, maxPaste));
            //    }
            //    $('#maxContentPost').text(1000 - t.length);
            //}
           
           
        }
    });

    $(".summerNote").on("summernote.paste", function (e, ne) {
            var t = e.currentTarget.innerText;
                var bufferText = ((e.originalEvent || e).clipboardData || window.clipboardData).getData('Text');
                e.preventDefault();
                var maxPaste = bufferText.length;
                if (t.length + bufferText.length > 1000) {
                    maxPaste = 1000 - t.length;
                }
                if (maxPaste > 0) {
                    document.execCommand('insertText', false, bufferText.substring(0, maxPaste));
                }
                $('#maxContentPost').text(1000 - t.length);
    });

    //----------------For Ticket Detail page-----We se value into summernot 
    //debugger;
    //var t1= $("#txtLinFoxNotes").val();
    //var t2 =$("#txtMedilawNotes").val();
    //var t3 = $("#txtHWLENotes").val();

    //var te1 = $("#hdnHwle").val();
    //var te2 = $("#hdnMedilow").val();
    //var te3 = $("#hdnLinfox").val();


    //$("#txtLinFoxNotes").val('');
    //$("#txtMedilawNotes").val('');
    //$("#txtHWLENotes").val('');

    if ($("#hdnLoggedInOrganisationId").val() == "1") {
        varSummerText = $("#txtHWLENotes").val();
        $(".summerNote").summernote('code', varSummerText);
    }
    else if ($("#hdnLoggedInOrganisationId").val() == "2") {
        varSummerText = $("#txtMedilawNotes").val();
        $(".summerNote").summernote('code', varSummerText);
    }
    else if ($("#hdnLoggedInOrganisationId").val() == "3") {
        varSummerText = $("#txtLinFoxNotes").val();
        $(".summerNote").summernote('code', varSummerText);
    }

    

});