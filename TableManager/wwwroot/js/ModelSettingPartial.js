window.ModelSettingPartial = {
    init() {
        //mi basta espandere il contenuto di questa funzione
        const select = document.getElementById("fillValueSelect");
        const input = document.getElementById("empty-value-sub");
        const divInput = document.getElementById("empty-value-div")
        const emptyValuefill = document.getElementById("empty-value-fill");
        const selectModel = document.getElementById("selectModel");
        const regressionCard = document.getElementById("regressionCard");
        selectModel.addEventListener("change", function () {
            if (this.value == 0)
                regressionCard.style.display = "none";
            if (this.value == 1)
                regressionCard.style.display = "block";

        })
        if (!select || !input) return;

        select.addEventListener("change", function () {
            input.style.display = (this.value === "3") ? "block" : "none";
            divInput.style.display = (this.value === "3") ? "block" : "none";
            emptyValuefill.style.display = (this.value === "0") ? "none" : "block";
        });

        document.getElementById("generateModel").addEventListener("click", function () {
            const type = document.getElementById("selectModel").value;
            //const name = "@Model.Name";
            const tableId = document.getElementById("pageData").dataset.id;
            const formData = new FormData();
            formData.append("type", type);
            let header;
            switch (type) {
                case "1":   // attenzione: value è stringa
                    const div = document.getElementById("headers-container-regression");
                    header = div.querySelectorAll("input.form-check-input:checked");
                    break;
                default:
                    alert("Seleziona un tipo di regressione");
                    return;
            }

            // Converti NodeList  array di ID
            const headerId = Array.from(header).map(h => h.id);
            formData.append("headerId", JSON.stringify(headerId));

            //formData.append("name", name);
            formData.append("modelId", tableId);
            $.ajax({
                url: "/MlInterface/RequestByModel",
                type: "POST",
                data: formData,
                processData: false,   // obbligatorio per FormData
                contentType: false,   // obbligatorio per FormData
                success: function (data) {
                    console.log("Risposta:", data);
                    switch(data.stato){
                        case "2":
                            alert("Modello generato!");
                            break;
                        case "1":
                            alert("Modello in preparazione")
                            break;
                        case "-1":
                            alert("Errore creazione modello!");
                            break;
                    }

                    //window.location.href = data.redirectUrl;
                },
                error: function (err) {
                    console.error(err);
                    console.log(err.message)
                }
            });
        });
        /*document.getElementById("submitSetting").addEventListener("click", function () {
            const regressionType = document.getElementById().value;
        });*/
        document.getElementById("normalize-column-btn")
            .addEventListener("click", () => sendCheckedColumns("headers-container-normalize", "/Home/NormalizeColumns"));

        document.getElementById("dummy-column-btn")
            .addEventListener("click", () => sendCheckedColumns("headers-container-dummy", "/Home/DummyColumn"));
        document.getElementById("empty-value-fill")
            .addEventListener("click", () => SendFillValue("empty-value-sub".value))
    }
};

function sendCheckedColumns(containerId, url) {

    const headerContainer = document.getElementById(containerId);
    const listCheck = headerContainer.querySelectorAll("input.form-check-input:checked");
    const checkValues = Array.from(listCheck).map(x => x.id);

    const id = document.getElementById("pageData").dataset.id;

    const formData = new FormData();
    formData.append("id", id);

    checkValues.forEach(v => formData.append("headerId", v));

    $.ajax({
        url: url,
        type: "POST",
        data: formData,
        processData: false,
        contentType: false,
        success: function (response) {
            console.log("OK", response);
        },
        error: function (err) {
            console.error("ERRORE", err);
        }
    });
}
function SendFillValue(value) {
    const id = document.getElementById("pageData").dataset.id;
    const type = document.getElementById("fillValueSelect").value;
    const formData = new FormData();
    formData.append("type", type);
    formData.append("value", value);
    formData.append("id", id);
    $.ajax({
        url: "/Home/FillEmptyCell",
        type: "POST",
        data: formData,
        processData: false,
        contentType: false,
        success: function (response) {
            console.log("OK", response);
        },
        error: function (err) {
            console.error("ERRORE", err);
        }
    });
}
function GetEmptyRows() {
    //devo poi inserire l'html
    const id = document.getElementById("pageData").dataset.id;
    $.ajax({
        url: "/Home/GetEmptyRow",
        type: "POST",
        processData: false,
        contentType: false,
        success: function (response) {
            console.log("OK", response);
        },
        error: function (err) {
            console.error("ERRORE", err);
        }
    });

}