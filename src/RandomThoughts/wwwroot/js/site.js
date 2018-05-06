// Write your JavaScript code.

var apiHost;
var currentId;
$(document).ready(function () {
    
    apiHost = $('#app-data').attr('data-apiHost');
    console.log(apiHost);

    $(".thought-view-btn").click(function() {
        let thoughtId = $(this).closest(".thought-inner-container").data("id");
        getThought(thoughtId);        
    });
    $(".comment-view-btn").click(function () {
        let commentId = $(this).data("id");
        getComment(commentId);
    });
    $(".thought-edit-btn").click(function() {
        let thoughtId = $(this).closest(".thought-inner-container").data("id");
        ModalBackground('.modal-dialog');
        displayThoughtEditModal(thoughtId);        
    });
    $(".comment-edit-btn").click(function () {
        let commentId = $(this).data("id");
        console.log(commentId);
        displayCommentEditModal(commentId);
    });
    $('#add-new-thought').click(function () {
        ModalBackground('.modal-dialog');
        $('#thought-create-modal').modal('show');
    });
    $('#add-new-comment').click(function () {
        let thoughtId = $(this).data("id");
        currentId = thoughtId;
        $('#comment-edit-modal').modal('show');
    });
    $('#create-thought-btn').click(function(e){
        e.preventDefault();
        saveNewThought();
    });
    $('#save-comment-btn').click(function (e) {
        e.preventDefault();
        let thoughtId = $(this).data("id");
        console.log("Here!!");
        console.log(thoughtId);
        currentId = thoughtId;
        saveNewComment();
    });
    $('#save-thoughtHole-btn').click(function(e){
        e.preventDefault();
        saveNewThoughtHole();
    });

    $('#thought-edit-modal').on('hidden.bs.modal', function () {
        cleanModal();
    });
    $('#comment-edit-modal').on('hidden.bs.modal', function () {
        cleanCommentModal();
    });

    $('#add-new-thoughtHole').click(function(){
        $('#thoughtHole-edit-modal').modal('show');
    });

});

/** Thought  functions Start */

/**
 * Get the Thought data from the server.
 * @param  {number} thoughtId The Id of the selected Thought
 */
function getThought(thoughtId){
    $.get(apiHost + 'thoughts/get/' + thoughtId, function (data) {
        console.log(data);
        console.log(data.createAtHumanized);
        displayThoughtDetails(data);
    });
}
/**
 * TODO: Add the dates to the modal.
 */
function displayThoughtDetails(data){
    $('#thought-display-modal .modal-title').html(data.title);
    $('#thought-display-modal .modal-body > p').html(data.body);
    $('#thought-display-modal #data-thought').html("Created " + data.createAtHumanized + " |  " + data.body.length + " words");

    let dataJson = {};
    let form = new FormData();
    

    dataJson['ParentId'] = data.id;
    currentId = data.id;
    console.log(dataJson);
    $.ajax({
        type: "GET",
        data: dataJson,
        url: apiHost + 'Thoughts/GetAllComments/',
        contentType: "application/json"

    }).done(function (res) {
        console.log('res', res);
        $('#comment-edit-modal .modal-footer > button').data("id", data.id);
        $('#thought-inner-container > button').data("id", data.id);
        console.log(data.id);
        $('#thought-display-modal .modalCommentTitle >').remove();
        console.log(res);
        for (var i = 0; i < res.length; i++) {
            $('#thought-display-modal .modalCommentTitle').append("<h5>" + res[i].applicationUserId + "</h5>");
            $('#thought-display-modal .modalCommentTitle').append("<span class=\"thought-action-container pull-right\"><button data-id=\"" + res[i].id + "\" class=\"comment-view-btn\"><span class=\"glyphicon glyphicon-eye-open\" aria-hidden=\"true\"></span></button><button data-id=\"" + res[i].id + "\" class=\"comment-edit-btn\"><span class=\"glyphicon glyphicon-pencil\" aria-hidden=\"true\"></span></button></span></div>");
            $('#thought-display-modal .modalCommentTitle').append("<div class=\"modalComment-body\"><p>" + res[i].body + "</p></div>");
        }
        $(".comment-view-btn").click(function () {
            let commentId = $(this).data("id");
            getComment(commentId);
        });
        $(".comment-edit-btn").click(function () {
            let commentId = $(this).data("id");
            console.log(commentId);
            displayCommentEditModal(commentId);
        });
        $('#thought-display-modal').modal('show');

        ModalBackground('.modal-dialog');        
        cleanCommentModal();
        
    });
}

function ModalBackground(param) {
    $(param).css('width', '35cm');
    $('#thoughts-container > ').css('filter', 'blur(2px)');
    $('#thoughts-header > ').css('filter', 'blur(2px)');
}

