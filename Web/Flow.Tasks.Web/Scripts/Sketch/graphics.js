// big object
var mRoot = {
    infoEntities: new Collection(),
    objEntities: new Collection(),
    infoConnections: new Collection(),
    objConnections: new Collection(),
    infoVariables: new Collection()
};

// type entity
var typeEntity = {
    t_start: 'start',
    t_decision: 'decision',
    t_switch: 'switch',
    t_approve: 'approve',
    t_generic: 'generic'
};

// draw toolbars
var mToolbox = [];
var icons = {
    iconStart: "Images/start-icon.png",
    iconDecision: "Images/decision-icon.png",
    iconSwitch: "Images/switch-icon.png",
    iconFlowchart: "Images/flowchart-icon.png",
    iconGeneric: "Images/generic-icon.png",
    iconApprove: "Images/approve-icon.png"
};

// current item hover
var currentItem;
var mCurrentConnection = new Connection();

// ---------------------------
// Added By New Ocean
// Mask device functions
// ---------------------------
var currentEntity = null, selectedEntity = null, selectedConnection = null, $deviceToolboxContainer = jQuery(".on-device #device-toolbox"), isConnecting = false, dblTimeout = null;

var isOnDevice = function () {
    return jQuery("html").hasClass("on-device");
}

var hideAllButtons = function () {
    $deviceToolboxContainer.find("button").addClass("hidden");
}

var showNodeFromSelection = function (show) {
    if (show) {
        jQuery(".on-device #nodes-from-direction").removeClass("hidden");
        jQuery(".on-device #nodes-from-direction-node").removeClass("hidden");
        jQuery(".on-device #nodes-section").removeClass("hidden");
    } else {
        jQuery(".on-device #nodes-from-direction").addClass("hidden");
        jQuery(".on-device #nodes-from-direction-node").addClass("hidden");
        jQuery(".on-device #nodes-section").addClass("hidden");
    }
}

var showNodeToSelection = function (show) {
    if (show) {
        jQuery(".on-device #nodes-to-direction").removeClass("hidden");
        jQuery(".on-device #nodes-to-direction-node").removeClass("hidden");
        jQuery(".on-device #nodes-section").removeClass("hidden");
    } else {
        jQuery(".on-device #nodes-to-direction").addClass("hidden");
        jQuery(".on-device #nodes-to-direction-node").addClass("hidden");
        jQuery(".on-device #nodes-section").addClass("hidden");
    }
}

var resetNodeSelection = function () {
    jQuery("#nodes-from-direction select").val('top');
    jQuery("#nodes-from-direction-node select").val(1);
}

var showButton = function (name, show) {
    $deviceToolboxContainer = jQuery(".on-device #device-toolbox");
    if (show) {
        $deviceToolboxContainer.find("button[data-name='" + name + "']").removeClass("hidden");
    } else {
        $deviceToolboxContainer.find("button[data-name='" + name + "']").addClass("hidden");
    }
}

var showNode = function (direction, name, show) {
    if (show) {
        jQuery(".on-device #nodes-section").removeClass("hidden");
        jQuery(".on-device #nodes-section select[data-name='" + name + "'][data-direction='" + direction + "']").parents("section").removeClass("hidden");
    } else {
        jQuery(".on-device #nodes-section select[data-name='" + name + "'][data-direction='" + direction + "']").parents("section").addClass("hidden");
    }
}

var deleteNodeOption = function (list, id) {
    var newList = [];
    for (var i = 0; i < list.length; i++) {
        var nodeOption = list[i];
        if (nodeOption.id != id) {
            newList.push(nodeOption);
        }
    }
    return newList;
}

var addDataNodes = function (direction, name) {
    var $selector = jQuery(".on-device #nodes-section select[data-name='" + name + "'][data-direction='" + direction + "']");
    showNode(direction, name, false);

    if (name == typeEntity.t_decision) {
        // Clear old options
        $selector.find('option').remove();
        // Add new options
        var trueNode = selectedEntity.holders[5];           // True
        var falseNode = selectedEntity.holders[4];          // False
        var array = [{ id: trueNode.id, name: 'True' }, { id: falseNode.id, name: 'False' }];

        for (var i = 0; i < mRoot.infoConnections.count; i++) {
            var conn = mRoot.infoConnections.collection[i];
            if (conn.from == selectedEntity.id) {
                if (conn.fromHolder === trueNode.id) {
                    array = deleteNodeOption(array, trueNode.id);
                }
                if (conn.fromHolder === falseNode.id) {
                    array = deleteNodeOption(array, falseNode.id);
                }
            }
        }

        for (var i = 0; i < array.length; i++) {
            $selector.append('<option value="' + array[i].id + '">' + array[i].name + '</option>');
        }

        if (array.length > 0) {
            showNode(direction, name, true);
        }
    }
    else if (name == typeEntity.t_start) {

    }
    else if (name == typeEntity.t_switch) {

    }
    else if (name == typeEntity.t_approve) {

    }
    else if (name == typeEntity.t_generic) {

    }


}

var hideNodes = function () {
    jQuery(".on-device #nodes-section").addClass("hidden");
    jQuery(".on-device #nodes-section select").parents("section").addClass("hidden");
}

var getHolderByPosition = function (holders, pos) {
    var array = [];
    for (var i = 0; i < holders.length; i++) {
        if (holders[i].type == pos) {
            array.push(holders[i]);
        }
    }
    return array;
}

var linkShape = function (from, to) {

    if (from != null && from.id != to.id) {
        // ------------------------------------------------
        //           Select Node from Shape FROM
        // ------------------------------------------------
        var holderPosition = 0;
        var fromHolderId = from.holders[holderPosition].id;                                 // Default
        var position = jQuery("#nodes-from-direction select").val();                        // Get Position (Top, Left, Bottom, Right) selected
        var positionIndex = parseInt(jQuery("#nodes-from-direction-node select").val());    // Get Position Index (1, 2, 3)
        var holders = getHolderByPosition(from.holders, position);
        if (from.info.type == typeEntity.t_decision) {                                      // From Decision Shape
            fromHolderId = jQuery(".on-device #nodes-section select[data-name='decision'][data-direction='from']").val();
            if (fromHolderId != '') {
                fromHolderId = parseInt(fromHolderId);
            }
        }
        else if (from.info.type == typeEntity.t_start) {                                    // From Start Shape
            // Top: 0, Bottom: 1, Right: 2, Left: 3
            // Start Shape only have 1 node for each side
            if (holders.length) {
                fromHolderId = holders[holderPosition].id;
            }
        }
        else if (from.info.type == typeEntity.t_switch) {                                   // From Switch Shape
            // Start Shape have 3 nodes for each side
            if (holders.length) {
                holderPosition = positionIndex == 1 ? 1 : positionIndex == 2 ? 0 : 2;
                fromHolderId = holders[holderPosition].id;
            }
        }
        else if (from.info.type == typeEntity.t_approve) {                                  // From Approve Shape
            // Approve Shape only have 1 node for each side
            if (holders.length) {
                fromHolderId = holders[holderPosition].id;
            }
        }
        else if (from.info.type == typeEntity.t_generic) {                                  // From Generic Shape
            // Generic Shape only have 1 node for each side
            if (holders.length) {
                fromHolderId = holders[holderPosition].id;
            }
        }

        // ------------------------------------------------
        //        End Select Node from Shape From
        // ------------------------------------------------


        // ------------------------------------------------
        //           Select Node from Shape TO
        // ------------------------------------------------
        var toHolderId = -1;
        var toholderPosition = 0;
        position = jQuery("#nodes-to-direction select").val();                            // Get Position (Top, Left, Bottom, Right) selected
        positionIndex = parseInt(jQuery("#nodes-to-direction-node select").val());        // Get Position Index (1, 2, 3)
        if (to.info.type == typeEntity.t_decision) {                                      // To Decision Shape
            // Decision Shape have 3 Nodes on Top and one on Bottom
            position = (position == "right" || position == "left") ? "top" : position;    // Convert Right, Left position to top position
            holders = getHolderByPosition(to.holders, position);
            if (holders.length) {
                if (position == "top") {
                    toholderPosition = positionIndex == 1 ? 1 : positionIndex == 2 ? 0 : 2;
                } else {
                    toholderPosition = 0;
                }

                toHolderId = holders[toholderPosition].id;
            }
        }
        else if (to.info.type == typeEntity.t_switch) {                                   // To Switch Shape
            // Start Shape have 3 nodes for each side
            holders = getHolderByPosition(to.holders, position);
            if (holders.length) {
                toholderPosition = positionIndex == 1 ? 1 : positionIndex == 2 ? 0 : 2;
                toHolderId = holders[toholderPosition].id;
            }
        }
        else if (to.info.type == typeEntity.t_approve) {                                  // To Approve Shape
            // Approve Shape only have 1 node for each side
            holders = getHolderByPosition(to.holders, position);
            if (holders.length) {
                toHolderId = holders[0].id;
            }
        }
        else if (to.info.type == typeEntity.t_generic) {                                  // To Generic Shape
            // Generic Shape only have 1 node for each side
            holders = getHolderByPosition(to.holders, position);
            if (holders.length) {
                toHolderId = holders[0].id;
            }
        }

        // ------------------------------------------------
        //       End Select Node from Shape TO
        // ------------------------------------------------

        isHolder = true;
        mDummy.move = true;
        mCurrentConnection = new Connection();
        //mCurrentConnection.id = mDummy.id;
        mCurrentConnection.from = from.id;
        mCurrentConnection.to = mDummy.id;
        mCurrentConnection.fromHolder = fromHolderId;
        mCurrentConnection.toHolder = toHolderId;

        mDummy.entity = mDummy;
        var conn = createConnection(mCurrentConnection);
        mRoot.objConnections.add(conn);

        // call mouse up event
        doMouseUp.call(to);

        showButton('delete', false);
        showButton('connect', false);
        showButton('deconnect', false);

        hideNodes();
        resetNodeSelection();

        selectedEntity = null;
        isConnecting = false;

        // Display Nodes of All shapes
        $(document).trigger("DISPLAY:NODES", false);
    }
}

