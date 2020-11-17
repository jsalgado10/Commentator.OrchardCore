function AddArticleComment() {

    var formData = $('#newCommentForm').serialize();
    postArticleComment(formData);
}

function postArticleComment(model) {
    if ($('#txtArticleComment').val().trim().length === 0) {
        swal("Please enter your comment", '', "error");
        return;
    }

    //console.log(model);
    console.log('Postig New Comment');
    $.post(newCommentLink, model)
        .done(function (data,status,xhr) {
            console.log(data);
            //console.log(status);
            //console.log(xhr);
            console.log('done');
            $('#new_comment').html(data);
            $('#divComments').load(commentsLink);
            swal("Thank you", "Your Comment was submitted", "success");
        })
        .fail(function (data, status, xhr) {
            //console.log(data);
            //console.log(status);
            //console.log(xhr);
            console.log('fail');
            swal("Sorry!", "Failed to add your comment", "error");
        });
}