function saveNewThought(){
    let inputsSelector = $('#thought-create-modal input, #thought-create-modal textarea, #thought-create-modal select');
    var data = inputsSelector.serializeArray();
    let hasError = false;
    for(key in data){
        if(data[key].value==""&&data[key].name!="Id"&&data[key].name!="ThoughtHoleId"){
            var outerSelector = $('input[name="'+data[key].name+'"]').closest('.form-group');
            if(outerSelector.length == 0)   
                outerSelector = $('textarea[name="'+data[key].name+'"]').closest('.form-group');
            outerSelector.addClass('has-error');
            console.log(data[key]);
            hasError = true;
        }
    }
    if(hasError){
        toastr.error('There are some error in the form, please fix them', 'Opps');
        return false;
    }

    let dataJson = {};
    let form = new FormData();
    data.map(function (x) {
        dataJson[x.name] = x.value;
    });

    console.log(data);
    $.ajax({
        type: "POST",
        data: JSON.stringify(dataJson),
        url: apiHost + 'Thoughts/post',
        contentType: "application/json"

    }).done(function (res) {
        console.log('res', res);      
        toastr.success("The Thought have been created!!!");     
        insertNewThought(res);
        cleanModal();

    }).error(function (jqXHR, textStatus, errorThrown) {
        toastr.error('There is an error in the server, please try again :(', 'Opps');
    });
}

function cleanModal(){
    $('#thought-edit-modal .form-group').removeClass('has-error');
    $('#thought-edit-modal input, #thought-edit-modal textarea').val('');
    $('#thought-edit-modal').modal('hide');
}

function displayThoughtEditModal(thoughtId){
    var thoughtCntSel = $('.thought-inner-container[data-id="'+thoughtId+'"]');
    let title = thoughtCntSel.find('.thought-title').text();
    let body = thoughtCntSel.find('.thought-body').text();
    let mood = thoughtCntSel.attr('data-mood');

    GetModifyOfThought(thoughtId);
    

    $('#thought-edit-modal #thought-id').val(thoughtId);

    $('#thought-edit-modal #thought-title').val(title);
    $('#thought-edit-modal #thought-body').val(body);
    $('#thought-edit-modal #thought-mood').val(mood);
    

    $('#save-thought-btn').text("Edit").unbind('click').
        click(function () {            
        saveThoughtChanges(thoughtId);
    });
    $('#thought-edit-modal').modal('show');

}

function GetModifyOfThought(thoughtId) {
    $.get(apiHost + 'thoughts/get/' + thoughtId, function (data) {
        console.log(data.modifiedAtHumanized)
        $('#thought-edit-modal #data-thought').html("Last Modify " + data.modifiedAtHumanized);
    });
}

function saveThoughtChanges(thoughtId){
    let inputsSelector = $('#thought-edit-modal input, #thought-edit-modal textarea, #thought-edit-modal select');
    var data = inputsSelector.serializeArray();
    let hasError = false;
    for(key in data){
        if(data[key].value==""){
            var outerSelector = $('input[name="'+data[key].name+'"]').closest('.form-group');

            if(outerSelector.length == 0)   
                 outerSelector = $('textarea[name="'+data[key].name+'"]').closest('.form-group');

            outerSelector.addClass('has-error');
            console.log(data[key]);
            hasError = true;
        }
    }
    if(hasError){
        toastr.error('There are some error in the form, please fix them', 'Opps');
        return false;
    }

    let dataJson = {};
    let form = new FormData();
    data.map(function (x) {
        dataJson[x.name] = x.value;
    });

    console.log(data);
    $.ajax({
        type: "PUT",
        data: JSON.stringify(dataJson),
        url: apiHost + 'Thoughts/put/'+thoughtId,
        contentType: "application/json"

    }).done(function (res) {
        console.log('res', res);      
        toastr.success("The Thought have been modified!!!");     
        $(".thought-edit-btn").unbind('click').click(function() {
            let thoughtId = $(this).closest(".thought-inner-container").data("id");
            displayThoughtEditModal(thoughtId);        
        });
        cleanModal();
        editThoughtCard(res);
    }).error(function (jqXHR, textStatus, errorThrown) {
        toastr.error('There is an error in the server, please try again :(', 'Opps');
    });
}

function editThoughtCard(thought){
    $('.thought-inner-container[data-id="'+thought.id + '"] .thought-title').text(thought.title);
    $('.thought-inner-container[data-id="'+thought.id + '"] .thought-body').text(thought.body);
    $('.thought-inner-container[data-id="'+thought.id + '"]').attr('data-mood', thought.mood);
}

