<script setup>
import { ref, onMounted, computed, watch } from 'vue';
import axios from 'axios';
import _, { assign } from 'lodash';
import { useCheckOutStore } from '../../stores/CheckOutStore';
import { useRouter } from 'vue-router';
import { ElMessage } from 'element-plus'
import { mdiCloseCircle } from '@mdi/js';
import { mdiCheckCircle } from '@mdi/js';
import { userStateStore } from '../../stores/UserStateStore.js';
import { useCartStore } from '../../stores/CartStore';
import { storeToRefs } from 'pinia';
import CartEmpty from './CartEmpty.vue';

const cartStore = useCartStore();
const { items } = storeToRefs(cartStore);

const router = useRouter();

const checkoutStore = useCheckOutStore();
// const items = ref([]);
const total = computed(() => {
    return _.sumBy(select.value, function (o) {
        return o.subTotal;
    })
})

const levels = ref({
    1: "腕力會員",
    2: "豪力會員",
    3: "怪力會員"
})
const levelred = ref({
    1: "紅利累積 2%",
    2: "紅利累積 5%",
    3: "紅利累積 8%"
})

// items.value.map(item => Object.assign({}, { ...item, checked: true }));
const select = computed(() => items.value.filter(item => item.checked));
const unSelect = computed(() => items.value.filter(item => !item.checked));
const showFreebies = ref([]);
const discounts = ref([]);

const useStore = userStateStore();
const memberId = useStore.userId;

onMounted(async () => {
    try {
        await cartStore.updateCartItems();

        const dto = { cartVM: items.value };
        const resDiscount = await axios.post('/api/ActivityDiscounts/GetDiscount', dto);
        discounts.value = resDiscount.data;

        const filteredDiscounts = discounts.value.filter(discount => !discount.activityDiscount.freebieName);
        const freebieDiscounts = discounts.value.filter(discount => discount.activityDiscount.freebieName);

        items.value = items.value.map((item, index) => {
            let discount = 0;
            let activityDetailNameAry = [];
            let activityDetailName = '';
            let freebieName = '';
            let freebieImg = '';

            if (filteredDiscounts.length !== 0) {
                filteredDiscounts.map((filDiscount, i) => {
                    // let discountIndex = filDiscount.productConditions[index] === true ? i : -1;
                    filDiscount.discountProducts.map(((product, j) => {
                        if (product.productId == item.productId && filDiscount.discountPerPrices.length !== 0) {
                            discount = filDiscount.discountPerPrices[j];
                            activityDetailName = filDiscount.activityDiscount.activityDetailName;
                            activityDetailNameAry.push(activityDetailName);
                        }
                    }))

                }
                );
            }

            if (freebieDiscounts.length !== 0) {
                freebieDiscounts.map((freeDiscount, i) => {
                    let discountIndex = freeDiscount.productConditions[index] === true ? i : -1;
                    if (discountIndex !== -1) {
                        var freebieActivityDetailName = freebieDiscounts[i].activityDiscount.activityDetailName || '';
                        activityDetailNameAry.push(freebieActivityDetailName);
                        freebieName = freebieDiscounts[i].activityDiscount.freebieName || '';
                        freebieImg = freebieName !== '' ? "/files/images/freebie/" + freebieDiscounts[i].activityDiscount.freebieImg : '';
                    }
                }
                );
            }

            return {
                ...item,
                discountPerPrices: discount,
                activityDetailNames: activityDetailNameAry,
                freebieName: freebieName,
                freebieImg: freebieImg,
                subTotal: item.price * item.quantity - discount
            }
        })

        showFreebies.value = items.value.map(item => item.freebieName !== '');

    } catch (err) {
        console.error(err);
    }
})

// onMounted(async () => {
//     try {
//         const res = await axios.get(`/api/cart?id=${userStore.userId}`);
//         items.value = res.data.map(item => Object.assign({}, { ...item, checked: true }));
//     } catch (err) {
//         console.error(err);
//     }
// })


