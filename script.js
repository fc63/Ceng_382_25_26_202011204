let loginAttempts = [];

document.getElementById("loginButton").addEventListener("click", function () {
    let username = document.getElementById("username").value;
    let password = document.getElementById("password").value;
    
    if (username && password) {
        loginAttempts.push({ username, password });
        console.log("Login Attempts:", loginAttempts);
    } else {
        alert("Please enter both username and password.");
    }
});

function updateClock() {
    let now = new Date();
    let timeString = now.toLocaleTimeString();
    document.getElementById("liveClock").textContent = timeString;
}
setInterval(updateClock, 1000);
updateClock();

document.addEventListener("keydown", function (event) {
    if (event.key.toLowerCase() === "h") {
        let forms = document.querySelectorAll("input, button");
        forms.forEach(el => {
            el.style.display = (el.style.display === "none") ? "block" : "none";
        });
    }
});
