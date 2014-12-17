var appConfig = {
    dateFormat: "mm/dd/yy",
    momentDateFormat: "MM/DD/YYYY",
    momentServerDateFormat: "YYYY-MM-DD"
};

var culture = $("#currentCulture").val();

if (culture === 'en-AU') {
    appConfig.dateFormat = "dd/mm/yy";
    appConfig.momentDateFormat = "DD/MM/YYYY";
}
else if (culture === 'en-GB') {
    appConfig.dateFormat = "dd/mm/yy";
    appConfig.momentDateFormat = "DD/MM/YYYY";
}
else if (culture === 'pt-BR') {
    appConfig.dateFormat = "dd/mm/yy";
    appConfig.momentDateFormat = "DD/MM/YYYY";
}
else if (culture === 'it-IT') {
    appConfig.dateFormat = "dd/mm/yy";
    appConfig.momentDateFormat = "DD/MM/YYYY";
}
else if (culture === 'fr-FR') {
    appConfig.dateFormat = "dd/mm/yy";
    appConfig.momentDateFormat = "DD/MM/YYYY";
}
else if (culture === 'fr-FR') {
    appConfig.dateFormat = "dd/mm/yy";
    appConfig.momentDateFormat = "DD/MM/YYYY";
}
else if (culture === 'es-ES') {
    appConfig.dateFormat = "dd/mm/yy";
    appConfig.momentDateFormat = "DD/MM/YYYY";
}