const emit = defineEmits(['cartData'])

watch(items, (newItems) => {
    emit('cartData', newItems);

})

watch(() => items.value.map(item => ({ quantity: item.quantity, checked: item.checked })), async (newValue, oldValue) => {
    try {
        const changeVal = select.value;
        const dto = { cartVM: changeVal, memberId: memberId };

        if (changeVal.length !== 0) {
            const res = await axios.post('/api/ActivityDiscounts/GetDiscount', dto);
            discounts.value = res.data;
        }

        getItemDiscount(select, unSelect, discounts);
    }
    catch (err) {
        console.error(err);
    }
});

watch(select, async (newValue, oldValue) => {
    try {
        const changeVal = select.value;
        const dto = { cartVM: changeVal, memberId: memberId };

        if (changeVal.length !== 0) {
            const res = await axios.post('/api/ActivityDiscounts/GetDiscount', dto);
            discounts.value = res.data;
        }

        getItemDiscount(select, unSelect, discounts);
    }
    catch (err) {
        console.error(err);
    }
});

function getItemDiscount(selectAry, unSelectAry, discountAry) {
    const filteredDiscounts = discountAry.value.filter(discount => !discount.activityDiscount.freebieName);
    const freebieDiscounts = discountAry.value.filter(discount => discount.activityDiscount.freebieName);
    console.log(filteredDiscounts)
    selectAry.value.map((item, index) => {
        let discount = 0;
        let activityDetailNameAry = [];
        let activityDetailName = '';
        let freebieName = '';
        let freebieImg = '';

        if (filteredDiscounts.length !== 0) {
            filteredDiscounts.map((filDiscount, i) => {
                // let discountIndex = filDiscount.productConditions[index] === true ? i : -1;
                filDiscount.discountProducts.map(((product, j) => {
                    if (product.productId == item.productId && filDiscount.discountPerPrices.length !== 0) {
                        discount = filDiscount.discountPerPrices[j];
                        activityDetailName = filDiscount.activityDiscount.activityDetailName;
                        activityDetailNameAry.push(activityDetailName);
                    }
                }))

            }
            );
        }

        if (freebieDiscounts.length !== 0) {
            freebieDiscounts.map((freeDiscount, i) => {
                let discountIndex = freeDiscount.productConditions[index] === true ? i : -1;
                if (discountIndex !== -1) {
                    var freebieActivityDetailName = freebieDiscounts[i].activityDiscount.activityDetailName || '';
                    activityDetailNameAry.push(freebieActivityDetailName);
                    freebieName = freebieDiscounts[i].activityDiscount.freebieName || '';
                    freebieImg = freebieName !== '' ? "/files/images/freebie/" + freebieDiscounts[i].activityDiscount.freebieImg : '';
                }
            }
            );
        }

        item.subTotal = item.price * item.quantity - discount;
        item.discountPerPrices = discount;
        item.activityDetailNames = activityDetailNameAry;
        item.freebieName = freebieName;
        item.freebieImg = freebieImg;
    });

    unSelectAry.value.map((item) => {
        item.subTotal = item.price * item.quantity;
        item.discountPerPrices = 0;
        item.activityDetailNames = [];
        item.freebieName = '';
        item.freebieImg = '';
    })

    showFreebies.value = items.value.map(item => item.freebieName !== '');
}


async function changeQuantity(item) {
    const { quantity, cartDetailId: id } = item;
    try {
        await axios.post('/api/cart/CartDetailQuantity', {
            quantity,
            id
        });
    } catch (err) {
        ElMessage({
            showClose: true,
            message: '喔不，購物車更改失敗!',
            type: 'error',
        })
    }
}


async function deleteItem(id) {
    try {
        await axios.delete(`/api/cart/items/${id}`)
    } catch (err) {
        ElMessage({
            showClose: true,
            message: '喔不，購物車更改失敗!',
            type: 'error',
        })
        return;
    }

    items.value = items.value.filter(item => item.cartDetailId !== id);
}

