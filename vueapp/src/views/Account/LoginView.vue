<script setup>
import DefaultLayout from '../../components/layouts/DefaultLayout.vue'
import Login from '../../components/Account/Login.vue'
import AccountLayout from '../../components/Account/AccountLayout.vue';
import OAuthLogin from '../../components/Account/OAuthLogin.vue';
import { useTitle } from '@vueuse/core';

useTitle('Bump - Login');

</script>

<template>
    <DefaultLayout>
        <AccountLayout>
            <template #content>
                <Login></Login>
            </template>
            <template #footer>
                <OAuthLogin></OAuthLogin>
            </template>
        </AccountLayout>
    </DefaultLayout>
</template>
    
<style scoped></style>