$(document).ready(function () {

    $("#Name").keypress(function (e) {
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
    $("#LastName").keypress(function (e) {
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


    $('#AddUserForm').submit(function () {
        if ($(this).valid()) {
            $("#loaderContainer").css("display", "block");
            return true;
        }
    });

});


