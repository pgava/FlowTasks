// default setting of text/label
var textSettings = {
    labelSettings: { "font-size": "12px", "font-weight": "bold", "text-anchor": "start", fill: "#ffffff" },
    labelToolSetting: { "font-size": "12px", "font-weight": "bold", "text-anchor": "start", fill: "#333333" },
    labelToolHover: { "font-size": "12px", "font-weight": "bold", "text-anchor": "start", fill: "#000000" },
    labelEntitySetting: { "font-size": "12px", "font-weight": "normal", "text-anchor": "middle", fill: "#1C1C1C" },
    labelEntityFlowSetting: { "font-size": "12px", "font-weight": "normal", "text-anchor": "start", fill: "#1C1C1C" }
};

// default settings of entity
var entitySettings = {
    entityNormal: { fill: "#FFFFFF", "fill-opacity": 0.01, stroke: "#454545" },
    entityHover: { fill: "#FFFFFF", "fill-opacity": 0.01, stroke: "#00D0FF" },
    entitySelected: { fill: "#FFFFFF", "fill-opacity": 0.01, stroke: "#00D0FF" },
    entityMask: { fill: "#FFFFFF", "fill-opacity": 0.01, stroke: "#FFFFFF", "stroke-opacity": 0.01, "-ms-filter": "progid:DXImageTransform.Microsoft.Alpha(Opacity=1)", "filter": "alpha(opacity=1)", "-khtml-opacity": 0.01, "-moz-opacity": 0.01 }
};

// default settings of holder
var holderSettings = {
    holderNormal: { fill: "#FFFFFF", "fill-opacity": 0.01, stroke: "#454545", "stroke-opacity": 1 },
    holderHover: { fill: "#FFFFFF", "fill-opacity": 0.01, stroke: "#00D0FF", "stroke-opacity": 1 },
    holderSelected: { fill: "#FFFFFF", "fill-opacity": 0.01, stroke: "#00D0FF", "stroke-opacity": 1 },
    holderHide: { fill: "#FFFFFF", "fill-opacity": 0.01, stroke: "#FFFFFF", "stroke-opacity": 0.01 }
};

// default settings of line (connection)
var connectionSettings = {
    connectionNormal: { stroke: "#454545", "stroke-opacity": 1, "stroke-width": 1 },
    connectionHover: { stroke: "#00D0FF", "stroke-opacity": 1, "stroke-width": 1 },
    connectionSelected: { stroke: "#00D0FF", "stroke-opacity": 1, "stroke-width": 1 },
    connectionMask: { stroke: "#FFFFFF", "stroke-opacity": 0.01, "stroke-width": 5 },

    // arrow
    arrowNormal: { fill: "#454545", stroke: "#454545", "stroke-opacity": 1, "stroke-width": 1 },
    arrowHover: { fill: "#454545", stroke: "#00D0FF", "stroke-opacity": 1, "stroke-width": 1 },
    arrowSelected: { fill: "#454545", stroke: "#00D0FF", "stroke-opacity": 1, "stroke-width": 1 },

    // label
    labelNormal: { fill: "#FFFFFF", stroke: "#454545" },
    labelSelected: { fill: "#FFFFFF", stroke: "#00D0FF" }
};

// default settings of breakpoint
var breakpointSettings = {
    radius: 3,
    breakpointNormal: { fill: "#FFFFFF", "fill-opacity": 1, stroke: "#00D0FF", "stroke-opacity": 1 }
};

// shape types 
var shapeTypes = {
    label: 'label',
    connection: 'connection',
    entity: 'entity',
    holder: 'holder',
    breakpoint: 'breakpoint'
};

// function Raphael, move to
Raphael.el.moveTo = function (x, y) {
    if (this.shapeType === shapeTypes.entity) {
        var dx = x - this.x,
            dy = y - this.y;
        // move mask
        this.attr({
            x: x,
            y: y
        });
        // move entity
        this.entity.attr({
            x: this.entity.x + dx,
            y: this.entity.y + dy
        });
        this.icon.attr({
            x: this.icon.x + dx,
            y: this.icon.y + dy
        });
        // move label
        this.label.attr({
            x: this.label.x + dx,
            y: this.label.y + dy
        });
        // move holders
        for (var i = 0; i < this.holders.length; i++) {
            var holder = this.holders[i];
            holder.attr({ x: holder.x + dx, y: holder.y + dy });
            if (holder.left !== undefined) {
                holder.left.attr({ x: holder.left.x + dx, y: holder.left.y + dy });
            }
            if (holder.right !== undefined) {
                holder.right.attr({ x: holder.right.x + dx, y: holder.right.y + dy });
            }
        }
    }
}

