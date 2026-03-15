document.addEventListener('DOMContentLoaded', function () {
    const spinner = document.getElementById('spinner');
    const pageContainer = document.getElementById('page-container');

    function showSpinner() {
        spinner.style.display = 'flex';
        pageContainer.classList.add('disabled');
    }
    function hideSpinner() {
        spinner.style.display = 'none';
        pageContainer.classList.remove('disabled');
    }

    // start hidden (extra safety)
    hideSpinner();

    // Detect if the page already contains validation messages (server-side returned the form with errors).
    function pageHasValidationErrors() {
        // any data-valmsg-for with text
        const msgs = document.querySelectorAll('[data-valmsg-for]');
        for (const m of msgs) {
            if (m.textContent.trim()) return true;
        }
        // common validation classes used by frameworks
        if (document.querySelector('.validation-summary-errors, .field-validation-error, .input-validation-error, .text-danger')) return true;
        return false;
    }

    // Detect validation errors inside a specific form (used after client-side validation run)
    function formHasValidationErrors(form) {
        // check [data-valmsg-for] inside the form
        const msgs = form.querySelectorAll('[data-valmsg-for]');
        for (const m of msgs) {
            if (m.textContent.trim()) return true;
        }
        // inputs with error classes
        if (form.querySelector('.input-validation-error, .field-validation-error')) return true;
        // fallback to HTML5 invalid controls
        const invalidControl = form.querySelector(':invalid');
        if (invalidControl) return true;
        return false;
    }

    // If server returned validation errors, make sure spinner is hidden.
    if (pageHasValidationErrors()) {
        hideSpinner();
    }

    // Attach submit handlers for all forms
    document.querySelectorAll('form').forEach(form => {
        form.addEventListener('submit', function (e) {
            // only care about POSTs (adjust if you want GET covered)
            if (!form.method || form.method.toLowerCase() !== 'post') return;

            // If jQuery + jquery.validate is present, trigger it now (this populates validation messages synchronously)
            try {
                if (window.jQuery && jQuery(form).data('validator')) {
                    jQuery(form).valid(); // runs client validation and writes messages
                }
            } catch (err) {
                // ignore if jQuery not present or validate throws
            }

            // Defer decision until other submit handlers finish (unobtrusive validation handlers usually run in same event loop).
            setTimeout(function () {
                // If any validation messages found, don't show spinner
                if (formHasValidationErrors(form)) {
                    hideSpinner();
                    return;
                }

                // Use HTML5 checkValidity() as final fallback
                if (!form.checkValidity()) {
                    hideSpinner();
                    return;
                }

                // If the submit event was prevented by another handler, do not show spinner
                if (e.defaultPrevented) {
                    hideSpinner();
                    return;
                }

                // Otherwise we assume the submission will proceed — show spinner
                showSpinner();
            }, 0);
        }, false);
    });

    // Intercept XHR to hide spinner when requests end
    (function (open) {
        XMLHttpRequest.prototype.open = function (method, url, async, user, pass) {
            this.addEventListener('loadend', function () {
                hideSpinner();
            });
            open.call(this, method, url, async, user, pass);
        };
    })(XMLHttpRequest.prototype.open);

    // Wrap fetch to show/hide spinner around requests (optional; only used if fetch() is called)
    if (window.fetch) {
        const _fetch = window.fetch;
        window.fetch = function (...args) {
            showSpinner();
            return _fetch.apply(this, args).finally(() => hideSpinner());
        };
    }

    // If page restored from bfcache, ensure spinner hidden
    window.addEventListener('pageshow', function (event) {
        if (event.persisted) hideSpinner();
    });
});
