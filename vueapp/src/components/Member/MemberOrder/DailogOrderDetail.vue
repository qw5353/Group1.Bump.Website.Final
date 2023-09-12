<script setup>
import { ref, defineProps, computed, watch, defineEmits, nextTick } from 'vue';
import { ElMessage } from 'element-plus'
import { useRouter } from 'vue-router';
import axios from 'axios';

const router = useRouter();
const dialog = ref(false);
const props = defineProps(['inquire', 'orderDetailData']);
const emit = defineEmits(["close"]);
const modalCard = ref(null);
const statuses = [
    "已確認",
    "備貨中",
    "發貨中",
    "已發貨",
    "已到達",
    "已取貨",
    "七天鑑賞",
    "已完成"
];

const commentBTN = ref(false);

const currentStatusIndex = computed(() => props.orderDetailData.orderStatusId - 1)

dialog.value = props.inquire.dialog;
watch(props.inquire, function () {
    dialog.value = props.inquire.dialog;
    commentBTN.value = props.inquire.commentBTN;
})

watch(dialog, (newDialog) => {
    if (!newDialog) {
        emit('close');
    }
});

// watch(props.orderDetailData.details,function(){
//     userComment.value = props.orderDetailData.details
// })

function closeModal() {
    dialog.value = true;
    emit("close");
}
const userComment = ref([])

async function submitOrderComment() {
    try {
        await axios.post('/api/orderComment', props.orderDetailData.details);
        dialog.value = false;
        commentBTN.value = false;
    } catch (err) {
        ElMessage({
            showClose: true,
            message: '評論失敗!',
            type: 'warning',
        })
        // console.log(props.orderDetailData.details);
        dialog.value = false;
        commentBTN.value = false;
    }

}


async function goToRate() {
    commentBTN.value = true;

    await nextTick();

    modalCard.value.$el.scrollTop = modalCard.value.$el.scrollHeight;
}

