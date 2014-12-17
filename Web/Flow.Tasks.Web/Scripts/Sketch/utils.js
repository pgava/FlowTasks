/* --- global variables --- */
var currentViewPort = {
    width: 0,
    height: 0
};

// default values of program
var defaultValue = {
    entity: {// entity
        width: 70,
        height: 87,
        borderRadius: 5
    },
    entityStart: { // entity start
        width: 60,
        height: 76.5
    },
    flowchart: { // flowchart (task)
        width: 284,
        height: 52.8,
        borderRadius: 5
    },
    begin: { // point begin
        x: 0,
        y: 0
    },
    viewport: { // main view
        width: 614,
        height: 636,
        offsetLeft: 0,
        offsetTop: 0
    },
    toolbar: { // tool bar
        width: 300,
        height: 500
    },
    margin: 5, // margin
    marginLeft: 10, // margin left
    marginMeasure: 10, // margin mesure
    iconSize: { // icon size
        width: 50,
        height: 50
    },
    iconSizeFlow: {
        width: 15,
        height: 15
    },
    holder: { // holder size
        width: 10,
        height: 5
    }
};

var isHolder = false; // when drag-drop on holder for create connection
var isHolderChange = false; // when drag-drop breakpoint for change connection
var isCreate = false; // when drap-drop create a new entity

// object draw
var viewport = null, // object canvas draw viewport of flowchart
    toolbar = null, // object toolbar
    condition = null, // condition - edit for decision
    editor = null, // editor - edit entity/connection label
    expression = null,
    description = null,// description - descript entity/connection
    annotation = null;
var isEdit = false; // flag edit

var isModified = false;

// object
var mViewport = {
    width: 0,
    height: 0
},
    mSelectedItems = {
        items: new Collection(),
        isMultiSelect: false
    };

// selection area
var isMultiSelect = false;
var selectedArea = {
    rect: undefined,
    areaNormal: { fill: 'none', stroke: 'none' },
    areaSelect: { fill: '#9CFFFD', 'fill-opacity': 0.1, stroke: '#389CFF' }
};

// object to create bigger flowchart
var isPos = false;

// check is browsers
var isIE = false;
var isFlowchart = false;

// calculate width/height of viewport(left) & toolbar(right)
var calculateDrawArea = function () {
    var vp;
    // viewport
    vp = {
        width: defaultValue.viewport.width,
        height: defaultValue.viewport.height
    };

    defaultValue.viewport.width = vp.width;
    defaultValue.viewport.height = vp.height;

    mViewport = {
        width: vp.width,
        height: vp.height
    };

    defaultValue.begin = {
        x: (mViewport.width - defaultValue.entity.width) / 2,
        y: 20
    };
};

// resize window
$(window).resize(function () {
    $('#chart').css('width', '77%');
    $('#chart').jScrollPane();
});

// key down window
$(window).keydown(function (e) {
    switch (e.keyCode) {
        case 46: // delete
            if (!isEdit) {
                deleteSelectedItems();
            }
            break;
        case 13: // enter
            break;
        case 17: // ctrl
            mSelectedItems.isMultiSelect = true;
            break;
        case 27: //escape
            if (isEdit) {
                isEdit = false;
            }
            break;
        default:
            break;
    }
});

// key up window
$(window).keyup(function (e) {
    switch (e.keyCode) {
        case 17: // ctrl
            mSelectedItems.isMultiSelect = false;
            break;
    }
});

// mouse up window
$(window).mouseup(function (e) {
    e = e || window.event;
    if (e.button === 0) {
        if (isMultiSelect) {
            selectedArea.rect.remove();
            selectedArea.rect = undefined;
            isMultiSelect = false;
        }
    }
});

var mDummy;
var mMouse = {
    start: { x: 0, y: 0 },
    position: { x: 0, y: 0 },
    pos: { x: 0, y: 0 }
};