var startConnecting = function () {
    showButton('delete', false);
    showButton('connect', false);
    showButton('deconnect', true);
    showNodeToSelection(true);

    selectedEntity = currentEntity;
    isConnecting = true;

    fin.call(selectedEntity);

    if (selectedEntity.info.type == typeEntity.t_decision) {
        addDataNodes('from', 'decision');
        showNode('from', 'decision', true);
    }
    else if (selectedEntity.info.type == typeEntity.t_start) {
        showNodeFromSelection(true);
    }
    else if (selectedEntity.info.type == typeEntity.t_switch) {
        showNodeFromSelection(true);
    }
    else if (selectedEntity.info.type == typeEntity.t_approve) {
        showNodeFromSelection(true);
    }
    else if (selectedEntity.info.type == typeEntity.t_generic) {
        showNodeFromSelection(true);
    }

    // Display Nodes of All shapes
    $(document).trigger("DISPLAY:NODES", true);
}

var cancelConnecting = function () {
    if (selectedEntity.info.type != 'start') {
        showButton('delete', true);
    }
    showButton('connect', true);
    showButton('deconnect', false);

    fout.call(selectedEntity);
    showNodeToSelection(false);
    showNodeFromSelection(false);
    hideNodes();

    selectedEntity = null;
    isConnecting = false;

    // Display Nodes of All shapes
    $(document).trigger("DISPLAY:NODES", false);
}

function fnDelete() {
    deleteSelectedItems();
}

function fnStartConnecting() {
    startConnecting();
}

function fnCancelConnecting() {
    cancelConnecting();
}

jQuery(document).on("CONNECTIONLINE:DESELECT", function () {
    if (isOnDevice()) {
        selectedConnection = null;
        hideAllButtons();
    }
});

jQuery(document).on("CONNECTIONLINE:SELECT", function (e, d) {
    if (isOnDevice()) {
        selectedConnection = d;
        showButton('delete', true);
    }
});

// draw holder
function drawHolder(panel, x, y, pos) {
    var holder = null;
    if (pos === 'left' || pos === 'right') { // left | right
        holder = panel.rect(x, y, defaultValue.holder.height, defaultValue.holder.width).attr(holderSettings.holderHide);
    } else if (pos === 'top' || pos === 'bottom') { // top | bottom
        holder = panel.rect(x, y, defaultValue.holder.width, defaultValue.holder.height).attr(holderSettings.holderHide);
    }
    holder.shapeType = shapeTypes.holder;
    holder.hoverChange = function (isHover) {
        if (isHover) {
            holder.attr(holderSettings.holderSelected);
        } else {
            holder.attr(holderSettings.holderNormal);
        }
    };
    holder.show = function (isFlag) {
        if (!isFlag) {
            holder.attr(holderSettings.holderHide);
            if (holder.left !== undefined) {
                holder.left.attr({ 'fill-opacity': 0.01 });
            }
            if (holder.right !== undefined) {
                holder.right.attr({ 'fill-opacity': 0.01 });
            }
        } else {
            holder.attr(holderSettings.holderNormal);
        }
    }

    return holder;
}

// raphael function hover in object
function fin() {
    currentItem = this;
    if (this.shapeType === shapeTypes.entity) {
        if ((this.info.type === typeEntity.t_start && !isStartOnlyConnection()) || this.info.type !== typeEntity.t_start) {
            if (this.info.type === typeEntity.t_decision) {

                // Added By New Ocean
                if (isOnDevice() && isConnecting && selectedEntity.info.type != typeEntity.t_decision) {
                    for (var i = 0; i < this.holders.length; i++) {
                        if (this.holders[i].type === 'top' || this.holders[i].type === 'bottom') { // top/bottom
                            this.holders[i].attr(holderSettings.holderNormal);
                        }
                    }
                    return;
                }

                if (isHolder) { // hover when create a new connection 
                    if (checkSelectedItems(this)) {
                        for (var i = 0; i < this.holders.length; i++) {

                            if (this.holders[i].type === 'top' || this.holders[i].type === 'bottom') { // top/bottom
                                this.holders[i].attr(holderSettings.holderSelected);
                            }
                        }
                    } else {
                        for (var i = 0; i < this.holders.length; i++) {
                            if (this.holders[i].type === 'top' || this.holders[i].type === 'bottom') { // top/bottom
                                this.holders[i].attr(holderSettings.holderSelected);
                            }
                        }
                    }

                } else if (isHolderChange) { // hover when change connection to another entity
                    if (this.info.type === typeEntity.t_decision && mCurrentConnection.point === 'start') {
                        for (var i = 0; i < this.holders.length; i++) {
                            if (this.holders[i].left != undefined) {
                                this.holders[i].attr(holderSettings.holderSelected);
                                this.holders[i].left.attr({ "fill-opacity": 1 })
                            }
                            if (this.holders[i].right != undefined) {
                                this.holders[i].attr(holderSettings.holderSelected);
                                this.holders[i].right.attr({ "fill-opacity": 1 })
                            }
                        }
                    } else {
                        for (var i = 0; i < this.holders.length; i++) {
                            if (this.holders[i].type === 'top' || this.holders[i].type === 'bottom') { // top/bottom
                                this.holders[i].attr(holderSettings.holderSelected);
                            }
                        }
                    }
                } else { // hover normal when not active
                    // show label breakpoint of decision
                    var array = isDecisionConnection(this);
                    for (var i = 0; i < this.holders.length; i++) {
                        if (this.holders[i].id === 2 || this.holders[i].id === 4) { // top/bottom
                            if ($.inArray(this.holders[i].id, array) === -1) {
                                this.holders[i].attr(holderSettings.holderSelected);
                                if (this.holders[i].left !== undefined) {
                                    this.holders[i].left.attr({ "fill-opacity": 1 })
                                }
                                if (this.holders[i].right !== undefined) {
                                    this.holders[i].right.attr({ "fill-opacity": 1 })
                                }
                            }
                        }
                    }
                }
            } else {
                // not holder when generic/approve have connection
                if (!isAppGenOnlyConnection(this)) {
                    if (checkSelectedItems(this)) {
                        this.holders.attr(holderSettings.holderSelected);
                    } else {
                        this.holders.attr(holderSettings.holderNormal);
                    }
                }
            }
        }
    }
}

// raphel function hover out object
function fout() {
    if (this.shapeType === shapeTypes.entity) {
        this.holders.attr(holderSettings.holderHide);
        for (var i = 0; i < this.holders.length; i++) {
            if (this.holders[i].left != undefined) {
                this.holders[i].left.attr({ "fill-opacity": 0.01 })
            }
            if (this.holders[i].right != undefined) {
                this.holders[i].right.attr({ "fill-opacity": 0.01 })
            }
        }
    }
    currentItem = undefined;
}

// event mouse down of objects
function mousedown(e) {
    // mouse down entity
    if (this.shapeType === shapeTypes.entity) {

        if (mSelectedItems.items.count != 0) {
            // only select single connection
            if (mSelectedItems.items.item(0).shapeType === shapeTypes.connection) {
                var item = mSelectedItems.items.item(0);
                removeBreakPoint(item);
                deSelectedItems();
            }
            // check multi select
            if (!mSelectedItems.isMultiSelect) {
                if (mSelectedItems.items.count > 1) {
                    if (!checkSelectedItems(this)) {
                        deSelectedItems();
                        // add item if not existing
                        this.selectChange(true);
                        mSelectedItems.items.add(this, false);
                    }
                } else { // is 1
                    deSelectedItems();
                    // add item if not existing
                    this.selectChange(true);
                    mSelectedItems.items.add(this, false);
                }
            } else {
                if (checkSelectedItems(this)) {
                    // remove item if existing
                    this.selectChange(false);
                    mSelectedItems.items.remove(this);
                } else {
                    // add item if not existing
                    this.selectChange(true);
                    mSelectedItems.items.add(this, false);
                }
            }
        } else {
            // add item if not existing
            this.selectChange(true);
            mSelectedItems.items.add(this, false);
        }


        if ((this.info.type === typeEntity.t_start && !isStartOnlyConnection()) || this.info.type !== typeEntity.t_start) {
            // holders
            mMouse.start = mMouse.position
            for (var i = 0; i < this.holders.length; i++) {
                var holder = this.holders[i];
                var limit = {
                    min: {
                        x: holder.attr('x') - defaultValue.margin / 2,
                        y: holder.attr('y') - defaultValue.margin / 2
                    },
                    max: {
                        x: holder.attr('x') + holder.attr('width') + defaultValue.margin / 2,
                        y: holder.attr('y') + holder.attr('height') + defaultValue.margin / 2
                    }
                };

                if (limit.min.x < mMouse.start.x && mMouse.start.x < limit.max.x && limit.min.y < mMouse.start.y && mMouse.start.y < limit.max.y) {
                    if (this.info.type === typeEntity.t_decision) { // decision top/bottom not create connection
                        var array = isDecisionConnection(this);
                        if (holder.type === 'top' || holder.type === 'bottom') {
                            return;
                        }
                        if ($.inArray(holder.id, array) !== -1) {
                            return;
                        }
                    }
                    if (isAppGenOnlyConnection(this)) return;
                    isHolder = true;
                    mDummy.move = true;
                    mCurrentConnection = new Connection();
                    //mCurrentConnection.id = mDummy.id;
                    mCurrentConnection.from = this.id;
                    mCurrentConnection.to = mDummy.id;
                    mCurrentConnection.fromHolder = holder.id;
                    mCurrentConnection.toHolder = -1;

                    mDummy.attr({
                        x: mMouse.start.x,
                        y: mMouse.start.y
                    });
                    mDummy.entity = mDummy;
                    var conn = createConnection(mCurrentConnection);
                    mRoot.objConnections.add(conn);
                }
            }
        }
    }

    // mouse down connection
    if (this.shapeType === shapeTypes.connection) {
        if (mSelectedItems.items.count != 0) {
            if (mSelectedItems.items.item(0).shapeType === shapeTypes.connection) {
                var item = mSelectedItems.items.item(0);
                removeBreakPoint(item);
            }
        }
        deSelectedItems();
        this.parent.selectChange(true);

        this.parent.breakpoints = drawBreakPoint(viewport, this.parent);
        mSelectedItems.items.add(this.parent, false);
    }

    // mouse down breakpoint of connection
    if (this.shapeType === shapeTypes.breakpoint) {
        isHolderChange = true;
        mCurrentConnection = new Connection();
        mCurrentConnection = this.parent.info.clone();
        mCurrentConnection.label = "Default";

        // dummy
        mDummy.move = true;
        mDummy.attr({
            x: mMouse.position.x,
            y: mMouse.position.y
        })
        mCurrentConnection.point = this.point;
        mDummy.entity = mDummy;
        if (this.point === "start") {
            mCurrentConnection.from = mDummy.id;
            mCurrentConnection.fromHolder = -1;
        } else {
            mCurrentConnection.to = mDummy.id;
            mCurrentConnection.toHolder = -1;
        }

        var conn = createConnection(mCurrentConnection);
        mRoot.objConnections.add(conn);
    }
}

