
function poplayer(msg) {
    if ($(".popMain1").html() != undefined) {

        var prohtml = "<div class=\"popcon\"><span id=\"msgpop\">" + msg + "</span></div>";
        $('.popMain1').html(prohtml);
    }
    else {
       
        var prohtml = "<div class=\"popLayer\"></div><div class=\"popMain1\"><div class=\"popcon\"><span id=\"msgpop\">" + msg + "</span></div></div>";
        $('body').append(prohtml);
    }
    var pmgd = document.documentElement.clientHeight;
    var pmkd = document.documentElement.clientWidth;
    var ysgd = $(".popMain1").height();
    var topz = (parseInt(pmgd) - parseInt(ysgd)) / 2;
    $(".popMain1").css({ "top": topz + "px" });

    $('.popLayer').fadeIn();
    $('.popMain1').fadeIn();
    setTimeout(function () {
        $('.popLayer').fadeOut();
        $('.popMain1').fadeOut();
    }, 1000);
    $('.popLayer').click(function () {
        $('.popMain1').fadeOut();
        $('.popLayer').fadeOut();

    });
}