// fn initalize - begin when load
function initialize() {
    // event onmousedown
    viewport.canvas.onmousedown = function (e) {
        // event
        e = e || window.event;

        // mouse down viewport
        if (currentItem === undefined) {
            deSelectedItems();

            if ($.browser.msie) { // ie
                if (parseInt($.browser.version, 10) >= 9) {
                    mMouse.start = {
                        x: e.layerX - $(window).scrollLeft(),
                        y: e.layerY - $(window).scrollTop()
                    };
                } else { // ver 6/7/8
                    mMouse.start = {
                        x: e.offsetX,
                        y: e.offsetY
                    };
                }
            } else { // chrome, firefox
                mMouse.start = {
                    x: e.layerX,
                    y: e.layerY
                };
            }

            mDummy.attr({
                x: mMouse.start.x - 2,
                y: mMouse.start.y - 2
            });
            mDummy.move = true;

            if (isEdit) {
                isEdit = false;
            }

            mMouse.start = mMouse.position;
            isMultiSelect = true;
        }
    };

    // event onmousemove
    viewport.canvas.onmousemove = function (e) {
        // event
        e = e || window.event;
        // dumny move
        if (mDummy.move) {
            if ($.browser.msie) {
                mDummy.attr({
                    x: e.offsetX - 2,
                    y: e.offsetY - 2
                });
            } else {
                mDummy.attr({
                    x: e.layerX - 2,
                    y: e.layerY - 2
                });
            }
        }
        if ($.browser.msie) {
            if (parseInt($.browser.version, 10) >= 9) { // ie version >= 9
                mMouse.position = {
                    x: e.layerX - $(window).scrollLeft(),
                    y: e.layerY - $(window).scrollTop()
                };
            } else {
                mMouse.position = {
                    x: e.offsetX,
                    y: e.offsetY
                };
            }
        } else {
            mMouse.position = {
                x: e.layerX,
                y: e.layerY
            };
        }

        if (currentItem === undefined) {
            var x = mMouse.start.x > mMouse.position.x ? mMouse.position.x : mMouse.start.x,
                y = mMouse.start.y > mMouse.position.y ? mMouse.position.y : mMouse.start.y,
                w = Math.abs(mMouse.start.x - mMouse.position.x),
                h = Math.abs(mMouse.start.y - mMouse.position.y);

            if (isMultiSelect) {

                // draw multi select element area
                if (selectedArea.rect === undefined) {
                    selectedArea.rect = viewport.rect(x, y, w, h).attr(selectedArea.areaSelect);
                } else {
                    selectedArea.rect.attr({ x: x, y: y, width: w, height: h }).attr(selectedArea.areaSelect);
                }

                // select entities
                mRoot.objEntities.forEach(function (element) {
                    if (selectedArea.rect.contain(element)) { // check into
                        element.selectChange(true); // change selected
                        if (!checkSelectedItems(element)) { // check it existing
                            mSelectedItems.items.add(element, false);
                        }
                    } else {
                        //element.selectChange(false);
                        deSelectedItem(element);
                    }
                });
            }
        }
        if (isHolder || isHolderChange) {
            invalidateConnections();
        }
    };

    // event onmouseup

    var countPosition = function (e) {
        // event
        e = e || window.event;

        if (isCreate) {
            isPos = true;
        }
        if ($.browser.msie) {
            if (parseInt($.browser.version, 10) >= 9) { // ie version >= 9
                mMouse.pos = {
                    x: e.layerX - $(window).scrollLeft(),
                    y: e.layerY - $(window).scrollTop()
                };
            } else {
                mMouse.pos = {
                    x: e.offsetX,
                    y: e.offsetY
                };
            }
        } else {
            mMouse.pos = {
                x: e.layerX,
                y: e.layerY
            };
        }

        if (mDummy.move) {
            mDummy.move = false;
            deleteConnectionDummy();
        }

        isMultiSelect = false;
        if (selectedArea.rect !== undefined) {
            selectedArea.rect.remove();
            selectedArea.rect = undefined;
        }
    }

    // ------ Modified By Sang Dao ---------
    // Hand Drop object in mobile
    if ('ontouchstart' in document.documentElement) {
        $(document).on("DRAG:END", function (e, p) {
            countPosition(p);
        });
    } else {
        viewport.canvas.onmouseup = function (e) {
            countPosition(e);
        };
    }
    // ------ End Modification -----------------

    mDummy = viewport.rect(0, 0, 2, 2).attr({ fill: 'red', stroke: 'none', "fill-opacity": 0.01 });
    mDummy.id = -1000;
}

