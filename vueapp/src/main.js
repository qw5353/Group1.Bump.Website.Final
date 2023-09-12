import '@fortawesome/fontawesome-free/css/all.css'
import { createApp } from "vue";
import { createPinia } from "pinia";
import "vuetify/styles";
import { createVuetify } from "vuetify";
import { aliases, mdi } from "vuetify/iconsets/mdi-svg";
import { fa } from 'vuetify/iconsets/fa'
import '@fortawesome/fontawesome-free/css/all.css'

import App from "./App.vue";
import router from "./router";


const app = createApp(App);

app.use(
    createVuetify({
        icons: {
            defaultSet: "mdi",
            aliases,
            sets: {
                mdi,
                fa
            },
        },
    })
);
app.use(createPinia());
app.use(router);

app.mount("#app");
