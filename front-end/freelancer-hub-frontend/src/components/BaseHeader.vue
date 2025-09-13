<script setup lang="ts">
import { computed } from 'vue'
import { useI18n } from 'vue-i18n'

const props = defineProps<{
  model: Record<string, any>
  modelName?: string
}>()

const { t } = useI18n()

const fields = computed(() => {
  if (!props.model) return []
  return Object.entries(props.model).map(([key, value]) => ({
    key,
    label: t(key),
    value
  }))
})
</script>

<template>
  <header class="base-header">
    <h2 class="header-title">
      {{ t(modelName || 'details') }}
    </h2>

    <div class="header-fields">
      <div
        v-for="field in fields"
        :key="field.key"
        class="header-field"
      >
        <span class="header-label">{{ field.label }}:</span>
        <span class="header-value">{{ field.value }}</span>
      </div>
    </div>
  </header>
</template>

<style scoped>
.base-header {
  grid-column: 1 / -1; /* ocupa todas as colunas do grid */
  background: #f9f9f9;
  padding: 1.5rem;
  margin-bottom: 1.5rem;
  border-radius: 8px;
  box-shadow: 0 1px 3px rgba(0,0,0,0.1);
  width: 100%;
}

.header-title {
  margin: 0 0 1rem;
  font-size: 1.25rem;
  font-weight: 600;
}

.header-fields {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(220px, 1fr));
  gap: 0.75rem 1rem;
}

.header-field {
  display: flex;
  flex-direction: column;
}

.header-label {
  font-size: 0.85rem;
  font-weight: 500;
  color: #666;
}

.header-value {
  font-size: 1rem;
  color: #222;
}
</style>