// fn create entity
function createEntity(panel, entity) {

    var obj = null;
    if (entity.label !== 'Generic' && entity.label !== 'Approve') {
        obj = drawEntity(panel, entity);
    } else {
        obj = drawEntityFlow(panel, entity);
    }

    // regist
    mRoot.infoEntities.add(entity);
    mRoot.objEntities.add(obj);

    // measure position
    measureEntities(obj);
    increaseViewport(obj);

    // create connection
    if (isCreate && holderCreate !== undefined) {
        // check item for create connection
        if ((currentItem.type === typeEntity.t_start && !isStartOnlyConnection()) || currentItem.type !== typeEntity.t_start) {
            mCurrentConnection = new Connection();
            mCurrentConnection.from = currentItem.id;
            mCurrentConnection.fromHolder = currentItem.holder;
            mCurrentConnection.to = entity.id;
            mCurrentConnection.toHolder = 1; // default is top

            var conn = createConnection(mCurrentConnection);

            // draw label connection
            // can't draw label connnection begin/end is start
            if (mRoot.infoEntities.itemByID(mCurrentConnection.from).type !== typeEntity.t_start &&
                mRoot.infoEntities.itemByID(mCurrentConnection.to).type !== typeEntity.t_start) {
                // it's decision => set connection is true or false
                if (mRoot.infoEntities.item(mCurrentConnection.from).type === typeEntity.t_decision) {
                    if (mCurrentConnection.fromHolder == 2) {
                        mCurrentConnection.label = 'False';
                    } else {
                        mCurrentConnection.label = 'True';
                    }
                }

                // connection from generic/approve, it not labels
                if (mRoot.infoEntities.itemByID(mCurrentConnection.from).type !== typeEntity.t_generic &&
                    mRoot.infoEntities.itemByID(mCurrentConnection.from).type !== typeEntity.t_approve) {
                    // draw temp a label for get width/height.
                    var temp = viewport.text(-100, -100, mCurrentConnection.label).attr({ "font-size": "10px" });
                    conn.label = drawLabel(conn.posLabel.x, conn.posLabel.y, temp.getBBox().width, temp.getBBox().height, mCurrentConnection.label, viewport);
                    conn.label.mask.parent = conn;
                    temp.remove();
                }
            }

            // regist
            mRoot.infoConnections.add(mCurrentConnection);
            mRoot.objConnections.add(conn);
        }
        currentItem = undefined;// reset curentItem
    }
    checkModified(isModified = true); // modified

    isCreate = isPos = false;
}