// function Raphael, create a connection 
Raphael.fn.connection = function (start, end, info, connection) {
    var pairDistance = 20;

    if (start.line && start.from && start.to) {
        connection = start;
        info = connection.info;
        start = connection.from;
        end = connection.to;
        
    }

    // fn get point
    function getPoint(obj, holder) {
        var x = obj.attr('x'),
            y = obj.attr('y');
        var w = obj.attr('width'),
            h = obj.attr('height');

        var p = { x: 0, y: 0 };
        switch (holder) {
            case -1: // default is top
                p = { x: x + w / 2, y: y };
                if (info.fromHolder == -1) {
                    info.fromHolder = 1;
                } else if (info.toHolder == -1) {
                    info.toHolder = 1;
                }
                break;
            case 1: // top
                p = { x: x + w / 2, y: y };
                break;
            case 11: // top 1
                p = { x: x + w / 4, y: y };
                break;
            case 12: // top 2
                p = { x: x + w / 4 * 3, y: y };
                break;
            case 2: // right
                p = { x: x + w, y: y + h / 2 };
                break;
            case 21: // right 1
                p = { x: x + w, y: y + h / 4 };
                break;
            case 22:
                p = { x: x + w, y: y + h / 4 * 3 };
                break;
            case 3: // bottom
                p = { x: x + w / 2, y: y + h };
                break;
            case 31: // bottom 1
                p = { x: x + w / 4, y: y + h };
                break;
            case 32: // bottom 2
                p = { x: x + w / 4 * 3, y: y + h };
                break;
            case 4: // left
                p = { x: x, y: y + h / 2 };
                break;
            case 41:
                p = { x: x, y: y + h / 4 };
                break;
            case 42:
                p = { x: x, y: y + h / 4 * 3};
                break;
            default:
                p = { x: x + w / 2, y: y };
                break;
        }
        return p;
    }

    // fn get pair point
    function getPairPoint(obj, holder) {
        var x = obj.attr('x'),
            y = obj.attr('y');
        var w = obj.attr('width'),
            h = obj.attr('height');

        var p = { x: 0, y: 0 };
        switch (holder) {
            case -1: // default is top
                p = { x: x + w / 2, y: y - pairDistance };
                if (info.fromHolder == -1) {
                    info.fromHolder = 1;
                } else if (info.toHolder == -1) {
                    info.toHolder = 1;
                }
                break;
            case 1: // top
                p = { x: x + w / 2, y: y - pairDistance };
                break;
            case 11: // top 1
                p = { x: x + w / 4, y: y - pairDistance };
                break;
            case 12: // top 2
                p = { x: x + w / 4 * 3, y: y - pairDistance };
                break;
            case 2: // right
                p = { x: x + w + pairDistance, y: y + h / 2 };
                break;
            case 21: // right 1
                p = { x: x + w + pairDistance, y: y + h / 4 };
                break;
            case 22:
                p = { x: x + w + pairDistance, y: y + h / 4 * 3 };
                break;
            case 3: // bottom
                p = { x: x + w / 2, y: y + h + pairDistance };
                break;
            case 31: // bottom 1
                p = { x: x + w / 4, y: y + h + pairDistance };
                break;
            case 32: // bottom 2
                p = { x: x + w / 4 * 3, y: y + h + pairDistance };
                break;
            case 4: // left
                p = { x: x - pairDistance, y: y + h / 2 };
                break;
            case 41: // left 1
                p = { x: x - pairDistance, y: y + h / 4 };
                break;
            case 42: // left 2
                p = { x: x - pairDistance, y: y + h / 4 * 3 };
                break;
        }
        return p;
    }

    //get point to draw
    var point = {
        start: getPoint(start.entity, info.fromHolder),
        end: getPoint(end.entity, info.toHolder),
        pstart: getPairPoint(start.entity, info.fromHolder),
        pend: getPairPoint(end.entity, info.toHolder)
    };

    var pTemp = { x: 0, y: 0 };
    var arrow = {
        type: '',
        path: '',
        tangle: 5
    };

    var obj1 = {
        x: start.entity.attr('x'),
        y: start.entity.attr('y'),
        w: start.entity.attr('width'),
        h: start.entity.attr('height')
    }, obj2 = {
        x: end.entity.attr('x'),
        y: end.entity.attr('y'),
        w: end.entity.attr('width'),
        h: end.entity.attr('height')
    }

    var posLabel = {
        x: 0,
        y: 0
    };
    // calculate points to draw
    // top in left-middle-right
    if(info.fromHolder === 1 || info.fromHolder === 11 || info.fromHolder === 12){
        // top
        if(info.toHolder === 1 || info.toHolder === 11 || info.toHolder === 12){ 
            // start in left
            if (point.start.x < point.end.x){
                // top
                if (point.start.y < point.end.y) {
                    point.p1 = point.p2 = {
                        x: point.pend.x,
                        y: point.pstart.y
                    };
                    posLabel = {
                        x: (point.pend.x + point.pstart.x) / 2,
                        y: point.pstart.y
                    };
                } else { // bottom
                    point.p1 = point.p2 = {
                        x: point.pstart.x,
                        y: point.pend.y
                    };
                    posLabel = {
                        x: (point.pend.x + point.pstart.x) / 2,
                        y: point.pend.y
                    };
                }
            } else { // start in right
                // top
                if (point.start.y < point.end.y) {
                    point.p1 = point.p2 = {
                        x: point.pend.x,
                        y: point.pstart.y
                    };
                    posLabel = {
                        x: (point.pstart.x + point.pend.x) / 2,
                        y: point.pstart.y
                    };
                } else { // botom
                    point.p1 = point.p2 = {
                        x: point.pstart.x,
                        y: point.pend.y
                    };
                    posLabel = {
                        x: (point.pend.x + point.pstart.x) / 2,
                        y: point.pend.y
                    };
                }
            }
            arrow.type = 'bottom';
        }
        // right
        if (info.toHolder === 2 || info.toHolder === 21 || info.toHolder === 22){ 
            // start in left
            if (point.start.x < point.end.x) {
                // top
                if (point.start.y < point.end.y) {
                    // from x,y enity to holder
                    if (point.pstart.y > obj2.y - pairDistance) {
                        point.p1 = {
                            x: point.pstart.x,
                            y: obj2.y - pairDistance
                        };
                        point.p2 = {
                            x: point.pend.x,
                            y: obj2.y - pairDistance
                        };
                        posLabel = {
                            x: (point.pend.x + point.pstart.x) / 2,
                            y: obj2.y - pairDistance
                        };
                    } else { // over
                        point.p1 = point.p2 = {
                            x: point.pend.x,
                            y: point.pstart.y
                        };
                        posLabel = {
                            x: (point.pend.x + point.pstart.x) / 2,
                            y: point.pstart.y
                        };
                    }
                } else { // bottom
                    // from holder to over bottom of entity
                    if (point.pstart.y < obj2.y + obj2.h) {
                        point.p1 = {
                            x: point.pstart.x,
                            y: obj2.y - pairDistance
                        };
                        point.p2 = {
                            x: point.pend.x,
                            y: obj2.y - pairDistance
                        };
                        posLabel = {
                            x: (point.pend.x + point.pstart.x) / 2,
                            y: obj2.y - pairDistance
                        };
                    } else {
                        point.p1 = point.p2 = {
                            x: point.pend.x,
                            y: point.pstart.y
                        };
                        posLabel = {
                            x: (point.pend.x + point.pstart.x) / 2,
                            y: point.pstart.y
                        };
                    }
                }
            } else { // start in right
                // top
                if (point.start.y < point.end.y) {
                    point.p1 = point.p2 = {
                        x: point.pend.x,
                        y: point.pstart.y
                    };
                    posLabel = {
                        x: (point.pstart.x + point.pend.x) / 2,
                        y: point.pstart.y
                    };
                } else { // bottom
                    if (point.end.y > point.pstart.y && point.end.y < point.start.y) {
                        point.p1 = point.p2 = {
                            x: point.pend.x,
                            y: point.pstart.y
                        };
                        posLabel = {
                            x: (point.pstart.x - point.pend.x) / 2,
                            y: point.pstart.y
                        };
                    } else {
                        point.p1 = point.p2 = {
                            x: point.pstart.x,
                            y: point.pend.y
                        };
                        posLabel = {
                            x: (point.pstart.x + point.pend.x) / 2,
                            y: point.pend.y
                        };
                    }
                }
            }
            arrow.type = 'left';
        }
        // bottom
        if (info.toHolder === 3 || info.toHolder === 31 || info.toHolder === 32){
            // start in left
            if (point.start.x < point.end.x) {
                // top
                if (point.start.y < point.end.y) {
                    if (obj1.x + obj1.w < obj2.x - pairDistance) {
                        point.p1 = {
                            x: obj2.x - pairDistance,
                            y: point.pstart.y
                        };
                        point.p2 = {
                            x: obj2.x - pairDistance,
                            y: point.pend.y
                        };
                        posLabel = {
                            x: (point.pend.x + point.pstart.x) / 2,
                            y: point.pstart.y
                        };                                
                    } else {
                        point.p1 = {
                            x: obj1.x - pairDistance,
                            y: point.pstart.y
                        };
                        point.p2 = {
                            x: obj1.x - pairDistance,
                            y: point.pend.y
                        };
                        posLabel = {
                            x: point.p1.x,
                            y: (point.p1.y + point.p2.y) / 2
                        };
                    }
                    
                } else { // bottom
                    if (point.pend.y < point.pstart.y) {
                        point.p1 = point.p2 = {
                            x: point.pstart.x,
                            y: point.pend.y
                        }
                        posLabel = {
                            x: (point.pend.x + point.pstart.x) / 2,
                            y: point.pend.y
                        };
                    } else {
                        point.p2 = {
                            x: obj2.x - pairDistance,
                            y: point.pend.y
                        };
                        point.p1 = {
                            x: point.p2.x,
                            y: point.pstart.y
                        };
                        posLabel = {
                            x: (point.pend.x + point.pstart.x) / 2,
                            y: point.pstart.y
                        };
                    }
                }
            } else { // start in right
                // top
                if (point.start.y < point.end.y) {
                    if (obj2.x + obj2.w + pairDistance > obj1.x) {
                        point.p1 = {
                            x: obj2.x + obj2.w + pairDistance,
                            y: point.pstart.y
                        };
                        point.p2 = {
                            x: obj2.x + obj2.w + pairDistance,
                            y: point.pend.y
                        };
                        posLabel = {
                            x: (point.pstart.x + point.pend.x) / 2,
                            y: point.pend.y
                        };
                    } else {
                        point.p1 = {
                            x: obj2.x + obj2.w + pairDistance,
                            y: point.pstart.y
                        };
                        point.p2 = {
                            x: obj2.x + obj2.w + pairDistance,
                            y: point.pend.y
                        };
                        posLabel = {
                            x: (point.pstart.x + point.pend.x) / 2,
                            y: point.pstart.y
                        };
                    }

                } else { // bottom
                    if (point.pend.y < point.pstart.y) {
                        point.p1 = point.p2 = {
                            x: point.pstart.x,
                            y: point.pend.y
                        }
                        posLabel = {
                            x: (point.pend.x + point.pstart.x) / 2,
                            y: point.pend.y
                        };
                    } else {
                        point.p2 = {
                            x: obj2.x + obj2.w + pairDistance,
                            y: point.pend.y
                        };
                        point.p1 = {
                            x: point.p2.x,
                            y: point.pstart.y
                        };
                        posLabel = {
                            x: (point.pend.x + point.pstart.x) / 2,
                            y: point.pstart.y
                        };
                    }
                }
            }
            arrow.type = 'top';
        }
        // left
        if (info.toHolder === 4 || info.toHolder === 41 || info.toHolder === 42){
            // start in left
            if (point.start.x < point.end.x) {
                // top
                if (point.start.y < point.end.y) {
                    point.p1 = point.p2 = {
                        x: point.pend.x,
                        y: point.pstart.y
                    };
                    posLabel = {
                        x: (point.pend.x + point.pstart.x) / 2,
                        y: point.pstart.y
                    };
                } else { // bottom
                    if (point.start.y > point.end.y && point.end.y > point.pstart.y) {
                        point.p1 = point.p2 = {
                            x: point.pend.x,
                            y: point.pstart.y
                        };
                        posLabel = {
                            x: (point.pend.x + point.pstart.x) / 2,
                            y: point.pstart.y
                        };
                    } else {
                        point.p1 = point.p2 = {
                            x: point.start.x,
                            y: point.pend.y
                        };
                        posLabel = {
                            x: (point.pend.x + point.pstart.x) / 2,
                            y: point.pend.y
                        };
                    }
                }
            } else { // start in right
                // top
                if (point.start.y < point.end.y) {
                    var y = end.entity.attr('y');
                    // from x,y enity to holder
                    if (point.pstart.y > obj2.y) {
                        point.p1 = {
                            x: point.pstart.x,
                            y: obj2.y - pairDistance
                        };
                        point.p2 = {
                            x: point.pend.x,
                            y: obj2.y - pairDistance
                        };
                        posLabel = {
                            x: (point.pend.x + point.pstart.x) / 2,
                            y: obj2.y - pairDistance
                        };
                    } else { // over
                        point.p1 = point.p2 = {
                            x: point.pend.x,
                            y: point.pstart.y
                        };
                        posLabel = {
                            x: (point.pend.x + point.pstart.x) / 2,
                            y: point.pstart.y
                        };
                    }
                } else { // bottom
                    // from holder to over bottom of entity
                    if (point.pstart.y < obj2.y + obj2.h) {
                        point.p1 = {
                            x: point.pstart.x,
                            y: obj2.y - pairDistance
                        };
                        point.p2 = {
                            x: point.pend.x,
                            y: obj2.y - pairDistance
                        };
                        posLabel = {
                            x: (point.pend.x + point.pstart.x) / 2,
                            y: obj2.y - pairDistance
                        };
                    } else {
                        point.p1 = point.p2 = {
                            x: point.pend.x,
                            y: point.pstart.y
                        };
                        posLabel = {
                            x: (point.pend.x + point.pstart.x) / 2,
                            y: point.pstart.y
                        };
                    }
                }
            }
            arrow.type = 'right';
        }
    }
    // right head-center-under
    if (info.fromHolder === 2 || info.fromHolder === 21 || info.fromHolder === 22){
        // top
        if(info.toHolder === 1 || info.toHolder === 11 || info.toHolder === 12){
            // start in left
            if (point.start.x < point.end.x) {
                // top
                if (point.start.y < point.end.y) {
                    if (point.pstart.y < point.pend.y) {
                        if(point.pstart.x > point.pend.x){
                            point.p1 = point.p2 = {
                                x: point.pstart.x,
                                y: point.pend.y
                            }   
                        } else {
                            point.p1 = point.p2 = {
                                x: point.pend.x,
                                y: point.pstart.y
                            };
                        }
                        posLabel = {
                            x: (point.pend.x + point.pstart.x) / 2,
                            y: point.pstart.y
                        };    
                    } else {
                        point.p1 = point.p2 = {
                            x: point.pstart.x,
                            y: point.pend.y
                        };
                        posLabel = {
                            x: (point.pend.x + point.pstart.x) / 2,
                            y: point.pend.y
                        };
                    }
                    
                } else { // bottom
                    point.p1 = point.p2 = {
                        x: point.pstart.x,
                        y: point.pend.y
                    };
                    posLabel = {
                        x: (point.pend.x + point.pstart.x) / 2,
                        y: point.pend.y
                    };
                }
            } else { // start in right
                // top
                if (point.start.y < point.end.y) {
                    point.p1 = {
                        x: point.pstart.x,
                        y: obj1.y - pairDistance
                    };
                    point.p2 = {
                        x: point.pend.x,
                        y: obj1.y - pairDistance
                    }
                    posLabel = {
                        x: (point.pend.x + point.pstart.x) / 2,
                        y: obj1.y - pairDistance
                    };
                } else { // bottom
                    if (obj2.y < point.end.y) {
                        point.p1 = {
                            x: point.pstart.x,
                            y: obj1.y - pairDistance
                        };
                        point.p2 = {
                            x: point.pend.x,
                            y: obj1.y - pairDistance
                        }
                        posLabel = {
                            x: (point.pend.x + point.pstart.x) / 2,
                            y: obj1.y - pairDistance
                        };
                    } else {
                        point.p1 = point.p2 = {
                            x: point.pstart.x,
                            y: point.pend.y
                        };
                        posLabel = {
                            x: (point.pend.x + point.pstart.x) / 2,
                            y: point.pend.y
                        };
                    }
                }
                
            }
            arrow.type = 'bottom';
        }
        // right
        if(info.toHolder === 2 || info.toHolder === 21 || info.toHolder === 22){
            // start in left
            if (point.start.x < point.end.x) {
                // top
                if (point.start.y < point.end.y) {
                    if (point.start.y < obj2.y) {
                        point.p1 = point.p2 = {
                            x: point.pend.x,
                            y: point.pstart.y
                        }
                        posLabel = {
                            x: (point.pstart.x + point.pend.x) / 2,
                            y: point.pstart.y
                        };
                    } else {
                        point.p1 = {
                            x: point.pstart.x,
                            y: obj2.y - pairDistance
                        };
                        point.p2 = {
                            x: point.pend.x,
                            y: obj2.y - pairDistance
                        };
                        posLabel = {
                            x: (point.pstart.x + point.pend.x) / 2,
                            y: obj2.y - pairDistance
                        };
                    }
                } else { // bottom
                    if (point.start.y < obj2.y + obj2.h) {
                        point.p1 = {
                            x: point.pstart.x,
                            y: obj2.y + obj2.h + pairDistance
                        };
                        point.p2 = {
                            x: point.pend.x,
                            y: obj2.y + obj2.h + pairDistance
                        };
                        posLabel = {
                            x: (point.pstart.x + point.pend.x) / 2,
                            y: obj2.y + obj2.h + pairDistance
                        };
                    } else {
                        point.p1 = point.p2 = {
                            x: point.pend.x,
                            y: point.pstart.y
                        };
                        posLabel = {
                            x: (point.pstart.x + point.pend.x) / 2,
                            y: point.pstart.y
                        };
                    }
                }
            } else { // start in right
                // top
                if (point.start.y < point.end.y) {
                    if (obj1.y + obj1.h + pairDistance < point.pend.y) {
                        point.p1 = point.p2 = {
                            x: point.pstart.x,
                            y: point.pend.y
                        };
                        posLabel = {
                            x: (point.pstart.x + point.pend.x) / 2,
                            y: point.pend.y
                        };
                    } else {
                        point.p1 = {
                            x: point.pstart.x,
                            y: obj1.y + obj1.h + pairDistance
                        };
                        point.p2 = {
                            x: point.pend.x,
                            y: obj1.y + obj1.h + pairDistance
                        };
                        posLabel = {
                            x: (point.pstart.x + point.pend.x) / 2,
                            y: obj1.y + obj1.h + pairDistance
                        };
                    }
                } else { // bottom
                    if (point.pend.y < obj1.y - pairDistance) {
                        point.p1 = point.p2 = {
                            x: point.pstart.x,
                            y: point.pend.y
                        };
                        posLabel = {
                            x: (point.pstart.x + point.pend.x) / 2,
                            y: point.pend.y
                        };
                    } else {
                        point.p1 = {
                            x: point.pstart.x,
                            y: obj1.y - pairDistance
                        };
                        point.p2 = {
                            x: point.pend.x,
                            y: obj1.y - pairDistance
                        };
                        posLabel = {
                            x: (point.pstart.x + point.pend.x) / 2,
                            y: obj1.y - pairDistance
                        };
                    }
                }
            }
            arrow.type = "left";
        }
        // bottom
        if(info.toHolder === 3 || info.toHolder === 31 || info.toHolder === 32){
            // start in left
            if (point.start.x < point.end.x) {
                // top
                if (point.start.y < point.end.y) {
                    point.p1 = point.p2 = {
                        x: point.pstart.x,
                        y: point.pend.y
                    };
                    posLabel = {
                        x: (point.pstart.x + point.pend.x) / 2,
                        y: point.pend.y
                    };
                } else { // bottom
                    point.p1 = point.p2 = {
                        x: point.pend.x,
                        y: point.pstart.y
                    };
                    posLabel = {
                        x: (point.pstart.x + point.pend.x) / 2,
                        y: point.pstart.y
                    };
                }                        
            } else { // start in right
                // top
                if (point.start.y < point.end.y) {
                    point.p1 = point.p2 = {
                        x: point.pstart.x,
                        y: point.pend.y
                    };
                    posLabel = {
                        x: (point.pstart.x + point.pend.x) / 2,
                        y: point.pend.y
                    };
                } else { // bottom
                    if(obj1.y - pairDistance < obj2.y + obj2.h + pairDistance){
                        point.p1 = {
                            x: point.pstart.x,
                            y: obj1.y + obj1.h + pairDistance
                        }; 
                        point.p2 = {
                            x: point.pend.x,
                            y: obj1.y + obj1.h + pairDistance
                        };
                        posLabel = {
                        x: (point.pstart.x + point.pend.x) / 2,
                        y: point.p2.y
                    };
                    } else {
                        point.p1 ={
                            x: point.pstart.x,
                            y: obj1.y - pairDistance
                        };
                        point.p2 = {
                            x: point.pend.x,
                            y: obj1.y - pairDistance    
                        };    
                        posLabel = {
                            x: (point.pstart.x + point.pend.x) / 2,
                            y: point.p1.y
                        };
                    }
                }
            }
            arrow.type = 'top';
        }
        // left
        if(info.toHolder === 4 || info.toHolder === 41 || info.toHolder === 42){
            // start in left
            if (point.start.x < point.end.x) {
                // top
                if (point.start.y < point.end.y) {
                    point.p1 = point.p2 = {
                        x: point.pstart.x,
                        y: point.pend.y
                    };
                    posLabel = {
                        x: (point.pstart.x + point.pend.x) / 2,
                        y: point.pend.y
                    };
                } else { // bottom
                    point.p1 = point.p2 = {
                        x: point.pstart.x,
                        y: point.pend.y
                    };
                    posLabel = {
                        x: (point.pstart.x + point.pend.x) / 2,
                        y: point.pend.y
                    };
                }
            } else { // start in right
                // top
                if (point.start.y < point.end.y) {
                    if (obj1.y + obj1.h + pairDistance < obj2.y - pairDistance) {
                        point.p1 = {
                            x: point.pstart.x,
                            y: obj1.y + obj1.h + pairDistance
                        };
                        point.p2 = {
                            x: point.pend.x,
                            y: obj1.y + obj1.h + pairDistance
                        };
                        posLabel = {
                            x: (point.pstart.x + point.pend.x) / 2,
                            y: obj1.y + obj1.h + pairDistance
                        };
                    } else {
                        point.p1 = {
                            x: point.pstart.x,
                            y: obj1.y - pairDistance
                        };
                        point.p2 = {
                            x: point.pend.x,
                            y: obj1.y - pairDistance
                        };
                        posLabel = {
                            x: (point.pstart.x + point.pend.x) / 2,
                            y: obj1.y - pairDistance
                        };
                    }
                } else { // bottom
                    point.p1 = {
                        x: point.pstart.x,
                        y: obj1.y + obj1.h + pairDistance
                    };
                    point.p2 = {
                        x: point.pend.x,
                        y: obj1.y + obj1.h + pairDistance
                    };
                    posLabel = {
                        x: (point.pstart.x + point.pend.x) / 2,
                        y: obj1.y + obj1.h + pairDistance
                    };
                }
            }
            arrow.type = "right";
        }
    }
    // bottom left-middle-right
    if (info.fromHolder === 3 || info.fromHolder === 31 || info.fromHolder === 32){
        // top
        if(info.toHolder === 1 || info.toHolder === 11 || info.toHolder === 12){
            // start in left
            if (point.start.x < point.end.x) {
                // top
                if (point.start.y < point.end.y) {
                    if (point.pstart.y < obj2.y - pairDistance) {
                        point.p1 = point.p2 = {
                            x: point.pstart.x,
                            y: obj2.y - pairDistance
                        };
                        posLabel = {
                            x: (point.pstart.x + point.pend.x) / 2,
                            y: obj2.y - pairDistance
                        };
                    } else {
                        point.p1 = {
                            x: obj1.x + obj1.w + pairDistance,
                            y: point.pstart.y
                        };
                        point.p2 = {
                            x: obj1.x + obj1.w + pairDistance,
                            y: point.pend.y                                    
                        }
                        posLabel = {
                            x: (point.pstart.x + point.pend.x) / 2,
                            y: point.pend.y
                        };
                    }
                } else { // bottom
                    point.p1 = {
                        x: obj1.x + obj1.w + pairDistance,
                        y: point.pstart.y
                    };
                    point.p2 = {
                        x: obj1.x + obj1.w + pairDistance,
                        y: point.pend.y
                    }
                    posLabel = {
                        x: obj1.x + obj1.w + pairDistance,
                        y: (point.p1.y + point.p2.y) / 2
                    };
                }
            } else { // start in right
                // top
                if (point.start.y < point.end.y) {
                    if (obj1.y + obj1.h + pairDistance < obj2.y - pairDistance) {
                        point.p1 = point.p2 = {
                            x: point.pstart.x,
                            y: obj2.y - pairDistance
                        };
                        posLabel = {
                            x: (point.pstart.x + point.pend.x) / 2,
                            y: obj2.y - pairDistance
                        };
                    } else {
                        point.p1 = {
                            x: obj1.x - pairDistance,
                            y: point.pstart.y
                        };
                        point.p2 = {
                            x: obj1.x - pairDistance,
                            y: point.pend.y
                        }
                        posLabel = {
                            x: point.p1.x,
                            y: (point.p1.y + point.p2.y) / 2
                        };
                    }
                } else { // bottom
                    point.p1 = {
                        x: obj1.x - pairDistance,
                        y: point.pstart.y
                    };
                    point.p2 = {
                        x: obj1.x - pairDistance,
                        y: point.pend.y
                    }
                    posLabel = {
                        x: point.p1.x,
                        y: (point.p1.y + point.p2.y) / 2
                    };
                }
            }
            arrow.type = "bottom";
        }
        // right
        if(info.toHolder === 2 || info.toHolder === 21 || info.toHolder === 22){
            // start in left
            if (point.start.x < point.end.x) {
                // top
                if (point.start.y < point.end.y) {
                    if (obj1.y + obj1.h + pairDistance < obj2.y - pairDistance) {
                        point.p1 = {
                            x: point.pstart.x,
                            y: obj2.y - pairDistance
                        };
                        point.p2 = {
                            x: point.pend.x,
                            y: obj2.y - pairDistance
                        };
                        posLabel = {
                            x: (point.pstart.x + point.pend.x) / 2,
                            y: obj2.y - pairDistance
                        };
                    } else {
                        point.p1 = {
                            x: point.pstart.x,
                            y: obj2.y + obj2.h + pairDistance
                        };
                        point.p2 = {
                            x: point.pend.x,
                            y: obj2.y + obj2.h + pairDistance
                        };
                        posLabel = {
                            x: (point.pstart.x + point.pend.x) / 2,
                            y: obj2.y + obj2.h + pairDistance
                        };
                    }
                } else { // bottom
                    point.p1 = point.p2 = {
                        x: point.pend.x,
                        y: point.pstart.y
                    };
                    posLabel = {
                        x: (point.pstart.x + point.pend.x) / 2,
                        y: point.pstart.y
                    };
                }
            } else { // start in right
                // top
                if (point.start.y < point.end.y) {
                    point.p1 = point.p2 = {
                        x: point.pstart.x,
                        y: point.pend.y
                    };
                    posLabel = {
                        x: (point.pstart.x + point.pend.x) / 2,
                        y: point.pend.y
                    };
                } else { // bottom
                    point.p1 = point.p2 = {
                        x: point.pend.x,
                        y: point.pstart.y
                    };
                    posLabel = {
                        x: (point.pstart.x + point.pend.x) / 2,
                        y: point.pstart.y
                    };
                }
            }
            // start in right
            arrow.type = "left";
        }
        // bottom
        if(info.toHolder === 3 || info.toHolder === 31 || info.toHolder === 32){
            // start in left
            if (point.start.x < point.end.x) {
                // top
                if (point.start.y < point.end.y) {
                    point.p1 = point.p2 = {
                        x: point.pstart.x,
                        y: point.pend.y
                    };
                    posLabel = {
                        x: (point.pstart.x + point.pend.x) / 2,
                        y: point.pend.y
                    };
                } else { // bottom
                    point.p1 = point.p2 = {
                        x: point.pend.x,
                        y: point.pstart.y
                    };
                    posLabel = {
                        x: (point.pstart.x + point.pend.x) / 2,
                        y: point.pstart.y
                    };
                }                        
            } else { // start in right
                // top
                if (point.start.y < point.end.y) {
                    point.p1 = point.p2 = {
                        x: point.pstart.x,
                        y: point.pend.y
                    };
                    posLabel = {
                        x: (point.pstart.x + point.pend.x) / 2,
                        y: point.pend.y
                    };
                } else { // bottom
                    point.p1 = point.p2 = {
                        x: point.pend.x,
                        y: point.pstart.y
                    };
                    posLabel = {
                        x: (point.pstart.x + point.pend.x) / 2,
                        y: point.pstart.y
                    };
                }
            }
            arrow.type = 'top';
        }
        // left
        if(info.toHolder === 4 || info.toHolder === 41 || info.toHolder === 42){
            // start in left
            if (point.start.x < point.end.x) {
                // top
                if (point.start.y < point.end.y) {
                    if (point.pstart.y > point.end.y) {
                        point.p1 = {
                            x: obj1.x + obj1.w + pairDistance,
                            y: point.pstart.y
                        };
                        point.p2 = {
                            x: obj1.x + obj1.w + pairDistance,
                            y: point.pend.y
                        };
                        posLabel = {
                            x: (point.pstart.x + point.pend.x) / 2,
                            y: point.pend.y
                        };
                    } else {
                        point.p1 = point.p2 = {
                            x: point.pstart.x,
                            y: point.pend.y
                        };
                        posLabel = {
                            x: (point.pstart.x + point.pend.x) / 2,
                            y: point.pend.y
                        };
                    }
                } else { // bottom
                    point.p1 = point.p2 = {
                        x: obj2.x - pairDistance,
                        y: point.pstart.y
                    }
                    posLabel = {
                        x: (point.pstart.x + point.pend.x) / 2,
                        y: point.pstart.y
                    };
                }
            } else { // start in right
                // top
                if (point.start.y < point.end.y) {
                    if (point.pstart.y < obj2.y - pairDistance) {
                        point.p1 = point.p2 = {
                            x: point.pend.x,
                            y: point.pstart.y
                        };
                        posLabel = {
                            x: (point.pstart.x + point.pend.x) / 2,
                            y: point.pstart.y
                        };
                    } else {
                        point.p1 = {
                            x: point.pstart.x,
                            y: obj2.y + obj2.h + pairDistance
                        };
                        point.p2 = {
                            x: point.pend.x,
                            y: obj2.y + obj2.h + pairDistance
                        };
                        posLabel = {
                            x: (point.pstart.x + point.pend.x) / 2,
                            y: obj2.y + obj2.h + pairDistance
                        };
                    }
                } else { // bottom
                    point.p1 = point.p2 = {
                        x: point.pend.x,
                        y: point.pstart.y
                    };
                    posLabel = {
                        x: (point.pstart.x + point.pend.x) / 2,
                        y: point.pstart.y
                    };
                }
            }
            arrow.type = 'right';
        }
    }
    // left head-center-under
    if (info.fromHolder === 4 || info.fromHolder === 41 || info.fromHolder === 42){
        // top
        if(info.toHolder === 1 || info.toHolder === 11 || info.toHolder === 12){
            // start in left
            if (point.start.x < point.end.x) {
                // top
                if (point.start.y < point.end.y) {
                    if (obj1.y + obj1.h + pairDistance < point.pend.y) {
                        point.p1 = point.p2 = {
                            x: point.pstart.x,
                            y: point.pend.y
                        };
                        posLabel = {
                            x: (point.pstart.x + point.pend.x) / 2,
                            y: point.pend.y
                        };
                    } else {
                        point.p1 = {
                            x: point.pstart.x,
                            y: obj1.y - pairDistance
                        };
                        point.p2 = {
                            x: point.pend.x,
                            y: obj1.y - pairDistance
                        };
                        posLabel = {
                            x: (point.pstart.x + point.pend.x) / 2,
                            y: obj1.y - pairDistance
                        };
                    }
                } else { // bottom
                    point.p1 = point.p2 = {
                        x: point.pstart.x,
                        y: point.pend.y
                    };
                    posLabel = {
                        x: (point.pstart.x + point.pend.x) / 2,
                        y: point.pend.y
                    };
                }
            } else { // start in right
                // top
                if (point.start.y < point.end.y) {
                    if(point.start.y < point.end.y && point.start.y > point.pend.y){
                        point.p1 = point.p2 = {
                            x: point.pstart.x,
                            y: point.pend.y
                        };
                        posLabel = {
                            x: (point.pstart.x + point.pend.x) / 2,
                            y: point.pend.y
                        };
                    } else {
                        if(point.end.x >= point.pstart.x && point.end.x <= point.start.x){
                            point.p1 = point.p2 = {
                                x: point.pstart.x,
                                y: point.pend.y
                            }
                        } else {
                            point.p1 = point.p2 = {
                                x: point.pend.x,
                                y: point.pstart.y
                            };    
                        }
                        posLabel = {
                            x: (point.pstart.x + point.pend.x) / 2,
                            y: point.pstart.y
                        };
                    }
                } else { // bottom
                    point.p1 = point.p2 = {
                        x: obj1.x - pairDistance,
                        y: point.pend.y
                    };
                    posLabel = {
                        x: (point.pstart.x + point.pend.x) / 2,
                        y: point.pend.y
                    };
                }
            }
            arrow.type = "bottom";
        }
        // right
        if(info.toHolder === 2 || info.toHolder === 21 || info.toHolder === 22){
            // start in left
            if (point.start.x < point.end.x) {
                // top
                if (point.start.y < point.end.y) {
                    if (obj1.y + obj1.h + pairDistance < obj2.y - pairDistance) {
                        point.p1 = {
                            x: point.pstart.x,
                            y: obj1.y + obj1.h + pairDistance
                        };
                        point.p2 = {
                            x: point.pend.x,
                            y: obj1.y + obj1.h + pairDistance
                        };
                        posLabel = {
                            x: (point.pstart.x + point.pend.x) / 2,
                            y: obj1.y + obj1.h + pairDistance
                        };
                    } else {
                        point.p1 = {
                            x: point.pstart.x,
                            y: obj1.y - pairDistance
                        };
                        point.p2 = {
                            x: point.pend.x,
                            y: obj1.y - pairDistance
                        };
                        posLabel = {
                            x: (point.pstart.x + point.pend.x) / 2,
                            y: obj1.y - pairDistance
                        };
                    }
                } else { // bottom
                    if (obj2.y + obj2.h + pairDistance < obj1.y + pairDistance) {
                        point.p1 = {
                            x: point.pstart.x,
                            y: obj1.y - pairDistance
                        };
                        point.p2 = {
                            x: point.pend.x,
                            y: obj1.y - pairDistance
                        };
                        posLabel = {
                            x: (point.pstart.x + point.pend.x) / 2,
                            y: obj1.y - pairDistance
                        };
                    } else {
                        point.p1 = {
                            x: point.pstart.x,
                            y: obj1.y + obj1.h + pairDistance
                        };
                        point.p2 = {
                            x: point.pend.x,
                            y: obj1.y + obj1.h + pairDistance
                        };
                        posLabel = {
                            x: (point.pstart.x + point.pend.x) / 2,
                            y: obj1.y + obj1.h + pairDistance
                        };
                    }
                }
            } else { // start in right
                // top
                if (point.start.y < point.end.y) {
                    point.p1 = point.p2 = {
                        x: point.pend.x,
                        y: point.pstart.y
                    };
                    posLabel = {
                        x: (point.pstart.x + point.pend.x) / 2,
                        y: point.pstart.y
                    };
                } else { // bottom
                    point.p1 = point.p2 = {
                        x: point.pend.x,
                        y: point.pstart.y
                    };
                    posLabel = {
                        x: (point.pstart.x + point.pend.x) / 2,
                        y: point.pstart.y
                    };
                }
            }
            arrow.type = "left";
        }
        // bottom
        if(info.toHolder === 3 || info.toHolder === 31 || info.toHolder === 32){
            // start in left
            if (point.start.x < point.end.x) {
                // top
                if (point.start.y < point.end.y) {
                    point.p1 = point.p2 = {
                        x: point.pstart.x,
                        y: point.pend.y
                    };
                    posLabel = {
                        x: (point.pstart.x + point.pend.x) / 2,
                        y: point.pend.y
                    };
                } else { // bottom
                    point.p1 = point.p2 = {
                        x: point.pstart.x,
                        y: point.pend.y
                    };
                    posLabel = {
                        x: (point.pstart.x + point.pend.x) / 2,
                        y: point.pend.y
                    };
                   
                }
            } else { // start in right
                // top
                if (point.start.y < point.end.y) {
                    point.p1 = point.p2 = {
                        x: point.pstart.x,
                        y: point.pend.y
                    };
                    posLabel = {
                        x: (point.pstart.x + point.pend.x) / 2,
                        y: point.pend.y
                    };
                } else { // bottom
                    point.p1 = point.p2 = {
                        x: point.pend.x,
                        y: point.pstart.y
                    };
                    posLabel = {
                        x: (point.pstart.x + point.pend.x) / 2,
                        y: point.pstart.y
                    };
                }
            }
            arrow.type = "top";
        }
        // left
        if(info.toHolder === 4 || info.toHolder === 41 || info.toHolder === 42){
            // start in left
            if (point.start.x < point.end.x) {
                // top
                if (point.start.y < point.end.y) {
                    point.p1 = point.p2 = {
                        x: point.pstart.x,
                        y: point.pend.y
                    };
                    posLabel = {
                        x: (point.pstart.x + point.pend.x) / 2,
                        y: point.pend.y
                    };
                } else { // bottom
                    point.p1 = point.p2 = {
                        x: point.pstart.x,
                        y: point.pend.y
                    };
                    posLabel = {
                        x: (point.pstart.x + point.pend.x) / 2,
                        y: point.pend.y
                    };
                }
            } else { // start in right
                // top
                if (point.start.y < point.end.y) {
                    point.p1 = point.p2 = {
                        x: point.pend.x,
                        y: point.pstart.y
                    };
                    posLabel = {
                        x: (point.pstart.x + point.pend.x) / 2,
                        y: point.pstart.y
                    };
                } else { // bottom
                    point.p1 = point.p2 = {
                        x: point.pend.x,
                        y: point.pstart.y
                    };
                    posLabel = {
                        x: (point.pstart.x + point.pend.x) / 2,
                        y: point.pstart.y
                    };
                }
            }
            arrow.type = "right";
        }
    }
    
    var path = this.pathCorner(point, 5);
    var mask = ['M', point.start.x, point.start.y, "L", point.pstart.x, point.pstart.y, 'L', point.p1.x, point.p1.y, 'L', point.p2.x, point.p2.y, 'L', point.pend.x, point.pend.y, 'L', point.pend.x, point.pend.y, 'L', point.end.x, point.end.y];
    if (arrow.type == 'top') {
        arrow.path = ['M', point.end.x, point.end.y, 'L', point.end.x - arrow.tangle / 2, point.end.y + arrow.tangle, point.end.x + arrow.tangle / 2, point.end.y + arrow.tangle, 'Z', point.end.x, point.end.y];
    } else if (arrow.type == 'right') {
        arrow.path = ['M', point.end.x, point.end.y, 'L', point.end.x - arrow.tangle, point.end.y - arrow.tangle / 2, point.end.x - arrow.tangle, point.end.y + arrow.tangle / 2, 'Z', point.end.x, point.end.y];
    } else if (arrow.type == 'bottom') {
        arrow.path = ['M', point.end.x, point.end.y, 'L', point.end.x - arrow.tangle / 2, point.end.y - arrow.tangle, point.end.x + arrow.tangle / 2, point.end.y - arrow.tangle, 'Z', point.end.x, point.end.y];
    } else { // left
        arrow.path = ['M', point.end.x, point.end.y, 'L', point.end.x + arrow.tangle, point.end.y - arrow.tangle / 2, point.end.x + arrow.tangle, point.end.y + arrow.tangle / 2, 'Z', point.end.x, point.end.y];
    }
    
    // existing a line (existing a connection)
    if (connection && connection.line) {
        connection.line.attr({ path: path });
        connection.arrow.attr({ path: arrow.path });
        connection.mask.attr({ path: path });
        connection.posLabel = posLabel;

        // start is only single connection and it haven't label
        if (connection.label !== undefined) {
            // label
            var sizelabel = {
                w: connection.label.text.getBBox().width,
                h: connection.label.text.getBBox().height
            };
            connection.label.background.attr({
                x: posLabel.x - (sizelabel.w + defaultValue.margin * 2) / 2,
                y: posLabel.y - sizelabel.h / 2
            });
            connection.label.text.attr({ x: posLabel.x, y: posLabel.y });
            connection.label.mask.attr({
                x: posLabel.x - (sizelabel.w + defaultValue.margin * 2) / 2,
                y: posLabel.y - sizelabel.h / 2
            });
        }
    } else { // draw new connection
        var text = this.text(-100, -100, info.label);
        var sizelabel = {
            w: text.getBBox().width,
            h: text.getBBox().height
        };

        var conn = {
            line: this.path(path).attr(connectionSettings.connectionNormal), // line
            arrow: this.path(arrow.path).attr(connectionSettings.arrowNormal), // arrow
            posLabel: posLabel, // posLabel
            label: undefined,//drawLabel(posLabel.x, posLabel.y, sizelabel.w, sizelabel.h, info.label, this), // label
            mask: this.path(mask).attr(connectionSettings.connectionMask), // mask ahead
            from: start, // entity from
            to: end, // entity to
            info: info, // entity info
            selectChange: function (isSelect) {
                this.isSelected = isSelect;
                if (isSelect) { // select
                    $(document).trigger("CONNECTIONLINE:SELECT",this);                         // Added By New Ocean
                    this.line.attr(connectionSettings.connectionSelected);
                    this.arrow.attr(connectionSettings.arrowSelected);
                    if (this.label != undefined) {
                        this.label.background.attr(connectionSettings.labelSelected)
                    }
                } else { // unselect
                    $(document).trigger("CONNECTIONLINE:DESELECT");                         // Added By New Ocean
                    this.line.attr(connectionSettings.connectionNormal);
                    this.arrow.attr(connectionSettings.arrowNormal);
                    if (this.label != undefined) {
                        this.label.background.attr(connectionSettings.labelNormal);
                    }
                }
            }
        };

        text.remove();
        //conn.label.mask.parent = conn;
        conn.mask.parent = conn;
        return conn;
    }
}

