<script setup>
import { mdiMagnify } from '@mdi/js';
import { mdiDotsVertical } from '@mdi/js';
import { useRouter } from 'vue-router';
import { ref } from 'vue';
import axios from 'axios';

const links = ref([
    {
        to: "/BrandsIndex",
        text: "品牌介紹"
    },
    {
        to: "/productlist?navbarFilter=男裝",
        text: "男裝"
    },
    {
        to: "/productlist?navbarFilter=女裝",
        text: "女裝"
    },
    {
        to: "/productlist?navbarFilter=登山",
        text: "登山"
    },
    {
        to: "/productlist?navbarFilter=攀登",
        text: "攀登"
    },
    {
        to: "/Shop/OnSale",
        text: "優惠商品"
    },
]);
const keyword = ref("")
const router = useRouter();
const isDropdownOpen = ref('');
const dropDownContent = ref('');
const selectCategory = ref('');
const selectLink = ref('');
const inputHandler = () => {
    router.push({ path: '/productlist', query: { keyword: keyword.value } });
}


const openCategoryDropdown = async (link) => {
    selectLink.value = link
    try {
        let getProductCategories = "";
        switch (link.text) {
            case '優惠商品':
                getProductCategories = await axios.get(`/api/Shop/PromotionProductTags`);
                break;
            case '品牌':
            default:
                getProductCategories = await axios.get(`/api/Shop/ProductCategories?firstCategory=${link.text}`);
                break;
        }
        dropDownContent.value = getProductCategories.data;
    } catch (err) {
        console.error(err);
        dropDownContent.value = '';
        selectCategory.value = '';
    }
    isDropdownOpen.value = link.to;
    selectCategory.value = link.text;
};

const closeDropdown = () => {
    isDropdownOpen.value = '';
    selectCategory.value = '';
};
const openListDropdown = async () => {
    isDropdownOpen.value = selectLink.value.to;
    selectCategory.value = selectLink.value.text;
};

</script>



<template>
    <v-card class="navbar-container">
        <v-container>
            <v-row>
                <v-col cols="9" class="tabs-style d-lg-flex d-none ps-16">
                    <v-tab v-for="link in links" :href="link.to" :key="link.to" class="tab-style"
                        @mouseenter="openCategoryDropdown(link)" @mouseleave="closeDropdown()">
                        {{ link.text }}
                    </v-tab>
                </v-col>

                <v-col cols="2" class="pl-0 d-md-flex d-lg-none">
                    <v-menu>
                        <template v-slot:activator="{ props }">
                            <v-btn class="md-menu-style" :icon="mdiDotsVertical" color="#F0E2BD" v-bind="props"></v-btn>
                        </template>

                        <v-list bg-color="#F0E2BD" color="black">
                            <v-list-item v-for="link in links" :href="link.to" :key="link.to">
                                <v-list-item-title class="list-item-title">{{ link.text }}</v-list-item-title>
                            </v-list-item>
                        </v-list>
                    </v-menu>
                </v-col>

                <v-col cols="10" xl="3" lg="3" sm="8" xs="10" class="search-style">
                    <v-text-field v-model="keyword" density="compact" variant="solo" label="Search"
                        :append-inner-icon="mdiMagnify" single-line hide-details @keyup.enter="inputHandler"
                        @click:append-inner="inputHandler"></v-text-field>
                </v-col>
            </v-row>
        </v-container>
    </v-card>
    <transition name="dropdown-fade">
        <!-- 品牌寫這v-if="isDropdownOpen.startsWith('/brand')" -->
        <div v-if="isDropdownOpen.startsWith('/productlist?navbarFilter')" class="d-flex justify-center"
            style="position: fixed;top: 113px; width: 100%; pointer-events: none; z-index: 999;"
            @mouseenter="openListDropdown()" @mouseleave="closeDropdown()">
            <v-container
                style="width: 70%; height: 100%; background-color: rgba(255, 255, 255, 0.96); pointer-events: auto; box-shadow: 0px 14px 54px -21px rgba(0,0,0,0.29);"
                class="ma-0">
                <v-row style="width: 95%; margin-left: auto; margin-right:auto;">
                    <v-col cols="2" v-for="productCategory in dropDownContent" style="color:black">
                        <ul class="text-subtitle-1">
                            <a
                                :href="`/productlist?navbarFilter=${selectCategory}&mid=${productCategory.secondCategories}`">{{
                                    productCategory.secondCategories }}</a>
                            <li v-for="thirdCategory in productCategory.thirdCategories "
                                style="list-style:none; color:#706f6f" class="text-subtitle-2 my-2">
                                <a
                                    :href="`/productlist?navbarFilter=${selectCategory}&mid=${productCategory.secondCategories}&sub=${thirdCategory}`">{{
                                        thirdCategory }}</a>
                            </li>
                        </ul>
                    </v-col>
                </v-row>
            </v-container>
        </div>
        <div v-else-if="selectCategory === '優惠商品'" class="d-flex justify-center"
            style="position: fixed;top: 113px; right:30.2%; pointer-events: none; z-index: 999;"
            @mouseenter="openListDropdown()" @mouseleave="closeDropdown()">
            <v-container
                style="width: 70%; height: 100%; background-color: rgba(255, 255, 255, 0.96); pointer-events: auto; box-shadow: 0px 14px 54px -21px rgba(0,0,0,0.29);"
                class="ma-0">
                <v-row style="width: 100%; margin-left: auto; margin-right:auto;" class="d-flex justify-center flex-column">
                    <ul>
                        <li v-for="productCategory in dropDownContent" style=" width: 250px; list-style:none; color:black"
                            class="my-5 text-subtitle-2"> <a :href="`/Shop/OnSale?actName=${productCategory.tagName}`">{{
                                productCategory.tagName }}</a></li>
                    </ul>
                </v-row>
            </v-container>
        </div>

    </transition>
</template>

<style scoped>
ul a {
    text-decoration: none;
    color: inherit;
}

ul a:hover {
    color: #E6A947;
}

.tab-styles a:first-child {
    margin-left: 100px;
}

.navbar-container {
    height: 47px;
    background: #F0E2BD;
    width: 100%;
    position: sticky;
    top: 66px;
    z-index: 999;
    display: flex;
    align-items: center;
    justify-content: space-between;
}

header.v-toolbar {
    background: transparent;
}

.tabs-style {
    display: flex;
    margin-left: -5%;
}

.tab-style {
    height: 47px;
    flex: 1;
    text-align: center;
    border-radius: 0px;
    font-size: large;
    font-weight: bold;
    font-family: monospace !important;
    color: #474747;
    letter-spacing: 0.3em !important;
}

.tab-style:hover {
    border-bottom: 3px solid #E6A947 !important;
    color: black;
}

.search-style {
    display: flex;
    align-items: center;
    margin-left: auto;
    width: 100px;
}

.md-menu-style {
    box-shadow: 0 0 0;
}

.list-item-title {
    letter-spacing: 0.25rem;
    text-align: center;
    color: black;
}

.dropdown-fade-enter-active,
.dropdown-fade-leave-active {
    transition: opacity 0.5s, transform 0.5s;
}

.dropdown-fade-enter,
.dropdown-fade-leave-to {
    opacity: 0;
    transform: translateY(3px);
}

.dropdown-container {
    transform-origin: top center;
}
</style>
