$(document).ready(function () {
    $('a.nav-link').click(function (e) {
        e.preventDefault();
    });
});

function toggleAccountTab() {
    $('#account-details').toggle();
}
