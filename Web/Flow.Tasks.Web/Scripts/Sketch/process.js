var project = null;
var flagNew = false;
/*------------------------------------ NEW -------------------------------*/
// fn new
function actionNew(wf) {
    flagNew = true;
    project = wf;
    $("#viewport").show();
    $("#workflow-name").html(wf).show();
    $("#save").show();

    viewport.clear(); // reset viewport
    $('ul.variables').html(""); // reset variables
    // reset entity objects
    mRoot.infoEntities = new Collection();
    mRoot.objEntities = new Collection();

    // reset connection objects
    mRoot.infoConnections = new Collection();
    mRoot.objConnections = new Collection();

    // reset variable objects
    mRoot.infoVariables = new Collection();

    drawBegin(viewport);
}

/*------------------------------------ LOAD - SELECT -------------------------------*/
var workflowData = null;

// fn get entities of flowchart file .xamlx
function getEntities(obj, parent) {
    // decision
    if (obj["p:FlowDecision"] !== undefined) {
        var root_decisions = obj["p:FlowDecision"];
        if (typeof root_decisions === "object" && root_decisions.length !== undefined) { // is array objects
            for (var i = 0; i < root_decisions.length; i++) {
                var root_decision = root_decisions[i];
                var isAlias = root_decision["@sap2010:WorkflowViewState.IdRef"] != undefined ? root_decision["@sap2010:WorkflowViewState.IdRef"] : root_decision["sap2010:WorkflowViewState.IdRef"];
                if (root_decision["p:FlowDecision.True"] !== undefined) { // have obj in true connection
                    getEntities(root_decision["p:FlowDecision.True"], isAlias);
                }
                if (root_decision["p:FlowDecision.False"] !== undefined) { // have obj in false connection
                    getEntities(root_decision["p:FlowDecision.False"], isAlias);
                }
                // get info
                var _decision = new Entity();
                _decision.label = root_decision["@DisplayName"] !== undefined ? root_decision["@DisplayName"].replaceAll("\"", "&quot;") : "Decision";
                _decision.condition = root_decision["@Condition"] !== undefined ? root_decision["@Condition"].replaceAll("\"", "&quot;").substr(1, root_decision["@Condition"].length - 2) : (root_decision["p:FlowDecision.Condition"] !== undefined ? root_decision["p:FlowDecision.Condition"]["mca:CSharpValue"]["#text"].replaceAll("\"", "&quot;") : "");
                _decision.annotation = root_decision["@sap2010:Annotation.AnnotationText"] !== undefined ? root_decision["@sap2010:Annotation.AnnotationText"].replaceAll("\"", "&quot;") : "";
                _decision.type = typeEntity.t_decision;
                _decision.alias = isAlias;
                _decision.parent = parent !== undefined ? parent : '';

                mRoot.infoEntities.add(_decision);
            }
        } else { // is object
            var isAlias = root_decisions["@sap2010:WorkflowViewState.IdRef"] != undefined ? root_decisions["@sap2010:WorkflowViewState.IdRef"] : root_decisions["sap2010:WorkflowViewState.IdRef"];
            if (root_decisions["p:FlowDecision.True"] !== undefined) { // have obj in true connection
                getEntities(root_decisions["p:FlowDecision.True"], isAlias);
            }
            if (root_decisions["p:FlowDecision.False"] !== undefined) { // have obj in false connection
                getEntities(root_decisions["p:FlowDecision.False"], isAlias);
            }
            // get info
            var _decision = new Entity();
            _decision.label = root_decisions["@DisplayName"] !== undefined ? root_decisions["@DisplayName"].replaceAll("\"", "&quot;") : "Decision";
            _decision.condition = root_decisions["@Condition"] !== undefined ? root_decisions["@Condition"].replaceAll("\"", "&quot;").substr(1, root_decisions["@Condition"].length - 2) : (root_decisions["p:FlowDecision.Condition"] !== undefined ? root_decisions["p:FlowDecision.Condition"]["mca:CSharpValue"]["#text"].replaceAll("\"", "&quot;") : "");
            _decision.annotation = root_decisions["@sap2010:Annotation.AnnotationText"] !== undefined ? root_decisions["@sap2010:Annotation.AnnotationText"].replaceAll("\"", "&quot;") : "";
            _decision.type = typeEntity.t_decision;
            _decision.alias = isAlias;
            _decision.parent = parent !== undefined ? parent : '';
            mRoot.infoEntities.add(_decision);
        }
    }

    // switch
    if (obj["p:FlowSwitch"] !== undefined) {
        var root_switchs = obj["p:FlowSwitch"];
        // have 
        if (typeof (root_switchs) === "object" && root_switchs.length !== undefined) { // is array object
            for (var i = 0; i < root_switchs.length; i++) {
                var root_switch = root_switchs[i];
                var isAlias = root_switch["@sap2010:WorkflowViewState.IdRef"] != undefined ? root_switch["@sap2010:WorkflowViewState.IdRef"] : root_switch["sap2010:WorkflowViewState.IdRef"];
                // switch connection multi other decision/switch/step
                if (root_switch["p:FlowDecision"] !== undefined || root_switch["p:FlowSwitch"] !== undefined || root_switch["p:FlowStep"]) {
                    getEntities(root_switch, isAlias);
                }
                // only exitings a connect default by switch
                if (root_switch["p:FlowSwitch.Default"] !== undefined) { // connect default
                    getEntities(root_switch["p:FlowSwitch.Default"], isAlias);
                }
                // get info
                var _switch = new Entity();
                _switch.label = root_switch["@DisplayName"] !== undefined ? root_switch["@DisplayName"].replaceAll("\"", "&quot;") : "Switch"; // label if existing different default
                _switch.expression = root_switch["@Expression"] !== undefined ? root_switch["@Expression"].replaceAll("\"", "&quot;").substr(1, root_switch["@Expressionf"].length - 2) : (root_switch["p:FlowSwitch.Expression"] !== undefined ? root_switch["p:FlowSwitch.Expression"]["mca:CSharpValue"]["#text"] : "");
                _switch.annotation = root_switch["@sap2010:Annotation.AnnotationText"] !== undefined ? root_switch["@sap2010:Annotation.AnnotationText"].replaceAll("\"", "&quot;") : "";
                _switch.typeSwitch = root_switch["@x:TypeArguments"].split("x:")[1];
                _switch.type = typeEntity.t_switch;
                _switch.alias = isAlias;
                _switch.parent = parent !== undefined ? parent : "";
                mRoot.infoEntities.add(_switch);
            }
        } else { // if object
            var isAlias = root_switchs["@sap2010:WorkflowViewState.IdRef"] != undefined ? root_switchs["@sap2010:WorkflowViewState.IdRef"] : root_switchs["sap2010:WorkflowViewState.IdRef"];
            // only exitings a connect default by switch
            if (root_switchs["p:FlowSwitch.Default"] !== undefined) { // connect default
                getEntities(root_switchs["p:FlowSwitch.Default"], isAlias);
            }
            // switch connection multi other decision/switch/step
            if (root_switchs["p:FlowDecision"] !== undefined || root_switchs["p:FlowSwitch"] !== undefined || root_switchs["p:FlowStep"] !== undefined) {
                getEntities(root_switchs, isAlias);
            }

            // get info
            var _switch = new Entity();
            _switch.label = root_switchs["@DisplayName"] !== undefined ? root_switchs["@DisplayName"].replaceAll("\"", "&quot;") : "Switch"; // label if existing different default
            _switch.expression = root_switchs["@Expression"] !== undefined ? root_switchs["@Expression"].replaceAll("\"", "&quot;").substr(1, root_switchs["@Expression"].length - 2) : (root_switchs["p:FlowSwitch.Expression"] !== undefined ? root_switchs["p:FlowSwitch.Expression"]["mca:CSharpValue"]["#text"] : "");
            _switch.annotation = root_switchs["@sap2010:Annotation.AnnotationText"] !== undefined ? root_switchs["@sap2010:Annotation.AnnotationText"].replaceAll("\"", "&quot;") : "";
            _switch.typeSwitch = root_switchs["@x:TypeArguments"].split("x:")[1];
            _switch.type = typeEntity.t_switch;
            _switch.alias = isAlias;
            _switch.parent = parent !== undefined ? parent : "";
            mRoot.infoEntities.add(_switch);
        }
    }

    // step - unknown
    if (obj["p:FlowStep"] !== undefined) {
        var root_steps = obj["p:FlowStep"];
        if (typeof (root_steps) === "object" && root_steps.length !== undefined) {
            for (var i = 0; i < root_steps.length; i++) {
                var root_step = root_steps[i];
                // approve
                if (root_step["ftwa:ApproveTask"] !== undefined) {
                    var root_approve = root_step["ftwa:ApproveTask"];
                    // alias is step's name
                    var isAlias = root_step["@sap2010:WorkflowViewState.IdRef"] != undefined ? root_step["@sap2010:WorkflowViewState.IdRef"] : root_step["sap2010:WorkflowViewState.IdRef"];
                    if (root_step["p:FlowStep.Next"] !== undefined) {
                        getEntities(root_step["p:FlowStep.Next"], isAlias);
                    }
                    var _approve = new Entity();
                    _approve.label = root_approve["@DisplayName"] != undefined ? root_approve["@DisplayName"].replaceAll("\"", "&quot;") : "ApproveTask";
                    _approve.annotation = root_approve["@sap2010:Annotation.AnnotationText"] != undefined ? (root_approve["@sap2010:Annotation.AnnotationText"] !== "{x:Null}" ? root_approve["@sap2010:Annotation.AnnotationText"].replaceAll("\"", "&quot;") : "") : "";
                    // assign result to
                    if (root_approve["@AssignResultTo"] != undefined && root_approve["@AssignResultTo"] === "{x:Null}") {
                        _approve.AssignResultTo = "";
                    } else {
                        if (root_approve["ftwa:ApproveTask.AssignResultTo"]["p:OutArgument"]["mca:CSharpReference"]["#text"] !== undefined) {
                            _approve.AssignResultTo = root_approve["ftwa:ApproveTask.AssignResultTo"]["p:OutArgument"]["mca:CSharpReference"]["#text"].replaceAll("\"", "&quot;");
                        } else {
                            _approve.AssignResultTo = "";
                        }
                    }
                    _approve.AssignedToUsers = root_approve["@AssignedToUsers"] != undefined ? (root_approve["@AssignedToUsers"] !== "{x:Null}" ? root_approve["@AssignedToUsers"].replaceAll("\"", "&quot;") : "") : "";
                    _approve.CorrelationId = root_approve["@CorrelationId"] != undefined ? (root_approve["@CorrelationId"] !== "{x:Null}" ? root_approve["@CorrelationId"].replaceAll("\"", "&quot;") : "") : "";
                    _approve.DefaultResult = root_approve["@DefaultResult"] != undefined ? (root_approve["@DefaultResult"] !== "{x:Null}" ? root_approve["@DefaultResult"].replaceAll("\"", "&quot;") : "") : "";
                    _approve.Description = root_approve["@Description"] != undefined ? (root_approve["@Description"] !== "{x:Null}" ? root_approve["@Description"].replaceAll("\"", "&quot;") : "") : "";
                    _approve.ExpiresIn = root_approve["@ExpiresIn"] != undefined ? (root_approve["@ExpiresIn"] !== "{x:Null}" ? root_approve["@ExpiresIn"].replaceAll("\"", "&quot;") : "") : "";
                    _approve.ExpiresWhen = root_approve["@ExpiresWhen"] != undefined ? (root_approve["@ExpiresWhen"] !== "{x:Null}" ? root_approve["@ExpiresWhen"].replaceAll("\"", "&quot;") : "") : "";
                    _approve.HandOverUsers = root_approve["@HandOverUsers"] != undefined ? (root_approve["@HandOverUsers"] !== "{x:Null}" ? root_approve["@HandOverUsers"].replaceAll("\"", "&quot;") : "") : "";
                    _approve.OnComplete = root_approve["@OnComplete"] != undefined ? (root_approve["@OnComplete"] !== "{x:Null}" ? root_approve["@OnComplete"].replaceAll("\"", "&quot;") : "") : "";
                    _approve.OnInit = root_approve["@OnInit"] != undefined ? (root_approve["@OnInit"] !== "{x:Null}" ? root_approve["@OnInit"].replaceAll("\"", "&quot;") : "") : "";
                    _approve.TaskCode = root_approve["@TaskCode"] != undefined ? (root_approve["@TaskCode"] !== "{x:Null}" ? root_approve["@TaskCode"].replaceAll("\"", "&quot;") : "") : "";
                    _approve.Title = root_approve["@Title"] != undefined ? (root_approve["@Title"] !== "{x:Null}" ? root_approve["@Title"].replaceAll("\"", "&quot;") : "") : "";
                    _approve.UiCode = root_approve["@UiCode"] != undefined ? (root_approve["@UiCode"] !== "{x:Null}" ? root_approve["@UiCode"].replaceAll("\"", "&quot;") : "") : "";
                    _approve.type = typeEntity.t_approve;
                    _approve.alias = isAlias;
                    _approve.parent = parent !== undefined ? parent : "";
                    mRoot.infoEntities.add(_approve);
                }
                // generic
                if (root_step["ftwa:GenericTask"] !== undefined) {
                    // alias is step's name
                    var root_generic = root_step["ftwa:GenericTask"];
                    var isAlias = root_step["@sap2010:WorkflowViewState.IdRef"] != undefined ? root_step["@sap2010:WorkflowViewState.IdRef"] : root_step["sap2010:WorkflowViewState.IdRef"];
                    if (root_step["p:FlowStep.Next"] !== undefined) {
                        getEntities(root_step["p:FlowStep.Next"], isAlias);
                    }
                    var _generic = new Entity();
                    _generic.label = root_generic["@DisplayName"] != undefined ? root_generic["@DisplayName"] : "GenericTask";
                    _generic.annotation = root_generic["@sap2010:Annotation.AnnotationText"] != undefined ? (root_generic["@sap2010:Annotation.AnnotationText"] !== "{x:Null}" ? root_generic["@sap2010:Annotation.AnnotationText"].replaceAll("\"", "&quot;") : "") : "";
                    _generic.OnRun = root_generic["@OnRun"] != undefined ? (root_generic["@OnRun"] !== "{x:Null}" ? root_generic["@OnRun"].replaceAll("\"", "&quot;") : "") : "";
                    // assign result to
                    if (root_generic["@TaskCode"] !== undefined && root_generic["@TaskCode"] === "{x:Null}") {
                        _generic.TaskCode = "";
                    } else {
                        if (root_generic["ftwa:GenericTask.TaskCode"]["p:InArgument"]["mca:CSharpValue"]["#text"] !== undefined) {
                            _generic.TaskCode = root_generic["ftwa:GenericTask.TaskCode"]["p:InArgument"]["mca:CSharpValue"]["#text"].replaceAll("\"", "&quot;");
                        } else {
                            _generic.TaskCode = "";
                        }
                    }
                    _generic.type = typeEntity.t_generic;
                    _generic.alias = isAlias;
                    _generic.parent = parent !== undefined ? parent : "";
                    mRoot.infoEntities.add(_generic);
                }
            }
        } else {
            // approve
            if (root_steps["ftwa:ApproveTask"] !== undefined) {
                var root_approve = root_steps["ftwa:ApproveTask"];
                // alias is step's name
                var isAlias = root_steps["@sap2010:WorkflowViewState.IdRef"] != undefined ? root_steps["@sap2010:WorkflowViewState.IdRef"] : root_steps["sap2010:WorkflowViewState.IdRef"];
                if (root_steps["p:FlowStep.Next"] !== undefined) {
                    getEntities(root_steps["p:FlowStep.Next"], isAlias);
                }
                var _approve = new Entity();
                _approve.label = root_approve["@DisplayName"] != undefined ? root_approve["@DisplayName"].replaceAll("\"", "&quot;") : "ApproveTask";
                _approve.annotation = root_approve["@sap2010:Annotation.AnnotationText"] != undefined ? (root_approve["@sap2010:Annotation.AnnotationText"] !== "{x:Null}" ? root_approve["@sap2010:Annotation.AnnotationText"].replaceAll("\"", "&quot;") : "") : "";
                // assign result to
                if (root_approve["@AssignResultTo"] !== undefined && root_approve["@AssignResultTo"] === "{x:Null}") {
                    _approve.AssignResultTo = "";
                } else {
                    if (root_approve["ftwa:ApproveTask.AssignResultTo"]["p:OutArgument"]["mca:CSharpReference"]["#text"] !== undefined) {
                        _approve.AssignResultTo = root_approve["ftwa:ApproveTask.AssignResultTo"]["p:OutArgument"]["mca:CSharpReference"]["#text"].replaceAll("\"", "&quot;");
                    } else {
                        _approve.AssignResultTo = "";
                    }
                }
                _approve.AssignedToUsers = root_approve["@AssignedToUsers"] != undefined ? (root_approve["@AssignedToUsers"] !== "{x:Null}" ? root_approve["@AssignedToUsers"].replaceAll("\"", "&quot;") : "") : "";
                _approve.CorrelationId = root_approve["@CorrelationId"] != undefined ? (root_approve["@CorrelationId"] !== "{x:Null}" ? root_approve["@CorrelationId"].replaceAll("\"", "&quot;") : "") : "";
                _approve.DefaultResult = root_approve["@DefaultResult"] != undefined ? (root_approve["@DefaultResult"] !== "{x:Null}" ? root_approve["@DefaultResult"].replaceAll("\"", "&quot;") : "") : "";
                _approve.Description = root_approve["@Description"] != undefined ? (root_approve["@Description"] !== "{x:Null}" ? root_approve["@Description"].replaceAll("\"", "&quot;") : "") : "";
                _approve.ExpiresIn = root_approve["@ExpiresIn"] != undefined ? (root_approve["@ExpiresIn"] !== "{x:Null}" ? root_approve["@ExpiresIn"].replaceAll("\"", "&quot;") : "") : "";
                _approve.ExpiresWhen = root_approve["@ExpiresWhen"] != undefined ? (root_approve["@ExpiresWhen"] !== "{x:Null}" ? root_approve["@ExpiresWhen"].replaceAll("\"", "&quot;") : "") : "";
                _approve.HandOverUsers = root_approve["@HandOverUsers"] != undefined ? (root_approve["@HandOverUsers"] !== "{x:Null}" ? root_approve["@HandOverUsers"].replaceAll("\"", "&quot;") : "") : "";
                _approve.OnComplete = root_approve["@OnComplete"] != undefined ? (root_approve["@OnComplete"] !== "{x:Null}" ? root_approve["@OnComplete"].replaceAll("\"", "&quot;") : "") : "";
                _approve.OnInit = root_approve["@OnInit"] != undefined ? (root_approve["@OnInit"] !== "{x:Null}" ? root_approve["@OnInit"].replaceAll("\"", "&quot;") : "") : "";
                _approve.TaskCode = root_approve["@TaskCode"] != undefined ? (root_approve["@TaskCode"] !== "{x:Null}" ? root_approve["@TaskCode"].replaceAll("\"", "&quot;") : "") : "";
                _approve.Title = root_approve["@Title"] != undefined ? (root_approve["@Title"] !== "{x:Null}" ? root_approve["@Title"].replaceAll("\"", "&quot;") : "") : "";
                _approve.UiCode = root_approve["@UiCode"] != undefined ? (root_approve["@UiCode"] !== "{x:Null}" ? root_approve["@UiCode"].replaceAll("\"", "&quot;") : "") : "";
                _approve.type = typeEntity.t_approve;
                _approve.alias = isAlias;
                _approve.parent = parent !== undefined ? parent : "";
                mRoot.infoEntities.add(_approve);
            }
            // generic
            if (root_steps["ftwa:GenericTask"] !== undefined) {
                var root_generic = root_steps["ftwa:GenericTask"];
                // alias is step's name
                var isAlias = root_steps["@sap2010:WorkflowViewState.IdRef"] != undefined ? root_steps["@sap2010:WorkflowViewState.IdRef"] : root_steps["sap2010:WorkflowViewState.IdRef"];
                if (root_steps["p:FlowStep.Next"] !== undefined) {
                    getEntities(root_steps["p:FlowStep.Next"], isAlias);
                }
                var _generic = new Entity();
                _generic.label = root_generic["@DisplayName"] != undefined ? root_generic["@DisplayName"].replaceAll("\"", "&quot;") : "GenericTask";
                _generic.annotation = root_generic["@sap2010:Annotation.AnnotationText"] != undefined ? (root_generic["@sap2010:Annotation.AnnotationText"] !== "{x:Null}" ? root_generic["@sap2010:Annotation.AnnotationText"] : "") : "";
                _generic.OnRun = root_generic["@OnRun"] != undefined ? (root_generic["@OnRun"] !== "{x:Null}" ? root_generic["@OnRun"].replaceAll("\"", "&quot;") : "") : "";
                // assign result to
                if (root_generic["@TaskCode"] !== undefined && root_generic["@TaskCode"] === "{x:Null}") {
                    _generic.TaskCode = "";
                } else {
                    if (root_generic["ftwa:GenericTask.TaskCode"]["p:InArgument"]["mca:CSharpValue"]["#text"] !== undefined) {
                        _generic.TaskCode = root_generic["ftwa:GenericTask.TaskCode"]["p:InArgument"]["mca:CSharpValue"]["#text"].replaceAll("\"", "&quot;");
                    } else {
                        _generic.TaskCode = "";
                    }
                }
                _generic.type = typeEntity.t_generic;
                _generic.alias = isAlias;
                _generic.parent = parent !== undefined ? parent : "";
                mRoot.infoEntities.add(_generic);
            }
        }
    }
}
// fn get connections of flowchart file .xamlx
function getConnections(parent, infoManagers, label) {
    if (typeof (infoManagers) === "object" && infoManagers.length !== undefined) { // have many connections
        for (var i = 0; i < infoManagers.length; i++) {
            var conn = infoManagers[i];
            var key = conn["@x:Key"].split("Connector")[0];
            var points = conn["#text"].split(" ");
            var start = {
                x: parseFloat(points[0].split(",")[0]),
                y: parseFloat(points[0].split(",")[1])
            },
                end = {
                    x: parseFloat(points[points.length - 1].split(",")[0]),
                    y: parseFloat(points[points.length - 1].split(",")[1])
                };
            var pos = setPosition(start, end);

            var _conn = new Connection();
            // label of default case
            if (label !== undefined && !$.isNumeric(key) && key === "Default") {
                _conn.label = label;
                _conn.isDefault = true;
            } else { // case
                if (key.toLowerCase() === "default") {
                    _conn.isDefault = true;
                }
                _conn.label = key;
            }
            _conn.from = parent.id;
            _conn.to = pos.to;
            _conn.fromHolder = pos.start;
            _conn.toHolder = pos.end;

            if (_conn.to !== -1 && _conn.from !== -1) {
                mRoot.infoConnections.add(_conn);
            }
        }
    } else {
        var key = infoManagers["@x:Key"].split("Connector")[0];
        var points = infoManagers["#text"].split(" ");
        var start = {
            x: parseFloat(points[0].split(",")[0]),
            y: parseFloat(points[0].split(",")[1])
        },
        end = {
            x: parseFloat(points[points.length - 1].split(",")[0]),
            y: parseFloat(points[points.length - 1].split(",")[1])
        };
        var pos = setPosition(start, end);
        var _conn = new Connection();
        // default case with other name
        if (label !== undefined && key.toLowerCase() === "default") {
            _conn.label = label;
            _conn.isDefault = true;
        } else { // case
            if (key.toLowerCase() === "default") {
                _conn.isDefault = true;
            }
            _conn.label = key;
        }
        _conn.from = parent.id;
        _conn.to = pos.to;
        _conn.fromHolder = pos.start;
        _conn.toHolder = pos.end;
        if (_conn.to !== -1 && _conn.from !== -1) {
            mRoot.infoConnections.add(_conn);
        }
    }

    // set value of connection
    function setValue(conn, key) {
        if (key.toLowerCase() === "false") {
            conn.fromHolder = 2;
            conn.toHolder = 1;

        } else if (key === "true") {
            conn.fromHolder = 4;
            conn.toHolder = 1;
        } else {
            conn.fromHolder = 3;
            conn.toHolder = 1;
        }
    }

    // set value position of connection start/end
    function setPosition(start, end) {
        var pos = { to: -1, start: -1, end: -1 };
        mRoot.objEntities.forEach(function (entity) {
            var holders = entity.holders;
            for (var i = 0; i < holders.length; i++) {
                var holder = holders[i];
                var limit = {
                    min: {
                        x: holder.attr("x") - 5,
                        y: holder.attr("y") - 5
                    },
                    max: {
                        x: holder.attr("x") + holder.attr("width") + 5,
                        y: holder.attr("y") + holder.attr("height") + 5
                    }
                }
                if (limit.min.x <= start.x && start.x <= limit.max.x &&
                    limit.min.y <= start.y && start.y <= limit.max.y) {
                    pos.start = holder.id;
                }
                if (limit.min.x <= end.x && end.x <= limit.max.x &&
                    limit.min.y <= end.y && end.y <= limit.max.y) {
                    pos.end = holder.id;
                    pos.to = entity.id;
                }
            }
        });
        return pos;
    }
}

