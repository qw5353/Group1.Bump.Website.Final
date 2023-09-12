<script setup>
import DefaultLayout from '../../components/layouts/DefaultLayout.vue'
import AccountLayout from '../../components/Account/AccountLayout.vue';
import OAuthRegister from '../../components/Account/OAuthRegister.vue';

</script>

<template>
    <DefaultLayout>
        <AccountLayout>
            <template #content>
                <OAuthRegister></OAuthRegister>
            </template>
        </AccountLayout>
    </DefaultLayout>
</template>
    