function insertNewThought(thought) {
    let element = '<div data-mood="'+thought.mood +'" data-id="'+thought.id+'" class="thought-inner-container col-sm-3">\
    <div class="panel panel-default">\
        <div class="panel-heading">\
           <span class="thought-title">'+ thought.title + '</span>\
            <span class="thought-action-container pull-right">' +
                '<button class="thought-view-btn">' +              
                    '<span class="glyphicon glyphicon-eye-open" aria-hidden="true"></span>\
                </button>\
                <button class="thought-edit-btn">'+
                    '<span class="glyphicon glyphicon-pencil" aria-hidden="true"></span>\
                </button>\
            </span>\
        </div>\
        <div class="panel-body">\
        <span class="thought-body">'+thought.body+'</span>'+
        '</div>'+
    '</div>'+
'</div>';
    $('#thoughts-container').prepend(element);

    $(".thought-view-btn").click(function() {
        let thoughtId = $(this).closest(".thought-inner-container").data("id");
        getThought(thoughtId);        
    });
    $(".thought-edit-btn").click(function() {
        let thoughtId = $(this).closest(".thought-inner-container").data("id");
        displayThoughtEditModal(thoughtId);        
    });
}

/** Thought  functions End */

/** Comments functions Start */

function cleanCommentModal() {
    $('#comment-edit-modal .form-group').removeClass('has-error');
    $('#comment-edit-modal input, #comment-edit-modal textarea').val('');
    
    $('#comment-edit-modal').modal('hide');
}
function getComment(commentId) {
    console.log(commentId);
    $.get(apiHost + 'Thoughts/GetComment/' + commentId, function (data) {
        displayCommentsDetails(data);
    });
}

function displayCommentsDetails(data) {
    $('#comment-display-modal .modal-title > h1').html(data.applicationUserId);
    $('#comment-display-modal .modal-body > p').html(data.body);
    $('#comment-display-modal').modal('show');
}

function displayCommentEditModal(data) {
    var commentCntSel = $('.comment-inner-container[data-id="' + data + '"]');
    let body = commentCntSel.find('.comment-body').text();


    $('#comment-edit-modal #comment-body').val(body);


    $('#save-comment-btn').text("Save").unbind('click').
        click(function () {
            saveCommentChanges(data);
        });
    $('#comment-edit-modal').modal('show');

}

function saveCommentChanges(commentId) {
    let inputsSelector = $('#comment-edit-modal input, #comment-edit-modal textarea, #comment-edit-modal select');
    var data = inputsSelector.serializeArray();
    let hasError = false;
    for (key in data) {
        if (data[key].value == "" && data[key].name != "Id" && data[key].name != "ThoughtId") {
            var outerSelector = $('input[name="' + data[key].name + '"]').closest('.form-group');
            if (outerSelector.length == 0)
                outerSelector = $('textarea[name="' + data[key].name + '"]').closest('.form-group');
            outerSelector.addClass('has-error');
            console.log(data[key]);
            hasError = true;
        }
    }
    if (hasError) {
        toastr.error('There are some error in the form, please fix them', 'Opps');
        return false;
    }

    let dataJson = {};
    let form = new FormData();
    data.map(function (x) {
        dataJson[x.name] = x.value;
    });
    dataJson['ParentId'] = currentId;
    dataJson['Id'] = commentId;

    console.log(data);
    $.ajax({
        type: "PUT",
        data: JSON.stringify(dataJson),
        url: apiHost + 'Thoughts/PutComment/' + commentId,
        contentType: "application/json"

    }).done(function (res) {
        console.log('res', res);
        toastr.success("The Comment have been modified!!!");
        $(".comment-edit-btn").unbind('click').click(function () {
            let commentId = $(this).closest(".comment-inner-container").data("id");
            displayCommentEditModal(commentId);
        });
        cleanModal();
        editCommentCard(res);
    }).error(function (jqXHR, textStatus, errorThrown) {
        toastr.error('There is an error in the server, please try again :(', 'Opps');
    });
}

function editCommentCard(comment) {
    $('.comment-inner-container[data-id="' + comment.id + '"] .comment-body').text(comment.body);
}

function saveNewComment() {
    let inputsSelector = $('#comment-edit-modal input, #comment-edit-modal textarea, #comment-edit-modal select');
    var data = inputsSelector.serializeArray();
    let hasError = false;
    for (key in data) {
        if (data[key].value == "" && data[key].name != "Id" && data[key].name != "ThoughtId") {
            var outerSelector = $('input[name="' + data[key].name + '"]').closest('.form-group');
            if (outerSelector.length == 0)
                outerSelector = $('textarea[name="' + data[key].name + '"]').closest('.form-group');
            outerSelector.addClass('has-error');
            console.log(data[key]);
            hasError = true;
        }
    }
    if (hasError) {
        toastr.error('There are some error in the form, please fix them', 'Opps');
        return false;
    }

    let dataJson = {};
    let form = new FormData();
    data.map(function (x) {
        dataJson[x.name] = x.value;
    });

    dataJson['ParentId'] = currentId;

    console.log(dataJson);
    $.ajax({
        type: "POST",
        data: JSON.stringify(dataJson),
        url: apiHost + 'Thoughts/PostComment/',
        contentType: "application/json"

    }).done(function (res) {
        console.log('res', res);
        toastr.success("The Comment have been created!!!");
        insertNewComment(res);
        cleanModal();

    }).error(function (jqXHR, textStatus, errorThrown) {
        toastr.error('There is an error in the server, please try again :(', 'Opps');
    });
}