function doMouseUp(e) {

    // create connection when hover a new entity on a entity existing
    if (isCreate) {
        e = e || window.event;
        var mouse = { x: 0, y: 0 };
        if ($.browser.msie) {
            mouse = {
                x: e.offsetX,
                y: e.offsetY
            };
        } else {
            mouse = {
                x: e.layerX,
                y: e.layerY
            };
        }

        var holder = checkHolderCreate(this, mouse, false);
        if (holder === -1) {
            holderCreate = undefined;
        } else {
            holderCreate = this.info;
            holderCreate.holder = holder;
        }
    }

    // create connection
    if (isHolder) {
        // can't connect to itself
        deleteConnectionDummy();

        // not connection with end point is start entity
        if (this.info.type !== typeEntity.t_start) {
            if (mCurrentConnection.from != this.id) {
                mCurrentConnection.to = this.id;

                // Modified By Sang Dao
                if (!isOnDevice()) {
                    mCurrentConnection.toHolder = checkHolders(this, mMouse.position, false);
                }

                var flag = false;
                // check existing of connection
                mRoot.infoConnections.forEach(function (item) {
                    if (item.from === mCurrentConnection.from
                        && item.fromHolder === mCurrentConnection.fromHolder
                        && item.to === mCurrentConnection.to
                        && item.toHolder === mCurrentConnection.toHolder) {
                        flag = true;
                    }
                });

                if (!flag) {
                    // regist
                    var label = getLabelSwitch(mCurrentConnection);
                    if (label.toLowerCase() === "default") {
                        mCurrentConnection.isDefault = true;
                    }
                    mCurrentConnection.label = label;
                    var conn = createConnection(mCurrentConnection);

                    // can't draw label connnection begin/end is start

                    if (mRoot.infoEntities.itemByID(mCurrentConnection.from).type !== typeEntity.t_start &&
                        mRoot.infoEntities.itemByID(mCurrentConnection.to).type !== typeEntity.t_start) {
                        // it's decision => set connection is true or false
                        if (mRoot.infoEntities.itemByID(mCurrentConnection.from).type === typeEntity.t_decision) {
                            if (mCurrentConnection.fromHolder === 2) {
                                mCurrentConnection.label = 'False';
                            } else {
                                mCurrentConnection.label = 'True';
                            }
                        }
                        // description of connection from switch
                        mCurrentConnection.description = "";

                        // connection from generic/approve, it not labels
                        if (mRoot.infoEntities.itemByID(mCurrentConnection.from).type !== typeEntity.t_generic &&
                            mRoot.infoEntities.itemByID(mCurrentConnection.from).type !== typeEntity.t_approve) {
                            // draw temp a label for get width/height.
                            var temp = viewport.text(-100, -100, mCurrentConnection.label).attr({ "font-size": "10px" });
                            conn.label = drawLabel(conn.posLabel.x, conn.posLabel.y, temp.getBBox().width, temp.getBBox().height, mCurrentConnection.label, viewport);
                            conn.label.mask.parent = conn;
                            temp.remove();
                        } else {
                            mCurrentConnection.label = "";
                        }
                    }

                    mRoot.infoConnections.add(mCurrentConnection);
                    mRoot.objConnections.add(conn);
                    checkModified(isModified = true); // modified
                }
            }
        }
        isHolder = false;
    }

    // change connection
    if (isHolderChange) {
        deleteConnectionDummy();
        if (this.info.type === typeEntity.t_start && mCurrentConnection.point !== 'start') {
            isHolderChange = false;
            return;
        }
        // change start breakpoint
        if (mCurrentConnection.point === 'start') {
            var holder = checkHolders(this, mMouse.position, true);
            if (this.info.type === typeEntity.t_decision && holder === 1) {
                holder = 4;
            }
            // old position
            if (mCurrentConnection.from === this.id && mCurrentConnection.fromHolder === holder) {
                return;
            } else if (mCurrentConnection.to === this.id) {
                return;
            } else {
                mCurrentConnection.from = this.id;
                mCurrentConnection.fromHolder = holder; // default is top

                var label = getLabelSwitch(mCurrentConnection);
                if (label.toLowerCase() === "default") {
                    mCurrentConnection.isDefault = true;
                }
                mCurrentConnection.label = label;
                mRoot.infoConnections.change(mCurrentConnection);
            }
        }
            // change  end breakpoint
        else if (mCurrentConnection.point === 'end') {
            var holder = checkHolders(this, mMouse.position, false);
            // old position
            if (mCurrentConnection.to === this.id && mCurrentConnection.toHolder === holder) {
                return;
            } else if (mCurrentConnection.from === this.id) {
                return;
            } else {
                mCurrentConnection.to = this.id;
                mCurrentConnection.toHolder = holder;

                var flag = false;
                // check existing of connection
                mRoot.infoConnections.forEach(function (item) {
                    if (item.from === mCurrentConnection.from
                        && item.fromHolder === mCurrentConnection.fromHolder
                        && item.to === mCurrentConnection.to
                        && item.toHolder === mCurrentConnection.toHolder) {
                        flag = true;
                    }
                });
                if (!flag) {
                    mCurrentConnection.label = mSelectedItems.items.collection[0].info.label;
                    mRoot.infoConnections.change(mCurrentConnection);
                }
            }
        }

        if (!flag) {
            viewport.removeConnect(mCurrentConnection.id); // remove drawer old connection
            var conn = createConnection(mCurrentConnection); // draw new connection
            if (mRoot.infoEntities.itemByID(mCurrentConnection.from).type != typeEntity.t_start &&
                mRoot.infoEntities.itemByID(mCurrentConnection.to).type != typeEntity.t_start) {
                // it's decision => set connection is true or false
                if (mRoot.infoEntities.item(mCurrentConnection.from).type === typeEntity.t_decision) {
                    if (mCurrentConnection.fromHolder === 2) {
                        mCurrentConnection.label = 'False';
                    } else {
                        mCurrentConnection.label = 'True';
                    }
                }
                // description of connection from switch
                mCurrentConnection.description = "";

                if (mRoot.infoEntities.itemByID(mCurrentConnection.from).type !== typeEntity.t_generic &&
                    mRoot.infoEntities.itemByID(mCurrentConnection.from).type !== typeEntity.t_approve) {
                    var temp = viewport.text(-100, -100, mCurrentConnection.label).attr({ "font-size": "10px" });
                    conn.label = drawLabel(conn.posLabel.x, conn.posLabel.y, temp.getBBox().width, temp.getBBox().height, mCurrentConnection.label, viewport);
                    conn.label.mask.parent = conn;
                    temp.remove();
                } else {
                    mCurrentConnection.label = "";
                }
            }
            conn.id = mCurrentConnection.id; // set old id
            mRoot.objConnections.change(conn); // update
            checkModified(isModified = true); // modified
        }
        isHolderChange = false;
    }
}

// event mouse up of objects
function mouseup(e) {
    // entity
    if (this.shapeType === shapeTypes.entity) {
        if (isOnDevice()) {

            // Added By New Ocean
            if (dblTimeout == null) {
                dblTimeout = setTimeout(function () {
                    doMouseUp.call(this, e);
                    dblTimeout = null;
                }, 300);
            } else {
                window.clearTimeout(dblTimeout);
                dblTimeout = null;
                doubleClick.call(this);
            }

        } else {
            doMouseUp.call(this, e);
        }
    }
    // breakpoint
    if (this.shapeType === shapeTypes.breakpoint) {
        if (isHolderChange) {
            isHolderChange = false;
        }
    }
}

