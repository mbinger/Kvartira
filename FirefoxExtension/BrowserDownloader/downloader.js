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
		
		var xhr = new XMLHttpRequest();
		xhr.open("POST", 'localhost:82', true);
		xhr.setRequestHeader('Content-Type', 'application/json');
		xhr.send(JSON.stringify({
			url: document.URL,
			document: documentHtml
		}));		
		
		console.log('send document ok');
	}
	catch (ex)
	{
		console.log('send document error');
		console.log(ex);
	}
});