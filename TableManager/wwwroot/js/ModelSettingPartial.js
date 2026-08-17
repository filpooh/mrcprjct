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
            const type = document.getElementsByName("selectModel")[0].value;
            fetch(`/MlInterface/Index?name=@Model.Name&tableId=@Model.Id&type=${type}`)
                .then(r => r.json())
                .then(data => {
                    console.log("Risposta:", data);
                    alert("Modello generato!");
                })
                .catch(err => console.error(err));
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