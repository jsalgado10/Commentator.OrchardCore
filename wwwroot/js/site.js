function AddArticleComment(contentId) {
    console.log('Adding Article Comment');
    console.log('Content Id : ' + contentId);
    if (editorType == 'CKEditor') {
        $('#txtArticleComment_' + contentId).val(editors[contentId].getData());
        console.log(editors[contentId].getData());
    }
    var form = '#newCommentForm_' + contentId;
    var formData = $(form).serialize();
    postArticleComment(formData, contentId);
}

function postArticleComment(model, contentId) {
    if ($('#txtArticleComment_' + contentId).val().trim().length === 0) {
        swal("Please enter your comment", '', "error");
        return;
    }

    console.log('Posting New Comment');
    //console.log(model);
    $.post(newCommentLink, model)
        .done(function (data, status, xhr) {
            //console.log(data);
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

function AddNewCommentBox(parentId) {
    console.log('Add New Comment Box');
    $('#new_comment_' + parentId).load(newCommentLink, function () {
        $('#new_comment_' + parentId + ' input[name="CommentPost.CommentParent.Text"]').val(parentId);
    });
}

function CKEditorAddEmojis(editor) {
    var emojis = [];
    $.getJSON('/OrchardCore.Commentator/Assets/emoji.json', function (data) {
        $.each(data, function (index, value) {
            emojis.push({ title: value.id.replace(':', ''), character: value.symbol });
        })
        editor.plugins.get('SpecialCharacters').addItems('Emoji', emojis);
    });
    
}

function SetCKEditor(contentId) {
    var currentTxtComment = '#txtArticleComment_' + contentId;
    ClassicEditor
        .create(document.querySelector(currentTxtComment), {

            toolbar: {
                items: [
                    'heading',
                    '|',
                    'bold',
                    'italic',
                    'link',
                    'bulletedList',
                    'numberedList',
                    '|',
                    'indent',
                    'outdent',
                    '|',
                    'imageUpload',
                    'blockQuote',
                    'insertTable',
                    'mediaEmbed',
                    'undo',
                    'redo',
                    'alignment',
                    'code',
                    'codeBlock',
                    'fontSize',
                    'fontFamily',
                    'highlight',
                    'htmlEmbed',
                    'removeFormat',
                    'horizontalLine',
                    'specialCharacters',
                    'underline'
                ]
            },
            language: 'en',
            image: {
                toolbar: [
                    'imageTextAlternative',
                    'imageStyle:full',
                    'imageStyle:side'
                ]
            },
            table: {
                contentToolbar: [
                    'tableColumn',
                    'tableRow',
                    'mergeTableCells'
                ]
            },
            licenseKey: '',

        })
        .then(editor => {
            console.log('Editor was initialized ' + editor);
            CKEditorAddEmojis(editor);
            editors[contentId] = editor;
        })
        .catch(error => {
            console.error(error);
        });
}

function SetEditorTrumbowyg(contentId) {
    var currentTxtComment = '#txtArticleComment_' + contentId;

    jQuery.noConflict();
    console.log('Setting editor');
    $(currentTxtComment).trumbowyg({
        btns: [
            ['viewHTML'],
            ['undo', 'redo'],
            ['formatting'],
            ['strong', 'em', 'del'],
            ['fontfamily'],
            ['fontsize'],
            ['emoji'],
            ['giphy'],
            ['link'],
            ['insertImage'],
            ['justifyLeft', 'justifyCenter', 'justifyRight', 'justifyFull'],
            ['unorderedList', 'orderedList'],
            ['horizontalRule'],
            ['removeformat']
        ],
        plugins: {
            giphy: {
                apiKey: 'E7juuZxJbM4GPJsbUVqtDCqyYEUIT32c',
                noResultGifUrl: 'http://example.com/yourimage.gif'
            },
            fontsize: {
                allowCustomSize: true
            }
        },
        autogrow: true,
        urlProtocol: true,
        minimalLinks: true,
        tagsToRemove: ['script', 'link']
    });
    $(currentTxtComment).closest(".trumbowyg-box").css("min-height", "50px");
    $(currentTxtComment).prev(".trumbowyg-editor").css("min-height", "50px");
    console.log('Done Setting editor');
}

function ShowCommentReplies(parentId) {
    console.log('show Comment replies');
    var childDiv = '#post-children-' + parentId;
    var subCommentOptions = '&OnlySubComments=True&ParentId=' + parentId;
    $(childDiv).load(commentsLink + subCommentOptions);
    console.log('done showing Comment replies');
}

function SetEditor(contentId) {
    switch (editorType) {
        case 'Trumbowyg':
            SetEditorTrumbowyg(contentId);
            break;
        case 'CKEditor':
            SetCKEditor(contentId);
            break;
        default:
            console.log('No Editor was Set');
    }
}