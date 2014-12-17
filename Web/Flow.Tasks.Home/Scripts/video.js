function playVideo(videoUrl) {
    $(".lightBoxBack").fadeIn();
    $(".lightBoxContent").fadeIn();
    $(".lightBoxContent iframe").attr('src', videoUrl);
}

function setImage(id) {
    $(".lightBoxContent .feature").attr("src", documentURL + "/" + images[id]).attr("id", id);
    $(".lightBoxContent .title .text").html(titles[id]);
}

var video = function (options) {

    return {
        init: function () {
            $(".lightBoxBack").hide();
            $(".lightBoxContent").hide();

            $(".lightBoxContent .x").click(function () {
                $(".lightBoxBack").fadeOut();
                $(".lightBoxContent").fadeOut();
                $(".lightBoxContent .feature").attr("src", "");
            });

            $(".lightBoxContent .feature").click(function () {
                window.open($(this).attr("src"), "_blank");
            });

            $(".lightBoxContent .arrow").click(function () {
                var id = $(".lightBoxContent .feature").attr("id");
                if ($(this).attr("id") == "left") {
                    --id;
                    if (id < 0) {
                        id = images.length - 1;
                    }
                } else if ($(this).attr("id") == "right") {
                    ++id;
                    if (id >= images.length) {
                        id = 0;
                    }
                }
                setImage(id);
            });
        }

    };
}