// event double click
function doubleClick(e) {
    if (this.shapeType === shapeTypes.entity) {
        isEdit = true;
        // deselect all item if multi
        if (mSelectedItems.items.count > 1) {
            deSelectedItems();
            this.selectChange(true);
            mSelectedItems.items.add(this, false);
        }
        // remove table
        $('.table-properties').html('');
        if (this.info.type === typeEntity.t_start) { // start
            var desc = '<tr  class="desc">' +
                        '<td class="label">Description</td>' +
                        '<td class="value">' +
                            '<textarea id="annotation" type="text" cols="50" rows="5">' + this.info.annotation + '</textarea>' +
                        '</td>' +
                    '</tr>';
            $('.table-properties').append(desc);

            $('#popup-modal').dialog('option', 'height', 210); // height
        } else if (this.info.type === typeEntity.t_decision) {
            var condition = '<tr>' +
                        '<td class="label">Condition</td>' +
                        '<td class="value">' +
                            '<input id="condition" type="text" value="' + this.info.condition + '" maxlength="30"/>' +
                        '</td>' +
                    '</tr>';
            $('.table-properties').append(condition);
            var name = '<tr>' +
                        '<td class="label">Name</td>' +
                        '<td class="value">' +
                            '<input id="name" type="text" value="' + this.info.label + '" maxlength="30"/>' +
                        '</td>' +
                    '</tr>';
            $('.table-properties').append(name);
            var desc = '<tr  class="desc">' +
                        '<td class="label">Description</td>' +
                        '<td class="value">' +
                            '<textarea id="annotation" type="text">' + this.info.annotation + '</textarea>' +
                        '</td>' +
                    '</tr>';
            $('.table-properties').append(desc);

            $('#popup-modal').dialog('option', 'height', 270); // height
        } else if (this.info.type === typeEntity.t_switch) {
            var name = '<tr>' +
                        '<td class="label">Name</td>' +
                        '<td class="value">' +
                            '<input id="name" type="text" value="' + this.info.label + '" maxlength="30"/>' +
                        '</td>' +
                    '</tr>';
            $('.table-properties').append(name);
            var expression = '<tr>' +
                        '<td class="label">Expression</td>' +
                        '<td class="value">' +
                            '<input id="expression" type="text" value="' + this.info.expression + '"/>' +
                        '</td>' +
                    '</tr>';
            $('.table-properties').append(expression);
            var desc = '<tr class="desc">' +
                        '<td class="label">Description</td>' +
                        '<td class="value">' +
                            '<textarea id="annotation" type="text">' + this.info.annotation + '</textarea>' +
                        '</td>' +
                    '</tr>';
            $('.table-properties').append(desc);
            $('#popup-modal').dialog('option', 'height', 270); // height
        } else {
            var name = '<tr>' +
                        '<td class="label">Name</td>' +
                        '<td class="value">' +
                            '<input id="name" type="text" value="' + this.info.label + '" maxlength="30"/>' +
                        '</td>' +
                    '</tr>';
            $('.table-properties').append(name);
            var annotation = '<tr class="desc">' +
                        '<td class="label">Description</td>' +
                        '<td class="value">' +
                            '<textarea id="annotation" type="text">' + this.info.annotation + '</textarea>' +
                        '</td>' +
                    '</tr>';
            $('.table-properties').append(annotation);
            if (this.info.type === typeEntity.t_approve) {
                var desc = '<tr class="desc">' +
                            '<td class="label">Description</td>' +
                            '<td class="value">' +
                                '<textarea id="description" type="text">' + this.info.description + '</textarea>' +
                            '</td>' +
                        '</tr>';
                //$('.table-properties').append(desc);
            }
            $('#popup-modal').dialog('option', 'height', 250); // height
        }
        $('#popup-modal').dialog('open');
    }
    if (this.shapeType === shapeTypes.connection) {
        if (isSwitch(this.parent.info.from)) {
            isEdit = true;
            $('.table-properties').html('');
            var name = "";
            //get entity
            var entity = mRoot.infoEntities.itemByID(this.parent.info.from);
            if (entity.typeSwitch.toLowerCase() === "int32") {
                name = '<tr>' +
                            '<td class="label">Name</td>' +
                            '<td class="value">' +
                                '<input id="flag" type="hidden" value="' + (!$.isNumeric(this.parent.info.label) ? '1' : '0') + '" />' +
                                '<input id="name" type="text" value="' + this.parent.info.label + '" maxlength="20"/>' +
                            '</td>' +
                        '</tr>';
            } else if (entity.typeSwitch.toLowerCase() === "string") {
                name = '<tr>' +
                            '<td class="label">Name</td>' +
                            '<td class="value">' +
                                '<input id="flag" type="hidden" value="1" />' +
                                '<input id="name" type="text" value="' + this.parent.info.label + '" maxlength="20"/>' +
                            '</td>' +
                        '</tr>';
            }
            $('.table-properties').append(name);
            $('#popup-modal').dialog('option', 'height', 150);
            $('#popup-modal').dialog('open');
        }
    }
    return true;
}

// event mouse move when drag-drop objects
function onmove(dx, dy) {

    if (!isHolder && !isHolderChange) {
        drag(mSelectedItems.items);
    }
    if (isHolder || isHolderChange) return;
    var pos = {
        x: this.x + dx,
        y: this.y + dy
    }
    if (this.shapeType === shapeTypes.entity) {
        mSelectedItems.items.forEach(function (entity) {
            var pos = {
                x: entity.x + dx,
                y: entity.y + dy
            }
            entity.moveTo(pos.x, pos.y);
        })
        invalidateConnections();
        checkModified(isModified = true);
    } else {
        mDummy.attr({
            x: pos.x,
            y: pos.y
        });
    }
    function drag(items) {
        // hash entity
        var mouse = mMouse.position;
        var mainflag = false;
        for (var i = 0; i < mRoot.objEntities.count; i++) {
            var flag = false;
            var item = mRoot.objEntities.collection[i];
            for (var j = 0; j < items.count; j++) {
                if (item.info.id === items.collection[j].info.id) {
                    flag = true;
                }
            }
            if (!flag) {
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
                    mainflag = true;
                }
            }
        }
        if (mainflag) {
            $("#viewport").css("cursor", "not-allowed");
        } else {
            $("#viewport").css("cursor", "auto");
        }
    }
}

// event mouse start when drag-drop objects
function onstart(x, y) {
    // entity
    if (this.shapeType === shapeTypes.entity) {
        mMouse.startgroup = new Array();
        mSelectedItems.items.forEach(function (item) {
            // mask
            item.x = item.attr('x');
            item.y = item.attr('y');
            // entity
            item.entity.x = item.entity.attr('x');
            item.entity.y = item.entity.attr('y');

            // icon
            item.icon.x = item.icon.attr('x');
            item.icon.y = item.icon.attr('y');

            // label
            item.label.x = item.label.attr('x');
            item.label.y = item.label.attr('y');

            for (var i = 0; i < item.holders.length; i++) {
                item.holders[i].x = item.holders[i].attr('x');
                item.holders[i].y = item.holders[i].attr('y');
                item.holders[i].show(false);
                // decision holder (true/false)
                if (item.holders[i].left != undefined) {
                    item.holders[i].left.x = item.holders[i].left.attr('x');
                    item.holders[i].left.y = item.holders[i].left.attr('y');
                }
                if (item.holders[i].right != undefined) {
                    item.holders[i].right.x = item.holders[i].right.attr('x');
                    item.holders[i].right.y = item.holders[i].right.attr('y');
                }
            }

            mMouse.startgroup.push({ x: item.x, y: item.y });
        });
    }
    if (this.shapeType === 'over') {
        this.x = this.getBBox().x;
        this.y = this.getBBox().y;
    }
}

// event mouse end when drag-drop objects
function onend(e) {
    $("#viewport").css("cursor", "auto");
    e = e || window.event;
    // action with entity
    if (this.shapeType === shapeTypes.entity) {
        if (isHolder || isHolderChange) {
            deleteConnectionDummy();
        } else {
            var mainflag = false;
            var mouse = mMouse.position;
            for (var i = 0; i < mRoot.objEntities.count; i++) {
                var flag = false;
                var item = mRoot.objEntities.collection[i];
                for (var j = 0; j < mSelectedItems.items.count; j++) {
                    if (item.info.id === mSelectedItems.items.collection[j].info.id) {
                        flag = true;
                    }
                }
                if (!flag) {
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
                        mainflag = true;
                    }
                }
            }
            if (mainflag) {
                for (var i = 0; i < mSelectedItems.items.count; i++) {
                    var item = mSelectedItems.items.collection[i];
                    mMouse.start = mMouse.startgroup[i];
                    var currentPos = {
                        x: item.entity.attr('x'),
                        y: item.entity.attr('y'),
                        w: item.entity.attr('width'),
                        h: item.entity.attr('height')
                    };
                    animationObj(item, mMouse.start, function () { return false });
                }
            }

            // over limit
            for (var i = 0; i < mSelectedItems.items.count; i++) {
                var item = mSelectedItems.items.collection[i];
                mMouse.start = mMouse.startgroup[i];
                var currentPos = {
                    x: item.entity.attr('x'),
                    y: item.entity.attr('y'),
                    w: item.entity.attr('width'),
                    h: item.entity.attr('height')
                };

                if (currentPos.x + currentPos.w + defaultValue.margin < 0) {
                    animationObj(item, mMouse.start, function () { return false; });
                } else if (currentPos.y + currentPos.h + defaultValue.margin < 0) {
                    animationObj(item, mMouse.start, function () { return false; });
                } else if (currentPos.x - defaultValue.margin > mViewport.width) {
                    animationObj(item, mMouse.start, function () { return false; });
                } else if (currentPos.y - defaultValue.margin > mViewport.height) {
                    animationObj(item, mMouse.start, function () { return false; });
                }
                measureEntities(item);
                updateInfo(item);
            }

            // over limit
            for (var i = 0; i < mSelectedItems.items.count; i++) {
                var item = mSelectedItems.items.collection[i];
                mMouse.start = mMouse.startgroup[i];
                var currentPos = {
                    x: item.entity.attr('x'),
                    y: item.entity.attr('y'),
                    w: item.entity.attr('width'),
                    h: item.entity.attr('height')
                };

                if (currentPos.x + currentPos.w + defaultValue.margin < 0) {
                    animationObj(item, mMouse.start, function () { return false; });
                } else if (currentPos.y + currentPos.h + defaultValue.margin < 0) {
                    animationObj(item, mMouse.start, function () { return false; });
                } else if (currentPos.x - defaultValue.margin > mViewport.width) {
                    animationObj(item, mMouse.start, function () { return false; });
                } else if (currentPos.y - defaultValue.margin > mViewport.height) {
                    animationObj(item, mMouse.start, function () { return false; });
                } else if (currentPos.x < 0 && currentPos.y < 0) {
                    animationObj(item, { x: defaultValue.margin, y: defaultValue.margin }, function () { return false; });
                } else if (currentPos.x < 0) {
                    animationObj(item, { x: defaultValue.margin, y: item.entity.attr("y") }, function () { return false; });
                } else if (currentPos.y < 0) {
                    animationObj(item, { x: item.entity.attr("x"), y: defaultValue.margin }, function () { return false; });
                }
                measureEntities(item);
                updateInfo(item);
            }

            timer = setTimeout(function () {
                increaseViewport();
            }, 150);
        }
    }
    isHolder = false;
    isHolderChange = false;
}

