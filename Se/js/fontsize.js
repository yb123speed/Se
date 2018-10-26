$(function(){
	var html = document.documentElement;
	var windowWidth = html.clientWidth;
	html.style.fontSize = windowWidth / 7.5 + 'px';
	var wH=($('.main').outerHeight()<$(window).height())?$(window).height():'auto';
	$('.main').height(wH);
})
$(window).resize(function() {
	var html = document.documentElement;
	var windowWidth = html.clientWidth;
	html.style.fontSize = windowWidth / 7.5 + 'px';
	var wH=($('.main').outerHeight()<$(window).height())?$(window).height():'auto';
	$('.main').height(wH);
});	
