function docReady(fn) {
    // see if DOM is already available
    if (document.readyState === "complete" || document.readyState === "interactive") {
        // call on next available tick
        setTimeout(fn, 1);
    } else {
        document.addEventListener("DOMContentLoaded", fn);
    }
}

docReady(function() {
    try
	{
		console.log('send document....');
		var documentHtml = document.documentElement.innerHTML;
		console.log('sending...');
		
		var xhr = new XMLHttpRequest();
		xhr.open("POST", 'http://127.0.0.1:82', true);
		xhr.setRequestHeader('Content-Type', 'application/json');
		xhr.onerror = function(e) {
            console.log('error', e);
        };
		
		var content = JSON.stringify({
			url: document.URL,
			document: documentHtml
		});
		
		xhr.send(content);		
		
		
		console.log('send document ok');
	}
	catch (ex)
	{
		console.log('send document error');
		console.log(ex);
	}
});