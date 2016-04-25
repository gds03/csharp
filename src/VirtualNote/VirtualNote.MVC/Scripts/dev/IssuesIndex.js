function FilterRequest(page) {
    // Obter id que esta selected da dropdown
    var projectId = $('#projectId').children('option:selected').val();
    var sortBy = $('#sortBy').children('option:selected').val();

    if (page == undefined)
        page = 1;

    // Fazer pedido ajax
    $.ajax({
        type: 'POST',
        url: '/Issues/IndexPartial',
        data: String.format("projectId={0}&currentPage={1}&sortBy={2}", projectId, page, sortBy),     // ESTATICO NECESSARIO MUDAR
        success: function (data) { // Retorna partialView(Html com os dados do DTO)

            // Colocar este html na no div
            $('#requests').empty().html(data);
        }
    });
}


$(function () {
    $('div[id=pager] a').live('click', function (a) {
        var selectedPage = $(a.currentTarget).attr('data-page');
        FilterRequest(selectedPage);
    });

});