// draw Entity
function drawEntity(panel, info) {
    var deselectTimeout = null;     // Added By New Ocean

    var entity, // entity
        x, // position x
        y, // position y
        iconUrl, // url icon
        icon, // raphael draw icon
        label, // label 
        holders, // holders array of entity
        mask, // mask panel on top
        pos = {}; // position of holder
    // draw entity
    entity = panel.rect(info.x, info.y, info.width, info.height, defaultValue.entity.borderRadius).attr(entitySettings.entityNormal);

    // draw icon
    x = info.x + info.width / 2 - defaultValue.iconSize.width / 2;
    y = info.y + defaultValue.margin;
    iconUrl = '';
    switch (info.type) {
        case typeEntity.t_start:
            iconUrl = icons.iconStart;
            break;
        case typeEntity.t_decision:
            iconUrl = icons.iconDecision;
            break;
        case typeEntity.t_switch:
            iconUrl = icons.iconSwitch;
            break;
    }
    icon = panel.image(iconUrl, x, y, defaultValue.iconSize.width, defaultValue.iconSize.height);

    // draw label
    x = info.x + info.width / 2;
    y = info.y + (info.height / 3) * 2 + defaultValue.margin * 3;
    label = panel.text(x, y, decodeXml(info.label)).attr(textSettings.labelEntitySetting);
    label.shapeType = shapeTypes.label;

    /// draw holder of entity
    holders = viewport.set(); // array holders set by viewport
    // holder top
    x = entity.attr('x'); y = entity.attr('y');

    pos = { x: 0, y: 0 };

    // hide top/bottom with decision
    //if (info.type != typeEntity.decision) {
    // top
    pos = {
        x: x + (entity.attr('width') - defaultValue.holder.width) / 2,
        y: y - defaultValue.holder.height
    };
    var top = drawHolder(viewport, pos.x, pos.y, 'top');
    top.id = 1; top.type = 'top';
    holders.push(top);
    // draw 2 holder on top if this not start entity
    if (info.type !== typeEntity.t_start) {
        // top 1
        pos = {
            x: x + (entity.attr('width') / 4 - defaultValue.holder.width / 2),
            y: y - defaultValue.holder.height
        };
        var top1 = drawHolder(viewport, pos.x, pos.y, 'top');
        top1.id = 11; top1.type = 'top';
        holders.push(top1);
        // top 2
        pos = {
            x: x + (entity.attr('width') / 4 * 3 - defaultValue.holder.width / 2),
            y: y - defaultValue.holder.height
        };
        var top2 = drawHolder(viewport, pos.x, pos.y, 'top');
        top2.id = 12; top2.type = 'top';
        holders.push(top2);
    }


    // bottom
    pos = {
        x: x + (entity.attr('width') - defaultValue.holder.width) / 2,
        y: y + entity.attr('height')
    };
    var bottom = drawHolder(viewport, pos.x, pos.y, 'bottom');
    bottom.id = 3; bottom.type = 'bottom';
    holders.push(bottom);
    // draw 2 holder on top if this is switch
    if (info.type === typeEntity.t_switch) {
        // bottom 1
        pos = {
            x: x + (entity.attr('width') / 4 - defaultValue.holder.width / 2),
            y: y + entity.attr('height')
        };
        var bottom1 = drawHolder(viewport, pos.x, pos.y, 'bottom');
        bottom1.id = 31; bottom1.type = 'bottom';
        holders.push(bottom1);
        // bottom 2
        pos = {
            x: x + (entity.attr('width') / 4 * 3 - defaultValue.holder.width / 2),
            y: y + entity.attr('height')
        };
        var bottom2 = drawHolder(viewport, pos.x, pos.y, 'bottom');
        bottom2.id = 32; bottom2.type = 'bottom';
        holders.push(bottom2);
    }

    // right
    pos = {
        x: x + entity.attr('width'),
        y: y + (entity.attr('height') - defaultValue.holder.width) / 2 // portrait: width is heihgt
    };
    var right = drawHolder(viewport, pos.x, pos.y, 'right');
    right.id = 2; right.type = 'right';
    holders.push(right);
    // draw label of breakpoint decision
    if (info.type === typeEntity.t_decision) {
        right.right = viewport.text(pos.x + defaultValue.holder.height + defaultValue.margin, pos.y + defaultValue.margin, 'False').attr({ 'text-anchor': 'start', 'fill-opacity': 0.01 });
    }
    // draw 2 holder if this is switch
    if (info.type === typeEntity.t_switch) {
        // right 1
        pos = {
            x: x + entity.attr('width'),
            y: y + (entity.attr('height') / 4 - defaultValue.holder.width / 2)
        };
        var right1 = drawHolder(viewport, pos.x, pos.y, 'right');
        right1.id = 21; right1.type = 'right';
        holders.push(right1);
        // bottom 2
        pos = {
            x: x + entity.attr('width'),
            y: y + (entity.attr('height') / 4 * 3 - defaultValue.holder.width / 2)
        };
        var right2 = drawHolder(viewport, pos.x, pos.y, 'right');
        right2.id = 22; right2.type = 'right';
        holders.push(right2);
    }

    // left
    pos = {
        x: x - defaultValue.holder.height,
        y: y + (entity.attr('height') - defaultValue.holder.width) / 2 // portrait: width is heihgt
    };
    var left = drawHolder(viewport, pos.x, pos.y, 'left');
    left.id = 4; left.type = 'left';
    holders.push(left);
    // draw label of breakpoint decision
    if (info.type === typeEntity.t_decision) {
        left.left = viewport.text(pos.x - defaultValue.margin, pos.y + defaultValue.margin, 'True').attr({ 'text-anchor': 'end', 'fill-opacity': 0.01 });
    }
    // draw 2 holder if this is switch
    if (info.type === typeEntity.t_switch) {
        // right 1
        pos = {
            x: x - defaultValue.holder.height,
            y: y + (entity.attr('height') / 4 - defaultValue.holder.width / 2)
        };
        var left1 = drawHolder(viewport, pos.x, pos.y, 'left');
        left1.id = 41; left1.type = 'left';
        holders.push(left1);
        // bottom 2
        pos = {
            x: x - defaultValue.holder.height,
            y: y + (entity.attr('height') / 4 * 3 - defaultValue.holder.width / 2)
        };
        var left2 = drawHolder(viewport, pos.x, pos.y, 'left');
        left2.id = 42; left2.type = 'left';
        holders.push(left2);
    }

    // array holder object of entity
    holders.shapeType = shapeTypes.holder;

    // draw mask
    mask = panel.rect(info.x - defaultValue.holder.height, info.y - defaultValue.holder.height, info.width + defaultValue.holder.height * 2, info.height + defaultValue.holder.height * 2).attr(entitySettings.entityMask);
    mask.info = info; // set info
    mask.label = label; // set label
    mask.entity = entity; // set entity
    mask.icon = icon; // set icon
    mask.shapeType = shapeTypes.entity;

    // function setting of entity objec
    mask.hoverChange = function (isHover) {
        if (isHover) {
            entity.attr(entitySettings.entityHover);
        } else {
            entity.attr(entitySettings.entityNormal);
        }
    };

    mask.selectChange = function (isSelect) {
        if (isSelect) {
            if (isOnDevice()) {
                currentEntity = mask;
                // Added By New Ocean
                if (selectedEntity != null && isConnecting) {
                    window.clearTimeout(deselectTimeout);
                    if (selectedEntity.id != mask.id) {
                        linkShape(selectedEntity, mask);
                    } else {
                        cancelConnecting();
                    }
                } else {
                    showButton('connect', true);
                }

                if (mask.info.type != 'start') {
                    showButton('delete', true);
                    // Hide connect button to Decision entity if it has already connection true and false
                    if (isDecisionConnection(mask).length >= 2) {
                        showButton('connect', false);
                    }

                    fin.call(mask);
                } else {
                    // Hide connect button to Start Entity if it has connection going out already
                    if (isStartOnlyConnection()) {
                        showButton('connect', false);
                    }
                }
            }
            entity.attr(entitySettings.entitySelected);
        } else {
            if (isOnDevice()) {
                // Added By New Ocean
                currentEntity = null;

                if (selectedEntity != null && isConnecting) {
                    deselectTimeout = setTimeout(function () {
                        selectedEntity = null;
                        isConnecting = false;
                    }, 300);
                }

                hideAllButtons();
            }
            entity.attr(entitySettings.entityNormal);
        }
    };

    holders.parent = mask;
    mask.holders = holders;

    mask.hover(fin, fout);

    // Modified By New Ocean
    if (!isOnDevice()) {
        mask.dblclick(doubleClick);
    }

    mask.mouseup(mouseup);
    mask.mousedown(mousedown);
    mask.drag(onmove, onstart, onend);

    // Added By Sang Dao
    // Listen show nodes event
    $(document).on("DISPLAY:NODES", function (e, d) {
        if (d) {
            fin.call(mask);
        } else {
            fout.call(mask);
        }
    });

    return mask;
}

