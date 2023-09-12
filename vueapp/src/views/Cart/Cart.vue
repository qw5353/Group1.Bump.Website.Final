<script setup>
import CartLayout from '../../components/Cart/CartLayout.vue';
import DefaultLayout from '../../components/layouts/DefaultLayout.vue';
import { useTitle } from '@vueuse/core';

useTitle('Bump - 購物車');
</script>

<template>
    <DefaultLayout>
        <main>
            <CartLayout></CartLayout>
        </main>
    </DefaultLayout>
</template>