function checkout() {
    checkoutStore.items = select.value;
    router.push({ path: "/checkout" });
}

</script>

<template>
    <v-container v-if="items.length !== 0" fluid class="p-0">
        <v-table>
            <thead>
                <tr>
                    <th class="text-left">

                    </th>
                    <th class="text-left">
                        商品
                    </th>
                    <th class="text-left">

                    </th>
                    <th class="text-left">
                        單價
                    </th>
                    <th class="text-left">
                        數量
                    </th>
                    <th class="text-left">
                        小計
                    </th>
                    <th class="text-left">

                    </th>
                </tr>
            </thead>
            <tbody>
                <tr v-for="item in items" :key="item.productStylesId">
                    <td> <v-checkbox v-model="item.checked" color="warning"></v-checkbox></td>
                    <td><a :href="`/products/${item.productId}`" target="_blank"><v-img :width="150" aspect-ratio="16/9"
                                cover :src="`/files/images/products/${item.img}`"
                                :href="`/products/${item.productId}`"></v-img></a>
                    </td>
                    <td width="430"><a :href="`/products/${item.productId}`" target="_blank"
                            class="text-decoration-none text-black">{{
                                item.name
                            }}</a><br>
                        <small class="text-grey-darken-1">{{ item.style }}</small>
                        <br>
                        <br>
                        <v-chip v-for="activityDetailName in item.activityDetailNames" class="text-caption mb-1"
                            :prepend-icon=mdiCheckCircle color="green" style="white-space: normal;">
                            已符合&emsp;<p class="font-weight-bold">{{ activityDetailName }}</p>
                        </v-chip>
                    </td>
                    <td>{{ item.price }}</td>
                    <td><v-select variant="underlined" :items="_.range(1, item.inventory + 1)" v-model="item.quantity"
                            @update:modelValue="changeQuantity(item)"></v-select>
                    </td>
                    <td v-if="item.discountPerPrices != 0" class="text-red">{{ item.subTotal
                    }}</td>
                    <td v-else>{{ item.subTotal }}</td>
                    <td class="ms-n5 px-0"><v-btn :icon="mdiCloseCircle" class="icon grey" variant="plain"
                            @click="deleteItem(item.cartDetailId)"></v-btn></td>
                </tr>
                <br>
            </tbody>
        </v-table>
        <v-row v-for="(item, index) in items" :key="item.name">
            <v-col v-if="showFreebies[index]" cols="1" class="d-flex justify-center align-center">
                <v-chip class="text-caption" color="green">贈品</v-chip>
            </v-col>
            <v-col v-if="showFreebies[index]" cols="1" class="d-flex justify-center align-center">
                <v-img :width="150" aspect-ratio="16/9" cover :src="`${item.freebieImg}`"></v-img>
            </v-col>
            <v-col v-if="showFreebies[index]" cols="2" class="d-flex align-center">
                {{ item.freebieName }}
            </v-col>
        </v-row>
        <br>
        <h2>訂單總計 : &emsp;${{ total }}</h2>
        <v-row no-gutters class="align-end d-flex">
            <v-col cols="1">
                <small v-if="items.length > 0">{{ levels[items[0].memberLevel] }}</small>
            </v-col>
            <v-col cols="9">
                <h2 v-if="items.length > 0" class="text-orange-darken-4">{{ levelred[items[0].memberLevel] }}</h2>
            </v-col>
            <v-col cols="2" class="d-flex">
                <v-btn @click="checkout" color="warning"
                    class="w-15 h-auto text-h5 font-weight-bold ms-auto py-2">去結帳</v-btn>
            </v-col>
        </v-row>
    </v-container>
    <v-container v-else class="d-flex justify-center">
        <CartEmpty />
    </v-container>
</template>