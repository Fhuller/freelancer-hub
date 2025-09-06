<script setup lang="ts">
defineProps<{
    as?: string
}>();
</script>

<template>
    <component :is="as || 'div'" class="content-section">
        <slot />
    </component>
</template>

<style scoped>
.content-section {
  margin: 2rem;
  background: var(--color-white);
  border-radius: 8px;
  padding: 30px;
  box-shadow: var(--shadow-default);
  height: 80vh;
  overflow: auto;
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(200px, 1fr));
  gap: 20px;
  justify-items: start;
}
</style>