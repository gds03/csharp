$(function () {

    // Adiciona handler para click
    $('#add_comment').click(function () {
        $('#new_comment').css('display', 'block');  // Torna visivel
    });


    // Adiciona handler para click
    $('#submit_comment').click(function () {
        var txt = $('#comment_text').val();
        var issueId = $('#fieldSet_IssueId').attr('data-issueId');

        $.ajax({
            type: 'POST',
            url: String.format('/Issues/{0}/Comments/Create', issueId),
            data: String.format("description={0}", txt),
            success: function (data) { // Retorna partialView(Html com os dados do DTO)

                if (data.invalid != undefined && data.invalid == true) {
                    alert('Your comment must be between 5 and 2000 characters');
                    return;
                }

                // Esconde o div novamente
                $('#new_comment').fadeOut(0);

                // Apaga o qu esta na textarea
                $('#comment_text').val('');

                // Colocar este html na no div de list
                $('#comments_list').empty().html(data);
            }
        });
    });
});

function RemoveCommentWithId(element) {
    if (confirm('You want really remove this comment?')) {
        var issueId = $('#fieldSet_IssueId').attr('data-issueId');
        var commentId = $(element).attr('data-id')

        $.ajax({
            type: 'POST',
            url: String.format('/Issues/{0}/Comments/Remove/{1}', issueId, commentId),
            success: function (data) { // Retorna partialView(erros 

                // Esconde o div novamente
                $('#new_comment').fadeOut(0);

                // Apaga o qu esta na textarea
                $('#comment_text').val('');

                if (!data.Success) {
                    alert('An error has happened');
                }

                // Colocar este html na no div de list
                $('#comments_list').empty().html(data.ListItems);
            }
        });
    }


}


function MakeRequestForPage(pageNumber) {
    var issueId = $('#fieldSet_IssueId').attr('data-issueId');

    $.ajax({
        type: 'POST',
        url: String.format('/Issues/{0}/Comments/IndexPaging', issueId),
        data: String.format("page={0}", pageNumber),
        success: function (data) { // Retorna partialView(Html com os dados do DTO)

            // Esconde o div novamente
            $('#new_comment').fadeOut(0);

            // Apaga o qu esta na textarea
            $('#comment_text').val('');

            // Colocar este html na no div de list
            $('#comments_list').empty().html(data);
        }
    });
}