// fn draw line connection with border-radius at corner
Raphael.fn.pathCorner = function (points, radius) {
    // start
    var array = new Array();
    array = array.concat(['M', points.start.x, points.start.y]);

    array = array.concat(setCorner(points.start, points.pstart, points.p1, radius));
    // p1 is p2
    if (points.p1 === points.p2) {
        if (points.p2.x === points.pstart.x && points.p2.y === points.pstart.y) { // special
            array = array.concat(setCorner(points.start, points.pstart, points.pend, radius));
        } else if (points.p2.x === points.pend.x && points.p2.y === points.pend.y) { // special
            array = array.concat(setCorner(points.pstart, points.pend, points.end, radius));
        }else {
            array = array.concat(setCorner(points.pstart, points.p1, points.pend, radius));
        }
    } else {
        array = array.concat(setCorner(points.pstart, points.p1, points.p2, radius));
        array = array.concat(setCorner(points.p1, points.p2, points.pend, radius));
    }
    array = array.concat(setCorner(points.p2, points.pend, points.end, radius));
    // specials
    if (points.p1 === points.p2 && points.p2 === points.pend) { // p1, p2, pend in a position
        array = array.concat(setCorner(points.pstart, points.pend, points.end, radius));
    }
    
    array = array.concat(['L', points.end.x, points.end.y]);

    function setCorner(p1, p2, p3, r) {
        if ((p1.x === p2.x && p2.x === p3.x) || (p1.y === p2.y && p2.y === p3.y)) {
            return [];
        }
        if (p1.x === p2.x) { //horizontal  
            if ((Math.abs(p1.y - p2.y) < r * 2) || (p1.y !== p2.y && Math.abs(p2.x - p3.x) < r * 2)) {
                r = 0;
            }
            if (p1.y > p2.y) { // corner in bottom   
                if (p2.x > p3.x) { // end in left   
                    return ['L', p2.x, p2.y + r, 'Q', p2.x, p2.y, p2.x - r, p2.y];
                } else { // end in right
                    return ['L', p2.x, p2.y + r, 'Q', p2.x, p2.y, p2.x + r, p2.y];
                }
            } else { // corner in top
                if (p2.x > p3.x) { // end in left
                    return ['L', p2.x, p2.y - r, 'Q', p2.x, p2.y, p2.x - r, p2.y];
                } else { // end in right
                    return ['L', p2.x, p2.y - r, 'Q', p2.x, p2.y, p2.x + r, p2.y];
                }
            }
        } else { // vertical
            if ((Math.abs(p1.x - p2.x) < r * 2) || (p1.x !== p2.x && Math.abs(p2.y - p3.y) < r * 2)) {
                r = 0;
            }
            if (p1.x < p2.x) { // corner in right      
                if (p2.y > p3.y) { // end in top
                    return ['L', p2.x - r, p2.y, 'Q', p2.x, p2.y, p2.x, p2.y - r];
                } else { // end in bottom
                    return ['L', p2.x - r, p2.y, 'Q', p2.x, p2.y, p2.x, p2.y + r];
                }
            } else { // corner in left
                if (p2.y > p3.y) { // end in top
                    return ['L', p2.x + r, p2.y, 'Q', p2.x, p2.y, p2.x, p2.y - r];
                } else { // end in bottom
                    return ['L', p2.x + r, p2.y, 'Q', p2.x, p2.y, p2.x, p2.y + r];
                }
            }
        }
        return [];
    }
    return array;
}

