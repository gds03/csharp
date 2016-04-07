

AddGridClickEvent = function (callback)
{
    var selector = "div div[class*=table_default] table tbody tr";
    var clickableClass = "table_default_with_hover";

    $(document).on("click", selector, function ()
    {
        if (!$(this).parent().parent().parent().parent().hasClass(clickableClass)) {
            return;
        }

        // get the identity
        var hiddenIdentity = $(this).children('td:first').find('input[type=hidden]');

        // if not found, stop
        if (hiddenIdentity == undefined)
            return;             // click for tr event in some grid, must not be done because was not rendered by the webgrid control in mvc

        // name of the current grid that was clicked
        var grid_name = hiddenIdentity.attr('data-gridcontainerid');

        // get all siblings of hiddenIdentity parent and pass them with a key, value structure.
        var siblings = hiddenIdentity.parent().siblings();

        var kvpObject = {};
        siblings.each(function (idx, e) {
            var hidden = $(e).children('input[type=hidden]');

            if (hidden.length == 1) {
                var propName = hidden.val();
                var propValue = hidden.parent().text();

                // add to dynamic object
                kvpObject[propName] = propValue;
            }
            else {
                if (hidden.length == 2) {
                    var propName = $(hidden[0]).val();
                    var propValue = $(hidden[1]).val();

                    // add to dynamic object
                    kvpObject[propName] = propValue;
                }
            }
        });


        // call func function
        callback(hiddenIdentity.attr('id'), hiddenIdentity.val(), grid_name, kvpObject)
    });
}


//
// Force to update the links of pager & header of that grid

UpdateGridAnchorsWithFormFilters = function (selector, formName)
{
    $(selector).each(function (idx, element)
    {
        var aJquery = $(element);
        var aHref = aJquery.attr('href');
        var nHref = serializeForm(formName);
        if (nHref.length > 1)
            aJquery.attr('href', aHref + "&" + serializeForm(formName));
        else
            aJquery.attr('href', aHref);
    });
}

UpdateGridAnchorsWhenLoaded = function (gridName, formName) {
    var selector = gridName + " tfoot tr a";
    UpdateGridAnchorsWithFormFilters(selector, formName);

    selector = gridName + " thead tr a";
    UpdateGridAnchorsWithFormFilters(selector, formName);
}

AddGridHiddenColumnsEvent = function (gridName) {
    $(document).on("DOMNodeInserted", gridName, function () {
        AdjustHeaderGrid(gridName);
    });
}




AdjustHeaderGrid = function (gridName)
{
    if (gridName == undefined)
        gridName = "div div[class*=table_default]"

    var table = $(gridName);

    table.each(function (idx1, v1) {
        var ctxTable = $(v1);
        var tableTh = ctxTable.find('thead tr th');

        var bodyTr = ctxTable.find('tbody tr:first');
        var bodyTd = bodyTr.find('td.none');

        bodyTd.each(function (idx, v) {

            //
            // hide th based on idx of the cell

            var th = tableTh.eq(idx);
            th.addClass("none");
        });
    });

    AdjustFooterGrid(gridName);
}



AdjustFooterGrid = function (gridName)
{

    // 
    // replace < and > text with img src.

    var grid = $(gridName);

    grid.find('tfoot tr td a').each(function (idx, v) {
        var anchor = $(v);

        if (anchor.text() == "<") {
            anchor.text("");
            anchor.html("<img src='/Images/seta_nav_pag_left.png' alt='backward' />");
        }

        if (anchor.text() == ">") {
            anchor.text("");
            anchor.html("<img src='/Images/seta_nav_pag_right.png' alt='forward' />");
        }

        // add spacing between anchors



        //
        // | separator between each anchor (add text node with | simbol between anchors)

    });
}
