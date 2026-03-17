function showToast(message, type = "success") {

    let bgColor = type === "success" ? "#1e7e34" : "#c82333";

    const toast = document.createElement("div");
    toast.className = "toast-item";
    toast.style.background = bgColor;
    toast.innerText = message;

    document.getElementById("toast-container").appendChild(toast);

    setTimeout(() => {
        toast.classList.add("show");
    }, 100);

    setTimeout(() => {
        toast.classList.remove("show");
        setTimeout(() => toast.remove(), 300);
    }, 3000);
}
