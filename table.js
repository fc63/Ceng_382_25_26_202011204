document.getElementById("addClass").addEventListener("click", function () {
    let className = document.getElementById("className").value;
    let numPeople = document.getElementById("numPeople").value;
    let description = document.getElementById("description").value;

    if (className && numPeople && description) {
        let tableBody = document.getElementById("classTable").getElementsByTagName("tbody")[0];
        let newRow = tableBody.insertRow();
        
        let cell1 = newRow.insertCell(0);
        let cell2 = newRow.insertCell(1);
        let cell3 = newRow.insertCell(2);

        cell1.textContent = className;
        cell2.textContent = numPeople;
        cell3.textContent = description;

        // Satırın üzerine gelince renk değiştir
        newRow.addEventListener("mouseover", function () {
            newRow.style.backgroundColor = "lightgray";
        });

        // Fare çıkınca eski haline dön
        newRow.addEventListener("mouseout", function () {
            newRow.style.backgroundColor = "";
        });

        // Çift tıklayınca satırı kaldır
        newRow.addEventListener("dblclick", function () {
            newRow.remove();
            console.log("Row removed.");
        });

        // Formu temizle
        document.getElementById("className").value = "";
        document.getElementById("numPeople").value = "";
        document.getElementById("description").value = "";
		
	sortTable();
    } else {
        alert("Please fill all fields.");
    }
});

// **TABLOYA (ama satırlara değil) TIKLAMA EVENTİ EKLE**
document.getElementById("classTable").addEventListener("click", function (event) {
    if (event.target.tagName !== "TD") {  // Eğer tıklanan eleman bir hücre değilse
        console.log("Table clicked - Showing all data:");
        let rows = document.querySelectorAll("#classTable tbody tr");
        let allData = [];
        rows.forEach(row => {
            let rowData = [];
            row.querySelectorAll("td").forEach(cell => rowData.push(cell.textContent));
            allData.push(rowData);
        });
        console.log(allData);
    }
});

// Form alanına odaklanınca stil değiştir
let inputs = document.querySelectorAll("input, textarea");
inputs.forEach(input => {
    input.addEventListener("focus", function () {
        input.style.border = "2px solid white";
    });

    input.addEventListener("blur", function () {
        input.style.border = "";
    });
});

function sortTable() {
    let table = document.getElementById("classTable");
    let tbody = table.getElementsByTagName("tbody")[0];
    let rows = Array.from(tbody.getElementsByTagName("tr"));

    rows.sort((a, b) => {
        let textA = a.cells[0].textContent.trim().toLowerCase();
        let textB = b.cells[0].textContent.trim().toLowerCase();
        return textA.localeCompare(textB);
    });

    tbody.innerHTML = "";
    rows.forEach(row => tbody.appendChild(row));
}
