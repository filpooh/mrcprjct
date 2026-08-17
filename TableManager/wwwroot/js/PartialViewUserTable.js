const mainCards = document.getElementById("mainCards");
const tablesContainer = document.getElementById("tablesContainer");
const btnBack = document.getElementById("btnBack");
const btnManageTables = document.getElementById("btnManageTables");
const partialTitle = document.getElementById("partialTitle");
const subDiv = document.getElementById("subDiv");
const btnManageModel = document.getElementById("btnManageModels");
//const btnDeleteFile = document.querySelectorAll("[id='DeleteFile']");
if (!mainCards || !tablesContainer || !btnBack || !btnManageTables || !btnManageModel) {
    console.warn("PartialViewTable.js: elementi non trovati nella pagina.");
} else {

    btnManageTables.addEventListener("click", function (e) {

        mainCards.style.display = "none";
        subDiv.style.display = "inline-block";//inutile?
        btnBack.style.display = "inline-block";
        partialTitle.style.display = "inline-block";
        partialTitle.textContent = "Select A Table";

        fetch('/Home/LoadUserTables')
            .then(response => response.text())
            .then(html => {
                tablesContainer.innerHTML = html;
            });
        
    });
    btnManageModel.addEventListener("click", function () {
        mainCards.style.display = "none";
        subDiv.style.display = "inline-block";//inutile?
        btnBack.style.display = "inline-block";
        partialTitle.style.display = "inline-block";
        partialTitle.textContent = "Select A Model";
        fetch('/Home/ModelList')
            .then(response => response.text())
            .then(html => {
                tablesContainer.innerHTML = html;
            });
    });

    btnBack.addEventListener("click", function () {

        mainCards.style.display = "block";
        tablesContainer.innerHTML = "";
        btnBack.style.display = "none";
        partialTitle.style.display = "none";
        subDiv.style.display = "none";

    });
    tablesContainer.addEventListener("click", function (e) {
        const btn = e.target.closest(".DeleteFile");
        const btnmodel = e.target.closest(".DeleteModel");
        if (btn) {
            const id = e.target.dataset.id;

            const tabel = confirm("Vuoi procedere con la cancellazione della tabella?");
            if (tabel) {

                const formData = new FormData();
                formData.append("id", id);
                formData.append("model", confirm("Vuoi eliminare anche tutti i modelli?"));

                $.ajax({
                    url: "/Home/DeleteFile",
                    type: "POST",
                    data: formData,
                    processData: false,
                    contentType: false,
                    success: function (response) {
                        console.log("OK");
                        window.location.href = response.redirect;
                    },
                    error: function (err) {
                        console.error("ERRORE", err);
                    }
                });
            }
        }
        if (btnmodel) {
            const mod = confirm("Vuoi eliminare il modello?")
            if (mod) {
                const id = e.target.dataset.id;

                const formData = new FormData();
                formData.append("id", id);
                $.ajax({
                    url: "/Home/DeleteModel",
                    type: "POST",
                    data: formData,
                    processData: false,
                    contentType: false,
                    success: function (response) {
                        console.log("OK");
                        window.location.href = response.redirect;
                    },
                    error: function (err) {
                        console.error("ERRORE", err)
                    }
                });
            }
        }
    });
}

 /*
}
function showConfirm(message) {
 return new Promise(resolve => {

     // --- CREAZIONE ELEMENTI ---
     const overlay = document.createElement('div');
     overlay.style.position = 'fixed';
     overlay.style.top = '0';
     overlay.style.left = '0';
     overlay.style.width = '100%';
     overlay.style.height = '100%';
     overlay.style.background = 'rgba(0,0,0,0.5)';
     overlay.style.display = 'flex';
     overlay.style.justifyContent = 'center';
     overlay.style.alignItems = 'center';
     overlay.style.zIndex = '9999';

     const box = document.createElement('div');
     box.style.background = '#fff';
     box.style.padding = '20px';
     box.style.borderRadius = '8px';
     box.style.minWidth = '250px';
     box.style.textAlign = 'center';

     const text = document.createElement('p');
     text.textContent = message;

     const btnYes = document.createElement('button');
     btnYes.textContent = 'Sì';
     btnYes.style.marginRight = '20px';

     const btnNo = document.createElement('button');
     btnNo.textContent = 'No';

     // --- EVENTI ---
     btnYes.onclick = () => {
         document.body.removeChild(overlay);
         resolve(true);
     };

     btnNo.onclick = () => {
         document.body.removeChild(overlay);
         resolve(false);
     };

     // --- ASSEMBLAGGIO ---
     box.appendChild(text);
     box.appendChild(btnYes);
     box.appendChild(btnNo);
     overlay.appendChild(box);
     document.body.appendChild(overlay);
 });
}*/