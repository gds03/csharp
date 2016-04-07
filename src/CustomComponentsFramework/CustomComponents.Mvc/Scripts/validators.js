

/* Validators Methods - Section */


//Less = 0,
//Greater = 1,
//Equal = 2,
//LessOrEqual = 3,
//GreaterOrEqual = 4


validatorsFunction = function (v, element, params) {

    // Value is the element to be validated, element is the HTML element that the validator is attached to,
    // params is the array of name/value pairs of the parameters extracted from the adapter, 

    //
    // return the condition that must be satisfied!

    var modelType = params.modeltype;
    var value = params.value;
    var signal = params.signal;
    var otherPropertyName = params.otherpropertyname;

    if (value == undefined) {
        otherPropertyName = "#" + otherPropertyName;
        value = $(otherPropertyName).val();             // value must have ore one or another.
    }

    if (v == "")
        return true;

    var context = 0;
    var comparer = 0;

    // TIMESPAN

    if (modelType == "TimeSpan" || modelType == "Nullable<TimeSpan>") {
        //FORMATO -> HH:mm
        var valueParts = v.split(':');
        var valueTime = parseInt(valueParts[0]) * 60;
        valueTime = valueTime + parseInt(valueParts[1]);

        var paramParts = value.split(':');
        var valueParam = parseInt(paramParts[0]) * 60;
        valueParam = valueParam + parseInt(paramParts[1]);

        // bind
        context = valueTime;
        comparer = valueParam;
    }

    // DATETIME

    if (modelType == "DateTime" || modelType == "Nullable<DateTime>") {
        // bind
        context = v.parseDateString().getTime();
        comparer = value.parseDateString().getTime();
    }

    if (signal == 0) // <
        return context < comparer;

    if (signal == 1) // >
        return context > comparer;

    if (signal == 2) // =
        return context == comparer;

    if (signal == 3) // <=
        return context <= comparer;

    if (signal == 4) // >=
        return context >= comparer;

    throw "Not supported";
}






/* EO Validators Methods - Section */



/* Validators Adapters - Section */

typeDependentValidatorParameter = function () {

    //
    // parameters that are returned from mvc server.

    return ["signal", "signaldescriptor", "otherpropertyname", "modeltype"];
};




typeDependentValueValidatorParameters = function () {
    var params = typeDependentValidatorParameter();
    params.push("value");
    return params;
}





$.validator.addMethod("typedependentpropertyvalidator_function", validatorsFunction);
$.validator.addMethod("typedependentvaluevalidator_function", validatorsFunction);



/* Validators Adapters - Section */



$.validator.unobtrusive.adapters.add(
    "typedependentpropertyvalidator",
    typeDependentValidatorParameter(),     // params from mvc
    function (options) {
        options.rules["typedependentpropertyvalidator_function"] = options.params;
        options.messages["typedependentpropertyvalidator_function"] = options.message;
    }
);




$.validator.unobtrusive.adapters.add(
    "typedependentvaluevalidator",
    typeDependentValueValidatorParameters(),     // params from mvc
    function (options) {
        options.rules["typedependentvaluevalidator_function"] = options.params;
        options.messages["typedependentvaluevalidator_function"] = options.message;
    }
);



/* EO Validators Adapters - Section */