// draw Entity Flowchart
function drawEntityFlow(panel, info) {
    var deselectTimeout = null;             // Added By New Ocean
    // draw entity
    var entity = panel.rect(info.x, info.y, info.width, info.height, defaultValue.entity.borderRadius).attr(entitySettings.entityNormal);

    // draw icon
    var x = info.x + defaultValue.margin,
        y = info.y + 2;
    var icon = '';
    switch (info.type) {
        case typeEntity.t_generic:
            icon = icons.iconGeneric;
            break;
        case typeEntity.t_approve:
            icon = icons.iconApprove;
            break;
    }
    var icon = panel.image(icon, x, y, defaultValue.iconSizeFlow.width, defaultValue.iconSizeFlow.height);

    // draw label
    // get  position
    var x = info.x + defaultValue.margin * 2 + defaultValue.iconSizeFlow.width;
    y = info.y + 10;
    var label = panel.text(x, y, decodeXml(info.label)).attr(textSettings.labelEntityFlowSetting);
    label.shapeType = shapeTypes.label;

    /// draw holder of entity
    var holders = viewport.set(); // array holders set by viewport
    // holder top
    var x = entity.attr('x'), y = entity.attr('y');

    var pos = { x: 0, y: 0 };

    // top
    pos = {
        x: x + (entity.attr('width') - defaultValue.holder.width) / 2,
        y: y - defaultValue.holder.height
    };
    var top = drawHolder(viewport, pos.x, pos.y, 'top');
    top.id = 1; top.type = 'top';
    holders.push(top);

    // bottom
    pos = {
        x: x + (entity.attr('width') - defaultValue.holder.width) / 2,
        y: y + entity.attr('height')
    };
    var bottom = drawHolder(viewport, pos.x, pos.y, 'bottom');
    bottom.id = 3; bottom.type = 'bottom';
    holders.push(bottom);

    // right
    pos = {
        x: x + entity.attr('width'),
        y: y + (entity.attr('height') - defaultValue.holder.width) / 2 // portrait: width is heihgt
    };
    var right = drawHolder(viewport, pos.x, pos.y, 'right');
    right.id = 2; right.type = 'right';
    holders.push(right)

    // left
    pos = {
        x: x - defaultValue.holder.height,
        y: y + (entity.attr('height') - defaultValue.holder.width) / 2 // portrait: width is heihgt
    };
    var left = drawHolder(viewport, pos.x, pos.y, 'left');
    left.id = 4; left.type = 'left';
    holders.push(left);

    // array holder object of entity
    holders.shapeType = shapeTypes.holder;

    // draw mask
    var mask = panel.rect(info.x - defaultValue.holder.height, info.y - defaultValue.holder.height, info.width + defaultValue.holder.height * 2, info.height + defaultValue.holder.height * 2).attr(entitySettings.entityMask);
    mask.info = info; // set info
    mask.label = label; // set label
    mask.entity = entity; // set entity
    mask.icon = icon; // set icon
    mask.shapeType = shapeTypes.entity;
    // function setting of entity objec
    mask.hoverChange = function (isHover) {
        if (isHover) {
            entity.attr(entitySettings.entityHover);
        } else {
            entity.attr(entitySettings.entityNormal);
        }
    };
    mask.selectChange = function (isSelect) {
        if (isSelect) {
            if (isOnDevice()) {
                currentEntity = mask;
                // Added By New Ocean
                if (selectedEntity != null && isConnecting) {
                    window.clearTimeout(deselectTimeout);
                    if (selectedEntity.id != mask.id) {
                        linkShape(selectedEntity, mask);
                    }
                } else {
                    // Hide connect button to Generic Entity if it has already connection going out
                    if (!isAppGenOnlyConnection(mask)) {
                        showButton('connect', true);
                    }
                }

                if (mask.info.type != 'start') {
                    showButton('delete', true);
                }
            }
            entity.attr(entitySettings.entitySelected);
        } else {
            if (isOnDevice()) {
                currentEntity = null;

                if (selectedEntity != null && isConnecting) {
                    deselectTimeout = setTimeout(function () {
                        selectedEntity = null;
                        isConnecting = false;
                    }, 300);
                }

                hideAllButtons();
            }
            entity.attr(entitySettings.entityNormal);
        }
    };

    holders.parent = mask;
    mask.holders = holders;

    // event of entity object
    mask.hover(fin, fout);
    mask.mousedown(mousedown);
    mask.mouseup(mouseup);

    // Modified By New Ocean
    if (!isOnDevice()) {
        mask.dblclick(doubleClick);
    }
    mask.drag(onmove, onstart, onend);

    // Added By Sang Dao
    // Listen show nodes event
    $(document).on("DISPLAY:NODES", function (e, d) {
        if (d) {
            fin.call(mask);
        } else {
            fout.call(mask);
        }
    });

    return mask;
}

// draw workflow empty (when begin a flowchart)
function drawBegin(panel) {
    var entity = new Entity();
    entity.label = "Start";
    entity.description = "";
    entity.type = typeEntity.t_start;
    entity.x = defaultValue.begin.x;
    entity.y = defaultValue.begin.y;
    entity.width = defaultValue.entityStart.width;
    entity.height = defaultValue.entityStart.height;

    // regist
    mRoot.infoEntities.add(entity); // begin
    mRoot.objEntities.add(drawEntity(panel, entity)); // begin
}

// deselect items
function deSelectedItems() {
    mSelectedItems.items.forEach(function (item) {
        if (item.shapeType == shapeTypes.entity) {
            item.selectChange(false);
        }
        if (item.shapeType == shapeTypes.connection) {
            removeBreakPoint(item);
            item.selectChange(false);
        }
    })

    mSelectedItems.items = new Collection();
}

// fn deselect single item
function deSelectedItem(element) {
    mSelectedItems.items.forEach(function (item) {
        if (item.id == element.id) {
            item.selectChange(false);
            mSelectedItems.items.remove(item);
        }
    });

    if (mSelectedItems.items.count === 0) {
        mSelectedItems.items = new Collection();
    }
}

// fn check existing in array selected items
function checkSelectedItems(item) {
    for (var i = 0; i < mSelectedItems.items.count; i++) {
        if (item.id == mSelectedItems.items.item(i).id) {
            return true;
        }
    }
    return false;
}

// fn delete selected items
function deleteSelectedItems() {
    if (mSelectedItems.items.count != 0) {
        mSelectedItems.items.forEach(function (item) {
            if (item.shapeType == shapeTypes.entity) {
                removeEntity(item);
                checkModified(isModified = true);
                if (isOnDevice()) {
                    selectedEntity = null; // Added By New Ocean
                }
            } else if (item.shapeType == shapeTypes.connection) {
                removeConnetion(item);
                checkModified(isModified = true);
                if (isOnDevice()) {
                    selectedConnection = null; // Added By New Ocean
                }
            }
        });
        if (isOnDevice()) {
            hideAllButtons();  // Added By New Ocean                           
        }
        mSelectedItems.items = new Collection();
    }
    return false;
}

// fn create a new connection between entities
function createConnection(info) {
    var start, end;
    if (info.from != mDummy.id && info.to != mDummy.id) {
        // draw: 2 entities
        start = mRoot.objEntities.itemByID(info.from);
        end = mRoot.objEntities.itemByID(info.to);
    } else if (info.to == mDummy.id) {
        // draw from dummy
        start = mRoot.objEntities.itemByID(info.from);
        end = mDummy;
        // get start: dummy, end: entity
    } else if (info.from == mDummy.id) {
        // draw to dummy
        start = mDummy;
        end = mRoot.objEntities.itemByID(info.to);
        // get start: entity, end: dummy
    }

    var conn = viewport.connection(start, end, info);
    // shapetype - connection
    conn.shapeType = shapeTypes.connection;

    // set event for connection (mask)
    conn.mask.shapeType = shapeTypes.connection;
    conn.mask.hover(fin, fout);
    conn.mask.mousedown(mousedown);
    conn.mask.mouseup(mouseup);
    conn.mask.drag(onmove, onstart, onend);
    conn.mask.dblclick(doubleClick);

    return conn;
}

// fn invalidate connections when drag-drop entity
function invalidateConnections() {
    if (mRoot.objConnections.count != 0) {
        mRoot.objConnections.forEach(function (conn) {
            viewport.connection(conn);
        });
    }
}

// fn draw breakpoint of connection
function drawBreakPoint(panel, item) {
    var points = item.line.attr('path');
    var bp = {
        x: 0,
        y: 0
    };
    // draw start
    bp = {
        x: points[0][1],
        y: points[0][2]
    };
    var start = panel.circle(bp.x, bp.y, breakpointSettings.radius).attr(breakpointSettings.breakpointNormal);
    start.point = 'start';
    // draw end
    bp = {
        x: points[points.length - 1][1],
        y: points[points.length - 1][2]
    };
    var end = panel.circle(bp.x, bp.y, breakpointSettings.radius).attr(breakpointSettings.breakpointNormal);
    end.point = 'end';

    // group breakpoints
    var breakpoints = panel.set();

    breakpoints.push(start, end);
    for (var i = 0; i < breakpoints.length; i++) {
        var bp = breakpoints[i];
        bp.shapeType = shapeTypes.breakpoint;
        bp.parent = item;
        bp.hover(fin, fout);
        bp.mousedown(mousedown);
        bp.mouseup(mouseup);
        //bp.drag(onmove, onstart, onend);
    }

    return breakpoints;
}