</script>
<template>
    <v-dialog v-model="dialog" width="1000" @close="commentBTN = false">
        <v-card ref="modalCard">
            <p class="d-flex justify-end align-start mt-2 me-3 font-weight-bold">{{ props.orderDetailData.formattedCreateAt
            }}</p>
            <div class="d-flex flex-row mx-auto mt-2">
                <div style="width:100px;" class="">
                    <p>訂單編號</p>
                </div>
                <div style="width:100px;" class="ms-6">
                    <p>會員名稱</p>
                </div>
                <div style="width:100px;" class="ms-6">
                    <p>收件人名稱</p>
                </div>
                <div style="width:150px;" class="ms-6">
                    <p>收件人電話</p>
                </div>
                <div style="width:300px;" class="ms-6">
                    <p>收件人email</p>
                </div>
            </div>
            <div class="d-flex flex-row mx-auto">
                <div style="width:100px;">
                    <v-text-field variant="outlined" density="compact" readonly>{{ props.inquire.orderId }}</v-text-field>
                </div>
                <div style="width:100px;" class="ms-6">
                    <v-text-field variant="outlined" density="compact" readonly>{{ props.orderDetailData.memberName
                    }}</v-text-field>
                </div>
                <div style="width:100px;" class="ms-6">
                    <v-text-field variant="outlined" density="compact" readonly>{{ props.orderDetailData.recipientName
                    }}</v-text-field>
                </div>
                <div style="width:150px;" class="ms-6">
                    <v-text-field variant="outlined" density="compact" readonly>{{ props.orderDetailData.recipientPhone
                    }}</v-text-field>
                </div>
                <div style="width:300px;" class="ms-6">
                    <v-text-field variant="outlined" density="compact" readonly>{{ props.orderDetailData.recipientEmail
                    }}</v-text-field>
                </div>
            </div>
            <div class="d-flex flex-row mx-auto">
                <div style="width: 850px;" class="">
                    <p>收件地址</p>
                </div>
            </div>
            <div class="d-flex flex-row mx-auto">
                <div style="width: 850px;" class="">
                    <v-text-field variant="outlined" density="compact" readonly>{{ props.orderDetailData.recipientAddress
                    }}</v-text-field>
                </div>
            </div>
            <div class="d-flex justify-center">
                <v-table style="width: 750px;">
                    <thead>
                        <tr>
                            <th class="text-left">
                            </th>
                            <th class="text-left">

                            </th>
                            <th class="text-left">
                            </th>
                            <th class="text-left">
                            </th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr v-for="item in props.orderDetailData.details" :key="item.subTotal">
                            <td><v-img :width="150" aspect-ratio="16/9" cover
                                    :src="`/files/images/products/${item.thumbnail}`"></v-img>
                            </td>
                            <td width="430">{{ item.productName }}<br>
                                <small class="text-grey-darken-1">{{ item.style }}</small>
                                <br>
                                <br>
                            </td>
                            <td>{{ item.quantity }}件</td>
                            <td>${{ item.subtotal }}</td>
                        </tr>
                        <br>
                    </tbody>
                </v-table>

            </div>
            <v-row>
                <v-col cols="7">

                </v-col>
                <v-col cols="2">
                    <p class="font-weight-medium text-subtitle-1">運費:</p>
                    <p class="font-weight-medium text-subtitle-1">折價券:</p>
                    <p class="font-weight-medium text-subtitle-1">使用紅利:</p>
                    <p class="font-weight-bold text-h6 text-orange">總金額:</p>
                </v-col>
                <v-col cols="3">
                    <p class="font-weight-medium text-subtitle-1">{{ props.orderDetailData.deliverPrice }}</p>
                    <p class="font-weight-medium text-subtitle-1">{{ props.orderDetailData.discountPrice }}</p>
                    <p class="font-weight-medium text-subtitle-1">{{ props.orderDetailData.usedPoint }}</p>
                    <p class="font-weight-bold text-h6 text-orange">{{ props.orderDetailData.totalProductsPrice }}</p>
                </v-col>
            </v-row>
            <div class="d-flex flex-row mx-auto">
                <div style="width: 850px;" class="">
                    <p>備註</p>
                </div>
            </div>
            <div class="d-flex flex-row mx-auto">
                <div style="width: 850px;" class="">
                    <v-textarea variant="outlined" density="compact" readonly no-resize rows="2"
                        v-model="props.orderDetailData.note"></v-textarea>
                </div>
            </div>
            <div class="d-flex flex-row mx-auto" style="width: 850px;">
                <v-timeline direction="horizontal" side="end">
                    <v-timeline-item v-for="(status, index) in statuses" :key="index" size="small"
                        :dot-color="index <= currentStatusIndex ? 'orange' : ''">
                        <template v-slot:default>
                            <p>{{ status }}</p>
                        </template>
                    </v-timeline-item>
                </v-timeline>
            </div>
            <div class="d-flex justify-end me-3 mb-2">
                <small class="text-danger mt-3 me-8"
                    v-if="props.orderDetailData.orderStatusId == 8">您的評價能讓消費者更了解這項商品!</small>
                <v-card-actions style="width: 100px !important;" class="me-15"
                    v-if="props.orderDetailData.orderStatusId == 8">
                    <v-btn variant="tonal" color="orange-darken-3" @click="goToRate">前往評價</v-btn>
                </v-card-actions>
                <v-card-actions style="width: 100px !important;" class="me-8">
                    <v-btn variant="tonal" color="orange-darken-3" block
                        @click="dialog = false; commentBTN = false">關閉</v-btn>
                </v-card-actions>
            </div>
            <div v-if="commentBTN">
                <div class="d-flex flex-row mx-auto" style="width: 900px;">
                    <v-divider :thickness="2" class="border-opacity-75" color="warning"></v-divider>
                </div>

                <div v-for="item in props.orderDetailData.details" class="mt-5 d-flex justify-center">
                    <div style="width: 750px;">
                        <v-row no-gutters class="">
                            <v-col cols="4">
                                <v-img :width="150" aspect-ratio="16/9" cover
                                    :src="`/files/images/products/${item.thumbnail}`"></v-img>
                            </v-col>
                            <v-col cols="8" class="">
                                <a :href="`/products/${item.productId}`" target="_blank"
                                    class="text-decoration-none text-black ms-1">
                                    {{ item.productName }}</a><br>

                                <small class="text-grey-darken-1 d-block my-2  ms-1">{{ item.style }}</small>
                                <v-rating v-model="item.rating" hover color="yellow-darken-2" density="compact"></v-rating>
                                <v-textarea variant="outlined" density="compact" no-resize rows="2"
                                    v-model="item.userComment" style="width: 400px !important;"></v-textarea>
                            </v-col>
                        </v-row>
                    </div>
                </div>
                <div class="d-flex  justify-end  align-end mb-5 me-5">
                    <v-btn class="" variant="tonal" color="orange-darken-3" @click="submitOrderComment">送出</v-btn>
                </div>
            </div>
        </v-card>
    </v-dialog>
</template>
<style scoped>
p {
    line-height: 1rem !important;
    margin-bottom: 1rem;
}

.v-timeline-item__avatar {
    width: 30px;
    height: 30px;
}

.v-timeline-item__card {
    padding: 8px;
}

.text-center {
    text-align: center;
}
</style>