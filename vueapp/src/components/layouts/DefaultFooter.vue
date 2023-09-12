<script setup>
import footerLogo from '../../assets/footer-logo.png';
import { mdiYoutube, mdiFacebook, mdiInstagram, mdiTwitter, mdiCopyright } from '@mdi/js';
import { ref } from 'vue';

const links = ref([
    {
        title: "服務條款",
        href: "/terms"
    },
    {
        title: "隱私權政策",
        href: "/privacy"
    },
    {
        title: "聯繫我們",
        href: "/contact-us"
    }
])
</script>

<template>
    <footer class="default-footer-text-spacing">
        <v-container fluid class="px-0 bg-primary-orange text-white">
            <v-row class="px-16 mx-auto d-md-flex d-none align-center">
                <v-col class="justify-center" cols="3">
                    <v-row><img :src="footerLogo" alt="" class="footer-logo mx-auto"></v-row>
                    <v-row class="px-4 justify-center d-flex social-media-icon-row">
                        <v-icon :icon="mdiYoutube" color="white" size="x-large"></v-icon>
                        <v-icon :icon="mdiInstagram" color="white" size="x-large"></v-icon>
                        <v-icon :icon="mdiTwitter" color="white" size="x-large"></v-icon>
                        <v-icon :icon="mdiFacebook" color="white" size="x-large"></v-icon>
                    </v-row>
                    <v-row class="mt-8">
                        <small class="mb-4 mx-auto"><v-icon color="white" :icon="mdiCopyright" /> 2020 - {{ new
                            Date().getFullYear() }}. Bump Inc. All rights reserved.</small>
                    </v-row>
                </v-col>
                <v-spacer></v-spacer>
                <v-col class="d-flex align-center" cols="4">
                    <v-row class="mb-6 ma-0">
                        <dl>
                            <dt>
                                <h2>對於課程, 商品有疑問?</h2>
                            </dt>
                            <dd class="mt-3">
                                歡迎來電, 使用 Email, 或私訊粉絲專頁洽詢.
                            </dd>
                            <dd class="mt-3">
                                服務時間 | 周一至周日, <time>12:00 ~ 21:00</time>, 全年無休.
                            </dd>
                            <dd class="mt-3">
                                連絡電話 | (02)2929-1234
                            </dd>
                            <dd class="mt-3">
                                電子信箱 | service@bump.com
                            </dd>
                        </dl>
                    </v-row>
                </v-col>
                <v-col class="d-flex align-center ma-0">
                    <v-list bg-color="transparent" density="compact">
                        <v-list-item v-for="item in links" :key="item.href" :href="item.href">
                            <template #title>
                                <span class="text-white text-h6" style="letter-spacing: 0.125rem !important;">{{ item.title
                                }}</span>
                            </template>
                        </v-list-item>
                    </v-list>
                </v-col>
                <v-spacer></v-spacer>
            </v-row>
            <v-row class="px-2 mx-auto d-sm-flex d-md-none">
                <v-col class="justify-center" cols="12">
                    <v-row><img :src="footerLogo" alt="" class="footer-logo mx-auto"></v-row>
                    <v-row class="px-4 justify-center d-flex social-media-icon-row">
                        <v-icon :icon="mdiYoutube" color="white" size="x-large"></v-icon>
                        <v-icon :icon="mdiInstagram" color="white" size="x-large"></v-icon>
                        <v-icon :icon="mdiTwitter" color="white" size="x-large"></v-icon>
                        <v-icon :icon="mdiFacebook" color="white" size="x-large"></v-icon>
                    </v-row>
                    <v-row class="mt-8">
                        <small class="mb-4 mx-auto"><v-icon color="white" :icon="mdiCopyright" /> 2020 - {{ new
                            Date().getFullYear() }}. Bump Inc. All rights reserved.</small>
                    </v-row>
                </v-col>
                <v-col class="d-flex align-center" cols="12">
                    <v-row class="mb-6 ma-0 justify-center">
                        <dl>
                            <dt>
                                <h2>對於課程, 商品有疑問?</h2>
                            </dt>
                            <dd class="mt-3">
                                歡迎來電, 使用 Email, 或私訊粉絲專頁洽詢.
                            </dd>
                            <dd class="mt-3">
                                服務時間 | 周一至周日, <time>12:00 ~ 21:00</time>, 全年無休.
                            </dd>
                            <dd class="mt-3">
                                連絡電話 | (02)2929-1234
                            </dd>
                            <dd class="mt-3">
                                電子信箱 | service@bump.com
                            </dd>
                        </dl>
                    </v-row>
                </v-col>
                <v-col class="d-flex align-center ma-0 justify-center" cols="12">
                    <span v-for="(item, index) in links" :key="item.href">
                        <a :href="item.href">{{ item.title }}</a>{{ index !== links.length - 1 ? "&nbsp;|&nbsp;" : "" }}
                    </span>
                </v-col>
            </v-row>
        </v-container>
    </footer>
</template>


<style scoped>
.bg-primary-orange {
    background: linear-gradient(90deg, #E6A947 40%, #D68924);
}

.footer-logo {
    width: 80%;
    object-fit: cover;
}

.default-footer-text-spacing {
    box-shadow: 0px -10px 10px -4px rgba(0, 0, 0, 0.12);
    -webkit-box-shadow: 0px -10px 10px -4px rgba(0, 0, 0, 0.12);
    -moz-box-shadow: 0px -10px 10px -4px rgba(0, 0, 0, 0.12);
    letter-spacing: 0.0825rem;
}

.social-media-icon-row i {
    margin: 0 16px 0 16px;
}
</style>