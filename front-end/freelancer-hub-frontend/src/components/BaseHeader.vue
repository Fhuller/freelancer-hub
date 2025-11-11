<script setup lang="ts">
import { computed, ref } from 'vue'
import { useI18n } from 'vue-i18n'

const props = defineProps<{
  model?: Record<string, any>
  modelName?: string
  searchable?: boolean
  placeholder?: string
}>()

const emit = defineEmits(['search'])

const { t, locale } = useI18n()

function formatValue(value: any) {
  if (!value) return ''

  if (value instanceof Date) {
    return new Intl.DateTimeFormat(locale.value, {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric'
    }).format(value)
  }

  if (typeof value === 'string') {
    const isoDateRegex = /^\d{4}-\d{2}-\d{2}(T.*)?$/
    if (isoDateRegex.test(value)) {
      return new Intl.DateTimeFormat(locale.value, {
        day: '2-digit',
        month: '2-digit',
        year: 'numeric'
      }).format(new Date(value))
    }
  }

  return value
}

function isIdField(key: string) {
  return (
    key.toLowerCase() === 'id' ||
    key.toLowerCase().endsWith('_id') ||
    key.endsWith('Id')
  )
}

const fields = computed(() => {
  if (!props.model) return []
  return Object.entries(props.model)
    .filter(([key]) => !isIdField(key))
    .map(([key, value]) => ({
      key,
      label: t(key),
      value: formatValue(value)
    }))
})

// --- searchable input logic ---
const searchQuery = ref('')
function emitSearch() {
  emit('search', searchQuery.value.trim())
}
</script>

<template>
  <header class="base-header">
    <div class="header-top">
      <h2 class="header-title">
        {{ t(modelName || 'details') }}
      </h2>

      <!-- simple search input, visible when searchable prop is true -->
      <div v-if="props.searchable" class="header-search">
        <input
          v-model="searchQuery"
          @input="emitSearch"
          :placeholder="props.placeholder || t('search')"
          class="header-search-input"
        />
      </div>
    </div>

    <div class="header-fields" v-if="fields.length">
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
  grid-column: 1 / -1;
  background: #f9f9f9;
  padding: 1rem 1.5rem;
  border-radius: 8px;
  box-shadow: 0 1px 3px rgba(0,0,0,0.1);
  width: 100%;
}

.header-top {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.header-title {
  margin: 0 0 0.5rem 0;
  font-size: 1.25rem;
  font-weight: 600;
}

.header-search {
  min-width: 220px;
}

.header-search-input {
  width: 100%;
  padding: 0.5rem 0.75rem;
  border-radius: 8px;
  border: 1px solid #d1d5db;
  font-size: 0.95rem;
}

.header-fields {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(220px, 1fr));
  gap: 0.75rem 1rem;
  margin-top: 0.75rem;
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
