window.setTimeout(function () {
    $(".alert").fadeTo(500, 0).slideUp(500, function () {
        $(this).remove();
    });
}, 3000);

function ValidateForm(form, id) {
    if (id === "Categories" && form.ChosenCategoryGuid.selectedIndex === 0) {
        alert("Vælg en kategori, du vil redigere");
        return false;
    }
    if (id === "Participate") {
        if (form.ChosenGameGuid.selectedIndex === 0) {
            alert("Vælg et spil, du vil deltage i");
            return false;
        }
        if (form.ChosenCategoryGuid.selectedIndex === 0) {
            alert("Vælg en kategori, du bruge i spillet");
            return false;
        }
    }
    if (id === "AddGame" && form.ChosenCategoryGuid.selectedIndex === 0) {
        alert("Vælg en kategori, du vil bruge i spillet");
        return false;
    }
    if (id === "SelectWinner" && form.ChosenWinnerId.selectedIndex === 0) {
        alert("Vælg den spiller, der vandt runden");
        return false;
    }
    form.submit();
}

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