function removeBreakPoint(item) {
    // remove breakpoints
    item.breakpoints.remove();
    // change status of connection
    item.selectChange(false);
}

// fn check position holders of entity
function checkHolders(obj, mPos, flag) {
    mMouse.end = mPos; // position of mouse
    for (var i = 0; i < obj.holders.length; i++) {
        var holder = obj.holders[i];
        var limit = {
            min: {
                x: holder.attr('x') - defaultValue.margin / 2,
                y: holder.attr('y') - defaultValue.margin / 2
            },
            max: {
                x: holder.attr('x') + holder.attr('width') + defaultValue.margin / 2,
                y: holder.attr('y') + holder.attr('height') + defaultValue.margin / 2
            }
        };

        if (limit.min.x < mMouse.end.x && mMouse.end.x < limit.max.x && limit.min.y < mMouse.end.y && mMouse.end.y < limit.max.y) {
            if (obj.info.type === typeEntity.t_decision) {
                if (holder.type === 'top' || holder.type === 'bottom') {
                    if (flag) { // start
                        return 4;
                    } else { // end
                        return holder.id;
                    }
                } else {
                    if (flag) { // start
                        return holder.id;
                    } else { // end
                        return 1;
                    }
                }
            }
            return holder.id; // return id holder
        }
    }
    return 1; // default top
}

// fn check holder to create connection when create new entity
function checkHolderCreate(obj, mPos) {
    mMouse.end = mPos;
    for (var i = 0; i < obj.holders.length; i++) {
        var holder = obj.holders[i];
        var limit = {
            min: {
                x: holder.attr('x'),
                y: holder.attr('y')
            },
            max: {
                x: holder.attr('x') + holder.attr('width'),
                y: holder.attr('y') + holder.attr('height')
            }
        };

        if (limit.min.x < mMouse.end.x && mMouse.end.x < limit.max.x && limit.min.y < mMouse.end.y && mMouse.end.y < limit.max.y) {
            if (obj.info.type === typeEntity.t_decision) {
                if (holder.type === 'left' || holder.type === 'right') {
                    return holder.id;
                } else {
                    return -1;
                }
            }
            return holder.id; // return id holder
        }
    }
    return -1;
}

// fn remove Entity
function removeEntity(item) {
    if (item.info.type == 'start') {
        return;
    }
    mRoot.infoEntities.remove(item);
    viewport.removeEntity(item.id);
    hideAllButtons();
}

// fn remove Connection
function removeConnetion(item) {
    mRoot.infoConnections.remove(item);
    viewport.removeConnection(item.id);
}

var measure = {
    flag: false,
    count: 0
};
// fn auto set position
function measureEntities(obj) {
    return;
    var margin = 0;
    mRoot.objEntities.forEach(function (item) {
        if (item.info.id === obj.info.id) {
            return;
        }
        var obj1 = {
            x: obj.entity.attr('x'),
            y: obj.entity.attr('y'),
            w: obj.entity.attr('width'),
            h: obj.entity.attr('height')
        }, obj2 = {
            x: item.entity.attr('x'),
            y: item.entity.attr('y'),
            w: item.entity.attr('width'),
            h: item.entity.attr('height')
        };

        if (measure.count > 5) {
            measure = {
                flag: false,
                count: 0
            }
            return;
        }

        if (measure.flag) {
            return;
        }

        // move to left
        if (obj1.x < obj2.x) {
            if ((obj1.y <= obj2.y && obj1.y + obj1.h >= obj2.y) || (obj1.y < obj2.y + obj2.h && obj1.y + obj1.h > obj2.y + obj2.h)) {
                if (obj1.x + obj1.w > obj2.x - margin) {
                    animationObj(obj, {
                        x: obj2.x - margin - obj1.w,
                        y: obj1.y
                    }, function () {
                        measure.count++;
                        if (measure.count > 1) {
                            measure.flag = true;
                        }
                        measureEntities(obj);
                    });
                }
            }
        } else { // move to right
            if ((obj1.y <= obj2.y && obj1.y + obj1.h >= obj2.y) || (obj1.y < obj2.y + obj2.h && obj1.y + obj1.h > obj2.y + obj2.h)) {
                if (obj2.x + obj2.w > obj1.x - margin) {
                    animationObj(obj, {
                        x: obj2.x + obj2.w + margin,
                        y: obj1.y
                    }, function () {
                        measure.count++;
                        if (measure.count > 1) {
                            measure.flag = true;
                        }
                        measureEntities(obj);
                    });
                }
            }
        }
    });
}

// handle animate entity
function animationObj(obj, val, callback) {
    var second = 100;
    var delta = {
        x: (val.x - obj.entity.attr('x')),
        y: (val.y - obj.entity.attr('y'))
    };
    // holders
    for (var i = 0; i < obj.holders.length; i++) {
        obj.holders[i].attr({
            x: obj.holders[i].attr('x') + delta.x,
            y: obj.holders[i].attr('y') + delta.y
        });
        if (obj.holders[i].left !== undefined) {
            obj.holders[i].left.attr({
                x: obj.holders[i].left.attr('x') + delta.x,
                y: obj.holders[i].left.attr('y') + delta.y
            });
        }
        if (obj.holders[i].right !== undefined) {
            obj.holders[i].right.attr({
                x: obj.holders[i].right.attr('x') + delta.x,
                y: obj.holders[i].right.attr('y') + delta.y
            });
        }
    }
    // entity
    obj.entity.attr({
        x: obj.entity.attr('x') + delta.x,
        y: obj.entity.attr('y') + delta.y
    });
    // icon
    obj.icon.attr({
        x: obj.icon.attr('x') + delta.x,
        y: obj.icon.attr('y') + delta.y
    });
    //label
    obj.label.attr({
        x: obj.label.attr('x') + delta.x,
        y: obj.label.attr('y') + delta.y
    });
    // mask
    obj.attr({
        x: obj.attr('x') + delta.x,
        y: obj.attr('y') + delta.y
    });

    invalidateConnections();
    increaseViewport(obj);
    callback();
}

// fn delete connection dummy
function deleteConnectionDummy() {
    mRoot.objConnections.forEach(function (conn) {
        if (conn.info.to == mDummy.id || conn.info.from == mDummy.id) {
            viewport.removeConnection(conn.id);
        }
    });
}

