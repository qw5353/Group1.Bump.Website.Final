<script setup>
import DefaultLayout from '../../components/layouts/DefaultLayout.vue'
import Login from '../../components/Account/Login.vue'
import AccountLayout from '../../components/Account/AccountLayout.vue';
import OAuthLogin from '../../components/Account/OAuthLogin.vue';
import ForgetPwd from '../../components/Account/ForgetPwd.vue';

</script>

<template>
    <DefaultLayout>
        <AccountLayout>
            <template #content>
                <ForgetPwd></ForgetPwd>
            </template>
            <template #footer>
                <OAuthLogin></OAuthLogin>
            </template>
        </AccountLayout>
    </DefaultLayout>
</template>
    
<style scoped></style>