<script setup lang="ts">
const props = defineProps<{
  label?: string;
  onClick?: () => void;
}>();

function handleClick() {
  if (props.onClick) props.onClick();
}
</script>

<template>
  <div class="add-card" @click="handleClick">
    <div class="icon">+</div>
    <div v-if="label" class="label">{{ label }}</div>
  </div>
</template>

<style scoped>
.add-card {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;

  width: 200px;
  height: 220px;
  background-color: var(--color-light-gray);
  border: 1px solid var(--color-gray);
  border-radius: 8px;
  cursor: pointer;

  transition: background-color 0.2s ease, transform 0.1s ease;
}

.add-card:hover {
  background-color: #dee2e6;
  transform: scale(1.02);
}

.icon {
  font-size: 48px;
  color: var(--color-dark-gray);
  line-height: 1;
}

.label {
  margin-top: 8px;
  font-size: 14px;
  color: var(--color-dark-gray);
  text-align: center;
}
</style>
