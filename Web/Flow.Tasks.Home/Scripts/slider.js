var slider = function (options) {

    var index = 0;
    var animating = null;
    var target = null;
    var autoSlide = null;
    var pause = false;
    var stop = false;
    var tooltipTarg = null;
    var tooltipCurr = null;
    var tooltipAnimating = false;
    var tooltipTop = 80;
    var tooltipAnimDist = 10;
    var tooltipAnimTime = 150;
    var animTime = 1000;
    var currentHeight = 0;

    var next = function () {

        if (stop) return;

        if (pause) {
            pause = false;
            return;
        }

        var maxH = 0;
        $(".slideImg").each(function () {

            if ($(this).height() > maxH) {
                maxH = $(this).height();
            }
        });
        currentHeight = maxH;

        $(".slides").css("height", currentHeight);
        $(".slide").each(function () {
            $(this).css("margin-bottom", -currentHeight);
        });

        var temp = index;

        temp++;
        if (temp >= options.slides.length) {
            temp = 0;
        }

        change(temp);
    };

    var change = function (to) {

        target = to;

        $(".slideButton.selected").removeClass("selected");
        $($(".slideButton")[target]).addClass("selected");

        slide();
    };

    var slide = function () {

        if (index == target) {
            clearInterval(animating);
            animating = null;
            return;
        }

        // Performe the slide
        $($(".slides > *")[index])
	        .show()
	        .css('z-index', '0')
	        .animate({
	            opacity: 0,
	            top: -currentHeight / 4
	        }, animTime - 50, function () {
	            $(this).hide();
	    });

        $($(".slides > *")[target])
            .show()
            .css('z-index', '1')
            .css('top', currentHeight / 4)
            .css('opacity', '0')
            .animate({
                opacity: 1,
                top: 0
            }, animTime - 50);

        index = target;
    };

    var tooltipIn = function (index) {
        tooltipTarg = index;
        tooltipGo();
    };

    var tooltipOut = function (index) {
        if (tooltipTarg == index) {
            tooltipTarg = null;
            tooltipGo();
        }
    };

    var tooltipGo = function () {

        if (!tooltipAnimating && tooltipCurr != tooltipTarg) {

            if (tooltipCurr == null) {

                $(".sliderTip p").html(options.slides[tooltipTarg].tip);
                var offset1 = $($(".slideButton")[tooltipTarg]).offset();
                var offset2 = $($(".slideButton")[tooltipTarg]).position();

                var offset = {};
                offset.top = offset1.top - (36 + tooltipTop);
                offset.left = offset2.left + (30 - $(".sliderTip").width() / 2);

                $(".sliderTip")
                .css('opacity', '0')
                .css('left', offset.left)
                .css('top', offset.top - tooltipAnimDist)
                .css('display', 'block')
                .animate({
                    opacity: 1,
                    top: offset.top
                }, tooltipAnimTime, function () {
                    tooltipAnimating = false;
                    tooltipGo();
                });
                tooltipAnimating = true;

                tooltipCurr = tooltipTarg;

            } else if (tooltipCurr != tooltipTarg) {

                var offset = $($(".slideButton")[tooltipCurr]).offset();
                offset.top -= (36 + tooltipTop);

                $(".sliderTip")
                .animate({
                    opacity: 0,
                    top: offset.top - tooltipAnimDist
                }, tooltipAnimTime, function () {
                    tooltipAnimating = false;
                    $(this).css('display', 'none');
                    tooltipGo();
                });
                tooltipAnimating = true;

                tooltipCurr = null;

            }
        }
    };

    return {
        init: function () {

            currentHeight = options.height;

            $.each(options.slides, function (index, slide) {

                divSlide = $("<div>")
                    .css("z-index", options.slides.length - index)
                    .css("opacity", 0)
                    .css("margin-bottom", -currentHeight)
                    .addClass("slide");

                var link = $("<a>")
                        .attr('href', slide.link)
                        .css('display', 'block');

                var img = $("<img>")
                    .attr('src', slide.slide)
                    .attr('alt', slide.tip)
                    .addClass("slideImg");

                divSlide.append(link);
                link.append(img);


                $(".slides").append(divSlide);
                $(".slides").css("height", currentHeight);

                var divBtn = $("<div>")
                    .addClass("slideButton")

                $(".slideControls").append(divBtn);
            });

            index = 0;

            $($(".slides > *").hide()[index]).show();

            $($(".slides > *")[index]).css("opacity", 1);

            //stop sliding when mouse hover
            $(".slides").hover(function () {
                stop = true;
            });

            $(".slides").mouseleave(function () {
                stop = false;
            });

            $(".slideButton").hover(function () {
                tooltipIn($(this).index());
            }, function () {
                tooltipOut($(this).index());
            });

            $(".sliderTip").hover(function () {
                tooltipIn(tooltipCurr);
            }, function () {
                tooltipOut(tooltipCurr);
            });

            $(".slideControls > *:first-child").addClass("selected");

            $(".slideControls > *").each(function (index) {
                $(this).click(function () {

                    clearInterval(autoSlide);
                    autoSlide = setInterval(next, options.autoTime);
                    pause = true;

                    change(index);
                });
            });

            autoSlide = setInterval(next, options.autoTime);
        }
    }
}
