window.showMap = (lat, lon) => {
    const mapDiv = document.getElementById("map");
    mapDiv.innerHTML = ""; // Clean up old map instance

    const map = L.map("map").setView([lat, lon], 13);

    L.tileLayer("https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png", {
        attribution: '© OpenStreetMap contributors'
    }).addTo(map);

    L.marker([lat, lon]).addTo(map)
        .bindPopup("Ти си тук!")
        .openPopup();
};
