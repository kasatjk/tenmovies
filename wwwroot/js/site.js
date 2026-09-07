// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Smooth animations for validation error messages
document.addEventListener('DOMContentLoaded', function () {
    // Reset animation on validation errors to trigger it every time
    const form = document.querySelector('form');
    if (form) {
        form.addEventListener('submit', function () {
            // Remove and re-add animation classes to trigger animations
            const errorElements = document.querySelectorAll('[data-valmsg-summary], .validation-summary-errors, .text-danger');
            errorElements.forEach(el => {
                el.style.animation = 'none';
                setTimeout(() => {
                    el.style.animation = '';
                }, 10);
            });
        });
    }

    // Observer for dynamically added error messages (e.g., from AJAX validation)
    if (window.MutationObserver) {
        const observer = new MutationObserver(function (mutations) {
            mutations.forEach(function (mutation) {
                if (mutation.addedNodes.length) {
                    mutation.addedNodes.forEach(function (node) {
                        if (node.nodeType === 1) { // Element node
                            if (node.classList && (node.classList.contains('text-danger') || 
                                                   node.classList.contains('validation-summary-errors') ||
                                                   node.hasAttribute('data-valmsg-summary'))) {
                                node.style.animation = 'slideInError 0.3s ease-out forwards';
                            }
                        }
                    });
                }
            });
        });

        observer.observe(document.body, {
            childList: true,
            subtree: true
        });
    }
});
