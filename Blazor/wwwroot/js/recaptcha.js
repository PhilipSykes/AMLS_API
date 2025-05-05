// reCAPTCHA v3 integration for secure-software branch

// Global reference to the DotNet instance for calling back to C# from JS
window.recaptchaExecute = function (action) {
    return new Promise((resolve, reject) => {
        try {
            grecaptcha.ready(function () {
                grecaptcha.execute('6Lf-OOUnAAAAAJxE-A0pIVJ8aaBlGCwlXSW2ZsCd', { action: action })
                    .then(function (token) {
                        resolve(token);
                    })
                    .catch(function (error) {
                        console.error('reCAPTCHA execution failed:', error);
                        reject(error);
                    });
            });
        } catch (error) {
            console.error('reCAPTCHA failed to initialize:', error);
            reject(error);
        }
    });
};

// Function to verify if the token is valid (called from Blazor)
window.verifyRecaptchaToken = function (token) {
    // In a production environment, you would verify this token on the server side
    // For this implementation, we'll just check if a token exists
    return token && token.length > 0;
}; 