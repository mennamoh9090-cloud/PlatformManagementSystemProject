async function apiRequest(url, method = 'GET', data = null) {

    const token = sessionStorage.getItem("token");

    const options = {
        method: method,
        headers: {
            "Content-Type": "application/json",
            "Authorization": "Bearer " + token
        }
    };

    if (data)
        options.body = JSON.stringify(data);

    const response = await fetch("https://localhost:7102/" + url, options);

    if (!response.ok) {
        throw new Error("API Error");
    }

    return await response.json();
}

async function loadPage(url) {
    const response = await fetch(url);
    const html = await response.text();
    document.getElementById("content").innerHTML = html;
}
