let nH = 1;
let h = false;
const selectCsvFile = document.getElementById("csvFile");
const txtTableName = document.getElementById("table-name");
selectCsvFile.addEventListener("change", function () {
    
    const file = selectCsvFile.files[0];
    if (!file) return;

    // Prende il nome del file senza estensione
    const nameWithoutExt = file.name.replace(/\.[^/.]+$/, "");
    txtTableName.value = nameWithoutExt;
    if (!file) {
        alert("Seleziona un file CSV");
        return;
    }

    var formData = new FormData();
    formData.append("file", file);

    $.ajax({
        url: "/Home/UploadCsv",
        type: "POST",
        data: formData,
        processData: false,
        contentType: false,
        success: function (headers) {

            let html = "<h4>Seleziona le colonne da mantenere:</h4>";

            headers.forEach((h, i) => {
                html += `
                <div class="d-flex align-items-center mt-2" style="width:300px;">
                    <input type="checkbox" class="csv-header me-2" value="${h}" checked>
                    <span>${h}</span>
                </div>
            `;
            });

            //  html += `<button onclick="confirmHeaders()">Conferma</button>`;
            document.getElementById("sumbitTable").style.display = "block"
            $("#csvResult").html(html);
        },
        error: function (xhr) {
            alert("Errore: " + xhr.responseText);
        }
    });
});
function addHeader() {
    nH++;
    const hb = document.querySelector("#header-row-1 button");
    const ht = document.querySelector("#header-row-1 input");
    const newDiv = document.createElement("div");
    newDiv.id = "header-row-" + nH;
    newDiv.className = "d-flex align-items-center mb-2 ms-5";

    const newHeader = document.createElement("input");
    newHeader.name = "header-" + nH;
    newHeader.value = "";
    newHeader.className = ht.className;

    const newButton = document.createElement("button");
    newButton.innerText = "-";
    newButton.className = hb.className + " remove-btn";
    newButton.onclick = () => newDiv.remove();

    newDiv.appendChild(newHeader);
    newDiv.appendChild(newButton);
    // Trova l'ultima riga esistente
    const lastRow = document.getElementById("header-row-" + (nH - 1));
    lastRow.after(newDiv);

}

function submitTable() {
    //deve mandare gli header al controller 
    const selected = Array.from(document.querySelectorAll(".csv-header:checked"))
        .map(cb => cb.value);

    var file = document.getElementById("csvFile").files[0];

    var formData = new FormData();
    formData.append("file", file);
    formData.append("name", file.name.split('.').slice(0, -1).join('.'))
    formData.append("headers", JSON.stringify(selected));

    $.ajax({
        url: "/Home/SubmitTable",
        type: "POST",
        data: formData,
        processData: false,
        contentType: false,
        success: function (response) {
            console.log("OK", response);

            if (response.redirect) {
                window.location.href = response.redirect;
            }
        },
        error: function (err) {
            console.error("Errore", err);
        }
    });
    /*name = document.getElementById().innerText;
    $(".header-input").each(function () {
        model.Headers.push($(this).val());
    });

    $.ajax({
        url: "/Home/SubmitHeader",
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify(model),
        success: function (response) {
            console.log("Salvato!", response);
            h = true;
        },
        error: function (xhr) {
            console.error("Errore:", xhr.responseText);
        }
    });
    //deve mandare al controller un ajax con tutte le info*/
   /* var model = {
        name: document.getElementById("table-name").value,
        Headers: []
    };
    $(".header-input").each(function () {
        model.Headers.push($(this).val());
    });

    $.ajax({
        url: "/Home/SubmitTable",
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify(model),
        success: function (response) {
            console.log("Salvato!", response);
            if (response.redirect)
                window.location.href = response.redirect;
        },
        error: function (xhr) {
            console.error("Errore:", xhr.responseText);
        }
    });*/
}
function UploadCsv() {

    var fileInput = document.getElementById("csvFile");
    var file = fileInput.files[0];

    if (!file) {
        alert("Seleziona un file CSV");
        return;
    }

    var formData = new FormData();
    formData.append("file", file);

    $.ajax({
        url: "/Home/UploadCsv",
        type: "POST",
        data: formData,
        processData: false,
        contentType: false,
        success: function (headers) {

            let html = "<h4>Seleziona le colonne da mantenere:</h4>";

            headers.forEach((h, i) => {
                html += `
                <div class="d-flex align-items-center mt-2" style="width:300px;">
                    <input type="checkbox" class="csv-header me-2" value="${h}" checked>
                    <span>${h}</span>
                </div>
            `;
            });

            //  html += `<button onclick="confirmHeaders()">Conferma</button>`;
            document.getElementById("sumbitTable").style.display = "block"
            $("#csvResult").html(html);
        },
        error: function (xhr) {
            alert("Errore: " + xhr.responseText);
        }
    });
}