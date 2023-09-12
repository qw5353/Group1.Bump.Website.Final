<script setup>
defineProps(["questions"]);
</script>

<template>
    <v-expansion-panels class="w-100 mh-720">
      <v-expansion-panel
        v-for="question in questions"
        :key="question.id"
      >
      <v-expansion-panel-title class="text-h6">
        {{ question.title }}
      </v-expansion-panel-title>


      <v-expansion-panel-text style="white-space: pre-wrap;">
        {{ question.description }}
      </v-expansion-panel-text>
    </v-expansion-panel>
    </v-expansion-panels>
</template>

<style scoped>
.mh-720 {
  max-height: 720px;
  overflow-y: auto;
}
</style>