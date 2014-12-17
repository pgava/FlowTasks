var debugJs = function () {
return {
        initDebug: function () {
            if (typeof console === "undefined" || typeof console.log === "undefined") {
                console = {};
                console.log = function () { };
            }
        }
    }
}