// fn get variable of flowchart file .xamlx
function getVariables(obj) {
    if (obj !== undefined && obj !== null) {
        var root_variables = obj["p:Variable"];
        if (typeof (root_variables) === "object" && root_variables.length !== undefined) {
            for (var i = 0; i < root_variables.length; i++) {
                var root_variable = root_variables[i];

                var _var = new Variable();
                _var.name = decodeXml(root_variable["@Name"]);
                _var.type = decodeXml(root_variable["@x:TypeArguments"].split("x:")[1]);
                //_var.value = "Flowchart";
                if (root_variable["@Default"] === undefined) {
                    if (root_variable["p:Variable.Default"] !== undefined) {
                        var _default = root_variable["p:Variable.Default"];
                        if (_default["mca:CSharpValue"] !== undefined) {
                            _var.value = decodeXml(_default["mca:CSharpValue"]["#text"]);
                        } else if (_default["p:Literal"] === undefined) {
                            _var.value = decodeXml(_default["p:Literal"]["#text"]);
                        }
                    } else {
                        _var.value = "";
                    }
                } else {
                    var type = root_variable["@x:TypeArguments"].split("x:")[1];
                    if (type.toLowerCase() === "string") {
                        _var.value = decodeXml("\"" + root_variable["@Default"] + "\"");
                    } else {
                        _var.value = decodeXml(root_variable["@Default"]);
                    }
                }

                mRoot.infoVariables.add(_var);
                var id = mRoot.infoVariables.collection[mRoot.infoVariables.count - 1].id;
                var item = '<li class="item-var' + id + '" onclick="return activeVariables(this)">'
                                + '<div>' + encodeXml(_var.name) + '</div>'
                                + '<a href="javascript:void(0)" class="delButton" onclick="return removeVariable(' + id + ');">'
                                    + '<img width="10px" src="Images/delete-icon.png" title="Delete" />'
                                + '</a>'
                            + '</li>'
                $('ul.variables').append(item);

                // active envent click of variable
            }
        } else {
            var _var = new Variable();
            _var.name = decodeXml(root_variables["@Name"]);
            _var.type = decodeXml(root_variables["@x:TypeArguments"].split("x:")[1]);
            //_var.value = "Flowchart";
            if (root_variables["@Default"] === undefined) {
                if (root_variables["p:Variable.Default"] !== undefined) {
                    var _default = root_variables["p:Variable.Default"];
                    if (_default["mca:CSharpValue"] !== undefined) {
                        _var.value = decodeXml(_default["mca:CSharpValue"]["#text"]);
                    } else if (_default["p:Literal"] === undefined) {
                        _var.value = decodeXml(_default["p:Literal"]["#text"]);
                    }
                } else {
                    _var.value = "";
                }
            } else {
                var type = root_variables["@x:TypeArguments"].split("x:")[1];
                if (type.toLowerCase() === "string") {
                    _var.value = decodeXml("\"" + root_variables["@Default"] + "\"");
                } else {
                    _var.value = decodeXml(root_variables["@Default"]);
                }
            }

            mRoot.infoVariables.add(_var);
            var id = mRoot.infoVariables.collection[mRoot.infoVariables.count - 1].id;
            var item = '<li class="item-var' + id + '" onclick="return activeVariables(this)">'
                            + '<div>' + encodeXml(_var.name) + '</div>'
                            + '<a href="javascript:void(0)" class="delButton" onclick="return removeVariable(' + id + ');">'
                                + '<img width="10px" src="Images/delete-icon.png" title="Delete" />'
                            + '</a>'
                        + '</li>'
            $('ul.variables').append(item);
        }
    }
}

