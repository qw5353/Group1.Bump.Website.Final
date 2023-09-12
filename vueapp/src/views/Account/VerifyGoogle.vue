<script setup>
import { userStateStore } from '../../stores/UserStateStore';
import { useRouter } from 'vue-router';
import { onMounted } from 'vue';

const router = useRouter()
const userStore = userStateStore()

onMounted(async () => {
    await userStore.GetUserInfo();
    userStore.setAuthentication(true);

    router.push('/');
})

</script>

<template></template>
    