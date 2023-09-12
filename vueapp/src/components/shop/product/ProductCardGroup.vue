<script setup>
import { ref } from 'vue';
import ProductCard from './ProductCard.vue';

defineProps(["title", "products", "link", "prependIcon", "appendIcon"])

</script>

<template>
    <div>
        <div class="d-flex flex-column align-center mt-16">
            <v-row>
                <img v-if="prependIcon" style="height:30px; width:30px; position:relative; top:3px; margin-right: 6px;"
                    :src="prependIcon">

                <h2 style="align-self: center;" :style="{
                    marginLeft: appendIcon && !prependIcon ? '36px' : '0px',
                    marginRight: !appendIcon && prependIcon ? '36px' : '0px'
                }">
                    <slot name="icon-element-prepend"></slot>{{ title }}<slot name="icon-element-append"></slot>
                </h2>
                <img v-if="appendIcon" style="height:30px; width:30px; position:relative; top:3px; margin-left: 6px;"
                    :src="appendIcon">
            </v-row>
            <div class="title_style mt-5">
            </div>
        </div>
        <v-slide-group class="pa-4 mx-auto width-RWD" selected-class="bg-success" show-arrows>
            <template #prev>
                <v-icon icon="fa:fa-solid fa-caret-left fa-xxl" color="#FFB23E" size="x-large"></v-icon>
            </template>
            <template #next>
                <v-icon icon="fa:fa-solid fa-caret-right fa-xxl" color="#FFB23E" size="x-large"></v-icon>
            </template>
            <v-slide-group-item v-for="product in products">
                <ProductCard :id="product.id" :name="product.name" :brandName="product.brandName" :price="product.price"
                    :thumbnail="product.thumbnail">
                </ProductCard>
            </v-slide-group-item>
        </v-slide-group>
        <v-btn class="rounded-pill mx-auto d-flex font-weight-black font-italic btn-more" color="rgba(255, 178, 62, 0.62)"
            :href="link">More...</v-btn>
    </div>
</template>

<style scoped>
/* XL 尺寸 */
@media (max-width: 9999px) {
    .width-RWD {
        width: 1185px;
    }
}

/* LG 尺寸 */
@media (max-width: 1300px) {
    .width-RWD {
        width: 1000px;
    }
}

/* MD 尺寸 */
@media (max-width: 1100px) {
    .width-RWD {
        width: 800px;
    }
}

/* SM 尺寸 */
@media (max-width: 900px) {
    .width-RWD {
        width: 600px;
    }
}

/* XS 尺寸 */
@media (max-width: 600px) {
    .width-RWD {
        width: 395px;
    }
}

/* XXS 尺寸 */
@media (max-width: 390px) {
    .width-RWD {
        width: 350px;
    }
}


.fa-xxl {
    font-size: 4em;
}

.title_style {
    margin-top: 0.75rem;
    border-top: 3px solid rgb(255, 178, 62);
    width: 24px;
    border-radius: 12px;
}

.font-italic {
    font-style: italic;
}

.btn-more {
    width: 100px;
    top: -20px;
}
</style>

