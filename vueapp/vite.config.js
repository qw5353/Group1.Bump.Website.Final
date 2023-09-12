import { fileURLToPath, URL } from "node:url";
import { defineConfig } from "vite";
import mkcert from "vite-plugin-mkcert";
import vue from "@vitejs/plugin-vue";
import vuetify from "vite-plugin-vuetify";
import ElementPlus from "unplugin-element-plus/vite";

// https://vitejs.dev/config/
export default defineConfig({
    plugins: [vue(), mkcert(), vuetify(), ElementPlus()],
    resolve: {
        alias: {
            "@": fileURLToPath(new URL("./src", import.meta.url)),
        },
    },
    server: {
        proxy: {
            "/api": {
                target: "https://localhost:7139/",
                secure: false,
                changeOrigin: true,
                rewrite: (path) => path.replace(/^\/api/, ""),
            },
            "/files": {
                target: "https://localhost:7139/",
                secure: false,
                changeOrigin: true,
            },
            "/chat/negotiate": {
                target: "https://localhost:7139/",
                secure: false,
                changeOrigin: true,
            },
            "/chat": {
                target: "wss://localhost:7139/",
                secure: false,
                changeOrigin: true,
                ws: true,
            },
        },
        port: 5002,
    },
    preview: {
        port: 5002
    },
});
