<script setup>
</script>

<template>
    <v-container class="height">
        <div class="d-flex align-center flex-column align-items-center mt-5">
            <v-card width="500" height="630">
                <template v-slot:title>
                    <v-card-actions>
                        <v-btn to="/Login"> 登入 </v-btn>
                        <v-btn to="/Register"> 註冊 </v-btn>
                    </v-card-actions>
                </template>
                <template v-slot:text>
                    <slot name="content"></slot>
                </template>
                <template v-slot:actions>
                    <slot name="footer"></slot>
                </template>
            </v-card>
            <div>
                <slot name="extra"></slot>
            </div>
        </div>
    </v-container>
</template>
    
<style scoped>
.height {
    height: calc(100vh - 66px)
}
</style>