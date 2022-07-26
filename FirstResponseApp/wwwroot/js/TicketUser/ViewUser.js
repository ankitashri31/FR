
var WebsitePath;
$(document).ready(function () {

    WebsitePath = $("#hdnWebsitePath").val();

    $('#btnConfirmDeactive').click(function (e) {
        $("#loaderContainer").css("display", "block");
        $("#frmDeactive").submit();
    })

    $('#btnConfirmResetPassword').click(function (e) {
        $("#loaderContainer").css("display", "block");
        $("#frmResetPassword").submit();
    })

    $("#UserManagement").addClass('active');

});

function OpenConfirmActivePopup(id) {
    $("#ActiveHeader").text("Confirm Deactivate");
    $("#ActiveBody").text("Are you sure you want to deactivate user?");
    $('#DeativePopup').modal('show');
}

function OpenConfirmDeactivePopup(id) {
    $("#ActiveHeader").text("Confirm Activate");
    $("#ActiveBody").text("Are you sure you want to activate user?");
    $('#DeativePopup').modal('show');
}

function OpenResetPassword(id) {
    $('#ResetPasswordPopup').modal('show');

}
function RedirectToDashbaord() {
    window.location.href = WebsitePath + "TicketUser/AllUser";
}