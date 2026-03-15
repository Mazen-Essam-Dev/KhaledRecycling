
document.addEventListener("DOMContentLoaded", function () {
    const indexList = document.getElementById("autoIndex");
    const headings = document.querySelectorAll("h2");
	
	if (!indexList) return;
	
    headings.forEach((heading, i) => {
        // Generate ID automatically if missing
        if (!heading.id) {
            heading.id = "section-" + (i + 1);
        }

        const li = document.createElement("li");
        const a = document.createElement("a");

        a.href = "#" + heading.id;
        a.textContent = heading.textContent;
		
		// // attribute For Print show tooltips if long index name
        //a.setAttribute("data-fulltext", heading.textContent);
		
        li.appendChild(a);
        indexList.appendChild(li);
    });
});
