const id = document.getElementById("pageData").dataset.id;
/**
 * 
 * riguardare js in quanto come carico i setting è il metodo corretto 
 * in quanto faccio tornare solo una volta i dati dal controller
 *  e posso prendere i dati direttamente dall'html
 */
function loadPartial(url, target) {
    fetch(url)
        .then(response => {
            console.log("Status:", response.status);

            if (!response.ok) {
                throw new Error("HTTP " + response.status);
            }

            return response.text();
        })
        .then(html => {
            switch (target) {
                case "setting":
                    document.getElementById("tableSettingDiv").innerHTML = html;
                    loadPartial(`/Home/LoadTableData?id=${id}`, "data");
                    if (window.TableSettings?.init)
                        window.TableSettings.init();//serve per far partire il js 
                    break;
                case "data":
                    document.getElementById("tableDataDiv").innerHTML = html;
                    break;
                default:
                    loadPartial(url, "setting");
                    break;
            }
        })
        .catch(err => console.error(err));
}

// Carica il tab di default quando la pagina è pronta
document.addEventListener("DOMContentLoaded", function () {
    loadPartial(`/Home/LoadTableSettings?id=${id}`,"setting");
});
/*
// Eventi click dei tab
document.getElementById("settings-tab").addEventListener("click", function () {
    loadPartial(`/Home/LoadTableSettings?id=${id}`,"setting");
});

document.getElementById("data-tab").addEventListener("click", function () {
    loadPartial(`/Home/LoadTableData?id=${id}`,"data");//sistemare url 
});

                    /*document.getElementById("advanced-tab").addEventListener("click", function () {
    loadPartial(`/MlInterface/LoadAdvanced?id=@Model.Id`);
                    });*/