// action create a new entity
var holderCreate;
function actionToolbars(e, label) {

    var mouse = mMouse.pos,
        // when create on a nother entities
        flag = false;

    for (var i = 0; i < mRoot.objEntities.count; i++) {

        var item = mRoot.objEntities.collection[i];

        var limit = {
            min: {
                x: item.entity.attr("x"),
                y: item.entity.attr("y")
            },
            max: {
                x: item.entity.attr("x") + item.entity.attr("width"),
                y: item.entity.attr("y") + item.entity.attr("height")
            }
        }
        if (limit.min.x < mouse.x && mouse.x < limit.max.x &&
            limit.min.y < mouse.y && mouse.y < limit.max.y) {
            flag = true;
        }
    }

    if (flag) {
        return;
    }

    if (!isPos && isCreate) {
        return;
    }

    var entity = new Entity(); // new object
    entity.x = mouse.x;
    entity.y = mouse.y;
    entity.annotation = ""; // add description

    switch (label) {
        case "Decision": {
            entity.type = typeEntity.t_decision;
            entity.label = label;
            entity.condition = "";

            createEntity(viewport, entity);
            break;
        }
        case "Switch": {
            var html = "<div style='font-size: 16px'>FlowSwitch</div>"
                    + "<div style=margin-top: 30px'>"
                        + "<select id='type-switch' style='width: 100%'>"
                            + "<option value='Int32'>Int32</option>"
                            + "<option value='String'>String</option>"
                        + "</select>"
                    + "</div>"

            $("#confirm-popup-modal").html(html).dialog({
                title: "Select Types",
                buttons: [
                    {
                        text: "OK", click: function () {
                            entity.type = typeEntity.t_switch;
                            entity.label = label;
                            entity.expression = "";
                            entity.typeSwitch = $("#type-switch").val();
                            createEntity(viewport, entity);
                            $(this).dialog("close");
                        },
                    },
                    {
                        text: "Cancel", click: function () {
                            $(this).dialog("close");
                        }
                    }
                ]
            }).dialog("open");
            break;
        }
        case "Generic": {
            entity.width = defaultValue.flowchart.width;
            entity.height = defaultValue.flowchart.height;
            entity.type = typeEntity.t_generic;
            entity.label = label;
            createEntity(viewport, entity);
            break;
        }
        case "Approve": {
            entity.width = defaultValue.flowchart.width;
            entity.height = defaultValue.flowchart.height;
            entity.type = typeEntity.t_approve;
            entity.label = label;
            entity.annotation = "";
            createEntity(viewport, entity);
            break;
        }
    }
}

// not create new entity when drag on entities is existing
function setPositionDrag(obj) {
    var mouse = mMouse.position;
    var flag = false;
    for (var i = 0; i < mRoot.objEntities.count; i++) {
        var item = mRoot.objEntities.collection[i];
        var limit = {
            min: {
                x: item.entity.attr("x"),
                y: item.entity.attr("y")
            },
            max: {
                x: item.entity.attr("x") + item.entity.attr("width"),
                y: item.entity.attr("y") + item.entity.attr("height")
            }
        }
        if (limit.min.x < mouse.x && mouse.x < limit.max.x &&
            limit.min.y < mouse.y && mouse.y < limit.max.y) {
            flag = true;
            //$("#viewport").css("cursor", "not-allowed");
        }
    }
    if (flag) {
        $("#viewport").css("cursor", "not-allowed");
    } else {
        $("#viewport").css("cursor", "auto");
    }
}


// fn handle toolbar (drag-drop item to create entities)
function handleToolBar() {
    // decision
    $('#decision').draggable({
        cursor: "pointer",
        cursorAt: { top: -15, left: -15 },
        helper: function (event) {
            return $("<div class='ui-widget-header entity-shadow'></div>");
        },
        start: function (event) {
            isCreate = true;

        },
        stop: function (event) {
            // ------ Modified By Sang Dao ---------
            var pos = { layerX: event.clientX - jQuery("#viewport").offset().left, layerY: event.clientY - jQuery("#viewport").offset().top };
            $(document).trigger("DRAG:END", pos);
            // ------ End Modification -----------------
            actionToolbars(event, 'Decision');
            isCreate = false;
        },
        drag: function (event) {
            setPositionDrag();
        }
    });

    // switch
    $('#switch').draggable({
        cursor: "pointer",
        cursorAt: { top: -10, left: -10 },
        cursorAt: { top: -15, left: -15 },
        helper: function (event) {
            return $("<div class='ui-widget-header entity-shadow'></div>");
        },
        start: function (event) {
            isCreate = true;
        },
        stop: function (event) {
            // ------ Modified By Sang Dao ---------
            var pos = { layerX: event.clientX - jQuery("#viewport").offset().left, layerY: event.clientY - jQuery("#viewport").offset().top };
            $(document).trigger("DRAG:END", pos);
            // ------ End Modification -----------------
            actionToolbars(event, 'Switch');
            isCreate = false;
        },
        drag: function (event) {
            setPositionDrag();
        }
    });
}