// fn get objects flowchar from file .xamlx of server
function getDataBySystem(jdata) {
    viewport.clear(); // reset viewport
    $('ul.variables').html(""); // reset variables
    // reset entity objects
    mRoot.infoEntities = new Collection();
    mRoot.objEntities = new Collection();

    // reset connection objects
    mRoot.infoConnections = new Collection();
    mRoot.objConnections = new Collection();

    // reset variable objects
    mRoot.infoVariables = new Collection();

    var root = jdata.WorkflowService["ftwa:StartWorkflow"]["ftwa:StartWorkflow.Activities"]["p:Flowchart"], // get from service to start workflow -> flowchart
        managers = jdata.WorkflowService["sap2010:WorkflowViewState.ViewStateManager"]["sap2010:ViewStateManager"]["sap2010:ViewStateData"], // get from service
        variables = root["p:Flowchart.Variables"]; // 

    // get start
    var root_start = root["@StartNode"] !== undefined ? root["@StartNode"] : root["p:Flowchart.StartNode"],
        start = new Entity();
    start.label = "Start";
    start.annotation = root["@sap2010:Annotation.AnnotationText"] !== undefined ? root["@sap2010:Annotation.AnnotationText"] : "";
    start.type = typeEntity.t_start;
    start.alias = root["@sap2010:WorkflowViewState.IdRef"] === undefined ? root["sap2010:WorkflowViewState.IdRef"] : root["@sap2010:WorkflowViewState.IdRef"];
    mRoot.infoEntities.add(start);

    // get variables 
    getVariables(variables);
    // get decisions and switchs
    getEntities(root);

    // get position info and connection
    mRoot.infoEntities.forEach(function (entity) {
        for (var i = 0; i < managers.length; i++) {
            var node = managers[i];
            if (node["@Id"] === entity.alias) {
                var points = node["sap:WorkflowViewStateService.ViewState"]["scg:Dictionary"]["av:Point"]["#text"].split(",");
                var sizes = node["sap:WorkflowViewStateService.ViewState"]["scg:Dictionary"]["av:Size"]["#text"].split(",");
                entity.x = parseFloat(points[0]);
                entity.y = parseFloat(points[1]);
                entity.width = parseFloat(sizes[0]);
                entity.height = parseFloat(sizes[1]);

                if (entity.type !== typeEntity.t_approve && entity.type !== typeEntity.t_generic) {
                    mRoot.objEntities.add(drawEntity(viewport, entity));
                } else {
                    mRoot.objEntities.add(drawEntityFlow(viewport, entity));
                }

                if (entity.type === typeEntity.t_start) { // size of flowchart belong to start
                    // resize viewport
                    var size = node["@sap:VirtualizedContainerService.HintSize"].split(","); // hash size w/h
                    mViewport = {
                        width: parseFloat(size[0]) < defaultValue.viewport.width ? defaultValue.viewport.width : parseFloat(size[0]),
                        height: parseFloat(size[1]) < defaultValue.viewport.height ? defaultValue.viewport.height : parseFloat(size[1])
                    };
                    $('#viewport').width(mViewport.width);
                    $('#viewport').height(mViewport.height);
                    viewport.setSize(mViewport.width, mViewport.height); //  set new size of viewport from data

                    // reset scroll
                    if (scrollbar !== null) {
                        scrollbar.destroy();
                    }
                    scrollbar = $('#chart').jScrollPane().data().jsp;
                }
            }
        }
    });

    mRoot.infoEntities.forEach(function (entity) {
        for (var i = 0; i < managers.length; i++) {
            var node = managers[i];
            if (node["@Id"] === entity.alias) {
                // get connections
                if (node["sap:WorkflowViewStateService.ViewState"]["scg:Dictionary"]["av:PointCollection"] !== undefined) {
                    var info = node["sap:WorkflowViewStateService.ViewState"]["scg:Dictionary"]["av:PointCollection"];
                    if (node["sap:WorkflowViewStateService.ViewState"]["scg:Dictionary"]["x:String"] !== undefined) {
                        labelDefault = node["sap:WorkflowViewStateService.ViewState"]["scg:Dictionary"]["x:String"]["#text"];
                        getConnections(entity, info, labelDefault);
                    } else {
                        getConnections(entity, info);
                    }
                }
            }
        }
    });
    mRoot.infoConnections.forEach(function (obj) {
        var connection = createConnection(obj);
        // can't draw label connnection begin/end is start
        if (mRoot.infoEntities.itemByID(obj.from).type !== typeEntity.t_start &&
            mRoot.infoEntities.itemByID(obj.to).type !== typeEntity.t_start) {
            // it's decision => set connection is true or false
            if (mRoot.infoEntities.itemByID(obj.from).type === typeEntity.t_decision) {
                if (obj.fromHolder == 2) {
                    obj.label = 'False';
                } else {
                    obj.label = 'True';
                }
            }
            // description of connection from switch
            obj.description = obj.label;

            // connection from generic/approve, it not labels
            if (mRoot.infoEntities.itemByID(obj.from).type !== typeEntity.t_generic &&
                mRoot.infoEntities.itemByID(obj.from).type !== typeEntity.t_approve) {
                // draw temp a label for get width/height.
                var temp = viewport.text(-100, -100, obj.label).attr({ "font-size": "10px" });
                connection.label = drawLabel(connection.posLabel.x, connection.posLabel.y, temp.getBBox().width, temp.getBBox().height, obj.label, viewport);
                connection.label.mask.parent = connection;
                temp.remove();
            }
        }

        mRoot.objConnections.add(connection);
    });
}

// fn load
function actionLoad() {
    var wf = $('#workflows').val(),
        oldproject;
    if ($.trim(wf) === '') {
        $('#workflows').focus();
        return;
    }
    oldproject = project;
    project = wf;

    $.ajax({ // get data xamlx
        type: 'post',
        url: 'Sketch/LoadWorkflow',
        data: { fname: wf },
        datatype: 'json',
        success: function (data) {
            if (data !== 'Error') {
                $("#viewport").show();
                $("#workflow-name").html(wf).show();
                $("#save").show();


                workflowData = null;
                data = $.parseJSON(data);
                workflowData = data;
                getDataBySystem(data);
            } else {
                $(".message").html("This Workflow does not exist! Please create a new Workflow");

                $('#confirm-popup-modal').dialog("option", "buttons", [
                    {
                        text: "Close", click: function () {
                            project = oldproject;
                            $(this).dialog("close");
                        }
                    }
                ]).dialog("open");
            }
        },
        beforeSend: function (xhr) {
            showSketchLoading(true);
        }
    }).done(function () {
        showSketchLoading(false);
    });
}

/*------------------------------------ SAVE -------------------------------*/

// handle save - save a xamlx file on system
var formFlowchart = null;

// action save
function actionSave(callback) {
    formFlowchart = null;
    // get from data (sample content from file .xamlx)
    $.ajax({
        type: 'post',
        url: 'Sketch/GetFormFlowchart',
        data: {},
        datatype: 'json',
        success: function(data) {
            formFlowchart = $.parseJSON(data);
            if (formFlowchart != null) {
                // root is flowchart in file xamlxs
                var root = formFlowchart["WorkflowService"]["ftwa:StartWorkflow"]["ftwa:StartWorkflow.Activities"]["p:Flowchart"];
                var manage = formFlowchart["WorkflowService"]["sap2010:WorkflowViewState.ViewStateManager"]["sap2010:ViewStateManager"]["sap2010:ViewStateData"];
                // edit attribute root
                root = {
                    "@sap2010:WorkflowViewState.IdRef": "Flowchart"
                };
                // variable
                handleVariables(root["p:Flowchart.Variables"] = {}, setVariables());

                // check connection => 2 groups connections from/to
                handleConnections(setConnections());

                // handle tree entities with connection
                var objects = {};
                //getFlowchartObjectHandle(objects, setEntities());
                handleObjects(objects, setEntities());

                // handle data flowchart send server save
                handleFlowchart(objects, root, manage);

                // handle reference
                if (groupConnectionsIDTos.length !== 0) {
                    root["x:Reference"] = new Array();
                    for (var j = 0; j < groupConnectionsIDTos.length; j++) {
                        var flag = false;
                        for (var i = 0; i < arrUnReferences.length; i++) {
                            if (arrUnReferences[i] === groupConnectionsIDTos[j].idto) {
                                flag = true;
                                break;
                            }
                        }
                        if (!flag) {
                            root["x:Reference"].push("__ReferenceID" + j);
                        }
                    }
                }

                // rename
                function limitCharacter(str) {
                    return str.replaceAll(" ", "_").replaceAll("(", "").replaceAll(")", "");
                }

                var newname = project;
                newname = limitCharacter(newname);
                formFlowchart["WorkflowService"]["@ConfigurationName"] = newname;
                formFlowchart["WorkflowService"]["@Name"] = newname;
                // set references/namspaces
                formFlowchart["WorkflowService"]["@p:TextExpression.Namespaces"] = "{x:Reference __ReferenceID" + groupConnectionsIDTos.length + "}";
                formFlowchart["WorkflowService"]["@p:TextExpression.References"] = "{x:Reference __ReferenceID" + (groupConnectionsIDTos.length + 1) + "}";

                formFlowchart["WorkflowService"]["ftwa:StartWorkflow"]["p:TextExpression.Namespaces"]["sco:Collection"]["@x:Name"] = "__ReferenceID" + groupConnectionsIDTos.length;
                formFlowchart["WorkflowService"]["ftwa:StartWorkflow"]["p:TextExpression.References"]["sco:Collection"]["@x:Name"] = "__ReferenceID" + (groupConnectionsIDTos.length + 1);

                // parse xml + send servers
                formFlowchart["WorkflowService"]["ftwa:StartWorkflow"]["ftwa:StartWorkflow.Activities"]["p:Flowchart"] = root;
                var content = json2xml(formFlowchart);
                $.ajax({
                    type: 'post',
                    url: 'Sketch/SaveWorkflow',
                    data: {
                        oldname: $("#newname").val(),
                        fname: project,
                        content: content
                    },
                    datatype: 'json',
                    success: function(data) {
                        if (data !== 'Error') {
                            checkModified(isModified = false);
                        }
                        callback();
                        //handleFlowtask();
                        handleGetWorkflows();
                    },
                    error: function(error) {
                        var task = "<div>Error: Sorry there was an error while saving the workflow. Please contact your system administrator.</div>";

                        $(".message").html(task);
                        $("#confirm-popup-modal").dialog("option", "buttons", [
                            {
                                text: "Ok",
                                click: function() {
                                    $(this).dialog("close");
                                }
                            }
                        ]);
                        $("#confirm-popup-modal").dialog("option", "title", "Notification");
                        $("#confirm-popup-modal").dialog("option", "height", "140").dialog("open");
                    },
                    beforeSend: function(xhr) {
                        showSketchLoading(true);
                    }
                }).done(function() {
                    showSketchLoading(false);
                });
            }
        },
        error: function(error) {
            var task = "<div>Error: Sorry there was an error while saving the workflow. Please contact your system administrator.</div>";

            $(".message").html(task);
            $("#confirm-popup-modal").dialog("option", "buttons", [
                {
                    text: "Ok",
                    click: function() {
                        $(this).dialog("close");
                    }
                }
            ]);
            $("#confirm-popup-modal").dialog("option", "title", "Notification");
            $("#confirm-popup-modal").dialog("option", "height", "140").dialog("open");

            showSketchLoading(false);
        },
        beforeSend: function(xhr) {
            showSketchLoading(true);
        }
    });
}
// fn set array value of entities when submit to server
function setEntities() {
    var entitiesArr = new Array();
    var entitiesCurrent = new Array();
    for (var i = 0; i < mRoot.objEntities.count; i++) {
        var entity = mRoot.objEntities.collection[i];
        if (!checkIn(entity.info.id, entitiesCurrent)) {
            if (entity.info.type === typeEntity.t_start) {
                entitiesCurrent.push(entity.info.id);
                var item = {
                    x: entity.entity.attr("x"),
                    y: entity.entity.attr("y"),
                    width: entity.entity.attr("width"),
                    height: entity.entity.attr("height"),
                    id: entity.info.id,
                    annotation: entity.info.annotation,
                    label: entity.info.label,
                    type: entity.info.type,
                    sizeW: $("#viewport").width(),
                    sizeH: $("#viewport").height()
                }
                entitiesArr.push(item);
            } else {
                var parent = checkParent(entity);
                if (parent != entity) {
                    entitiesArr.push(checkChildren(parent));
                } else {
                    entitiesArr.push(checkChildren(entity));
                }
            }
        }
    }
    return entitiesArr;

    function checkChildren(entity) {
        // check connection
        if (entity.info._case !== undefined) {
            entity.info._case = undefined;
        }
        if (entity.info._true !== undefined) {
            entity.info["_true"] = undefined;
        }
        if (entity.info._false !== undefined) {
            entity.info["_false"] = undefined;
        }
        if (entity.info._next !== undefined) {
            entity.info["_next"] = undefined;
        }
        var item = entity.info;
        item.x = entity.entity.attr("x"),
        item.y = entity.entity.attr("y"),
        item.width = entity.entity.attr("width"),
        item.height = entity.entity.attr("height"),

        item.child = new Array();
        if (entity.info.type === typeEntity.t_decision) {
            item.condition = entity.info.condition;
        }
        if (entity.info.type === typeEntity.t_switch) {
            item.expression = entity.info.expression;
        }
        entitiesCurrent.push(entity.info.id);
        for (var i = 0; i < mRoot.objConnections.count; i++) {
            var conn = mRoot.objConnections.collection[i];
            if (conn.info.from === entity.info.id) {
                for (var j = 0; j < mRoot.objEntities.count; j++) {
                    var entityTo = mRoot.objEntities.collection[j];
                    if (!checkIn(entityTo.info.id, entitiesCurrent)) {
                        if (conn.info.to === entityTo.info.id) {
                            var obj = checkChildren(entityTo);
                            item.child.push(obj);
                        }
                    }
                }
            }
        }
        return item;
    }

    function checkParent(entity, parent) {
        for (var i = 0; i < mRoot.objConnections.count; i++) {
            var conn = mRoot.objConnections.collection[i];
            if (conn.info.to == entity.info.id) {
                for (var j = 0; j < mRoot.objEntities.count; j++) {
                    var entityFrom = mRoot.objEntities.collection[j];
                    if (parent !== undefined && parent === entityFrom) {
                        return entity;
                    }
                    if (conn.info.from == entityFrom.info.id) {
                        if (entityFrom.info.type === typeEntity.t_start) {
                            return entity;
                        }
                        if (entity === entityFrom) {
                            return entity;
                        }
                        return checkParent(entityFrom, entity);
                    }
                }
            }
        }
        return entity;
    }
}
function checkIn(id, arr) {
    for (var i = 0; i < arr.length; i++) {
        if (id === arr[i]) {
            return true;
        }
    }
    return false;
}
// fn set array value of connection when submit to server
function setConnections() {
    var connections = new Array();
    mRoot.objConnections.forEach(function (connection) {
        var points = new Array();
        var paths = connection.mask.attr("path");
        for (var i = 0; i < paths.length; i++) {
            var pos = String(paths[i][1]) + "," + String(paths[i][2]);
            var flag = false; // flag
            for (var j = 0; j < points.lenght; j++) {
                if (pos === points[j]) { // set this position existing in points array?
                    flag = true; // turn on flag
                }
            }
            if (!flag) { // not existing
                points.push(pos); // push it
            }
        }
        var item = {
            points: points,
            label: connection.info.label,
            from: connection.info.from,
            to: connection.info.to,
            id: connection.info.id,
            isDefault: connection.info.isDefault === undefined ? false : connection.info.isDefault
        };
        connections.push(item);
    });
    return connections;
}
// fn set array value of variables when submit to server
function setVariables() {
    var vars = new Array();
    mRoot.infoVariables.forEach(function (variable) {
        var _var = {
            name: variable.name,
            type: variable.type,
            value: variable.value
        };
        vars.push(_var);
    });
    return vars;
}

