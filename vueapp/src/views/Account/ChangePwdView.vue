<script setup>
import DefaultLayout from '../../components/layouts/DefaultLayout.vue'
import AccountLayout from '../../components/Account/AccountLayout.vue';
import ChangePwd from '../../components/Account/ChangePwd.vue';

</script>

<template>
    <DefaultLayout>
        <AccountLayout>
            <template #content>
                <ChangePwd></ChangePwd>
            </template>
        </AccountLayout>
    </DefaultLayout>
</template>