// handle flowtask action
function handleFlowtask() {
    $.ajax({
        type: 'POST',
        url: 'Sketch/GetFlowTasks',
        success: function (data) {
            $('.flowtask').html('');
            for (var i = 0; i < data.length; i++) {
                var li = '<li id="' + data[i].toLowerCase() + '">';
                if (data[i].toLowerCase() === "generic") {
                    li += '<img width="15px" src="Images/generic-icon.png" />';
                } else {
                    li += '<img width="15px" src="Images/approve-icon.png" />';
                }
                li += '<span>' + data[i] + '</span></li>';
                $('.flowtask').append(li);
            }
            $('.flowtask').show();
            // generic
            $('#generic').draggable({
                cursor: "pointer",
                cursorAt: { top: -10, left: -10 },
                cursorAt: { top: -15, left: -15 },
                helper: function (event) {
                    return $("<div class='ui-widget-header entity-shadow'></div>");
                },
                start: function (event) {
                    isCreate = true;
                },
                stop: function (event) {
                    // ------ Modified By Sang Dao ---------
                    var pos = { layerX: event.clientX - jQuery("#viewport").offset().left, layerY: event.clientY - jQuery("#viewport").offset().top };
                    $(document).trigger("DRAG:END", pos);
                    // ------ End Modification -------------
                    actionToolbars(event, 'Generic');
                    isCreate = false;
                },
                drag: function (event) {
                    setPositionDrag();
                }
            });

            // approve
            $('#approve').draggable({
                cursor: "pointer",
                cursorAt: { top: -10, left: -10 },
                cursorAt: { top: -15, left: -15 },
                helper: function (event) {
                    return $("<div class='ui-widget-header entity-shadow'></div>");
                },
                start: function (event) {
                    isCreate = true;
                },
                stop: function (event) {
                    // ------ Modified By Sang Dao ---------
                    var pos = { layerX: event.clientX - jQuery("#viewport").offset().left, layerY: event.clientY - jQuery("#viewport").offset().top };
                    $(document).trigger("DRAG:END", pos);
                    // ------ End Modification -------------
                    actionToolbars(event, 'Approve');
                    isCreate = false;
                },
                drag: function (event) {
                    setPositionDrag();
                }
            });
        }
    });
}

