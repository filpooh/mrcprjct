const id = document.getElementById("pageData").dataset.id;
document.addEventListener("DOMContentLoaded", () => {
    //non carica in maniera corretta
    // Avvia il JS del tab attivo al caricamento
//    if (window.ModelSettings?.init)
    window.ModelSettingPartial.init();
    const d = document.getElementById("settings");
    // Quando clicchi il tab Settings
    document.getElementById("settings-tab").addEventListener("click", () => {
        if (window.ModelSettings?.init) window.ModelSettingPartial.init();
    });

    // Quando clicchi il tab Data
    document.getElementById("data-tab").addEventListener("click", () => {
        if (window.ModelData?.init) window.ModelData.init();
    });

    // Quando clicchi il tab Stat
    document.getElementById("stat-tab").addEventListener("click", () => {
        if (window.ModelStat?.init) window.ModelStat.init();
    });
});