function insertNewComment(comment) {
    let element = '<div data-user="' + comment.ApplicationUserId + '" data-id="' + comment.id + '" class="comment-inner-container col-sm-3">\
    <div class="panel panel-default">\
        <div class="panel-heading">\
           <span class="thought-title">'+ comment.ApplicationUserId + '</span>\
            <span class="thought-action-container pull-right">' +
        '<button class="comment-view-btn">' +
        '<span class="glyphicon glyphicon-eye-open" aria-hidden="true"></span>\
                </button>\
                <button class="comment-edit-btn">'+
        '<span class="glyphicon glyphicon-pencil" aria-hidden="true"></span>\
                </button>\
            </span>\
        </div>\
        <div class="panel-body">\
        <span class="comment-body">'+ comment.body + '</span>' +
        '</div>' +
        '</div>' +
        '</div>';
    $('#comments-container').prepend(element);

    $(".comment-view-btn").click(function () {
        let commentId = $(this).data("id");
        getComment(commentId);
    });
    $(".comment-edit-btn").click(function () {
        let commentId = $(this).data("id");
        displayCommentEditModal(commentId);
    });
}
/** Comments function End */

/** Thought Holes functions Start */
function saveNewThoughtHole(){
    let inputsSelector = $('#thoughtHole-edit-modal input, #thoughtHole-edit-modal textarea');
    var data = inputsSelector.serializeArray();
    let hasError = false;
    for(key in data){
        if(data[key].value==""&&data[key].name!="Id"){
            var outerSelector = $('input[name="'+data[key].name+'"]').closest('.form-group');
            if(outerSelector.length == 0)   
                outerSelector = $('textarea[name="'+data[key].name+'"]').closest('.form-group');
            outerSelector.addClass('has-error');
            console.log(data[key]);
            hasError = true;
        }
    }
    if(hasError){
        toastr.error('There are some error in the form, please fix them', 'Opps');
        return false;
    }

    let dataJson = {};
    let form = new FormData();
    data.map(function (x) {
        dataJson[x.name] = x.value;
    });

    console.log(data);
    $.ajax({
        type: "POST",
        data: JSON.stringify(dataJson),
        url: apiHost + 'ThoughtHoles/post',
        contentType: "application/json"

    }).done(function (res) {
        console.log('res', res);      
        toastr.success("The Hole have been created!!!");     
        insertNewThoughtHole(res);
        cleanHoleModal();

    }).error(function (jqXHR, textStatus, errorThrown) {
        toastr.error('There is an error in the server, please try again :(', 'Opps');
    });
}

function insertNewThoughtHole(hole) {
    let element = '<div data-likes="'+hole.likes +'" data-views="'+hole.views+'"  data-id="'+hole.Id+'" class="thoughtHole-inner-container col-sm-3">\
    <div class="panel panel-default">\
        <div class="panel-heading">\
           <span class="thought-title">'+ hole.name + '</span>'+
            '<span class="thought-action-container pull-right">' +    
                '<button class="thought-view-btn">' +                 
                    '<span class="glyphicon glyphicon-eye-open" aria-hidden="true"></span>\
                </button>\
                <button class="thought-edit-btn">\
                    <span class="glyphicon glyphicon-pencil" aria-hidden="true"></span>\
                </button>\
            </span>\
        </div>\
        <div class="panel-body">\
        <span class="thought-body">'+hole.description +'</span>\
        </div>\
    </div>\
</div>';
//<a href="@Url.Action("HoleThoughts", new {holeId = thoughtHole.Id})">
    $('#thoughtHoles-container').prepend(element);

    $(".thoughtHole-view-btn").click(function() {
        let thoughtId = $(this).closest(".thought-inner-container").data("id");
        getThoughtHole(thoughtId);        
    });
    $(".thoughtHole-edit-btn").click(function() {
        let thoughtId = $(this).closest(".thought-inner-container").data("id");
        displayThoughtHoleEditModal(thoughtId);        
    });
}

function cleanHoleModal(){
    $('#thoughtHole-edit-modal .form-group').removeClass('has-error');
    $('#thoughtHole-edit-modal input, #thoughtHole-edit-modal textarea').val('');
    $('#thoughtHole-edit-modal').modal('hide');
}
/** Thought Holes functions End */