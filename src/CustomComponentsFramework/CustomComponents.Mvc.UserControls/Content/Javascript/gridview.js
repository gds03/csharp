
function GridCallback($table, $element, calledFromHeader) {
    var $container = $table.parent();               // div 

    var $footer = $table.children("tfoot");
    var $header = $table.children("thead").children("tr:first");

    var containerId = $container.attr("id");
    var baseUrl = $table.attr("data-callbackurl");
    var formID = $table.attr("data-formid");

    var sortColumn;
    var sortAscending;
    var currentPage;

    if (calledFromHeader) {

        // adjust sort variables with $element attributes
        sortColumn = $element.attr("data-columnname");
        sortAscending = $element.attr("data-ascending") == "1" ? "0" : "1";
        currentPage = $footer.find("span[class=pager_current]").attr("data-value").trim();
    }

    else {
        // -> CALLED FROM PAGER
        // adjust sort variables with $header attributes
        sortColumn = $header.attr("data-current-sortby");
        sortAscending = $header.attr("data-current-sortascending");
        currentPage = $element.attr("data-value").trim();
    }

    var queryString = "?___contextid=" + containerId + "&page=" + currentPage + "&sortBy=" + sortColumn + "&sortAscending=" + sortAscending;

    if (formID != undefined && formID != "") {
        userInput = $("#" + formID).serialize();

        //
        // before to put on query string we must have sure that we merge results from userInput


        queryString += "&" + userInput + "&formId=" + formID;
    }

    var urlRequest = baseUrl + queryString;

    $.ajax({ url: urlRequest }).done(function (data) {
        $("#" + containerId).replaceWith(data);
    });
}



//
// Attach click event handler for current and future elemts came from the header (sort)

$(document).on("click", "table thead tr th[class~=hand]", function (e) {
    var table = $(this).parent()    // tr
                       .parent()    // thead
                       .parent();   // table

    GridCallback(table, $(this), true);
});



//
// Attach click event handler for current and future elemts came from the pager (page)

$(document).on("click", "table tfoot tr td span", function (e) {
    var table = $(this).parent()   // td
                       .parent()   // tr
                       .parent()   // tfoot
                       .parent();  // table

    GridCallback(table, $(this), false);
});





AddGridClickEvent = function (callback) {
    $(document).on("click", "table tbody tr[class*=IsClickable]", function (e) {

        var id = $(this).attr("data-id");
        callback(id);

    });
}


// callback will be called for many rows that will be on grid with row element, and object in JSON
AddGridLoadEvent = function (callbackTable, callbackTRs) {
    var selector = "div[data-controlname=basicgrid]";

    var tableToTRobjects = function (calledFromDocumentReady, $table) {
        var $trs = $table.children("tbody").children("tr");

        var createObject = function ($tr) {
            var obj = {};
            var $theadTR = $tr.parent().parent().children("thead").children("tr");
            $theadTR.children("th").each(function (idx, elem) {

                var isIdentity = $(elem).attr("data-identity") == "1";
                var key = $(elem).attr("data-columnname");
                var value;

                if (isIdentity) {
                    // columnValue is on $tr
                    value = $tr.attr("data-id");
                }
                else {
                    value = $tr.children("td").eq(idx - 1).text().trim();
                }


                obj[key] = value;

            });

            return obj;
        }

        $trs.each(function (idx, elem) {
            var obj = createObject($(elem));
            var bIsLastRow = (idx == $trs.length - 1);
            callbackTRs(calledFromDocumentReady, $(elem), obj, bIsLastRow);
        });
    }

    var sharedFunc = function (calledFromDocumentReady, divContainer) {
        var $table = $(divContainer).children("table");

        if (callbackTable != undefined)
            callbackTable(calledFromDocumentReady, $table);

        if (callbackTRs != undefined)
            tableToTRobjects(calledFromDocumentReady, $table);
    }

    // when document became ready
    $(function () {
        $(selector).each(function (idx, elem) {
            sharedFunc(true, elem);
        });
    });

    // when possible ajax was called
    $(document).on("DOMNodeInserted", selector, function (event) {
        sharedFunc(false, this);
    });
}






