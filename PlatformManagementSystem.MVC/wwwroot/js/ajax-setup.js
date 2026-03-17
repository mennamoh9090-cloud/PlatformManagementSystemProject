$(document).ready(function () {

    $(document).ajaxStart(function () {
        $("#globalLoader").addClass("active");
    });

    $(document).ajaxStop(function () {
        $("#globalLoader").removeClass("active");
    });

    $(document).ajaxError(function () {
        $("#globalLoader").removeClass("active");
        alert("Something went wrong 😥");
    });
    $(document).ajaxSuccess(function (event, xhr) {
        showToast("Operation completed successfully", "success");
    });

    $(document).ajaxError(function (event, xhr) {
        showToast("Something went wrong", "error");
    });

});