var currentHeight = 0;
// fn handle popup
function handlePopup() {
    // properties entities popup
    $('#popup-modal').dialog({
        height: 250,
        width: 500,
        resizable: true,
        modal: true,
        autoOpen: false,
        buttons: {
            'Ok': function () {
                if (actionEdit()) {
                    isEdit = false;
                    checkModified(isModified = true);
                    $(this).dialog('close');
                }
            },
            cancel: function () {
                $(this).dialog('close');
                isEdit = false;
            }
        },
        resizeStart: function () {
            currentHeight = $('#annotation').height();
        },
        resize: function (e, ui) {
            $('#annotation').height(currentHeight + (ui.size.height - ui.originalSize.height));
        }
    });

    // variable popup
    $('#popup-variable-modal').dialog({
        height: 250,
        width: 500,
        resizable: false,
        modal: true,
        autoOpen: false,
        buttons: {
            'Ok': function () {
                var name = $('#var-name').val();
                var type = $("#var-type").val();
                var value = $('#var-value').val();


                if ($.trim(name) === '') {
                    $('#var-name').focus();
                    return;
                }

                if (parseInt(type, 10) === -1) {
                    $("#var-type").focus();
                    return;
                }
                if ($.trim(value) === '') {
                    //$('#var-value').focus();
                    //return;
                }

                if (!isEdit) {
                    if (mRoot.infoVariables.itemByName(name) !== -1) {
                        $(".message").html("This variable name is existing. Please choose a different name");
                        $("#confirm-popup-modal").dialog("option", "buttons", [
                            {
                                text: "Ok", click: function () {
                                    $(this).dialog("close");
                                    $('#var-name').val("Res" + mRoot.infoVariables.count).focus();
                                }
                            }
                        ]).dialog("open");
                        return;
                    }

                    var variable = new Variable();
                    variable.name = decodeXml(name);
                    variable.type = decodeXml(type);
                    variable.value = decodeXml(value);

                    // regist
                    mRoot.infoVariables.add(variable);

                    var id = mRoot.infoVariables.collection[mRoot.infoVariables.count - 1].id;
                    var item = '<li class="item-var' + id + '" onclick="return activeVariables(this)">'
                                + '<div>' + encodeXml(variable.name) + '</div>'
                                + '<a href="javascript:void(0)" class="delButton" onclick="return removeVariable(' + id + ');">'
                                    + '<img width="10px" src="Images/delete-icon.png" title="Delete" />'
                                + '</a>'
                            + '</li>'
                    $('ul.variables').append(item);
                    checkModified(isModified = true); // modified
                } else {
                    // regist
                    var item = mRoot.infoVariables.itemByID(mCurrentVariable.id);
                    item.name = decodeXml(name);
                    item.type = decodeXml(type);
                    item.value = decodeXml($.trim(value));

                    mRoot.infoVariables.change(item);

                    mCurrentVariable.obj.children[0].innerHTML = encodeXml(item.name);
                    checkModified(isModified = true); // modified
                }
                isEdit = false;
                $(this).dialog('close');

            },
            cancel: function () {
                isEdit = false;
                $(this).dialog('close');
            }
        }
    });

    // confirm
    $('#confirm-popup-modal').dialog({
        height: 200,
        width: 300,
        resizable: false,
        modal: true,
        autoOpen: false,
        title: "Confirm"
    });
}

// fn checck case name existing
function checkExistingCaseName(connection, label) {
    for (var i = 0; i < mRoot.infoConnections.count; i++) {
        var item = mRoot.infoConnections.collection[i];
        if (connection.id !== item.id) {
            if (item.from === connection.from && item.label === label) {
                return true;
            }
        }
    }

    return false;
}

// fn action edit label of entities and connections
function actionEdit() {
    var item = mSelectedItems.items.collection[0];
    if (item === undefined) {
        $('#popup-modal').dialog("close");
        isEdit = false;
        return;
    }
    editor = $('#name');
    if (item.info.shape === shapeTypes.entity) {
        if (item.info.type !== typeEntity.t_start) {
            // check name
            if ($.trim(editor.val()) === '') {
                $('#name').focus();
                return false;
            }

        }
        if (item.info.type === typeEntity.t_decision) {
            condition = $('#condition');
        }
        if (item.info.type === typeEntity.t_switch) {
            expression = $('#expression');
        }
        if (item.info.type === typeEntity.t_approve) {
            description = $('#description');
        }
        // check description
        annotation = $('#annotation');

        // set value
        editLabel();
        return true;
    }

    // connection
    if (item.info.shape === shapeTypes.connection) {
        if (parseInt($("input#flag").val()) === 0 && !$.isNumeric($.trim(editor.val()))) {
            html = '"' + editor.val() + '" is not a valid value Int32';
            $("#confirm-popup-modal").html("<div class='message'>" + html + "</div>").dialog("option", "buttons", [
                {
                    text: "Ok", click: function () {
                        $("#name").focus();
                        $(this).dialog("close");
                    }
                }
            ]).dialog("open");
            return false;
        }
        // name is existing

        if (checkExistingCaseName(item.info, editor.val())) {
            $(".message").html('A switch case with a key "' + editor.val() + '" already existes. Please choose a different key!');
            $("#confirm-popup-modal").dialog("option", "buttons", [
                {
                    text: "Ok", click: function () {
                        $("#name").focus();
                        $(this).dialog("close");
                    }
                }
            ]).dialog("open");
            return false;
        }

        editLabel();
        return true;
    }

}

