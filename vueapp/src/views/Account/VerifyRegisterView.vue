<script setup>
import axios from 'axios';
import { ref, onMounted } from 'vue'
import { useRoute } from 'vue-router';
import { useRouter } from 'vue-router';
import { userStateStore } from '../../stores/UserStateStore';
import { ElMessage } from 'element-plus'

const userStore = userStateStore()
const route = useRoute();
const router = useRouter()

const msg = ref('')
const errorMsg = ref('')

const data = ref({
    id: "",
    confirmCode: ""
})

data.value.id = route.query.id
data.value.confirmCode = route.query.confirmCode

async function verifyRegister() {
    try {
        const response = await axios.post('api/Account/ActiveRegister', data.value)
        msg.value = response.data;
        ElMessage({
            showClose: true,
            message: msg.value,
            type: 'success',
        })
        if (userStore.authenticate) {
            await userStore.GetUserInfo();
            setTimeout(() => { router.replace('/') }, 500)
        } else {
            setTimeout(() => { router.replace('/Login') }, 500)
        }

    } catch (err) {
        console.log(err)
        errorMsg.value = err.response.data
        ElMessage({
            showClose: true,
            message: errorMsg.value,
            type: 'error',
        })
        setTimeout(() => { router.replace('/') }, 500)
    }
}

onMounted(async () => {
    await verifyRegister()
})

</script>

<template>
    <router-view></router-view>
</template>
    
    