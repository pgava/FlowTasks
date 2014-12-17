/*========= object controller ==========*/

// base object
function BaseEntity(label) {
    this.id = -1;
    this.label = label || "";
}

// convert string value of object
BaseEntity.prototype.toString = function () {
    return this.x + " - " + this.y + ": " + this.label;
}

/* ============== workflow objects ============= */

/*--------- entity --------*/
Entity.prototype = new BaseEntity();
Entity.prototype.constructor = Entity;
function Entity(label, icon) {
    BaseEntity.call(this);
    this.label = label != undefined ? label : 'Default';
    this.annotation = '';

    this.x = 0;
    this.y = 0;

    this.width = defaultValue.entity.width;
    this.height = defaultValue.entity.height;

    this.dx = 0;
    this.dy = 0;

    this.shape = shapeTypes.entity;
}

/*--------- connection --------*/
Connection.prototype = new BaseEntity();
Connection.prototype.constructor = Connection;
Connection.prototype.clone = clone;
Connection.prototype.remove = Connection;
function Connection(label) {
    this.label = label != undefined ? label : 'Default';
    this.description = '';

    this.from = null;
    this.to = null;
    this.fromHolder = null;
    this.toHolder = null;

    this.shape = shapeTypes.connection;
}

function clone() {
    var res = new Connection();
    res.id = this.id;
    res.label = this.label;
    res.from = this.from;
    res.to = this.to;
    res.fromHolder = this.fromHolder;
    res.toHolder = this.toHolder;
    res.shape = this.shape;
    return res;
}

/*--------- collection ------------*/
function Collection() {
    this.collection = new Array();
    this.count = 0;

    // add item
    this.add = function (item, isKey) {
        if(isKey || isKey == undefined){
            item.id = this.getAutoKey();
        }
        // push item into array
        this.collection.push(item);

        this.count++;
        return this.count;
    };

    // update item
    this.change = function (newVal) {
        for (var i = 0; i < this.count; i++) {
            if (this.collection[i].id == newVal.id) {
                this.collection[i] = newVal;
                return true;
            }
        }
        return false;
    }

    // remove item
    this.remove = function (item) {
        for (var i = 0; i < this.count; i++) {
            var obj = this.collection[i];
            if (item.id == obj.id) {
                this.collection.splice(i, 1);
                this.count--;
                return true;
            }
        }
        return false;
    };

    // get item
    this.item = function (index) {
        if (this.collection[index] == undefined) {
            return -1;
        }
        return this.collection[index];
    };

    // get item by id
    this.itemByID = function (id) {
        for (var i = 0; i < this.count; i++) {
            if (this.collection[i].id == id) {
                return this.collection[i];
            }
        }
        return -1;
    };

    // get item by name
    this.itemByName = function (name) {
        for (var i = 0; i < this.count; i++) {
            if (this.collection[i].name == name) {
                return this.collection[i];
            }
        }
        return -1;
    };

    // get auto key
    this.getAutoKey = function () {
        if (this.count <= 0) {
            return 0;
        }
        return this.collection[this.count - 1].id + 1;
    };

    // get each item in array items (loop for - foreach)
    this.forEach = function (callback) {
        if( typeof callback == "function"){
            for (var i = 0; i < this.count; i++) {
                // get callback run with each value
                callback.call(false, this.collection[i], i, this);
            }
        }
    }
}

/*--------- variables ------------*/
Variable.prototype.constructor = Variable;
function Variable() {
    this.id = -1;
    this.name = 'Res';
    this.type = 'Integer';
    this.value = "Flowchart";
}