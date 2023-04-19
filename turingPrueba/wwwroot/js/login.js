$(document).ready(function(){
    $(".veen .rgstr-btn button").click(function(){
        $('.veen .wrapper').addClass('move');
        $('.veen').css('background','#025949');
        $(".veen .login-btn button").removeClass('active');
        $(this).addClass('active');

    });
    $(".veen .login-btn button").click(function(){
        $('.veen .wrapper').removeClass('move');
        $('.veen').css('background','#025373');
        $(".veen .rgstr-btn button").removeClass('active');
        $(this).addClass('active');
    });
});