function getFlowchartObjectHandle(root, entities) {
    for (var i = 0; i < entities.length; i++) {
        var entity = entities[i];
        if (entity.type === typeEntity.t_start) {
            root.start = entity;
        } else if (entity.type === typeEntity.t_decision) {
            if (root.decision !== undefined) {
                if (typeof (root.decision) === "object" && root.decision.length !== undefined) {
                    root.decision.push(entity);
                } else {
                    var temp = root.decision;
                    root.decision = new Array();
                    root.decision.push(temp, entity);
                }
            } else {
                root.decision = entity;
            }
            if (entity.child.length != 0) {
                if (root.decision.length !== undefined) {
                    root.decision[root.decision.length - 1].link = {};
                    getFlowchartObjectHandle(root.decision[root.decision.length - 1].link, entity.child);
                } else {
                    root.decision.link = {};
                    getFlowchartObjectHandle(root.decision.link, entity.child);
                }
            }
        } else if (entity.type === typeEntity.t_switch) {
            if (root.switch !== undefined) {
                if (typeof (root.switch) === "object" && root.switch.length !== undefined) {
                    root.switch.push(entity);
                } else {
                    var temp = root.switch;
                    root.switch = new Array();
                    root.switch.push(temp, entity);
                }

            } else {
                root.switch = entity;
            }
            if (entity.child.lenght != 0) {
                if (root.switch.length !== undefined) {
                    root.decision[root.switch.length - 1].link = {};
                    getFlowchartObjectHandle(root.switch[root.switch.length - 1].link, entity.child);
                } else {
                    root.switch.link = {};
                    getFlowchartObjectHandle(root.switch.link, entity.child);
                }
            }
        } else {
            if (root.step !== undefined) {
                if (typeof (root.step) === "object" && root.step.length !== undefined) {
                    root.step.push(entity);
                } else {
                    var temp = root.step;
                    root.step = new Array();
                    root.step.push(temp, entity);
                }

            } else {
                root.step = entity;
            }
            if (entity.step.length != 0) {
                if (root.switch.length !== undefined) {
                    root.decision[root.step.length - 1].link = {};
                    getFlowchartObjectHandle(root.switch[root.step.length - 1].link, entity.child);
                } else {
                    root.switch.link = {};
                    getFlowchartObjectHandle(root.step.link, entity.child);
                }
            }
        }
    }
}

// check in connection
function checkConnection(item, connections) {
    for (var i = 0; i < connections.length; i++) {
        var conn = connections[i];
        if (conn.from == item.id) {
            return true;
        }
    }
    return false;
}

var groupConnectionsIDFroms = new Array();
var groupConnectionsIDTos = new Array();

// handle get group connections from/to 
function handleConnections(connections) {
    groupConnectionsIDFroms = new Array();
    groupConnectionsIDTos = new Array();

    var idFroms = getIDFroms(connections);
    for (var i = 0; i < idFroms.length; i++) {
        var key = 0;
        var arr = new Array();
        while (key < connections.length) {
            var conn = connections[key];
            if (conn.from == idFroms[i]) {
                arr.push(conn);
            }
            key++;
        }
        groupConnectionsIDFroms.push({
            idfrom: idFroms[i],
            connections: arr
        });
    }
    //groupConnectionsIDTos = getIDTos(connections);
    groupConnectionsIDTos = new Array();
    var idTos = getIDTos(connections);
    for (var i = 0; i < idTos.length; i++) {
        var key = 0;
        var arr = new Array();
        while (key < connections.length) {
            var conn = connections[key];
            if (conn.to == idTos[i]) {
                arr.push(conn);
            }
            key++;
        }
        groupConnectionsIDTos.push({
            idto: idTos[i],
            connections: arr
        });
    }

    // get list id by connection form
    function getIDFroms(connections) {
        var arr = new Array();
        for (var i = 0; i < connections.length; i++) {
            var conn = connections[i];
            if (!checkIn(conn.from, arr)) {
                arr.push(conn.from);
            }
        }
        return arr;
    }

    // get list id by connection to
    function getIDTos(connections) {
        var arr = new Array();
        for (var i = 0; i < connections.length; i++) {
            var conn = connections[i];
            if (!checkIn(conn.to, arr)) {
                arr.push(conn.to);
            }
        }
        return arr;
    }

    // check existing of value in array
    function checkIn(value, arr) {
        for (var i = 0; i < arr.length; i++) {
            if (value == arr[i]) {
                return true;
            }
        }
        return false;
    }
}

// handle variable
function handleVariables(root, variables) {
    if (variables.length !== 0) {
        if (root["p:Variable"] === undefined) {
            root["p:Variable"] = new Array();
        }
        for (var i = 0; i < variables.length; i++) {
            var _var = {
                "@Name": encodeXml(variables[i].name),
                "@x:TypeArguments": "x:" + variables[i].type
            };
            if (variables[i].value !== "") {
                if (variables[i].type.toLowerCase === "object") {
                    _var["p:Variable.Default"] = {
                        "p:Literal": {
                            "@x:TypeArguments": "x:" + variables[i].type,
                            "#text": encodeXml(variables[i].value)
                        }
                    };
                } else {
                    _var["p:Variable.Default"] = {
                        "mca:CSharpValue": {
                            "@x:TypeArguments": "x:" + variables[i].type,
                            "#text": encodeXml(variables[i].value)
                        }
                    };
                }
                //_var["@Default"] = encodeXml(variables[i].value);
            }
            root["p:Variable"].push(_var);
        }
    }
}

function getConnectionFrom(id, arr) {
    for (var i = 0; i < arr.length; i++) {
        if (id == arr[i].idfrom) {
            return arr[i].connections;
        }
    }
    return null;
}

function getInfoConection(from, to, arr) {
    for (var i = 0; i < arr.length; i++) {
        if (form == arr[i].from && to == arr[i].to) {
            return arr[i];
        }
    }
    return null;
}

function countLinking(obj) {
    var counter = 0;
    for (item in obj) {
        if (obj.hasOwnProperty(item)) {
            counter++;
        }
    }
    return counter
}

// array can't show references
var arrUnReferences = new Array();

// handle object entitie in flowchart sketchjs
function handleObjects(root, entities) {
    for (var i = 0; i < entities.length; i++) {
        var entity = entities[i];
        if (entity.child !== undefined && entity.child.length != 0) {
            for (var j = 0; j < entity.child.length; j++) {
                var child = entity.child[j];
                check(child, entity);
            }
        }
        if (entity.type === typeEntity.t_start) {
            root.start = entity;
        } else if (entity.type === typeEntity.t_decision) {
            if (root.decision === undefined) {
                root.decision = new Array();
            }
            root.decision.push(entity);
        } else if (entity.type === typeEntity.t_switch) {
            if (root.switch === undefined) {
                root.switch = new Array();
            }
            root.switch.push(entity);
        } else {
            if (root.step === undefined) {
                root.step = new Array();
            }
            root.step.push(entity);
        }
    }

    function check(entity, parent) {
        if (entity.child.length !== 0) {
            for (var i = 0; i < entity.child.length; i++) {
                var child = entity.child[i];
                check(child, entity);
            }
        }
        if (parent.type === typeEntity.t_decision) {
            if (checkconnection(parent.id, entity.id, groupConnectionsIDFroms).toLowerCase() === "false") {
                parent._false = entity;
            }
            if (checkconnection(parent.id, entity.id, groupConnectionsIDFroms).toLowerCase() === "true") {
                parent._true = entity;
            }
        }

        if (parent.type === typeEntity.t_switch) {
            if (parent._case === undefined) {
                parent._case = new Array();
            }
            if (checkconnection(parent.id, entity.id, groupConnectionsIDFroms).toLowerCase() !== "true"
                || checkconnection(parent.id, entity.id, groupConnectionsIDFroms).toLowerCase() !== "false"
                || checkconnection(parent.id, entity.id, groupConnectionsIDFroms).toLowerCase() !== "") {
                parent._case.push(entity);
            }
        }
        if (parent.type === typeEntity.t_approve || parent.type === typeEntity.t_generic) {
            if (checkconnection(parent.id, entity.id, groupConnectionsIDFroms).toLowerCase() === "default"
                || checkconnection(parent.id, entity.id, groupConnectionsIDFroms).toLowerCase() === "") {
                parent._next = entity;
            }
        }
    }

    function checkconnection(from, to, connections) {
        for (var i = 0; i < connections.length; i++) {
            var conn = connections[i];
            if (conn.idfrom == from) {
                for (var j = 0; j < conn.connections.length; j++) {
                    var entityTo = conn.connections[j];
                    if (entityTo.to == to) {
                        return entityTo.label;
                    }
                }
            }
        }
        return "";
    }
}

