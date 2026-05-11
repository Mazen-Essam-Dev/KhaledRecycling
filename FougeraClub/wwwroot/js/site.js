(() => {
    const reduceMotion = window.matchMedia("(prefers-reduced-motion: reduce)").matches;
    let revealObserver = null;

    const revealSelectors = [
        ".card",
        ".data-part",
        ".icon_box_all",
        ".table-responsive",
        ".records-count-box",
        ".filterImg",
        ".c-edit-form",
        ".modal-content",
        ".row.my-3",
        "main",
        "form"
    ];

    const prepareRevealElement = (element, index = 0) => {
        if (element.classList.contains("no-reveal") || element.closest(".no-reveal")) {
            return;
        }

        element.classList.add("reveal", "reveal-up");
        element.style.transitionDelay = `${Math.min(index % 8, 7) * 45}ms`;

        if (revealObserver) {
            revealObserver.observe(element);
        }
    };

    const enhanceRevealTargets = (root = document) => {
        root.querySelectorAll(revealSelectors.join(",")).forEach((element, index) => {
            if (element.classList.contains("no-reveal") || element.closest(".no-reveal")) {
                return;
            }

            prepareRevealElement(element, index);
        });
    };

    const revealImmediately = () => {
        document.querySelectorAll(".reveal").forEach((element) => {
            element.classList.add("reveal-visible");
            element.style.transitionDelay = "";
        });
    };

    const initRevealObserver = () => {
        if (reduceMotion || !("IntersectionObserver" in window)) {
            revealImmediately();
            return;
        }

        revealObserver = new IntersectionObserver((entries) => {
            entries.forEach((entry) => {
                if (!entry.isIntersecting) {
                    return;
                }

                entry.target.classList.add("reveal-visible");
                revealObserver.unobserve(entry.target);
            });
        }, {
            root: null,
            threshold: 0.12,
            rootMargin: "0px 0px -48px 0px"
        });

        document.querySelectorAll(".reveal").forEach((element) => revealObserver.observe(element));
    };

    const enhanceInteractiveStates = () => {
        document.querySelectorAll(".table").forEach((table) => {
            table.classList.add("align-middle");
        });

        document.querySelectorAll("img").forEach((image) => {
            if (!image.closest(".navbar-brand") && !image.closest(".nav-left") && !image.closest(".img-hover")) {
                return;
            }

            image.style.transition = image.style.transition || "transform 260ms cubic-bezier(.2, .8, .2, 1)";
        });
    };

    document.addEventListener("DOMContentLoaded", () => {
        enhanceRevealTargets();
        enhanceInteractiveStates();
        initRevealObserver();

        const mutationObserver = new MutationObserver((mutations) => {
            mutations.forEach((mutation) => {
                mutation.addedNodes.forEach((node) => {
                    if (!(node instanceof HTMLElement)) {
                        return;
                    }

                    if (node.matches(revealSelectors.join(","))) {
                        prepareRevealElement(node);
                    }

                    enhanceRevealTargets(node);
                    enhanceInteractiveStates();
                });
            });
        });

        mutationObserver.observe(document.body, {
            childList: true,
            subtree: true
        });
    });
})();
