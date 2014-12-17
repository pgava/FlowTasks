$(document).ready(function () {
    var myNavbarImpl = navbarImpl();
    myNavbarImpl.setSketchActive();

    $("body").on({
        // When ajaxStart is fired, add 'loading' to body class
        ajaxStart: function () {
            $(this).addClass("loading");
        },
        // When ajaxStop is fired, rmeove 'loading' from body class
        ajaxStop: function () {
            $(this).removeClass("loading");
        }
    });

});