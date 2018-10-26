$(document).on('click', '.navbar-collapse.in', function (e) {
    if ($(e.target).is('a')) {
        $(this).collapse('hide');
    }
});

var connection = null;
var signalRIsConnected = false;

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
    return gameId;
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
        alert("Du skal navngive din kategori");
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
    var r = confirm('Kategorien slettes og kan ikke gendannes!');
    if (r === true) {
        $.ajax({
            url: '/Category/Delete',
            type: 'post',
            data: { chosenCategoryGuid: categoryId },
            async: false
        }).done(function (data) {
            if (data) {
                loadCategoriesDeleted();
            }
        });
    }
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
    loadGame();
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
        alert("Vælg en kategori, du vil bruge i spillet");
        return false;
    }
    $.ajax({
        url: '/Game/CompleteParticipation',
        data: form.serialize(),
        async: false
    });
    loadGame();
};
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
var eatYourNote = function (button) {
    promptAnswer = confirm("Bekræft, at ingen kunne gætte spørgsmålet.");
    if (!promptAnswer) {
        return false;
    }

    if (button != null) {
        addSpinner(button);
    }
    $.ajax({
        url: '/Game/EatYourNote',
        type: 'post',
        success: function () {
            loadGame();
        }
    });
};

var leaveGame = function (button) {
    promptAnswer = confirm("Bekræft, at du vil forlade det aktuelle spil.");
    if (!promptAnswer) {
        return false;
    }

    if (button != null) {
        addSpinner(button);
    }
    $.ajax({
        url: '/Game/LeaveGame',
        type: 'post',
        success: function () {
            loadMain();
        },
        error: function () {
            removeSpinner(button);
            alert('Du kan ikke forlade spillet, når det er din tur til at gøre noget.');
        }
    });
};
