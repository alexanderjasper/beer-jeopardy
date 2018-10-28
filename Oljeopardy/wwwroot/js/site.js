$(document).on('click', '.navbar-collapse.in', function (e) {
    if ($(e.target).is('a')) {
        $(this).collapse('hide');
    }
});

var connection = null;
var signalRIsConnected = false;
var categoryToDeleteId = null;

$(document).ready(function () {
    $(document).click(function (event) {
        var clickover = $(event.target);
        var _opened = $(".navbar-collapse").hasClass("navbar-collapse collapse in");
        if (_opened === true && !clickover.hasClass("navbar-toggle")) {
            $("button.navbar-toggle").click();
        }
    });
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

function addSpinner(button) {
    button.classList.add('active');
}

function removeSpinner(button) {
    button.classList.remove('active');
}

function checkIfGameChanged() {
    var gameId = '';
    $.ajax({
        url: "/getGameId",
        async: false,
        dataType: 'text',
        success: function (data) {
            gameId = data;
        }
    });

    if (gameId !== '' && !signalRIsConnected) {
        reconnectWebsocket(gameId);
    }
    return gameId;
}

function reconnectWebsocket(gameId) {
    if (connection != null){
        connection.stop();
    }

    connection = new signalR.HubConnectionBuilder()
        .withUrl('/gameUpdate')
        .configureLogging(signalR.LogLevel.Information)
        .build();

    connection.on('gameUpdated', function (data) {
        connection.stop();
        gameBottomMargin = $('#gameBottomMargin');
        if (gameBottomMargin.length && gameBottomMargin.length > 0) {
            loadGame();
        }
    });

    connection.onclose( function () {
        signalRIsConnected = false;
        loadGame();
    });

    connection.start()
        .then(function () { 
            connection.invoke('joinGroup', gameId);
            signalRIsConnected = true;
    });
}

function selectAnswerQuestion(elem) {
    var labels = document.getElementsByClassName('category-radio-button-label');
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
var loadCategoriesDeleted = function () {
    $("#master-container").load('/Home/Categories?pageAction=DeletedCategory');
};
var loadGame = function () {
    var gameId = checkIfGameChanged();
    if (gameId === '') {
        $("#master-container").load('/Home/Main');
    }
    else {
        $("#master-container").load('/Home/Game');
    }
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
    var name = $('#Name')[0].value;
    var form = $('#categoryForm');
    var id = $('#Id')[0].value;
    if (name === null || $.trim(name) == '') {
        showElement('mustNameCategoryWarning');
        return false;
    }
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
var editCategory = function (categoryId) {
    $.post('/Category/Edit', { chosenCategoryGuid: categoryId }).done(function (data) {
        $("#master-container").html(data);
    });
};
var deleteCategory = function (categoryId) {
    categoryToDeleteId = categoryId;
    showElement('deleteCategoryModal');
};
var deleteCategoryConfirm = function (button) {
    addSpinner(button);
    $.ajax({
        url: '/Category/Delete',
        type: 'post',
        data: { chosenCategoryGuid: categoryToDeleteId },
        async: false
    }).done(function (data) {
        if (data) {
            hideElement('deleteCategoryModal');
            loadCategoriesDeleted();
        }
    });
    removeSpinner(button);
}

var completeAddGame = function () {
    var form = $('#addGameForm');
    if (validateCompleteAddGame(form)){
        $.ajax({
            url: '/Game/Completeadd',
            data: form.serialize(),
            async: false
        });
        loadGame();
    }
};

var validateCompleteAddGame = function (form) {
    var gameName = form[0][0].value;
    var selectedIndex = form[0][1].selectedIndex;
    var categoryMissing = selectedIndex === 0;
    chooseCategoryWarning(categoryMissing);
    var gameNameMissing = gameName === null || gameName === '';
    enterGameNameWarning(gameNameMissing);

    if (categoryMissing || gameNameMissing){
        return false;
    }
    return true;
}

var chooseCategoryWarning = function (shouldShow) {
    var warning = document.getElementById('chooseCategoryWarning');
    if (shouldShow){
        warning.style.display = "block";
    }
    else {
        warning.style.display = "none";
    }
}

var enterGameNameWarning = function (shouldShow) {
    var warning = document.getElementById('enterGameNameWarning');
    if (shouldShow){
        warning.style.display = "block";
    }
    else {
        warning.style.display = "none";
    }
}

var completeParticipation = function () {
    var form = $('#completeParticipationForm');
    if (validateCompleteParticipation(form)){
        $.ajax({
            url: '/Game/CompleteParticipation',
            data: form.serialize(),
            async: false
        });
        loadGame();
    }
};
var validateCompleteParticipation = function (form) {
    var gameIndex = form[0][0].selectedIndex;
    var categoryIndex = form[0][1].selectedIndex;
    var gameMissing = gameIndex === 0;
    showHideElement('gameMissingWarning', gameMissing);
    var categoryMissing = categoryIndex === 0;
    showHideElement('categoryMissingWarning', categoryMissing);
    
    if (gameMissing || categoryMissing) {
        return false;
    }
    return true;
}

var selectWinner = function (winnerId, gameId, button) {
    addSpinner(button);
    $.ajax({
        url: '/Game/SelectWinner',
        type: 'post',
        data: { ChosenWinnerId: winnerId, Game: { Id: gameId } },
        async: false
    });
    loadGame();
};
var selectAnswer = function (button, chosenAnswerQuestionId, gameId) {
    if (button != null) {
        addSpinner(button);
    }
    $.ajax({
        url: '/Game/SelectAnswer',
        data: {
            ChosenAnswerQuestionGuid: chosenAnswerQuestionId,
            GameId: gameId
        },
        async: false
    });
    loadGame();
};

var eatYourNoteConfirm = function (button) {
    if (button != null) {
        addSpinner(button);
    }

    $.ajax({
        url: '/Game/EatYourNote',
        type: 'post',
        success: function () {
            hideElement('eatYourNoteModal');
            loadGame();
            removeSpinner(button);
        }
    });
}

var leaveGameConfirm = function (button) {
    if (button != null) {
        addSpinner(button);
    }
    $.ajax({
        url: '/Game/LeaveGame',
        type: 'post',
        success: function () {
            loadMain();
            hideElement('leaveGameModal');
            removeSpinner(button);
        },
        error: function () {
            removeSpinner(button);
            hideElement('leaveGameModal');
            showElement('leaveGameError');
        }
    });
};

var showElement = function(elementId) {
    element = document.getElementById(elementId);
    element.style.display = 'block';
}
var hideElement = function(elementId) {
    element = document.getElementById(elementId);
    element.style.display = 'none';
}
var showHideElement = function(elementId, shouldShow) {
    if (shouldShow) {
        showElement(elementId);
    }
    else {
        hideElement(elementId);
    }
}