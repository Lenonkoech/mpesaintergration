// Format JSON response into a readable format
function formatResponsePreview(response) {
    // Create a table to show the key-value pairs from JSON
    let html = "<table class='table table-bordered table-striped col-md-4'>";
    for (const [key, value] of Object.entries(response)) {
        html += `<tr><th>${key}</th><td>${value}</td></tr>`;
    }
    html += "</table>";
    return html;
}

document.addEventListener("DOMContentLoaded", () => {
    const registerBtn = document.getElementById("registerBtn");
    const registerFeedback = document.getElementById("registerFeedback");
    const simulateForm = document.getElementById("simulateForm");
    const simulateFeedback = document.getElementById("simulateFeedback");
    const stkForm = document.getElementById("stkForm");
    const stkResponse = document.getElementById("stkResponse");

    // Register Button
    registerBtn.addEventListener("click", async () => {
        registerBtn.textContent = "Registering...";
        registerBtn.disabled = true;

        try {
            const res = await fetch("/register-urls");
            const data = await res.json();
            registerFeedback.innerHTML = formatResponsePreview(data);
        } catch (error) {
            registerFeedback.textContent = "Error registering URLs.";
            console.error(error);
        }

        registerBtn.textContent = "Register URL";
        registerBtn.disabled = false;
    });

    // Simulate Payment Form
    simulateForm.addEventListener("submit", async (e) => {
        e.preventDefault();
        simulateFeedback.textContent = "Sending payment...";

        try {
            const res = await fetch("/makePayment", { method: "POST" });
            const data = await res.json();
            simulateFeedback.innerHTML = formatResponsePreview(data);
        } catch (error) {
            simulateFeedback.textContent = "Error simulating C2B payment.";
            console.error(error);
        }
    });

    // STK Push Form
    stkForm.addEventListener("submit", async (e) => {
        e.preventDefault();

        const phoneInput = document.getElementById('phoneNumber').value.trim();
        const regex = /^(07\d{8}|01\d{8})$/; // Allow both 07 and 01 prefixes
        if (!regex.test(phoneInput)) {
            stkResponse.textContent = 'Invalid phone number. Format: 07XXXXXXXX or 01XXXXXXXX';
            return;
        }

        // Convert to 2547XXXXXXXX or 2541XXXXXXXX (for 07 or 01 prefixes)
        const formattedPhone = '254' + phoneInput.slice(1); // Remove leading 0 and prepend 254

        const formData = new FormData();
        formData.append('phoneNumber', formattedPhone);

        // Show loading message
        stkResponse.textContent = 'Processing payment, please wait...';

        try {
            // Make the API call to M-Pesa Express payment endpoint
            const response = await fetch('/mpesaExpressPayment', { method: 'POST', body: formData });

            if (!response.ok) {
                throw new Error('Payment request failed');
            }

            const result = await response.json();
            stkResponse.innerHTML = formatResponsePreview(result);
        } catch (error) {
            stkResponse.textContent = `Error: ${error.message}`;
            console.error(error);
        }
    });
});