function handleFlowchart(objects, root, manage) {
    arrUnReferences = new Array();
    if (objects.start !== undefined) {
        if (checkConnection(objects.start, setConnections())) {
            var id = 0;
            for (var i = 0; i < groupConnectionsIDFroms.length; i++) {
                if (groupConnectionsIDFroms[i].idfrom === objects.start.id) {
                    for (var j = 0; j < groupConnectionsIDTos.length; j++) {
                        if (groupConnectionsIDFroms[i].connections[0].to === groupConnectionsIDTos[j].idto) {
                            id = j;
                        }
                    }
                }
            }
            root["@StartNode"] = "{x:Reference __ReferenceID" + id + "}";
        } else {
            root["p:Flowchart.StartNode"] = {
                "x:Null": null
            }
        }
        if (objects.start.annotation !== "") {
            root["@sap2010:Annotation.AnnotationText"] = encodeXml(objects.start.annotation);
        }
        for (var i = 0; i < manage.length; i++) {
            var item = manage[i];
            // edit infor flowchart
            if (item["@Id"].toLowerCase() === "flowchart") {
                item["@sap:VirtualizedContainerService.HintSize"] = objects.start["sizeW"] + "," + objects.start["sizeH"];
                item["sap:WorkflowViewStateService.ViewState"]["scg:Dictionary"]["av:Point"]["#text"] = objects.start["x"] + "," + objects.start["y"];
                if (checkConnection(objects.start, setConnections())) {
                    item["sap:WorkflowViewStateService.ViewState"]["scg:Dictionary"]["av:PointCollection"] = {
                        "@x:Key": "ConnectorLocation",
                        "#text": getPoints(objects.start.id, groupConnectionsIDFroms)[0]["points"]
                    }
                }
            }
        }
    }
    if (objects.decision !== undefined) {
        var decisions = objects.decision;
        if (typeof (decisions) === "object" && decisions.length !== undefined) {
            if (decisions.length > 1) {
                root["p:FlowDecision"] = new Array();
            } else {
                root["p:FlowDecision"] = {};
            }

            for (var i = 0; i < decisions.length; i++) {
                var decision = decisions[i];
                var item = {
                    "@sap2010:WorkflowViewState.IdRef": "FlowDecision_" + decision["id"]
                };
                // reference id
                var referenceID = getReference(decision.id, groupConnectionsIDTos)
                if (referenceID !== false) {
                    item["@x:Name"] = "__ReferenceID" + referenceID;
                    arrUnReferences.push(decision.id);
                }
                // label
                if (decision["label"].toLowerCase() !== "decision") {
                    item["@DisplayName"] = encodeXml(decodeXml(decision["label"]));
                }
                // condition
                if (decision["condition"] !== "") {
                    //item["@Condition"] = "[" + decision["condition"] + "]";
                    item["p:FlowDecision.Condition"] = {
                        "mca:CSharpValue": {
                            "@x:TypeArguments": "x:Boolean",
                            "#text": encodeXml(decodeXml(decision["condition"]))
                        }
                    };
                }
                // annotation
                if (decision.annotation !== "") {
                    item["@sap2010:Annotation.AnnotationText"] = encodeXml(decodeXml(decision.annotation));
                }
                var arrExisting = new Array();
                // connect to false
                if (decision._false !== undefined) {// false connection
                    item["p:FlowDecision.False"] = {};
                    check(decision._false, decision, item["p:FlowDecision.False"], manage);
                    arrExisting.push(decision._false.id);
                }
                // connect to true
                if (decision._true !== undefined) {// true connection
                    item["p:FlowDecision.True"] = {};
                    check(decision._true, decision, item["p:FlowDecision.True"], manage);
                    arrExisting.push(decision._true.id);
                }
                // get other reference
                if (decision._false !== undefined || decision._true !== undefined) {
                    var id = null, flag = false;
                    if (decision._false === undefined) { // not existing connection false
                        id = decision._true.id; // get id from true connection
                        flag = false; // flag is false
                    } else { // true
                        id = decision._false.id; // get id from false connection
                        flag = true; // flag is true
                    }
                    var references = getReferences(decision.id, id, groupConnectionsIDFroms);
                    if (references.length > 1) {
                        if (item["x:Reference"] === undefined) {
                            item["x:Reference"] = new Array();
                        }
                        for (var k = 1; k < references.length; k++) {
                            if (flag) {
                                item["p:FlowDecision.True"] = {
                                    "x:Reference": {
                                        "#text": "__ReferenceID" + getReferenceID(decision.id, id, groupConnectionsIDTos),
                                    }
                                }
                            } else {
                                item["p:FlowDecision.False"] = {
                                    "x:Reference": {
                                        "#text": "__ReferenceID" + getReferenceID(decision.id, id, groupConnectionsIDTos),
                                    }
                                }
                            }
                        }
                    }
                }

                var references = getConnectionOtherReferences(getOtherReference(decision.id, groupConnectionsIDFroms), groupConnectionsIDTos);
                if (references.length !== 0) {
                    for (var m = 0; m < references.length; m++) {
                        var refer = references[m];
                        for (var n = 0; n < refer.conn.connections.length; n++) {
                            if (!checkReferenceExisting(refer.conn.connections[n].to, arrExisting)) {
                                if (refer.conn.connections[n].label.toLowerCase() === "false" && refer.conn.connections[n].from == decision.id) {
                                    item["@False"] = "{x:Reference __ReferenceID" + refer.id + "}";
                                } else if (refer.conn.connections[n].label.toLowerCase() === "true" && refer.conn.connections[n].from == decision.id) {
                                    item["@True"] = "{x:Reference __ReferenceID" + refer.id + "}";
                                }
                            }
                        }
                    }
                }

                // push it json
                if (decisions.length > 1) {
                    root["p:FlowDecision"].push(item);
                } else {
                    root["p:FlowDecision"] = item;
                }
                // get info
                info = {
                    "@Id": "FlowDecision_" + decision["id"],
                    "@sap:VirtualizedContainerService.HintSize": decision["width"] + "," + decision["height"],
                    "sap:WorkflowViewStateService.ViewState": {
                        "scg:Dictionary": {
                            "@x:TypeArguments": "x:String, x:Object",
                            "av:Point": {
                                "#text": decision["x"] + "," + decision["y"],
                                "@x:Key": "ShapeLocation"
                            },
                            "av:Size": {
                                "#text": decision["width"] + "," + decision["height"],
                                "@x:Key": "ShapeSize"
                            },
                            "x:Boolean": {
                                "#text": "False",
                                "@x:Key": "IsExpanded"
                            }
                        }
                    }
                };
                // check connection get point collection
                if (checkConnection(decision, setConnections())) {
                    var list = getPoints(decision.id, groupConnectionsIDFroms);
                    if (list.length > 1) {
                        var points = new Array();
                        for (var m = 0; m < list.length; m++) {
                            var item = {
                                "#text": list[m]["points"],
                                "@x:Key": list[m]["label"] + "Connector"
                            }
                            points.push(item);
                        }
                        info["sap:WorkflowViewStateService.ViewState"]["scg:Dictionary"]["av:PointCollection"] = points;
                    } else if (list.length == 1) {
                        info["sap:WorkflowViewStateService.ViewState"]["scg:Dictionary"]["av:PointCollection"] = {
                            "#text": list[0]["points"],
                            "@x:Key": list[0]["label"] + "Connector"
                        }
                    }
                }

                manage.push(info);
            }
        }
    }
    if (objects.switch !== undefined) {
        var switchs = objects.switch;
        if (typeof (switchs) === "object" && switchs.length !== undefined) {
            if (switchs.length > 1) {
                root["p:FlowSwitch"] = new Array();
            } else {
                root["p:FlowSwitch"] = {};
            }
            for (var i = 0; i < switchs.length; i++) {
                var swit = switchs[i];
                var item = {
                    "@sap2010:WorkflowViewState.IdRef": "FlowSwitch_" + swit["id"],
                    "@x:TypeArguments": "x:" + swit["typeSwitch"]
                };
                // reference id
                var referenceID = getReference(swit.id, groupConnectionsIDTos);
                if (referenceID !== false) {
                    item["@x:Name"] = "__ReferenceID" + referenceID;
                    arrUnReferences.push(swit.id);
                }
                // label
                if (swit["label"].toLowerCase() !== "switch") {
                    item["@DisplayName"] = encodeXml(decodeXml(swit["label"]));
                }
                // expression
                if (swit.expression !== "") {
                    item["p:FlowSwitch.Expression"] = {
                        "mca:CSharpValue": {
                            "@x:TypeArguments": "x:" + swit.typeSwitch,
                            "#text": encodeXml(decodeXml(swit.expression))
                        }
                    }
                }
                // annotation
                if (swit.annotation !== "") {
                    item["@sap2010:Annotation.AnnotationText"] = encodeXml(decodeXml(swit.annotation));
                }
                // case
                if (swit._case !== undefined) {
                    arrExisting = new Array();
                    if (swit._case.length === 1) {
                        var connect = getInfoConnection(swit.id, swit._case[0].id, groupConnectionsIDFroms);
                        if (connect.isDefault) {
                            item["p:FlowSwitch.Default"] = {};
                            check(swit._case[0], swit, item["p:FlowSwitch.Default"], manage);
                        } else {
                            check(swit._case[0], swit, item, manage);
                        }
                        var references = getReferences(swit.id, swit._case[0].id, groupConnectionsIDFroms);

                        if (item["x:Reference"] === undefined) {
                            item["x:Reference"] = new Array();
                        }
                        for (var k = 0; k < references.length; k++) {
                            if (references[k].id !== connect.id) {
                                var refer = {
                                    "#text": "__ReferenceID" + getReferenceID(swit.id, swit._case[0].id, groupConnectionsIDTos),
                                    "x:Key": references[k].label
                                }
                                item["x:Reference"].push(refer);
                            }
                        }
                        arrExisting.push(swit._case[0].id);
                    } else {
                        for (var j = 0; j < swit._case.length; j++) {
                            var _case = swit._case[j];
                            var connect = getInfoConnection(swit.id, _case.id, groupConnectionsIDFroms);
                            if (connect.isDefault) {
                                item["p:FlowSwitch.Default"] = {};
                                check(_case, swit, item["p:FlowSwitch.Default"], manage);
                            } else { // case other
                                // decision
                                if (_case.type === typeEntity.t_decision) {
                                    if (item["p:FlowDecision"] === undefined) {
                                        var sub = {};
                                        check(_case, swit, sub, manage);
                                        item["p:FlowDecision"] = sub["p:FlowDecision"];
                                    } else {
                                        var temp = item["p:FlowDecision"];
                                        item["p:FlowDecision"] = new Array();
                                        var sub = {};
                                        check(_case, swit, sub, manage);
                                        item["p:FlowDecision"].push(temp, sub["p:FlowDecision"]);
                                    }
                                }
                                // switch
                                if (_case.type === typeEntity.t_switch) {
                                    if (item["p:FlowSwitch"] === undefined) {
                                        var sub = {};
                                        check(_case, swit, sub, manage);
                                        item["p:FlowSwitch"] = sub["p:FlowSwitch"];
                                    } else {
                                        var temp = item["p:FlowSwitch"];
                                        item["p:FlowSwitch"] = new Array();
                                        var sub = {};
                                        check(_case, swit, sub, manage);
                                        item["p:FlowSwitch"].push(temp, sub["p:FlowSwitch"]);
                                    }
                                }
                                // step
                                if (_case.type === typeEntity.t_approve || _case.type === typeEntity.t_generic) {
                                    if (item["p:FlowStep"] === undefined) {
                                        var sub = {};
                                        check(_case, swit, sub, manage);
                                        item["p:FlowStep"] = sub["p:FlowStep"];
                                    } else {
                                        var temp = item["p:FlowStep"];
                                        item["p:FlowStep"] = new Array();
                                        var sub = {};
                                        check(_case, swit, sub, manage);
                                        item["p:FlowStep"].push(temp, sub["p:FlowStep"]);
                                    }
                                }
                            }
                            arrExisting.push(_case.id);
                            var references = getReferences(swit.id, _case.id, groupConnectionsIDFroms);
                            if (references.length > 1) {
                                if (item["x:Reference"] === undefined) {
                                    item["x:Reference"] = new Array();
                                }
                                for (var k = 1; k < references.length; k++) {
                                    var refer = {
                                        "#text": "__ReferenceID" + getReferenceID(swit.id, _case.id, groupConnectionsIDTos),
                                        "x:Key": references[k].label
                                    }
                                    item["x:Reference"].push(refer);
                                }
                            }
                        }
                    }
                    // check other references
                    var references = getConnectionOtherReferences(getOtherReference(swit.id, groupConnectionsIDFroms), groupConnectionsIDTos);
                    if (references.length !== 0) {
                        if (item["x:Reference"] === undefined) {
                            item["x:Reference"] = new Array();
                        }
                        for (var m = 0; m < references.length; m++) {
                            var refer = references[m];
                            for (var n = 0; n < refer.conn.connections.length; n++) {
                                if (!checkReferenceExisting(refer.conn.connections[n].to, arrExisting)) {
                                    if (refer.conn.connections[n].from === swit.id) {
                                        if (refer.conn.connections[n].label !== "") {
                                            //if (refer.conn.connections[n].label.toLowerCase() === "default") 
                                            //if (!$.isNumeric(refer.conn.connections[n].label)) {
                                            var connect = refer.conn.connections[n];
                                            if (connect.isDefault !== undefined && connect.isDefault) {
                                                if (item["p:FlowSwitch.Default"] === undefined) {
                                                    item["@Default"] = "{x:Reference __ReferenceID" + refer.id + "}";
                                                }
                                            } else {
                                                //if (refer.conn.connections[n].label.toLowerCase() !== "false" && refer.conn.connections[n].label.toLowerCase() !== "true") {
                                                item["x:Reference"].push({
                                                    "#text": "__ReferenceID" + refer.id,
                                                    "x:Key": refer.conn.connections[n].label
                                                });
                                                //}
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                } else {
                    var references = getConnectionOtherReferences(getOtherReference(swit.id, groupConnectionsIDFroms), groupConnectionsIDTos);
                    if (references.length !== 0) {
                        if (item["x:Reference"] === undefined) {
                            item["x:Reference"] = new Array();
                        }
                        for (var m = 0; m < references.length; m++) {
                            var refer = references[m];
                            for (var n = 0; n < refer.conn.connections.length; n++) {
                                if (refer.conn.connections[n].from === swit.id) {
                                    if (refer.conn.connections[n].label !== "") {
                                        //if (refer.conn.connections[n].label.toLowerCase() === "default"){
                                        //if (!$.isNumeric(refer.conn.connections[n].label)) {
                                        var connect = refer.conn.connections[n];
                                        if (connect.isDefault !== undefined && connect.isDefault) {
                                            item["@Default"] = "{x:Reference __ReferenceID" + refer.id + "}";
                                        } else {
                                            if (refer.conn.connections[n].from === swit.id) {
                                                item["x:Reference"].push({
                                                    "#text": "__ReferenceID" + refer.id,
                                                    "x:Key": refer.conn.connections[n].label
                                                });
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                // push in json
                if (switchs.length > 1) {
                    root["p:FlowSwitch"].push(item);
                } else {
                    root["p:FlowSwitch"] = item;
                }
                // get info
                info = {
                    "@Id": "FlowSwitch_" + swit["id"],
                    "@sap:VirtualizedContainerService.HintSize": swit["width"] + "," + swit["height"],
                    "sap:WorkflowViewStateService.ViewState": {
                        "scg:Dictionary": {
                            "@x:TypeArguments": "x:String, x:Object",
                            "av:Point": {
                                "#text": swit["x"] + "," + swit["y"],
                                "@x:Key": "ShapeLocation"
                            },
                            "av:Size": {
                                "#text": swit["width"] + "," + swit["height"],
                                "@x:Key": "ShapeSize"
                            },
                            "x:Boolean": {
                                "#text": "False",
                                "@x:Key": "IsExpanded"
                            }
                        }
                    }
                };
                // check connection get point conllection
                if (checkConnection(swit, setConnections())) {
                    var list = getPoints(swit.id, groupConnectionsIDFroms);
                    if (list.length > 1) {
                        var points = new Array();
                        for (var n = 0; n < list.length; n++) {
                            var item = {};
                            var connect = list[n];
                            if (connect.isDefault !== undefined && connect.isDefault) {
                                item = {
                                    "#text": list[n]["points"],
                                    "@x:Key": "Default"
                                }
                                if (connect["label"] !== "Default") {
                                    info["sap:WorkflowViewStateService.ViewState"]["scg:Dictionary"]["x:String"] = {
                                        "@x:Key": "DefaultCaseDisplayName",
                                        "#text": connect["label"]
                                    };
                                }
                            } else {
                                item = {
                                    "#text": connect["points"],
                                    "@x:Key": connect["label"] + "Connector"
                                }
                            }
                            points.push(item);
                        }
                        info["sap:WorkflowViewStateService.ViewState"]["scg:Dictionary"]["av:PointCollection"] = points;
                    } else if (list.length == 1) {
                        //if (list[0]["label"].toLowerCase() === "default") {
                        //if ($.isNumeric(list[0]["label"])) {
                        var connect = list[0];
                        if (connect.isDefault !== undefined && connect.isDefault) {
                            info["sap:WorkflowViewStateService.ViewState"]["scg:Dictionary"]["av:PointCollection"] = {
                                "#text": connect["points"],
                                "@x:Key": "Default"
                            }
                            if (connect["label"] !== "Default") {
                                info["sap:WorkflowViewStateService.ViewState"]["scg:Dictionary"]["x:String"] = {
                                    "@x:Key": "DefaultCaseDisplayName",
                                    "#text": connect["label"]
                                };
                            }
                        } else {
                            info["sap:WorkflowViewStateService.ViewState"]["scg:Dictionary"]["av:PointCollection"] = {
                                "#text": connect["points"],
                                "@x:Key": connect["label"] + "Connector"
                            }
                        }
                    }
                }
                manage.push(info);
            }
        }
    }
    if (objects.step !== undefined) {
        var steps = objects.step;
        if (typeof (steps) === "object" && steps.length !== undefined) {
            if (steps.length > 1) {
                root["p:FlowStep"] = new Array();
            } else {
                root["p:FlowStep"] = {};
            }
            for (var i = 0; i < steps.length; i++) {
                var step = steps[i];
                var item = {
                    "@sap2010:WorkflowViewState.IdRef": "FlowStep_" + step["id"],
                };
                // reference
                var referenceID = getReference(step.id, groupConnectionsIDTos);
                if (referenceID !== false) {
                    item["@x:Name"] = "__ReferenceID" + referenceID;
                    arrUnReferences.push(step.id);
                }
                // approve
                if (step.type === typeEntity.t_approve) {
                    item["ftwa:ApproveTask"] = {
                        //"@AssignResultTo": step.AssignResultTo !== undefined && step.AssignResultTo !== "" ? encodeXml(decodeXml(step.AssignResultTo)) : "{x:Null}",
                        "@AssignedToUsers": step.AssignedToUsers !== undefined && step.AssignedToUsers !== "" ? (decodeXml(step.AssignedToUsers)) : "{x:Null}",
                        "@CorrelationId": step.CorrelationId !== undefined && step.CorrelationId !== "" ? encodeXml(decodeXml(step.CorrelationId)) : "0",
                        "@DefaultResult": step.DefaultResult !== undefined && step.DefaultResult !== "" ? encodeXml(decodeXml(step.DefaultResult)) : "{x:Null}",
                        "@Description": step.Description !== undefined && step.Description !== "" ? encodeXml(decodeXml(step.Description)) : "{x:Null}",
                        "@ExpiresIn": step.ExpiresIn !== undefined && step.ExpiresIn !== "" ? encodeXml(decodeXml(step.ExpiresIn)) : "{x:Null}",
                        "@ExpiresWhen": step.ExpiresWhen !== undefined && step.ExpiresWhen !== "" ? encodeXml(decodeXml(step.ExpiresWhen)) : "{x:Null}",
                        "@HandOverUsers": step.HandOverUsers !== undefined && step.HandOverUsers !== "" ? encodeXml(decodeXml(step.HandOverUsers)) : "{x:Null}",
                        "@OnComplete": step.OnComplete !== undefined && step.OnComplete !== "" ? encodeXml(decodeXml(step.OnComplete)) : "{x:Null}",
                        "@OnInit": step.OnInit !== undefined && step.OnInit !== "" ? encodeXml(decodeXml(step.OnInit)) : "{x:Null}",
                        "@TaskCode": step.TaskCode !== undefined & step.TaskCode !== "" ? encodeXml(decodeXml(step.TaskCode)) : "{x:Null}",
                        "@Title": step.Title !== undefined && step.Title !== "" ? encodeXml(decodeXml(step.Title)) : "{x:Null}",
                        "@UiCode": step.UiCode !== undefined && step.UiCode !== "" ? encodeXml(decodeXml(step.UiCode)) : "{x:Null}",
                        "@sap2010:Annotation.AnnotationText": step.annotation !== undefined && step.annotation !== "" ? encodeXml(decodeXml(step.annotation)) : "{x:Null}",
                        "@sap2010:WorkflowViewState.IdRef": "ApproveTask_" + step["id"]
                    };
                    if (step.AssignResultTo !== undefined && step.AssignResultTo !== "") {
                        item["ftwa:ApproveTask"]["ftwa:ApproveTask.AssignResultTo"] = {
                            "p:OutArgument": {
                                "@x:TypeArguments": "x:String",
                                "mca:CSharpReference": {
                                    "#text": encodeXml(decodeXml(step.AssignResultTo)),
                                    "@x:TypeArguments": "x:String"
                                }
                            }
                        }
                    } else {
                        item["ftwa:ApproveTask"]["@AssignResultTo"] = "{x:Null}";
                    }
                    if (step["label"].toLowerCase() !== "approvetask") {
                        item["ftwa:ApproveTask"]["@DisplayName"] = encodeXml(decodeXml(step["label"]));
                    }
                }
                    // generic
                else if (step.type === typeEntity.t_generic) {
                    item["ftwa:GenericTask"] = {
                        "@OnRun": step.OnRun !== undefined && step.OnRun !== "" ? encodeXml(decodeXml(step.OnRun)) : "{x:Null}",
                        "@sap2010:Annotation.AnnotationText": step.annotation !== "" ? encodeXml(decodeXml(step.annotation)) : "{x:Null}",
                        "@sap2010:WorkflowViewState.IdRef": "GenericTask_" + step["id"]
                    }
                    if (step.TaskCode !== undefined && step.TaskCode !== "") {
                        item["ftwa:GenericTask"]["ftwa:GenericTask.TaskCode"] = {
                            "p:InArgument": {
                                "@x:TypeArguments": "x:String",
                                "mca:CSharpValue": {
                                    "#text": encodeXml(decodeXml(step.TaskCode)),
                                    "@x:TypeArguments": "x:String"
                                }
                            }
                        }
                    } else {
                        item["ftwa:GenericTask"]["@TaskCode"] = "{x:Null}";
                    }
                    if (step["label"].toLowerCase() !== "generictask") {
                        item["ftwa:GenericTask"]["@DisplayName"] = encodeXml(decodeXml(step["label"]));
                    }
                }
                // next
                if (step._next !== undefined) {
                    item["p:FlowStep.Next"] = {};
                    check(step._next, step, item["p:FlowStep.Next"], manage);
                } else {
                    var references = getConnectionOtherReferences(getOtherReference(step.id, groupConnectionsIDFroms), groupConnectionsIDTos);
                    if (references.length !== 0) {
                        item["p:FlowStep.Next"] = {};
                        for (var m = 0; m < references.length; m++) {
                            var refer = references[m];
                            for (var n = 0; n < refer.conn.connections.length; n++) {
                                item["p:FlowStep.Next"] = {
                                    "x:Reference": {
                                        "#text": "__ReferenceID" + refer.id,
                                    }
                                };
                            }
                        }
                    }
                }
                // push in json
                if (steps.length > 1) {
                    root["p:FlowStep"].push(item);
                } else {
                    root["p:FlowStep"] = item;
                }

                var infoStep = {
                    "@Id": "FlowStep_" + step["id"],
                    "sap:WorkflowViewStateService.ViewState": {
                        "scg:Dictionary": {
                            "@x:TypeArguments": "x:String, x:Object",
                            "av:Point": {
                                "#text": step["x"] + "," + step["y"],
                                "@x:Key": "ShapeLocation"
                            },
                            "av:Size": {
                                "#text": step["width"] + "," + step["height"],
                                "@x:Key": "ShapeSize"
                            }
                        }
                    }
                }
                if (checkConnection(step, setConnections())) {
                    var list = getPoints(step.id, groupConnectionsIDFroms);
                    if (list.length > 1) {
                        var points = new Array();
                        for (var m = 0; m < list.length; m++) {
                            var item = {
                                "#text": list[m]["points"],
                                "@x:Key": list[m]["label"] + "Connector"
                            }
                            points.push(item);
                        }
                        infoStep["sap:WorkflowViewStateService.ViewState"]["scg:Dictionary"]["av:PointCollection"] = points;
                    } else if (list.length == 1) {
                        infoStep["sap:WorkflowViewStateService.ViewState"]["scg:Dictionary"]["av:PointCollection"] = {
                            "#text": list[0]["points"],
                            "@x:Key": "ConnectorLocation"
                        }
                    }
                }
                var infoObj = {
                    "@sap:VirtualizedContainerService.HintSize": step["width"] + "," + step["height"],
                    "sap:WorkflowViewStateService.ViewState": {
                        "scg:Dictionary": {
                            "@x:TypeArguments": "x:String, x:Object",
                            "x:Boolean": [{
                                "#text": "True",
                                "@x:Key": "IsExpanded"
                            }, {
                                "#text": "False",
                                "@x:Key": "IsAnnotationDocked"
                            }]
                        }
                    }
                };
                if (steps.type === typeEntity.t_approve) {
                    infoObj["@Id"] = "FlowApprove_" + step["id"];
                } else {
                    infoObj["@Id"] = "FlowGeneric_" + step["id"];
                }

                manage.push(infoStep);
                manage.push(infoObj);
            }
        }
    }

    function check(entity, parent, root, manage) {
        if (entity.type === typeEntity.t_decision) {
            root["p:FlowDecision"] = {};
            var item = {
                "@sap2010:WorkflowViewState.IdRef": "FlowDecision_" + entity["id"],
                "@x:Name": "__ReferenceID" + getReferenceID(parent.id, entity.id, groupConnectionsIDTos)
            };
            // key if parent is Switch
            if (parent.type === typeEntity.t_switch) {
                var connect = getInfoConnection(parent.id, entity.id, groupConnectionsIDFroms);
                if (connect.isDefault !== undefined && connect.isDefault) {
                    item["@x:Key"] = "Default";
                } else {
                    item["@x:Key"] = connect["label"];
                }
            }
            // display name of entity
            if (entity["label"].toLowerCase() !== "decision") {
                item["@DisplayName"] = encodeXml(decodeXml(entity["label"]));
            }
            // condition
            if (entity["condition"] !== "") {
                item["p:FlowDecision.Condition"] = {
                    "mca:CSharpValue": {
                        "@x:TypeArguments": "x:Boolean",
                        "#text": encodeXml(decodeXml(entity["condition"]))
                    }
                };
            }
            // annotation
            if (entity.annotation !== "") {
                item["@sap2010:Annotation.AnnotationText"] = encodeXml(decodeXml(entity.annotation));
            }
            var arrExisting = new Array();
            // false link
            if (entity._false !== undefined) {// false connection
                item["p:FlowDecision.False"] = {};
                check(entity._false, entity, item["p:FlowDecision.False"], manage);
                arrExisting.push(entity._false.id);
            }
            // true link
            if (entity._true !== undefined) {// true connection
                item["p:FlowDecision.True"] = {};
                check(entity._true, entity, item["p:FlowDecision.True"], manage);
                arrExisting.push(entity._true.id);
            }
            // get other reference
            if (entity._false !== undefined || entity._true !== undefined) {
                var id = null, flag = false;
                if (entity._false === undefined) { // not existing connection false
                    id = entity._true.id; // get id from true connection
                    flag = false; // flag is false
                } else { // true
                    id = entity._false.id; // get id from false connection
                    flag = true; // flag is true
                }
                var references = getReferences(entity.id, id, groupConnectionsIDFroms);
                if (references.length > 1) {
                    if (item["x:Reference"] === undefined) {
                        item["x:Reference"] = new Array();
                    }
                    for (var k = 1; k < references.length; k++) {
                        if (flag) {
                            item["p:FlowDecision.True"] = {
                                "x:Reference": {
                                    "#text": "__ReferenceID" + getReferenceID(entity.id, id, groupConnectionsIDTos),
                                }
                            }
                        } else {
                            item["p:FlowDecision.False"] = {
                                "x:Reference": {
                                    "#text": "__ReferenceID" + getReferenceID(entity.id, id, groupConnectionsIDTos),
                                }
                            }
                        }
                    }
                }
            }

            var references = getConnectionOtherReferences(getOtherReference(entity.id, groupConnectionsIDFroms), groupConnectionsIDTos);
            if (references !== 0) {
                for (var i = 0; i < references.length; i++) {
                    var refer = references[i];
                    for (var j = 0; j < refer.conn.connections.length; j++) {
                        if (!checkReferenceExisting(refer.conn.connections[j].to, arrExisting)) {
                            if (refer.conn.connections[j].label.toLowerCase() === "false" && refer.conn.connections[j].from === entity.id) {
                                item["@False"] = "{x:Reference __ReferenceID" + refer.id + "}";
                            } else if (refer.conn.connections[j].label.toLowerCase() === "true" && refer.conn.connections[j].from === entity.id) {
                                item["@True"] = "{x:Reference __ReferenceID" + refer.id + "}";
                            }
                        }
                    }
                }
            }

            root["p:FlowDecision"] = item;

            info = {
                "@Id": "FlowDecision_" + entity["id"],
                "@sap:VirtualizedContainerService.HintSize": entity["width"] + "," + entity["height"],
                "sap:WorkflowViewStateService.ViewState": {
                    "scg:Dictionary": {
                        "@x:TypeArguments": "x:String, x:Object",
                        "av:Point": {
                            "#text": entity["x"] + "," + entity["y"],
                            "@x:Key": "ShapeLocation"
                        },
                        "av:Size": {
                            "#text": entity["width"] + "," + entity["height"],
                            "@x:Key": "ShapeSize"
                        },
                        "x:Boolean": {
                            "#text": "False",
                            "@x:Key": "IsExpanded"
                        }
                    }
                }
            };
            if (checkConnection(entity, setConnections())) {
                var list = getPoints(entity.id, groupConnectionsIDFroms);
                if (list.length > 1) {
                    var points = new Array();
                    for (var i = 0; i < list.length; i++) {
                        var item = {
                            "#text": list[i]["points"],
                            "@x:Key": list[i]["label"] + "Connector"
                        }
                        points.push(item);
                    }
                    info["sap:WorkflowViewStateService.ViewState"]["scg:Dictionary"]["av:PointCollection"] = points;
                } else if (list.length == 1) {
                    info["sap:WorkflowViewStateService.ViewState"]["scg:Dictionary"]["av:PointCollection"] = {
                        "#text": list[0]["points"],
                        "@x:Key": list[0]["label"] + "Connector"
                    }
                }
            }

            manage.push(info);
        }
        if (entity.type === typeEntity.t_switch) {
            root["p:FlowSwitch"] = {};
            // item flowswitch
            var item = {
                "@sap2010:WorkflowViewState.IdRef": "FlowSwitch_" + entity["id"],
                "@x:TypeArguments": "x:" + entity["typeSwitch"],
                "@x:Name": "__ReferenceID" + getReferenceID(parent.id, entity.id, groupConnectionsIDTos)
            };
            // key if parent is Switch
            if (parent.type === typeEntity.t_switch) {
                var connect = getInfoConnection(parent.id, entity.id, groupConnectionsIDFroms);
                if (connect.isDefault !== undefined && connect.isDefault) {
                    item["@x:Key"] = "Default";
                } else {
                    item["@x:Key"] = connect["label"];
                }
            }
            // display name
            if (entity["label"].toLowerCase() !== "switch") {
                item["@DisplayName"] = encodeXml(decodeXml(entity["label"]));
            }
            // expression
            if (entity.expression !== "") {
                item["p:FlowSwitch.Expression"] = {
                    "mca:CSharpValue": {
                        "@x:TypeArguments": "x:" + entity.typeSwitch,
                        "#text": encodeXml(decodeXml(entity.expression))
                    }
                }
            }
            // annotation
            if (entity.annotation !== "") {
                item["@sap2010:Annotation.AnnotationText"] = encodeXml(decodeXml(entity.annotation));
            }
            // if case link to other entities
            if (entity._case !== undefined) {
                var arrExisting = new Array();
                if (entity._case.length === 1) {
                    var connect = getInfoConnection(entity.id, entity._case[0].id, groupConnectionsIDFroms);
                    if (connect.isDefault !== undefined && connect.isDefault) {
                        item["p:FlowSwitch.Default"] = {};
                        check(entity._case[0], entity, item["p:FlowSwitch.Default"], manage);
                    } else {
                        check(entity._case[0], entity, item, manage);
                    }
                    var references = getReferences(entity.id, entity._case[0].id, groupConnectionsIDFroms);
                    if (item["x:Reference"] === undefined) {
                        item["x:Reference"] = new Array();
                    }
                    for (var k = 0; k < references.length; k++) {
                        if (references[k].id !== connect.id) {
                            var refer = {
                                "#text": "__ReferenceID" + getReferenceID(entity.id, entity._case[0].id, groupConnectionsIDTos),
                                "x:Key": references[k].label
                            }
                            item["x:Reference"].push(refer);
                        }
                    }
                    arrExisting.push(entity._case[0].id);
                } else { // > 1 => 1: default, 2-n: other case
                    for (var j = 0; j < entity._case.length; j++) {
                        var _case = entity._case[j];
                        var connect = getInfoConnection(entity.id, _case.id, groupConnectionsIDFroms);
                        if (connect.isDefault !== undefined && connect.isDefault) {
                            item["p:FlowSwitch.Default"] = {};
                            check(_case, entity, item["p:FlowSwitch.Default"], manage);
                        } else { // case other
                            // decision
                            if (_case.type === typeEntity.t_decision) {
                                if (item["p:FlowDecision"] === undefined) {
                                    var sub = {};
                                    check(_case, entity, sub, manage);
                                    item["p:FlowDecision"] = sub["p:FlowDecision"];
                                } else {
                                    var temp = item["p:FlowDecision"];
                                    item["p:FlowDecision"] = new Array();
                                    var sub = {};
                                    check(_case, entity, sub, manage);
                                    item["p:FlowDecision"].push(temp, sub["p:FlowDecision"]);
                                }
                            }
                            // switch
                            if (_case.type === typeEntity.t_switch) {
                                if (item["p:FlowSwitch"] === undefined) {
                                    var sub = {};
                                    check(_case, entity, sub, manage);
                                    item["p:FlowSwitch"] = sub["p:FlowSwitch"];
                                } else {
                                    var temp = item["p:FlowSwitch"];
                                    item["p:FlowSwitch"] = new Array();
                                    var sub = {};
                                    check(_case, entity, sub, manage);
                                    item["p:FlowSwitch"].push(temp, sub["p:FlowSwitch"]);
                                }
                            }
                            // step
                            if (_case.type === typeEntity.t_approve || _case.type === typeEntity.t_generic) {
                                if (item["p:FlowStep"] === undefined) {
                                    var sub = {};
                                    check(_case, entity, sub, manage);
                                    item["p:FlowStep"] = sub["p:FlowStep"];
                                } else {
                                    var temp = item["p:FlowStep"];
                                    item["p:FlowStep"] = new Array();
                                    var sub = {};
                                    check(_case, entity, sub, manage);
                                    item["p:FlowStep"].push(temp, sub["p:FlowStep"]);
                                }
                            }
                        }
                        arrExisting.push(_case.id);
                        var references = getReferences(entity.id, _case.id, groupConnectionsIDFroms);
                        if (references.length > 1) {
                            if (item["x:Reference"] === undefined) {
                                item["x:Reference"] = new Array();
                            }
                            for (var k = 1; k < references.length; k++) {
                                var refer = {
                                    "#text": "__ReferenceID" + getReferenceID(entity.id, _case.id, groupConnectionsIDTos),
                                    "x:Key": references[k].label
                                }
                                item["x:Reference"].push(refer);
                            }
                        }
                    }
                }
                // check other references
                var references = getConnectionOtherReferences(getOtherReference(entity.id, groupConnectionsIDFroms), groupConnectionsIDTos);
                if (references.length !== 0) {
                    if (item["x:Reference"] === undefined) {
                        item["x:Reference"] = new Array();
                    }
                    for (var m = 0; m < references.length; m++) {
                        var refer = references[m];
                        for (var n = 0; n < refer.conn.connections.length; n++) {
                            if (!checkReferenceExisting(refer.conn.connections[n].to, arrExisting)) {
                                if (refer.conn.connections[n].from === entity.id) {
                                    if (refer.conn.connections[n].label !== "") {
                                        var connect = refer.conn.connections[n];
                                        if (connect.isDefault !== undefined && connect.isDefault) {
                                            if (item["p:FlowSwitch.Default"] === undefined) {
                                                item["@Default"] = "{x:Reference __ReferenceID" + refer.id + "}";
                                            }
                                        } else {
                                            if (refer.conn.connections[n].label.toLowerCase() !== "false" && refer.conn.connections[n].label.toLowerCase() !== "true") {
                                                item["x:Reference"].push({
                                                    "#text": "__ReferenceID" + refer.id,
                                                    "x:Key": refer.conn.connections[n].label
                                                });

                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            } else {
                var references = getConnectionOtherReferences(getOtherReference(entity.id, groupConnectionsIDFroms), groupConnectionsIDTos);
                if (references.length !== 0) {
                    if (item["x:Reference"] === undefined) {
                        item["x:Reference"] = new Array();
                    }
                    for (var m = 0; m < references.length; m++) {
                        var refer = references[m];
                        for (var n = 0; n < refer.conn.connections.length; n++) {
                            if (refer.conn.connections[n].from === entity.id) {
                                if (refer.conn.connections[n].label !== "") {
                                    var connect = refer.conn.connections[n];
                                    if (connect.isDefault !== undefined && connect.isDefault) {
                                        item["@Default"] = "{x:Reference __ReferenceID" + refer.id + "}";
                                    } else {
                                        if (refer.conn.connections[n].from === entity.id) {
                                            item["x:Reference"].push({
                                                "#text": "__ReferenceID" + refer.id,
                                                "x:Key": refer.conn.connections[n].label
                                            });
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            root["p:FlowSwitch"] = item;

            info = {
                "@Id": "FlowSwitch_" + entity["id"],
                "@sap:VirtualizedContainerService.HintSize": entity["width"] + "," + entity["height"],
                "sap:WorkflowViewStateService.ViewState": {
                    "scg:Dictionary": {
                        "@x:TypeArguments": "x:String, x:Object",
                        "av:Point": {
                            "#text": entity["x"] + "," + entity["y"],
                            "@x:Key": "ShapeLocation"
                        },
                        "av:Size": {
                            "#text": entity["width"] + "," + entity["height"],
                            "@x:Key": "ShapeSize"
                        },
                        "x:Boolean": {
                            "#text": "False",
                            "@x:Key": "IsExpanded"
                        }
                    }
                }
            };
            if (checkConnection(entity, setConnections())) {
                var list = getPoints(entity.id, groupConnectionsIDFroms);
                if (list.length > 1) {
                    var points = new Array();
                    for (var i = 0; i < list.length; i++) {
                        var item = {};
                        var connect = list[i];
                        if (connect.isDefault !== undefined && connect.isDefault) {
                            item = {
                                "#text": connect["points"],
                                "@x:Key": "Default"
                            }
                            if (connect["label"] !== "Default") {
                                info["sap:WorkflowViewStateService.ViewState"]["scg:Dictionary"]["x:String"] = {
                                    "@x:Key": "DefaultCaseDisplayName",
                                    "#text": connect["label"]
                                };
                            }
                        } else {
                            item = {
                                "#text": connect["points"],
                                "@x:Key": connect["label"] + "Connector"
                            }
                        }
                        points.push(item);
                    }
                    info["sap:WorkflowViewStateService.ViewState"]["scg:Dictionary"]["av:PointCollection"] = points;

                } else if (list.length == 1) {
                    var connect = list[0];
                    if (connect.isDefault !== undefined && connect.isDefault) {
                        info["sap:WorkflowViewStateService.ViewState"]["scg:Dictionary"]["av:PointCollection"] = {
                            "#text": connect["points"],
                            "@x:Key": "Default"
                        }
                    } else {
                        info["sap:WorkflowViewStateService.ViewState"]["scg:Dictionary"]["av:PointCollection"] = {
                            "#text": connect["points"],
                            "@x:Key": connect["label"] + "Connector"
                        }
                    }
                }
            }
            manage.push(info);
        }
        if (entity.type === typeEntity.t_approve || entity.type === typeEntity.t_generic) {
            root["p:FlowStep"] = {};
            var item = {
                "@sap2010:WorkflowViewState.IdRef": "FlowStep_" + entity["id"],
                "@x:Name": "__ReferenceID" + getReferenceID(parent.id, entity.id, groupConnectionsIDTos)
            };

            // key if parent is Switch
            if (parent.type === typeEntity.t_switch) {
                //var key = getKey(parent.id, entity.id, groupConnectionsIDFroms);
                var connect = getInfoConnection(parent.id, entity.id, groupConnectionsIDFroms);
                if (connect.isDefault !== undefined && connect.isDefault) {
                    item["@x:Key"] = "Default";
                } else {
                    item["@x:Key"] = connect["label"];
                }
            }

            if (entity.type === typeEntity.t_approve) {
                item["ftwa:ApproveTask"] = {
                    //"@AssignResultTo": entity.AssignResultTo !== undefined && entity.AssignResultTo !== "" ? encodeXml(decodeXml(entity.AssignResultTo)) : "{x:Null}",
                    "@AssignedToUsers": entity.AssignedToUsers !== undefined && entity.AssignedToUsers !== "" ? (decodeXml(entity.AssignedToUsers)) : "{x:Null}",
                    "@CorrelationId": entity.CorrelationId !== undefined && entity.CorrelationId !== "" ? encodeXml(decodeXml(entity.CorrelationId)) : "0",
                    "@DefaultResult": entity.DefaultResult !== undefined && entity.DefaultResult !== "" ? encodeXml(decodeXml(entity.DefaultResult)) : "{x:Null}",
                    "@Description": entity.Description !== undefined && entity.Description !== "" ? encodeXml(decodeXml(entity.Description)) : "{x:Null}",
                    "@ExpiresIn": entity.ExpiresIn !== undefined && entity.ExpiresIn !== "" ? encodeXml(decodeXml(entity.ExpiresIn)) : "{x:Null}",
                    "@ExpiresWhen": entity.ExpiresWhen !== undefined && entity.ExpiresWhen !== "" ? encodeXml(decodeXml(entity.ExpiresWhen)) : "{x:Null}",
                    "@HandOverUsers": entity.HandOverUsers !== undefined && entity.HandOverUsers !== "" ? encodeXml(decodeXml(entity.HandOverUsers)) : "{x:Null}",
                    "@OnComplete": entity.OnComplete !== undefined && entity.OnComplete !== "" ? encodeXml(decodeXml(entity.OnComplete)) : "{x:Null}",
                    "@OnInit": entity.OnInit !== undefined && entity.OnInit !== "" ? encodeXml(decodeXml(entity.OnInit)) : "{x:Null}",
                    "@TaskCode": entity.TaskCode !== undefined & entity.TaskCode !== "" ? encodeXml(decodeXml(entity.TaskCode)) : "{x:Null}",
                    "@Title": entity.Title !== undefined && entity.Title !== "" ? encodeXml(decodeXml(entity.Title)) : "{x:Null}",
                    "@UiCode": entity.UiCode !== undefined && entity.UiCode !== "" ? encodeXml(decodeXml(entity.UiCode)) : "{x:Null}",
                    "@sap2010:Annotation.AnnotationText": entity.annotation !== undefined && entity.annotation !== "" ? encodeXml(decodeXml(entity.annotation)) : "{x:Null}",
                    "@sap2010:WorkflowViewState.IdRef": "ApproveTask_" + entity["id"]
                };
                if (entity.AssignResultTo !== undefined && entity.AssignResultTo !== "") {
                    item["ftwa:ApproveTask"]["ftwa:ApproveTask.AssignResultTo"] = {
                        "p:OutArgument": {
                            "@x:TypeArguments": "x:String",
                            "mca:CSharpReference": {
                                "#text": encodeXml(decodeXml(entity.AssignResultTo)),
                                "@x:TypeArguments": "x:String"
                            }
                        }
                    }
                } else {
                    item["ftwa:ApproveTask"]["@AssignResultTo"] = "{x:Null}";
                }
                if (entity["label"].toLowerCase() !== "approvetask") {
                    item["ftwa:ApproveTask"]["@DisplayName"] = encodeXml(decodeXml(entity["label"]));
                }
            } else if (entity.type === typeEntity.t_generic) {
                item["ftwa:GenericTask"] = {
                    "@OnRun": entity.OnRun !== undefined && entity.OnRun !== "" ? encodeXml(decodeXml(entity.OnRun)) : "{x:Null}",
                    "@sap2010:Annotation.AnnotationText": entity.annotation !== undefined && entity.annotation !== "" ? encodeXml(decodeXml(entity.annotation)) : "{x:Null}",
                    "@sap2010:WorkflowViewState.IdRef": "GenericTask_" + entity["id"]
                }
                if (entity.TaskCode !== undefined && entity.TaskCode !== "") {
                    item["ftwa:GenericTask"]["ftwa:GenericTask.TaskCode"] = {
                        "p:InArgument": {
                            "@x:TypeArguments": "x:String",
                            "mca:CSharpValue": {
                                "#text": encodeXml(decodeXml(entity.TaskCode)),
                                "@x:TypeArguments": "x:String"
                            }
                        }
                    }
                } else {
                    item["ftwa:GenericTask"]["@TaskCode"] = "{x:Null}";
                }
                if (entity["label"].toLowerCase() !== "generictask") {
                    item["ftwa:GenericTask"]["@DisplayName"] = encodeXml(decodeXml(entity["label"]));
                }
            }

            if (entity._next !== undefined) {
                item["p:FlowStep.Next"] = {};
                check(entity._next, entity, item["p:FlowStep.Next"], manage);
            } else {
                var references = getConnectionOtherReferences(getOtherReference(entity.id, groupConnectionsIDFroms), groupConnectionsIDTos);
                if (references.length !== 0) {
                    item["p:FlowStep.Next"] = {};//new Array();
                    for (var i = 0; i < references.length; i++) {
                        var refer = references[i];
                        for (var j = 0; j < refer.conn.connections.length; j++) {
                            item["p:FlowStep.Next"] = {
                                "x:Reference": {
                                    "#text": "__ReferenceID" + refer.id,
                                }
                            }
                        }
                    }
                }
            }

            root["p:FlowStep"] = item;

            var infoStep = {
                "@Id": "FlowStep_" + entity["id"],
                "sap:WorkflowViewStateService.ViewState": {
                    "scg:Dictionary": {
                        "@x:TypeArguments": "x:String, x:Object",
                        "av:Point": {
                            "#text": entity["x"] + "," + entity["y"],
                            "@x:Key": "ShapeLocation"
                        },
                        "av:Size": {
                            "#text": entity["width"] + "," + entity["height"],
                            "@x:Key": "ShapeSize"
                        }
                    }
                }
            }
            if (checkConnection(entity, setConnections())) {
                var list = getPoints(entity.id, groupConnectionsIDFroms);
                if (list.length > 1) {
                    var points = new Array();
                    for (var i = 0; i < list.length; i++) {
                        var item = {
                            "#text": list[i]["points"],
                            "@x:Key": list[i]["label"] + "Connector"
                        }
                        points.push(item);
                    }
                    infoStep["sap:WorkflowViewStateService.ViewState"]["scg:Dictionary"]["av:PointCollection"] = points;
                } else if (list.length == 1) {
                    infoStep["sap:WorkflowViewStateService.ViewState"]["scg:Dictionary"]["av:PointCollection"] = {
                        "#text": list[0]["points"],
                        "@x:Key": "ConnectorLocation"
                    }
                }
            }
            var infoObj = {
                "@sap:VirtualizedContainerService.HintSize": entity["width"] + "," + entity["height"],
                "sap:WorkflowViewStateService.ViewState": {
                    "scg:Dictionary": {
                        "@x:TypeArguments": "x:String, x:Object",
                        "x:Boolean": [{
                            "#text": "True",
                            "@x:Key": "IsExpanded"
                        }, {
                            "#text": "False",
                            "@x:Key": "IsAnnotationDocked"
                        }]
                    }
                }
            };
            if (entity.type === typeEntity.t_approve) {
                infoObj["@Id"] = "FlowApprove_" + entity["id"];
            } else {
                infoObj["@Id"] = "FlowGeneric_" + entity["id"];
            }

            manage.push(infoStep);
            manage.push(infoObj);
        }
    }


    // get list points connection from start
    function getPointsStart(connections) {
        for (var i = 0; i < connections.length; i++) {
            var conn = connections[i];
            if (conn.type === typeEntity.t_start) {
                return points(conn.points);
            }
        }
        return "";
    }

    // get list point connection from
    function getPoints(id, connections) {
        for (var i = 0; i < connections.length; i++) {
            var conn = connections[i];
            if (conn.idfrom == id) {
                var items = new Array();
                for (var j = 0; j < conn.connections.length; j++) {
                    var info = conn.connections[j];
                    var item = {
                        label: info.label,
                        points: getpoints(info.points),
                        isDefault: info.isDefault
                    }
                    items.push(item);
                }
                return items;
            }
        }
        return new Array();
    }

    function getpoints(connections) {
        var points = ""
        for (var i = 0; i < connections.length; i++) {
            var p = connections[i];
            points += p;
            if (i < connections.length - 1) {
                points += ' ';
            }
        }
        return points;
    }

    function getKey(from, to, arr) {
        for (var i = 0; i < arr.length; i++) {
            if (arr[i].idfrom === from) {
                for (var j = 0; j < arr[i].connections.length; j++) {
                    if (arr[i].connections[j].to === to) {
                        return arr[i].connections[j].label;
                    }
                }
            }
        }
    }

    function getReferenceID(from, to, arr) {
        for (var i = 0; i < arr.length; i++) {
            if (arr[i].idto === to) {
                for (var j = 0; j < arr[i].connections.length; j++) {
                    if (arr[i].connections[j].from === from) {
                        return i;
                    }
                }
            }
        }
        return false;
    }

    function getReference(id, arr) {
        for (var i = 0; i < arr.length; i++) {
            if (arr[i].idto === id) {
                return i;
            }
        }
        return false;
    }

    function getReferences(from, to, arr) {
        var references = new Array();
        for (var i = 0; i < arr.length; i++) {
            if (arr[i].idfrom === from) {
                for (var j = 0; j < arr[i].connections.length; j++) {
                    if (arr[i].connections[j].to === to) {
                        references.push(arr[i].connections[j]);
                    }
                }
            }
        }
        return references;
    }

    function getOtherReference(id, arr) {
        var array = new Array();
        for (var i = 0; i < arr.length; i++) {
            if (arr[i].idfrom == id) {
                for (var j = 0; j < arr[i].connections.length; j++) {
                    if (!checkIn(arr[i].connections[j].to, array)) {
                        array.push(arr[i].connections[j].to);
                    }
                }
                return array;
            }
        }
        return array;
    }

    function getConnectionOtherReferences(arr, arrTo) {
        var array = new Array();
        for (var i = 0; i < arr.length; i++) {
            var id = arr[i];
            for (var j = 0; j < arrTo.length; j++) {
                if (id == arrTo[j].idto) {
                    array.push({ conn: arrTo[j], id: j });
                }
            }
        }
        return array;
    }

    function checkReferenceExisting(id, arr) {
        for (var i = 0; i < arr.length; i++) {
            if (id === arr[i]) {
                return true;
            }
        }
        return false;
    }

    function getInfoConnection(from, to, arr) {
        for (var i = 0; i < arr.length; i++) {
            if (arr[i].idfrom === from) {
                for (var j = 0; j < arr[i].connections.length; j++) {
                    if (arr[i].connections[j].to === to) {
                        return arr[i].connections[j];
                    }
                }
            }
        }
    }
}


//////////////////// handle save ////////////////////////////
function handleNew() {
    if (project !== null && isModified) {
        //$(".message").html("Do you want to save this Workflow?");
        $("#confirm-popup-modal").html("<div class='message'>Do you want to save this Workflow?</div>").dialog("option", "buttons", [
            {
                text: "Yes", click: function () {
                    actionSave(function () {
                        actionNew("New Workflow");
                    });
                    $(this).dialog("close");
                }
            },
            {
                text: "No", click: function () {
                    actionNew("New Workflow");
                    $(this).dialog("close");
                }
            }
        ]).dialog("open");
    } else {
        actionNew("New Workflow");
    }
}

////////////////////////////// handle load /////////////////////////////
function handleLoad() {
    if (project !== null && isModified) {
        $("#confirm-popup-modal").html("<div class='message'>Do you want to save this Workflow?</div>").dialog("option", "buttons", [
            {
                text: "Yes", click: function () {
                    actionSave(actionLoad);
                    $(this).dialog("close");
                }
            },
            {
                text: "No", click: function () {
                    actionLoad();
                    $(this).dialog("close");
                }
            }
        ]).dialog({ title: "Confirm" }).dialog("open");
    } else {
        actionLoad();
    }
}

////////////////// handle save ///////////////////////
function handleSave() {
    if (project !== null && !isModified) {
        return;
    }

    var task = "<div>Do you want to save this Workflow </div>"
        + "<div> with name <input type='text' id='newname' value='" + project + "' maxlength='75' onkeypress='return restrictCharacter(event)' onpaste='return false;'/>?</div>";

    $("#confirm-popup-modal").html("<div class='message'>" + task + "</div>").dialog("option", "buttons", [
        {
            text: "Yes", click: function () {
                if ($.trim($("#newname").val()) === "") {
                    $("#newname").focus();
                    return;
                }
                actionSave(function () { return; });
                project = $.trim($("#newname").val());
                $("#workflow-name").html($.trim($("#newname").val()));
                $(this).dialog("close");
            }
        },
        {
            text: "No", click: function () {
                $(this).dialog("close");
            }
        }
    ]).dialog({ title: "Confirm" }).dialog("open");
}

////////////////// handle deploy ///////////////////////

function actionDeploy() {
    $.ajax({
        type: 'post',
        url: 'Sketch/DeployWorkflow',
        data: { fname: project },
        datatype: 'json',
        error: function (error) {
            var task = "<div>Error: Sorry there was an error while deploying the workflow. Please contact your system administrator.</div>";

            $(".message").html(task);
            $("#confirm-popup-modal").dialog("option", "buttons", [
                {
                    text: "Ok", click: function () {
                        $(this).dialog("close");
                    }
                }
            ]);
            $("#confirm-popup-modal").dialog("option", "title", "Notification");
            $("#confirm-popup-modal").dialog("option", "height", "140").dialog("open");
        },
        success: function (data) {

            var redirectForm = $('form#RedirectToTask');
            if (redirectForm) {
                //Remove - Prevent page redirect to old task list when finish edit sketch
                //redirectForm.submit();

                var toid = jQuery("#TaskOid").val();
                if (toid) {
                    window.parent.location.href = window.parent.location.pathname + "#/tasks?toid=" + jQuery("#TaskOid").val();
                }
            }

            Loading();
            actionNew("New Workflow");
        },
        beforeSend: function (xhr) {
            showSketchLoading(true);
        }
    }).done(function () {
        showSketchLoading(false);
    });
}

function handleDeploy() {
    if (project && isModified) {
        
        $("#confirm-popup-modal").html("<div class='message'>Please save the workflow first</div>").dialog("option", "buttons", [
            {
                text: "Ok", click: function () {
                    $(this).dialog("close");
                }
            }
        ]);
        $("#confirm-popup-modal").dialog("option", "title", "Notification");
        $("#confirm-popup-modal").dialog("option", "height", "140").dialog("open");
    }

    if (project && project !== 'New Workflow' && !isModified) {
        actionDeploy();
    }
}

jQuery(document).on("onChangingPage", function () {
    project = '';
    isModified = false;
});