// fn edit label
function editLabel() {
    var item = mSelectedItems.items.collection[0];
    // flowchart
    if (item.shapeType == shapeTypes.entity) {
        if (item.info.type == typeEntity.t_generic || item.info.type == typeEntity.t_approve) {
            var entity = mRoot.infoEntities.itemByID(item.id);
            // entity description
            entity.annotation = annotation.val().replaceAll("\"", "&quot;");
            item.info.annotation = annotation.val().replaceAll("\"", "&quot;");
            // entity label
            entity.label = editor.val().replaceAll("\"", "&quot;");
            item.info.label = editor.val().replaceAll("\"", "&quot;");
            item.label.attr({ 'text': editor.val() });

            // draw text temp for edit
            var temp = viewport.text(-100, -100, editor.val()).attr({ 'font-size': '12px' });
            var wNew = temp.getBBox().width;

            // greater
            if ((defaultValue.margin * 2 + defaultValue.iconSizeFlow.width + wNew > item.entity.attr('width')) ||
                (defaultValue.margin * 2 + defaultValue.iconSizeFlow.width + wNew < item.entity.attr('width')
                && defaultValue.margin * 2 + defaultValue.iconSizeFlow.width + wNew > defaultValue.flowchart.width)
            ) {
                item.entity.attr({
                    width: defaultValue.margin * 3 + defaultValue.iconSizeFlow.width + wNew
                });
                item.attr({
                    width: defaultValue.margin * 4 + defaultValue.iconSizeFlow.width + wNew
                });
            } else {
                item.entity.attr({
                    width: defaultValue.flowchart.width
                });
                item.attr({
                    width: defaultValue.flowchart.width + defaultValue.margin * 2
                });
            }
            // holders
            for (var i = 0; i < item.holders.length; i++) {
                var holder = item.holders[i];
                if (holder.type === 'top' || holder.type === 'bottom') {
                    holder.attr({
                        x: item.entity.attr('x') + item.entity.attr('width') / 2 - defaultValue.holder.width / 2
                    })
                }
                if (holder.type === 'right') {
                    holder.attr({
                        x: item.entity.attr('x') + item.entity.attr('width')
                    })
                }
            }
        } else if (item.info.type == typeEntity.t_start) { // start
            var entity = mRoot.infoEntities.itemByID(item.id);
            entity.annotation = annotation.val().replaceAll("\"", "&quot;");
            item.info.annotation = annotation.val().replaceAll("\"", "&quot;");
            return;
        } else { // decistion/switch
            // edit label info entity
            var entity = mRoot.infoEntities.itemByID(item.id);
            if (item.info.type === typeEntity.t_decision) {
                entity.condition = condition.val().replaceAll("\"", "&quot;");
            }
            if (item.info.type === typeEntity.t_switch) {
                entity.expression = expression.val().replaceAll("\"", "&quot;");
            }
            entity.label = editor.val().replaceAll("\"", "&quot;");
            entity.annotation = annotation.val().replaceAll("\"", "&quot;");

            // edit entity
            var temp = viewport.text(-100, -100, editor.val()).attr({ 'font-size': '12px' });
            var wNew = temp.getBBox().width;

            // update connection
            item.label.attr({ 'text': editor.val() });  // draw label
            item.info.label = editor.val().replaceAll("\"", "&quot;");             // edit label
            item.info.annotation = annotation.val().replaceAll("\"", "&quot;");  // edit annotation

            // if new width > current width
            if (wNew + defaultValue.margin * 2 > item.entity.attr('width') ||
                wNew + defaultValue.margin * 2 < item.entity.attr('width') && wNew + defaultValue.margin * 2 > defaultValue.entity.width) {
                var midEntity = {
                    x: item.entity.attr('x') + item.entity.attr('width') / 2,
                    y: item.entity.attr('y')
                }

                // repair entity>entity
                item.entity.attr({
                    x: midEntity.x - (wNew + defaultValue.margin * 2) / 2,
                    width: wNew + defaultValue.margin * 2
                })
                // repair entity>mask
                item.attr({
                    x: midEntity.x - wNew / 2 - defaultValue.holder.height * 2,
                    width: wNew + defaultValue.holder.height * 4
                });
            } else if (wNew + defaultValue.margin * 2 < defaultValue.entity.width && wNew + defaultValue.margin * 2 < item.entity.attr('width')) {
                var midEntity = {
                    x: item.entity.attr('x') + item.entity.attr('width') / 2,
                    y: item.entity.attr('y')
                }

                // repair entity>entity
                item.entity.attr({
                    x: midEntity.x - defaultValue.entity.width / 2,
                    width: defaultValue.entity.width
                })
                // repair entity>mask
                item.attr({
                    x: midEntity.x - defaultValue.entity.width / 2 - defaultValue.holder.height,
                    width: defaultValue.entity.width + defaultValue.holder.height * 2
                });
            }

            for (var i = 0; i < item.holders.length; i++) {
                // holder right
                if (item.holders[i].type === 'right') {
                    item.holders[i].attr({ // right
                        x: item.entity.attr('x') + item.entity.attr('width')
                    });
                    if (item.holders[i].right !== undefined) {
                        item.holders[i].right.attr({ // fa;se
                            x: item.entity.attr('x') + item.entity.attr('width') + defaultValue.margin * 2
                        });
                    }
                }
                // holder left
                if (item.holders[i].type === 'left') {
                    item.holders[i].attr({ // left
                        x: item.entity.attr('x') - defaultValue.margin
                    });
                    if (item.holders[i].left !== undefined) {
                        item.holders[i].left.attr({ // true
                            x: item.entity.attr('x') - defaultValue.margin * 2
                        });
                    }
                }
                if (item.holders[i].type === 'top' || item.holders[i].type === 'bottom') {
                    // left
                    if (item.holders[i].id === 11 || item.holders[i].id === 31) {
                        item.holders[i].attr({
                            x: item.entity.attr('x') + item.entity.attr('width') / 4 - defaultValue.margin
                        });
                    }
                    // right
                    if (item.holders[i].id === 12 || item.holders[i].id === 32) {
                        item.holders[i].attr({
                            x: item.entity.attr('x') + item.entity.attr('width') / 4 * 3 - defaultValue.margin
                        });
                    }
                }
            }
            updateInfo(item);

            temp.remove();
        }
    }

    //edit label connection
    if (item.shapeType == shapeTypes.connection) {
        var connection = mRoot.infoConnections.itemByID(item.id);
        connection.label = editor.val();

        // edit obj connection
        item.label.text.attr({ 'text': editor.val() });
        item.info.label = editor.val();
        //item.info.description = description.val();

        var posLabel = {
            x: item.label.text.attr('x'),
            y: item.label.text.attr('y')
        };
        // edit connection
        var temp = viewport.text(-100, -100, editor.val()).attr({ 'font-size': '10px' });
        var wNew = temp.getBBox().width;

        // repair backgroundd
        item.label.background.attr({
            x: posLabel.x - (wNew + defaultValue.margin * 2) / 2,
            width: wNew + defaultValue.margin * 2
        });
        // repair mask
        item.label.mask.attr({
            x: posLabel.x - (wNew + defaultValue.margin * 2) / 2,
            width: wNew + defaultValue.margin * 2
        });

        temp.remove();
    }
    invalidateConnections();
}

// fn update info value in entity/connection
function updateInfo(obj) {
    // entity
    if (obj.shapeType == shapeTypes.entity) {
        var info = mRoot.infoEntities.itemByID(obj.id);
        info.x = obj.entity.attr('x');
        info.y = obj.entity.attr('y');
        info.width = obj.entity.attr('width');
        info.height = obj.entity.attr('height');
        info.label = obj.label.attr('text').replaceAll("\"", "&quot;");
    }

    // connection
    if (obj.shapeType == shapeTypes.connection) {
    }
}

// check entity start have a connection
function isStartOnlyConnection() {
    for (var i = 0; i < mRoot.infoConnections.count; i++) {
        var conn = mRoot.infoConnections.collection[i];
        for (var j = 0; j < mRoot.infoEntities.count; j++) {
            var entity = mRoot.infoEntities.collection[j];
            if (entity.type == typeEntity.t_start && (entity.id == conn.from || entity.id == conn.to)) {
                return true;
            }
        }
    };
    return false;
}

// check entity generic/approve have a connection from itself
function isAppGenOnlyConnection(obj) { // obj is a generic/approve
    for (var i = 0; i < mRoot.infoConnections.count; i++) {
        var conn = mRoot.infoConnections.collection[i];
        if (obj.info.type === typeEntity.t_generic || obj.info.type === typeEntity.t_approve) {
            if (obj.id === conn.from) {
                return true;
            }
        }
    }
    return false;
}

// check entity decision have connection from true/false
function isDecisionConnection(obj) {
    var array = new Array();
    for (var i = 0; i < mRoot.infoConnections.count; i++) {
        var conn = mRoot.infoConnections.collection[i];
        if (obj.info.type === typeEntity.t_decision && conn.from === obj.id) {
            array.push(conn.fromHolder);
        }
    }
    return array;
}


// fn increase size of viewport
var scrollbar = null;
function increaseViewport() {
    var newSize = {
        w: 0,
        h: 0
    }
    mRoot.objEntities.forEach(function (item) {
        if (newSize.w < item.entity.attr('x') + item.entity.attr('width') + defaultValue.margin * 2) {
            newSize.w = item.entity.attr('x') + item.entity.attr('width') + defaultValue.margin * 2;
        }
        if (newSize.h < item.entity.attr('y') + item.entity.attr('height') + defaultValue.margin * 2) {
            newSize.h = item.entity.attr('y') + item.entity.attr('height') + defaultValue.margin * 2;
        }
    });


    if (mViewport.width < newSize.w) {
        mViewport.width = newSize.w;
    }
    if (mViewport.height < newSize.h) {
        mViewport.height = newSize.h;
    }

    if (scrollbar !== null) {
        scrollbar.destroy();
    }

    $('#viewport').width(mViewport.width);
    $('#viewport').height(mViewport.height);
    viewport.setSize(mViewport.width, mViewport.height);
    scrollbar = $('#chart').jScrollPane().data().jsp;
}

// fn set limit
function getLimited() {
    var limit = { x: 0, y: 0 };
    mRoot.objEntities.forEach(function (item) {
        // width
        if (limit.x < item.entity.attr('x') + item.entity.attr('width') + defaultValue.margin * 2) {
            limit.x = item.entity.attr('x') + item.entity.attr('width') + defaultValue.margin * 2;
        }
        // height
        if (limit.y < item.entity.attr('y') + item.entity.attr('height') + defaultValue.margin * 2) {
            limit.y = item.entity.attr('y') + item.entity.attr('height') + defaultValue.margin * 2
        }
    });
    return limit;
}

// fn check switch
function isSwitch(obj) {
    var flag = false;
    mRoot.infoEntities.forEach(function (item) {
        if (obj == item.id && item.type == typeEntity.t_switch) {
            flag = true;
        }
    });
    return flag;
}

function getLabelSwitch(connection) {
    var label = "";
    var flag = false;
    for (var i = 0; i < mRoot.infoEntities.count; i++) {
        var entity = mRoot.infoEntities.collection[i];
        if (entity.id === connection.from && entity.type === typeEntity.t_switch) {
            if (entity.typeSwitch.toLowerCase() === "int32") {
                flag = false;
            } else if (entity.typeSwitch.toLowerCase() === "string") {
                flag = true;
            }
            label = -1;
            while (checkExisting(label, entity)) {
                label++;
            }

            if (label === -1) {
                return "Default";
            }
            if (!flag) {
                return label.toString();
            } else {
                return "Case" + label.toString();
            }
        }
    }
    return "";

    function checkExisting(label, entity) {
        var connections = new Array();
        for (var i = 0; i < mRoot.infoConnections.count; i++) {
            var connect = mRoot.infoConnections.collection[i];
            if (entity.id === connect.from) {
                connections.push(connect);
            }
        }

        for (var i = 0; i < connections.length; i++) {
            var connect = connections[i];
            if (connect.isDefault !== undefined && connect.isDefault && label === -1) {
                return true;
            } else {
                // int32
                if (entity.typeSwitch.toLowerCase() === "int32") {
                    if (label === parseInt(connect.label)) {
                        return true;
                    }
                }
                // string
                if (entity.typeSwitch.toLowerCase() === "string") {
                    if (("Case" + label).toLowerCase() === connect.label.toLowerCase()) {
                        return true;
                    }
                }
            }
        }
        return false;
    }
}