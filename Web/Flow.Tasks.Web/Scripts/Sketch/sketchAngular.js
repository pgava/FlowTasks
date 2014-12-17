var showSketchLoading = function(show) {
    if (show) {
        $('body').addClass("loading");
    } else {
        $('body').removeClass("loading");
    }
}