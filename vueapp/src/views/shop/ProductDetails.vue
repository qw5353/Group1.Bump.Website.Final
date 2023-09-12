<script setup>
import { useTitle } from '@vueuse/core';
import DefaultLayout from '../../components/layouts/DefaultLayout.vue';
import ShopNavbar from '../../components/shop/layouts/ShopNavbar.vue';
import ProductCardGroup from '../../components/shop/product/ProductCardGroup.vue';
import ScrollToTop from '../../components/shop/layouts/ScrollToTop.vue';
import Breadcrumbs from '../../components/shop/layouts/Breadcrumbs.vue';
import ProductCardSoldOutBadge from '../../components/shop/product/ProductCardSoldOutBadge.vue';
import ProductCardNewBadge from '../../components/shop/product/ProductCardNewBadge.vue';
import DefaultMemberIcon from '@/assets/images/shop/defaultMemberImg.jpg';
import { ref, watch, onMounted, computed } from 'vue';
import { useRoute } from 'vue-router';
import { mdiCartOutline } from '@mdi/js';
import { ElInputNumber } from 'element-plus';
import { ElSelect, ElOption } from 'element-plus';
import { ElMessage } from 'element-plus'
import { ElSkeletonItem, ElSkeleton } from 'element-plus';
import 'element-plus/es/components/skeleton-item/style/css'
import 'element-plus/es/components/skeleton/style/css'
import 'element-plus/es/components/input-number/style/css'
import axios from 'axios';
import ProductDiscountsCard from '../../components/shop/product/ProductDiscountsCard.vue'
import { userStateStore } from '../../stores/UserStateStore';
import ProductCardDiscountsHeader from '../../components/shop/product/ProductCardDiscountsHeader.vue'
import { useCartStore } from '../../stores/CartStore';

useTitle('Bump - 商品詳情');


const userState = userStateStore();
const cartStore = useCartStore();

const route = useRoute();

const breadcrumbs = ref([{
    title: '商品列表',
    disabled: false,
    href: '/productlist',
}]);

const productsDetails = ref({});
const productImage = ref();  //商品主圖
const productThumbnails = ref([]); //商品縮圖

const productStyleOptions = ref([]);
const selectedStyle = ref();
const quantity = ref(null);
const selectedSlide = ref(0);
const errorMsg = ref('');

const tab = ref(1);
const titles = ref(["商品介紹", "評論&評價", "品牌介紹"])
const productComments = ref([]);
const productCommentAVG = ref('');
const selectedCommentOrder = ref();
const commentsOrderItem = ref([{ key: 1, text: '日期由新到舊' }, { key: 2, text: '日期由舊到新' }, { key: 3, text: '評價由高到低' }, { key: 4, text: '評價由低到高' }])

const sellsWellProductGroup = ref([
    {
        name: "", brandName: "", price: "", thumbnail: "", totalPage: ""
    },
])

