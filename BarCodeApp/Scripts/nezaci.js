var nezaci = {
    layout: {},
    page: {
        handlers: {},
        startUp: null
    },
    services: {},
    ui: {
        notifications: {},
        startUp: null
    }
};


nezaci.layout.startUp = function () {

    //this does a null check on nezaci.page.startUp
    if (nezaci.page.startUp) {
        console.log("nezaci.page.startup");
        nezaci.page.startUp();
    }
};
$(document).ready(nezaci.layout.startUp);