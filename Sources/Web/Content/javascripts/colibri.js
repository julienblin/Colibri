function showIndicator() {
    $('indicator').show();
}

function hideIndicator() {
    $('indicator').hide();
}

function toggleIndicator() {
	$('indicator').toggle();
}

function dateTimeRandom() {
    var currentDate = new Date();
    return currentDate.getTime();
}

Ajax.Responders.register({
    onCreate: function(){
        showIndicator();
    },
    onComplete: function(){
        hideIndicator();
    }
});

SyntaxHighlighter.config.clipboardSwf = '/Content/javascripts/highlighter/clipboard.swf';