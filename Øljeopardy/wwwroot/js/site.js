window.setTimeout(function () {
    $(".alert").fadeTo(500, 0).slideUp(500, function () {
        $(this).remove();
    });
}, 3000);

function ValidateForm(form) {
    if (form.ChosenCategoryGuid.selectedIndex == 0) {
        alert("Vælg en kategori, du vil redigere");
        return false;
    }
    form.submit();
}