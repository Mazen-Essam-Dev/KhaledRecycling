(function () {
    const revealItems = document.querySelectorAll(".reveal");

    if (!revealItems.length) {
        return;
    }

    if (window.matchMedia("(prefers-reduced-motion: reduce)").matches || !("IntersectionObserver" in window)) {
        revealItems.forEach((item) => item.classList.add("reveal-visible"));
        return;
    }

    const observer = new IntersectionObserver((entries, currentObserver) => {
        entries.forEach((entry) => {
            if (!entry.isIntersecting) {
                return;
            }

            entry.target.classList.add("reveal-visible");
            currentObserver.unobserve(entry.target);
        });
    }, {
        threshold: 0.14,
        rootMargin: "0px 0px -40px 0px"
    });

    revealItems.forEach((item) => observer.observe(item));
})();
