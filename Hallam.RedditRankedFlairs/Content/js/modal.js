function modal(id) {
    $(id).addClass('modal-show');
}

modal.close = function () {
    $('.modal-show').removeClass('modal-show');
};

$(function () {
    // $('.modal-overlay').click(function () { $('.modal-show').removeClass('modal-show'); });
});