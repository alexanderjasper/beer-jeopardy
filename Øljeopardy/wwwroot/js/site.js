var removeAlert = setTimeout(function () {
    $(".alert").fadeTo(500, 0).slideUp(500, function () {
        $(this).remove();
    });
}, 3000);

$(document).on('click', '.navbar-collapse.in', function (e) {
    if ($(e.target).is('a')) {
        $(this).collapse('hide');
    }
});

function MoveAnswerQuestion(position, direction) {
    var thisQuestionId = 'q'.concat(position);
    var thisAnswerId = 'a'.concat(position);
    var targetQuestionId = (direction === 'up') ? 'q'.concat(position - 1) : 'q'.concat(position + 1);
    var targetAnswerId = (direction === 'up') ? 'a'.concat(position - 1) : 'a'.concat(position + 1);
    var thisQuestion = document.getElementById(thisQuestionId);
    var thisQuestionValue = thisQuestion.value;
    var thisAnswer = document.getElementById(thisAnswerId);
    var thisAnswerValue = thisAnswer.value;
    var targetQuestion = document.getElementById(targetQuestionId);
    var targetQuestionValue = targetQuestion.value;
    var targetAnswer = document.getElementById(targetAnswerId);
    var targetAnswerValue = targetAnswer.value;

    thisQuestion.value = targetQuestionValue;
    thisAnswer.value = targetAnswerValue;
    targetQuestion.value = thisQuestionValue;
    targetAnswer.value = thisAnswerValue;
}

function selectAnswerQuestion(elem) {
    var labels = document.getElementsByClassName('category-radio-button-label')
    for (i = 0; i < labels.length; i++) {
        labels[i].classList.remove('selected');
    }
    elem.classList.add('selected');
}

var loadRules = function () {
    $("#master-container").load('/Home/Rules');
};
var loadCategories = function () {
    $("#master-container").load('/Home/Categories');
};
var loadCategoriesAdded = function () {
    $("#master-container").load('/Home/Categories?pageAction=AddedCategory');
};
var loadCategoriesEdited = function () {
    $("#master-container").load('/Home/Categories?pageAction=EditedCategory');
};
var loadGame = function () {
    $("#master-container").load('/Home/Game');
};
var loadMain = function () {
    $("#master-container").load('/Home/Main');
};
var loadAddGame = function () {
    $("#master-container").load('/Game/Add');
};
var loadCreateCategory = function () {
    $("#master-container").load('/Category/Create');
};
var loadParticipateGame = function () {
    $("#master-container").load('/Game/Participate');
};

var submitCategory = function () {
    var form = $('#categoryForm');
    var id = $('#Id')[0].value;
    $.ajax({
        url: '/Category/SubmitCategory',
        data: form.serialize(),
        async: false
    });
    if (id === '') {
        loadCategoriesAdded();
    }
    else {
        loadCategoriesEdited();
    }
};
var editCategory = function () {
    var form = $('#categoryEditForm');
    var selectedIndex = form[0][0].selectedIndex;
    if (selectedIndex === 0) {
        alert("Vælg en kategori, du vil redigere");
        return false;
    }
    $.post('/Category/Edit', form.serialize()).done(function (data) {
        $("#master-container").html(data);
    });
};
var completeAddGame = function () {
    var form = $('#addGameForm');
    var selectedIndex = form[0][0].selectedIndex;
    if (selectedIndex === 0) {
        alert("Vælg en kategori, du vil bruge i spillet");
        return false;
    }
    $.ajax({
        url: '/Game/Completeadd',
        data: form.serialize(),
        async: false
    });
    loadGame()
};
var completeParticipation = function () {
    var form = $('#completeParticipationForm');
    var selectedIndex = form[0][0].selectedIndex;
    var selectedGameIndex = form[0][0].selectedIndex;
    if (selectedGameIndex === 0) {
        alert("Vælg et spil, du vil deltage i");
        return false;
    }
    var selectedCategoryIndex = form[0][1].selectedIndex;
    if (selectedCategoryIndex === 0) {
        alert("Vælg en kategori, du bruge i spillet");
        return false;
    }
    $.ajax({
        url: '/Game/CompleteParticipation',
        data: form.serialize(),
        async: false
    });
    loadGame()
};
var selectWinner = function () {
    var form = $('#selectWinnerForm');
    var selectedIndex = form[0][0].selectedIndex;
    if (selectedIndex === 0) {
        alert("Vælg den spiller, der vandt runden");
        return false;
    }
    $.ajax({
        url: '/Game/SelectWinner',
        data: form.serialize(),
        async: false
    });
    loadGame()
};
var selectAnswer = function () {
    var form = $('#selectAnswerForm');
    $.ajax({
        url: '/Game/SelectAnswer',
        data: form.serialize(),
        async: false
    });
    loadGame()
};