// fn handle resize flow work
function handleReize() {
    resizeTool = $('#viewport').resizable({
        minWidth: defaultValue.viewport.width,
        minHeight: defaultValue.viewport.height,
        resize: function (event) {
            viewport.setSize($(this).width(), $(this).height());

            if (scrollbar !== null) {
                scrollbar.destroy();
            }
            scrollbar = $('#chart').jScrollPane().data().jsp;
        },
        stop: function (event) {
            var limit = getLimited();
            mViewport = {
                width: $(this).width() > limit.x ? $(this).width() : limit.x,
                height: $(this).height() > limit.y ? $(this).height() : limit.y
            };

            $(this).width(mViewport.width);
            $(this).height(mViewport.height);
            viewport.setSize(mViewport.width, mViewport.height);

            if (scrollbar !== null) {
                scrollbar.destroy();
            }
            scrollbar = $('#chart').jScrollPane().data().jsp;
        }
    })
}

// fn handle get workflows
function handleGetWorkflows() {
    // ------ Removed By Sang Dao ----------------

    // widget category complete jquery ui
    //$.widget("custom.catcomplete", $.ui.autocomplete, {
    //    _renderMenu: function (ul, items) {
    //        var that = this,
    //          currentCategory = "";
    //        $.each(items, function (index, item) {
    //            if (item.category != currentCategory) {
    //                ul.append("<li class='ui-autocomplete-category'>" + item.category + "</li>");
    //                currentCategory = item.category;
    //            }
    //            that._renderItem(ul, item);
    //        });
    //    }
    //});
    
    $.ajax({
        type: 'POST',
        url: 'Sketch/GetWorkflowCodes',
        success: function (data) {
            if (data !== 'Error') {
                var sources = new Array();
                for (var i = 0; i < data.length; i++) {
                    var s = { label: data[i], category: '' };
                    sources.push(s);
                }

                // ------- Modified By Sang Dao ---------
                $('#workflows').autocomplete({
                    delay: 0,
                    source: sources
                });
                // ------- End Modification -------------
            } else {
                return;
            }
        }
    });
}

// fn handle add variable
function handleAddVariable() {
    // active click
    $('#addvariables .title').hover(function () {
        $(this).css('cursor', 'pointer');
    }).click(function () {
        if (project === null) {
            return;
        }
        isEdit = false;
        // name
        $('#var-name').val('Res' + mRoot.infoVariables.count);
        // type
        $('#var-type').html('');
        $.ajax({
            type: 'post',
            url: 'Sketch/GetTypes',
            success: function (data) {
                $("#var-type").html("").append("<option value='-1'>Choose Type</option>");
                for (var i = 0; i < data.length; i++) {
                    $("#var-type").append("<option value='" + data[i] + "'>" + data[i] + "</option>");
                }
            }
        });
        // value
        $('#var-value').val('');
        $('#popup-variable-modal').dialog('open');
    });
}

// handle active variables
var mCurrentVariable = new Variable();
function activeVariables(obj) {
    isEdit = true;
    mCurrentVariable = new Variable();
    mCurrentVariable = mRoot.infoVariables.itemByName(decodeXml(obj.children[0].innerHTML));
    mCurrentVariable.obj = obj;

    if (mCurrentVariable === -1) {
        return;
    }
    // name
    $('#var-name').val(decodeXml(mCurrentVariable.name));
    // type
    $('#var-type').html('');
    $.ajax({
        type: 'post',
        url: 'Sketch/GetTypes',
        success: function (data) {
            $("#var-type").html("").append("<option value='-1'>Choose Type</option>");
            for (var i = 0; i < data.length; i++) {
                var option = "<option value='" + data[i] + "' " + (data[i].toLowerCase() === mCurrentVariable.type.toLowerCase() ? "selected='selected'" : "") + ">"
                            + data[i]
                            + "</option>";
                $("#var-type").append(option);
            }
        }
    });
    // value
    $('#var-value').val(mCurrentVariable.value);

    $('#popup-variable-modal').dialog('open');
}

