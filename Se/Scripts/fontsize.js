$(function(){
	var html = document.documentElement;
	var windowWidth = html.clientWidth;
	html.style.fontSize = windowWidth / 7.5 + 'px';
})
$(window).resize(function() {
	var html = document.documentElement;
	var windowWidth = html.clientWidth;
	html.style.fontSize = windowWidth / 7.5 + 'px';
});	
