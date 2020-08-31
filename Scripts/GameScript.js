$(document).ready(function () {
    $('a.nav-link').click(function (e) {
        e.preventDefault();
    });
});

function toggleAccountTab() {
    $('#account-details').toggle();
}

/* Empire */
function empireHide() {
    $('#empire-content').slideUp();
}

function empireShow() {
    $.ajax({
        type: 'GET',
        url: '/Empire/Index',
        data: '',
        contentType: 'application/json',
        dataType: 'html',
        complete: function (response) {
            $('#empire-content').html(response.responseText);
            $('#empire-content').slideDown();
        }
    });

    fillPlayerHeader();
}

function createEmpire() {
    $('#empire-content').slideUp();

    $.ajax({
        type: 'GET',
        url: '/Empire/Create',
        data: '',
        contentType: 'application/json',
        dataType: 'html',
        complete: function (response) {
            $('#empire-content').html(response.responseText);
            $('#empire-content').slideDown();
        }
    });
}

function doCreateEmpire() {
    var data = $('#create-empire-form').serialize();

    $('#empire-content').slideUp();

    $.ajax({
        type: 'POST',
        url: '/Empire/Create',
        data: data,
        dataType: 'json',
        complete: function (response) {
            $('#empire-content').html(response.responseText);
            $('#empire-content').slideDown();
        }
    });
}

/* Player */
function fillPlayerHeader() {
}

function showEPanel1() {
    $('#epanel1').show();
    $('#epanel2').hide();
}

function showEPanel2() {
    $('#epanel1').hide();
    $('#epanel2').show();
}