<script setup lang="ts">
import { reactive, watchEffect } from 'vue'
import { useI18n } from 'vue-i18n'
import { useToast } from 'vue-toast-notification'

const props = defineProps<{
  visible: boolean
  model: Record<string, any>
  modelName?: string
  onSave: (data: Record<string, any>) => Promise<void> | void
}>()

const emit = defineEmits(['close'])
const toast = useToast()
const { t } = useI18n()

const formData = reactive<Record<string, any>>({})

watchEffect(() => {
  if (props.visible && props.model) {
    Object.keys(props.model).forEach(k => formData[k] = props.model[k])
  }
})

function resetForm() {
  Object.keys(formData).forEach((k) => (formData[k] = ''))
}

async function handleSave() {
  try {
    await props.onSave({ ...formData })
    toast.success(t('saved') || 'Salvo com sucesso!')
    resetForm()
    emit('close')
  } catch (err) {
    console.error(err)
    toast.error(t('errorSaving') || 'Erro ao salvar')
  }
}
</script>

<template>
  <div v-if="visible" class="modal-overlay" @click.self="emit('close')">
    <div class="modal-content">
      <h3 class="modal-title">{{ t(modelName || 'edit') }}</h3>

      <div class="modal-body">
        <div v-for="(value, key) in formData" :key="key" class="field">
          <label :for="key">{{ t(key) }}</label>

          <template v-if="typeof value === 'boolean'">
            <input type="checkbox" :id="key" v-model="formData[key]" />
          </template>
          <template v-else-if="typeof value === 'number'">
            <input type="number" :id="key" v-model.number="formData[key]" />
          </template>
          <template v-else>
            <input type="text" :id="key" v-model="formData[key]" />
          </template>
        </div>
      </div>

      <div class="modal-footer">
        <button class="cancel-btn" @click="() => { resetForm(); emit('close') }">
          {{ t('cancel') }}
        </button>
        <button class="save-btn" @click="handleSave">{{ t('save') }}</button>
      </div>
    </div>
  </div>
</template>

<style scoped>
.modal-overlay {
  position: fixed;
  inset: 0;
  background: rgba(0, 0, 0, 0.4);
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 50;
}
.modal-content {
  background: var(--color-white);
  border-radius: 8px;
  box-shadow: var(--shadow-default);
  width: 500px;
  max-height: 80vh;
  display: flex;
  flex-direction: column;
}
.modal-title {
  font-size: 18px;
  font-weight: 600;
  padding: 16px 20px;
  border-bottom: 1px solid #ddd;
}
.modal-body {
  flex: 1;
  overflow-y: auto;
  padding: 16px 20px;
  display: flex;
  flex-direction: column;
  gap: 12px;
}
.field label {
  font-size: 14px;
  font-weight: 500;
  margin-bottom: 4px;
  display: block;
  color: var(--color-dark-gray);
}
.field input[type='text'],
.field input[type='number'] {
  width: 100%;
  padding: 8px;
  border: 1px solid var(--color-gray);
  border-radius: 4px;
}
.modal-footer {
  display: flex;
  justify-content: flex-end;
  gap: 10px;
  padding: 12px 20px;
  border-top: 1px solid #ddd;
}
.cancel-btn,
.save-btn {
  padding: 8px 16px;
  border: none;
  border-radius: 4px;
  cursor: pointer;
}
.cancel-btn {
  background: #e9ecef;
}
.save-btn {
  background: var(--color-purple);
  color: white;
}
.save-btn:hover {
  opacity: 0.9;
}
</style>