onMounted(async () => {
    const productId = route.params.id;
    try {
        const getProductDetails = await axios.get(`/api/Shop/ProductDetails?id=${productId}`);
        productsDetails.value = getProductDetails.data;
        productsDetails.value.description = productsDetails.value.description.replace(/src="\/Uploads\/product\/images\//g, 'src="/files/images/products/');

        const getproductThumbnails = await axios.get(`/api/Shop/ProductImages?id=${productId}`);
        productThumbnails.value = getproductThumbnails.data.images;
        productThumbnails.value.unshift(productsDetails.value.productThumbnail);
        productImage.value = productThumbnails.value[0]

        const getSellsWellProductGroup = await axios.get('/api/shop/SellsWellProductGroup');
        sellsWellProductGroup.value = getSellsWellProductGroup.data;

        const getWhichProductCategory = await axios.get(`/api/Shop/WhichProductCategory?id=${productId}`)
        breadcrumbs.value = [
            {
                title: '商品列表',
                disabled: false,
                href: '/productlist',
            },
            {
                title: getWhichProductCategory.data.firstCategory,
                disabled: false,
                href: '/productlist?navbarFilter=' + getWhichProductCategory.data.firstCategory,
            },
            {
                title: getWhichProductCategory.data.secondCategory,
                disabled: false,
                href: '/productlist?navbarFilter=' + getWhichProductCategory.data.firstCategory + '&mid=' + getWhichProductCategory.data.secondCategory,
            },
            {
                title: getWhichProductCategory.data.thirdCategory,
                disabled: false,
                href: '/productlist?navbarFilter=' + getWhichProductCategory.data.firstCategory + '&mid=' + getWhichProductCategory.data.secondCategory + '&sub=' + getWhichProductCategory.data.thirdCategory
            },
            {
                title: productsDetails.value.name,
                disabled: true,
                href: ''
            }
        ];
    } catch (err) {
        console.error(err);
    }

});
const slideGroupStyle = computed(() => {
    //如果圖片<5張 不須留左右icon空間、>=5張需要留左右icon =>目的都是為了與上圖對齊置中;
    return productThumbnails.value.length < 5 ? 'width: 385px;' : 'width:475px';
});

async function getProductStyles() {
    const productId = route.params.id;
    try {
        const getProductStyles = await axios.get(`/api/Shop/ProductStyles?id=${productId}`);
        productStyleOptions.value = getProductStyles.data;
    } catch (err) {
        console.error(err);
    }
}
const remainingQuantity = computed(() => {
    if (selectedStyle.value) {
        return productStyleOptions.value.find(item => item.id === selectedStyle.value)?.inventory || 0;
    }
    return 0;
});

const noStock = computed(() => {
    return remainingQuantity.value === 0;
})

watch(selectedStyle, () => {
    quantity.value = noStock.value ? 0 : 1;
})

async function clickThumbnail(image, toggle) {
    toggle();
    productImage.value = image;
};

async function handleStyleChange() {
    if (remainingQuantity.value == 0) {
        errorMsg.value = "此商品已無庫存!";
    }
    else {
        errorMsg.value = "";
    }
}

const totalDiscount = ref(0)
const getDataFromComponent = (data) => {
    totalDiscount.value = data
};

async function clickAddCart() {
    if (userState.authenticate) {
        const data =
        {

            memberId: userState.userId,
            ProductStyleId: selectedStyle.value,
            AddProductStyleQuantity: quantity.value
        }
        try {
            await axios.post(`/api/Shop/AddCart`, data);
            ElMessage({
                showClose: true,
                message: '已成功加入購物車!',
                type: 'success',
            })
        } catch (err) {
            ElMessage({
                showClose: true,
                message: err.response.data,
                type: 'error',
            })
            getProductStyles();
        }
    } else {
        ElMessage({
            showClose: true,
            message: '請先登入，再加入購物車!',
            type: 'error',
        })
    }
    try {
        await cartStore.updateCartItems();
    } catch (err) {
        console.error(err);
    }
}
async function clickComments(index, orderKey) {
    if (index == 1) {
        try {
            const getProductComments = await axios.get(`/api/Shop/ProductComments?id=${route.params.id}&orderKey=${orderKey}`);
            const getProductCommentAVG = await axios.get(`/api/Shop/ProductCommentAvg?id=${route.params.id}`);
            productComments.value = getProductComments.data;
            productComments.value.map(at => {
                at.createAt = convertDateFormat(at.createAt);
                at.account = maskString(at.account);
                return at
            });
            productCommentAVG.value = getProductCommentAVG.data;
        } catch (err) {
            console.error(err);
        }
    }
}
function handleCommentOrderChange(OrderKey) {
    clickComments(1, OrderKey);
}
function convertDateFormat(originalDateString) {
    const originalDate = new Date(originalDateString);
    const year = originalDate.getFullYear();
    const month = originalDate.getMonth() + 1;
    const day = originalDate.getDate();
    const formattedMonth = month < 10 ? `0${month}` : month;
    const formattedDay = day < 10 ? `0${day}` : day;
    return `${year}/${formattedMonth}/${formattedDay}`;
}
const maskString = (str) => {
    let firstTwoChars = '';
    let lastTwoChars = '';
    let maskedStr = '';
    if (str.length == 2) {
        firstTwoChars = str.substring(0, 1);
        maskedStr = `${firstTwoChars}****`;
    } else if (str.length <= 5) {
        firstTwoChars = str.substring(0, 1);
        lastTwoChars = str.substring(str.length - 1);
        maskedStr = `${firstTwoChars}****${lastTwoChars}`;
    } else if (str.length > 5) {
        firstTwoChars = str.substring(0, 2);
        lastTwoChars = str.substring(str.length - 2);
        maskedStr = `${firstTwoChars}****${lastTwoChars}`;
    }
    return maskedStr;
};

</script>

<template>
    <DefaultLayout>
        <ShopNavbar></ShopNavbar>
        <main>
            <v-container class="mw-1350px">
                <v-row no-gutters class="d-flex justify-center">
                    <div style="width:1170px">
                        <Breadcrumbs :items="breadcrumbs" />
                    </div>
                </v-row>
                <v-row class="py-15 ma-0 pa-0" no-gutters>
                    <v-col cols="12" :md="5" style="height:500px; position: relative;">
                        <v-row class="h-75 w-100 ma-0 d-flex justify-center">
                            <v-img class="w-100" :src="`/files/images/products/${productImage}`" v-if="productImage">
                                <ProductCardNewBadge :id="productsDetails.id" class="ms-13 d-flex justify-end w-75">
                                </ProductCardNewBadge>
                                <template #placeholder>
                                    <el-skeleton style="width: 65%; height: 90%; margin: 0 auto 0 auto;" animated>
                                        <template #template>
                                            <el-skeleton-item variant="image" style="width: 100%; height: 100%" />
                                        </template>
                                    </el-skeleton>
                                </template>
                            </v-img>
                            <ProductCardSoldOutBadge v-if="productsDetails.id > 0" :id="productsDetails.id"
                                style="width: 80%; height: 8%; font-size: larger;" />
                        </v-row>
                        <v-row class="h-25 w-100 ma-0">
                            <v-slide-group class="pa-0  mx-auto" :style="slideGroupStyle" selected-class="bg-primary"
                                show-arrows mandatory v-model="selectedSlide">
                                <template #prev>
                                    <v-icon icon="fa:fa-solid fa-caret-left fa-lg" color="#FFB23E"></v-icon>
                                </template>
                                <template #next>
                                    <v-icon icon="fa:fa-solid fa-caret-right fa-lg" color="#FFB23E"></v-icon>
                                </template>
                                <v-slide-group-item v-for=" item  in  productThumbnails " :key="item"
                                    v-slot="{ isSelected, toggle }">
                                    <v-img :src="`/files/images/products/${item}`" class="ma-2 d-flex justify-start" :style="{
                                        height: '80px',
                                        width: '80px',
                                        //如果圖片=5張 不能用max-width(會變成顯示5張)
                                        ...(productThumbnails.length < 5 && { maxWidth: '80px' }),
                                        ...(isSelected && {
                                            border: '1px solid #FFB23E',
                                            borderRadius: '5px'
                                        })
                                    }
                                        " @click="!isSelected ? clickThumbnail(item, toggle) : undefined">
                                        <template #placeholder>
                                            <el-skeleton style="width: 100%; height: 100%; margin: 0 auto 0 auto;" animated>
                                                <template #template>
                                                    <el-skeleton-item variant="image" style="width: 100%; height: 100%" />
                                                </template>
                                            </el-skeleton>
                                        </template>
                                    </v-img>

                                </v-slide-group-item>
                            </v-slide-group>
                        </v-row>
                    </v-col>
                    <v-col cols="12" :md="7" class="px-2">
                        <v-row class="mb-1">
                            <v-col cols="12">
                                <ProductCardDiscountsHeader :id="route.params.id" :price="productsDetails.price">
                                </ProductCardDiscountsHeader>
                            </v-col>
                        </v-row>
                        <v-row style="max-height: 70px" class="align-center ma-0 mb-6">
                            <v-img v-if="productsDetails.brandThumbnail" max-height="100%" max-width="80" cover
                                :src="`/files/images/brands/${productsDetails.brandThumbnail}`"></v-img>
                        </v-row>
                        <v-row class="ma-0 flex-column">
                            <h2 style="letter-spacing: 1.5px;">{{ productsDetails.name }}</h2>
                            <h3 class="mt-3 font-weight-regular" style="color: rgba(0,0,0,0.6); font-size:medium;">
                                Product
                                Code: {{
                                    productsDetails.code }}</h3>
                        </v-row>
                        <v-row class="ma-0 mt-7 align-end">
                            <h2 v-if="totalDiscount === 0" style="color: #656565;" class="price_style">${{
                                productsDetails.price }}
                            </h2>
                            <div v-else class="d-flex align-center">
                                <h3 style="color: #a8a8a8;" class="price_style text-decoration-line-through mr-3">${{
                                    productsDetails.price }}</h3>
                                <h2 style="color: #D41010;" class="price_style">${{
                                    productsDetails.price - totalDiscount }}</h2>
                            </div>
                        </v-row>
                        <v-divider class="my-5"></v-divider>
                        <v-row class="ma-0 px-2" v-html="productsDetails.shortDescription"
                            style="font-Size: 1em; letter-spacing:0.8px; line-height: 25px">

                        </v-row>
                        <v-divider class="my-4"></v-divider>
                        <v-row class="ma-0 ms-0" no-gutters>
                            <v-col cols="12" class="d-flex align-center">
                                <el-select v-model="selectedStyle" class="m-2" placeholder="請選擇規格" size="large"
                                    @click="getProductStyles" @change="handleStyleChange">
                                    <el-option v-for="  item  in  productStyleOptions  " :key="item.id" :label="item.style"
                                        :value="item.id" />
                                </el-select>
                                <el-input-number v-model="quantity" size="large" class="ms-10 me-2"
                                    :min="remainingQuantity === 0 ? 0 : 1"
                                    :max="remainingQuantity === 0 ? 1 : remainingQuantity"
                                    :disabled="!selectedStyle || noStock" />
                                <div class="text-medium-emphasis text-subtitle-1 d-flex align-center">尚有:{{
                                    remainingQuantity }}件</div>
                            </v-col>
                            <v-col class="d-flex ms-10">
                                <div style="width: 256px;"></div>
                                <div class="me-14 mt-2" style="color:red">
                                    {{ errorMsg }}
                                </div>
                            </v-col>
                        </v-row>
                        <v-row class="mt-5 d-flex align-center">

                            <v-btn variant="outlined" class="addcart-style ms-3 me-5" :disabled="!selectedStyle || noStock"
                                @click="clickAddCart">
                                <v-icon class="ms-1" color="light">{{ mdiCartOutline }}</v-icon>
                                <span class="ms-1 font-weight-regular">加入購物車</span>
                            </v-btn>
                            <ProductDiscountsCard @send-Discount="getDataFromComponent"></ProductDiscountsCard>

                        </v-row>
                    </v-col>
                </v-row>
                <v-row class="d-flex justify-center" no-gutters>
                    <div style="width: 88%;">
                        <v-divider></v-divider>
                    </div>
                </v-row>
                <v-row class="mt-10 px-10 justify-center">
                    <v-card width="90%" class="border">
                        <v-tabs v-model="tab" color="orange-darken-2" grow align-tabs="center" bg-color="orange-lighten-5">
                            <v-tab v-for="( title, index ) in  titles " :key="index + 1" :value="index + 1"
                                @click="clickComments(index, 1)" class="w-15 h-auto text-h6 font-weight-bold spacing">
                                {{ title }}
                            </v-tab>
                        </v-tabs>
                        <v-window v-model="tab" class="pa-5">
                            <v-window-item :value="1" :key="1">
                                <div class="ma-0 px-2" v-html="productsDetails.description"
                                    style="font-Size: 1em; letter-spacing:0.6px; line-height: 30px">

                                </div>
                            </v-window-item>
                            <v-window-item :value="2" :key="2" class="overflow-auto px-5" style="max-height:700px">
                                <div class="d-flex justify-center">
                                    <div class=" d-flex align-center flex-column w-50">
                                        <div class="text-h2 mt-5">
                                            {{ productCommentAVG.avgRating ?? 0 }}
                                            <span class="text-h6 ml-n3">/5</span>
                                        </div>
                                        <v-rating :model-value="productCommentAVG.avgRating" color="yellow-darken-3"
                                            half-increments readonly></v-rating>
                                        <div>{{ productCommentAVG.commentCount }} ratings</div>
                                    </div>
                                    <div class="d-flex align-end">
                                        <div class="me-5" style="position: absolute; right:0;">
                                            <el-select v-model="selectedCommentOrder" class="m-2" placeholder="請選擇排序"
                                                size="small" @change="handleCommentOrderChange(selectedCommentOrder)">
                                                <el-option v-for="  order  in  commentsOrderItem  " :key="order.key"
                                                    :label="order.text" :value="order.key" />
                                            </el-select>
                                        </div>
                                    </div>
                                </div>
                                <v-card v-for=" productComment  in  productComments " class="my-3" v-if="productComments">
                                    <v-card-item>
                                        <div class="d-flex">
                                            <div>
                                                <v-img :width="45" :height="45" :src="productComment.memberThumbnail"
                                                    aspect-ratio="1/1" class="border" style="border-radius: 50%;"><template
                                                        v-slot:error>
                                                        <v-img :width="45" :height="45" spect-ratio="1/1"
                                                            :src="DefaultMemberIcon"></v-img>
                                                    </template></v-img>
                                            </div>
                                            <div class="ms-1">
                                                <h6 class="ms-2">{{ productComment.account }}</h6>
                                                <v-rating v-model="productComment.rating" density="compact"
                                                    color="yellow-darken-3" readonly></v-rating>
                                            </div>
                                        </div>
                                        <div class="text-subtitle-2 text-medium-emphasis ms-1" style="margin-top:-3px;">{{
                                            productComment.createAt }} | 規格:{{ productComment.productStyle }}
                                        </div>
                                    </v-card-item>
                                    <h4 class="ms-5 my-3">{{ productComment.description }}</h4>
                                </v-card>
                                <h2 v-if="productComments.length == 0" style="text-align:center;" class="my-15">
                                    尚無評價..
                                </h2>
                            </v-window-item>
                            <v-window-item :value="3" :key="3">
                                <div class="ma-0 px-2" v-html="productsDetails.brandDescription"
                                    v-if="productsDetails.brandDescription"
                                    style="font-Size: 1em; letter-spacing:0.6px; line-height: 30px">
                                </div>
                                <h2 v-else style="text-align:center;" class="my-15">
                                    尚無品牌資訊 🙇
                                </h2>
                            </v-window-item>
                        </v-window>
                        <!-- {{ tab }} -->
                    </v-card>
                </v-row>
            </v-container>
            <ScrollToTop />
            <ProductCardGroup title=" 熱門商品 " :products="sellsWellProductGroup"
                link="https://www.youtube.com/shorts/FwzG_Ux8vRE" class="my-16">
                <template #icon-element-append>
                    <i class="fa-solid fa-exclamation fa-beat fa-sm" style="color: black;"></i>
                </template>
            </ProductCardGroup>
        </main>
    </DefaultLayout>
</template>
    
    
<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Inria+Sans:wght@300&display=swap');
@import url('https://fonts.googleapis.com/css2?family=Inria+Sans:wght@300&display=swap');

@media (max-width: 9999px) {
    .mw-1350px {
        max-width: 1350px;
    }
}

.price_style {
    display: flex;
    justify-content: center;
    font-family: 'Inria Sans', sans-serif !important;
    /* font-family: fantasy !important; */
    font-weight: 600;
    text-align: center;
    letter-spacing: 1px;
}

.w-40 {
    width: 40%;
}

.addcart-style {
    border-radius: 40px;
    background: #FFB23E;
    border: 0px;
    height: 45px;
    width: 170px;
    mix-blend-mode: normal;
    color: #FFFF;
    font-size: large;
    font-weight: bold;
}
</style>