<script setup>
import axios from 'axios';
import { ref, computed, onMounted } from 'vue'
import { mdiFacebook } from '@mdi/js';
import { useRouter } from 'vue-router';
import { useScriptTag } from '@vueuse/core';
import { ElDivider } from 'element-plus';

useScriptTag("https://accounts.google.com/gsi/client", () => { }, { async: true, defer: true });

axios.defaults.withCredentials = true

const router = useRouter();

function handleCredentialResponse(response) {
    console.log("Encoded JWT ID token: " + response.credential);
}
window.onload = function () {
    google.accounts.id.initialize({
        client_id: "543452473505-arufhkof9smt0oomms8htbnlf4i82u0i.apps.googleusercontent.com",
        callback: handleCredentialResponse
    });

    google.accounts.id.prompt();
}

function loginFB() {
    FB.login(function (res) {
        if (res.status === 'connected') {
            FB.api('/me?fields=id,name,email', function (meResponse) {
                console.log(meResponse.email);
            })
        } else {

        }
    }, { scope: 'public_profile,email' })
}

function logout() {
    FB.logout(function (res) {

    })
}

// onMounted(() => {
//     FB.getLoginStatus(function (res) {
//         console.log(res)
//         statusChangeCallback(res)
//     })
// })


</script>

<template>
    <v-sheet width="300" class="mx-auto">
        <el-divider content-position="center" class="my-10">
            <p class="text-center">使用第三方認證登入</p>
        </el-divider>
        <div id="g_id_onload" data-client_id="543452473505-arufhkof9smt0oomms8htbnlf4i82u0i.apps.googleusercontent.com"
            data-context="signin" data-ux_mode="popup" data-login_uri="https://localhost:5002/api/Account/google"
            data-auto_prompt="false">
        </div>
        <v-row class="my-6 d-flex justify-center">
            <v-col cols="auto" class="pt-3">
                <div class="g_id_signin" data-type="icon" data-shape="pill" data-theme="outline" data-text="signin_with"
                    data-size="large" data-logo_alignment="left" data-callback="handleCredentialResponse">
                </div>
            </v-col>
            <v-col cols="auto">
                <v-btn :icon="mdiFacebook" @click="loginFB" density="compact" size="x-large" variant="outlined"
                    color="#006be4" class="facebook-login-btn"></v-btn>
            </v-col>
        </v-row>
    </v-sheet>
</template>

<style scoped>
.facebook-login-btn {
    border-color: lightgray;
    transform: scale(1.3);
}

.g_id_signin {
    transform: scale(1.45);
    margin-top: 2px;
}
</style>
    
