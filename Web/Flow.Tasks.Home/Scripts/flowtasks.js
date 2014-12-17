

$(document).ready(function () {

    var imgDir = $("#ImgDir").val();

    var sld = slider({
        width: 950,
        height: 288,
        autoTime: 5000,
        slides: [
            {
                slide: imgDir + "Slide1a.jpg",
                link: "javascript:playVideo('http://www.youtube.com/embed/02LBhSjiVAQ?rel=0&amp;autoplay=1');",
                tip: "Sketch"
            },
            {
                slide: imgDir + "Slide2.jpg",
                link: "javascript:playVideo('http://www.youtube.com/embed/4SkGzyzp5Po?rel=0&amp;autoplay=1');",
                tip: "Task List"
            },
            {
                slide: imgDir + "Slide3a.jpg",
                link: "javascript:playVideo('http://www.youtube.com/embed/4D9SgGxKwwo?rel=0&amp;autoplay=1');",
                tip: "Reports"
            },
            {
                slide: imgDir + "Slide4.jpg",
                link: "javascript:playVideo('http://www.youtube.com/embed/7U605qz-jIo?rel=0&amp;autoplay=1');",
                tip: "Topics"
            }
        ]
    });
    sld.init();

    $(".sliderTip").hide();

    var vid = video();
    vid.init();
});