// handle delete variables
function removeVariable(id) {
    var item = mRoot.infoVariables.itemByID(id);
    var flag = mRoot.infoVariables.remove(item);
    if (flag) {
        $("li.item-var" + id).fadeOut(500, "swing", function () {
            $(this).remove();
            checkModified(isModified = true);
        });

    }
}

// handle press key of input flowcharts
function handlePressKey(e) {
    switch (e.keyCode) {
        case 13: // enter
            handleLoad();
            break;
    }
}

// modified
function checkModified(modified) {
    if (modified) {
        var name = $("#workflow-name").html();
        if (name[name.length - 1] !== "*") {
            $("#workflow-name").html($("#workflow-name").html() + "*");
        }
        return;
    }
    $("#workflow-name").html($("#workflow-name").html().replaceAll("*", ""));
}

// replace all a character in string
String.prototype.replaceAll = function (strTarget, strSubString) {
    return this.split(strTarget).join(strSubString);
}

// restrict character
function restrictCharacter(e) {
    var k;
    document.all ? k = e.keyCode : k = e.which;
    return ((k > 64 && k < 91) || (k > 96 && k < 123) || k == 8 || k == 32 || (k >= 48 && k <= 57) || k == 40 || k == 41 || k == 45 || k == 95);
}

// encode xml
function encodeXml(str) {
    if (str === undefined) {
        return "";
    }
    return str.replaceAll("&", "&amp;").replaceAll("\"", "&quot;").replaceAll("'", "&apos;").replaceAll("<", "&lt;").replaceAll(">", "&gt;").replaceAll("\n", "\\n");
}

// decode xml
function decodeXml(str) {
    if (str === undefined) {
        return "";
    }
    return str.replaceAll("\\n", "\n").replaceAll("&quot;", "\"").replaceAll("&apos;", "'").replaceAll("&lt;", "<").replaceAll("&gt;", ">").replaceAll("&amp;", "&");
}

// number only
function numbersOnly(e, decimal) {
    var key;
    var keychar;
    if (window.event) {
        key = window.event.keyCode;
    }
    else if (e) {
        key = e.which;
    } else {
        return true;
    }
    keychar = String.fromCharCode(key);
    if ((key == null) || (key == 0) || (key == 8) || (key == 9) || (key == 13) || (key == 27)) {
        return true;
    } else if ((("-+0123456789").indexOf(keychar) > -1)) {
        return true;
    } else if (decimal && (keychar == ".")) {
        return true;
    } else {
        return false;
    }
}

// action begin onload
function Loading(wf) {
    // handle get workflows
    handleGetWorkflows();
    if (wf) {
        $("#workflows").val(wf);
    }
    else {
        $("#workflows").val("");
    }

    // flowtask action
    handleFlowtask();

    // deselect text in viewport
    $('#chart').bind('selectstart dragstart', function (e) {
        e.preventDefault();
        return false;
    });

    $('#chart').jScrollPane();

    // handle to toolbar (drag-drop to creates)
    handleToolBar();

    // calculating draw area
    calculateDrawArea();

    // create viewport
    viewport = Raphael('viewport', mViewport.width, mViewport.height);

    // initialize
    initialize();

    // draw entity begin
    drawBegin(viewport);
    $("#viewport").hide();

    // handle popup
    handlePopup();

    handleReize();

    // add variable action
    handleAddVariable();

    if (wf) {
        handleLoad();
    }
}