
$(document).ready(function () {

    var myDebug = debugJs();
    myDebug.initDebug();

    var myTasks = taskImpl();
    myTasks.initTaskList();

    var myHover = hoverImpl(myTasks);
    myHover.initHover();

    var myFilter = filterImpl(myHover);
    myFilter.initFilter();

    var myNav = navImpl(true);
    myNav.initNav();

    var myToolboxFeatures = toolboxFeatures();
    myToolboxFeatures.initToolboxFeatures();

    var myNavbarImpl = navbarImpl();
    myNavbarImpl.setTaskListActive();

});