// remove for update
Raphael.fn.removeConnect = function (id) {
    mRoot.objConnections.forEach(function (item) {
        if (item.id == id) {
            item.breakpoints.remove();
            if (item.label !== undefined) {
                item.label.background.remove();
                item.label.text.remove();
                item.label.mask.remove();
            }
            item.line.remove();
            item.arrow.remove();
            item.mask.remove();
        }
    });
}

// remove connection
Raphael.fn.removeConnection = function (id) {
    mRoot.objConnections.forEach(function (conn) {
        if (conn.id == id) {
            mRoot.objConnections.remove(conn);
            if (conn.breakpoints !== undefined) {
                conn.breakpoints.remove();
            }
            if (conn.label !== undefined) {
                conn.label.background.remove();
                conn.label.text.remove();
                conn.label.mask.remove();
            }
            conn.line.remove();
            conn.arrow.remove();
            conn.mask.remove();
        }
    });
}

// remove entity
Raphael.fn.removeEntity = function (id) {
    mRoot.objEntities.forEach(function (entity) {
        if (entity.id == id) {
            for (var i = mRoot.infoConnections.count - 1; i >= 0; i--) {
                var conn = mRoot.infoConnections.collection[i];
                if (conn.from == id || conn.to == id) {
                    // remove connection
                    mRoot.infoConnections.remove(conn);
                    viewport.removeConnection(conn.id);
                }
            }

            mRoot.objEntities.remove(entity);
            for (var i = 0; i < entity.holders.length; i++) {
                if (entity.holders[i].left !== undefined) {
                    entity.holders[i].left.remove();
                }
                if (entity.holders[i].right !== undefined) {
                    entity.holders[i].right.remove();
                }
            }
            entity.holders.remove();
            entity.label.remove();
            entity.icon.remove();
            entity.entity.remove();
            entity.remove();
        }
    });
}

function drawLabel(x, y, w, h, label, obj) {
    var result = {
        background: obj.rect(x - (w + defaultValue.margin * 2) / 2, y - h / 2, w + defaultValue.margin * 2, h, 5).attr(connectionSettings.labelNormal),
        text: obj.text(x, y, label).attr({ 'text-anchor': 'middle' }),
        mask: obj.rect(x - (w + defaultValue.margin * 2) / 2, y - h / 2, w + defaultValue.margin * 2, h, 5).attr({ fill: "#ffffff", "fill-opacity": "0.01", stroke: "#454545", 'stroke-opacity': '0.01' })
    };

    result.mask.shapeType = shapeTypes.connection;
    result.mask.hover(fin, fout);
    result.mask.mousedown(mousedown);
    result.mask.mouseup(mouseup);
    result.mask.dblclick(doubleClick);
    return result;
}

Raphael.el.contain = function (element) {
    var area = this.getBBox();
    if (element.shapeType == shapeTypes.entity) {
        var ele = element.entity.getBBox();
        if (ele.x > area.x &&
            ele.y > area.y &&
            ele.x + ele.width < area.x + area.width &&
            ele.y + ele.height < area.y + area.height) {
            return true;
        }
        return false;
    }
}
