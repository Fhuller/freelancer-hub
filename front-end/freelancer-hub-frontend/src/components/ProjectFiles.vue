<template>
  <div class="files-section">
    <div class="files-header">
      <h3 class="section-title">
        <i class="fas fa-folder-open"></i>
        Arquivos do Projeto
        <span class="files-count" v-if="files.length > 0">({{ files.length }})</span>
      </h3>
      <div class="file-actions">
        <input
          type="file"
          id="file-upload"
          class="file-input"
          @change="handleFileUpload"
          :disabled="isLoadingFiles"
        />
        <label for="file-upload" class="upload-button">
          <i class="fas fa-plus"></i>
          <span v-if="isLoadingFiles">Enviando...</span>
          <span v-else>Adicionar Arquivo</span>
        </label>
      </div>
    </div>

    <div v-if="fileError" class="error-message file-error">
      <i class="fas fa-exclamation-triangle"></i>
      {{ fileError }}
    </div>

    <div v-if="isLoadingFiles" class="loading-files">
      <i class="fas fa-spinner fa-spin"></i>
      Carregando arquivos...
    </div>

    <div v-else-if="files.length === 0" class="no-files">
      <i class="fas fa-folder-open"></i>
      <p>Nenhum arquivo adicionado ao projeto</p>
    </div>

    <div v-else class="files-container">
      <div class="files-list">
        <div
          v-for="file in files"
          :key="file.id"
          class="file-card"
        >
          <div class="file-icon">
            <i :class="['fas', getFileIcon(file.fileExtension)]"></i>
          </div>
          <div class="file-content">
            <div class="file-info">
              <h4 class="file-name">{{ file.fileName }}{{ file.fileExtension }}</h4>
              <p class="file-meta">
                <span class="file-size">
                  <i class="fas fa-hdd"></i>
                  {{ formatFileSize(file.fileSize) }}
                </span>
                <span class="file-date">
                  <i class="fas fa-calendar"></i>
                  {{ new Date(file.createdAt).toLocaleDateString('pt-BR') }}
                </span>
              </p>
            </div>
            <div class="file-actions">
              <button
                class="action-button download-button"
                @click="handleDownloadFile(file)"
                title="Download"
              >
                <i class="fas fa-download"></i>
              </button>
              <button
                class="action-button delete-button"
                @click="handleDeleteFile(file.id)"
                title="Excluir"
              >
                <i class="fas fa-trash"></i>
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
defineProps<{
  files: any[]
  isLoadingFiles: boolean
  fileError: string
}>()

const emit = defineEmits<{
  'file-upload': [event: Event]
  'download-file': [file: any]
  'delete-file': [fileId: string]
}>()

function handleFileUpload(event: Event) {
  emit('file-upload', event)
}

function handleDownloadFile(file: any) {
  emit('download-file', file)
}

function handleDeleteFile(fileId: string) {
  emit('delete-file', fileId)
}

// Helper para formatar tamanho do arquivo
function formatFileSize(bytes: number): string {
  if (bytes === 0) return '0 Bytes'
  
  const k = 1024
  const sizes = ['Bytes', 'KB', 'MB', 'GB']
  const i = Math.floor(Math.log(bytes) / Math.log(k))
  
  return Number.parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i]
}

// Helper para obter ícone baseado na extensão do arquivo
function getFileIcon(extension: string): string {
  const iconMap: { [key: string]: string } = {
    '.pdf': 'fa-file-pdf',
    '.doc': 'fa-file-word',
    '.docx': 'fa-file-word',
    '.xls': 'fa-file-excel',
    '.xlsx': 'fa-file-excel',
    '.ppt': 'fa-file-powerpoint',
    '.pptx': 'fa-file-powerpoint',
    '.jpg': 'fa-file-image',
    '.jpeg': 'fa-file-image',
    '.png': 'fa-file-image',
    '.gif': 'fa-file-image',
    '.zip': 'fa-file-archive',
    '.rar': 'fa-file-archive',
    '.txt': 'fa-file-alt',
    '.js': 'fa-file-code',
    '.ts': 'fa-file-code',
    '.html': 'fa-file-code',
    '.css': 'fa-file-code',
    '.json': 'fa-file-code',
  }
  
  return iconMap[extension.toLowerCase()] || 'fa-file'
}
</script>

<style scoped>
.files-section {
  background: #fff;
  border-radius: 12px;
  border: 1px solid #e1e5e9;
  padding: 1.5rem;
  margin: 0;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.08);
  width: 100%;
}

.files-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1.5rem;
  border-bottom: 2px solid #f1f5f9;
  padding: 0 0 1rem 0;
}

.section-title {
  font-size: 1.5rem;
  font-weight: 700;
  color: #1e293b;
  margin: 0;
  display: flex;
  align-items: center;
  gap: 0.75rem;
}

.section-title i {
  color: #3b82f6;
  font-size: 1.375rem;
}

.files-count {
  background: #3b82f6;
  color: #fff;
  border-radius: 20px;
  padding: 0.25rem 0.75rem;
  font-size: 0.875rem;
  font-weight: 600;
  margin-left: 0.5rem;
}

.file-input {
  display: none;
}

.upload-button {
  background: linear-gradient(135deg, #3b82f6, #1d4ed8);
  color: #fff;
  border: none;
  border-radius: 8px;
  padding: 0.75rem 1.5rem;
  font-size: 0.875rem;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
  display: flex;
  align-items: center;
  gap: 0.5rem;
  box-shadow: 0 2px 4px rgba(59, 130, 246, 0.3);
}

.upload-button:hover:not(:disabled) {
  transform: translateY(-1px);
  box-shadow: 0 4px 8px rgba(59, 130, 246, 0.4);
  background: linear-gradient(135deg, #1d4ed8, #1e40af);
}

.upload-button:disabled {
  background: #94a3b8;
  cursor: not-allowed;
  transform: none;
  box-shadow: none;
}

.file-error {
  margin-bottom: 1rem;
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.file-error i {
  color: #dc2626;
}

.loading-files {
  text-align: center;
  padding: 3rem;
  color: #64748b;
  font-size: 1.125rem;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.75rem;
}

.loading-files i {
  color: #3b82f6;
}

.no-files {
  text-align: center;
  padding: 3rem;
  color: #94a3b8;
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 1rem;
}

.no-files i {
  font-size: 3rem;
  color: #cbd5e1;
}

.no-files p {
  font-size: 1.125rem;
  margin: 0;
}

.files-container {
  width: 100%;
}

.files-list {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
}

.file-card {
  display: flex;
  align-items: center;
  background: #f8fafc;
  border: 1px solid #e2e8f0;
  border-radius: 10px;
  padding: 1.25rem;
  transition: all 0.3s ease;
  gap: 1rem;
}

.file-card:hover {
  border-color: #3b82f6;
  box-shadow: 0 4px 12px rgba(59, 130, 246, 0.15);
  transform: translateY(-2px);
  background: #fff;
}

.file-icon {
  width: 60px;
  height: 60px;
  background: linear-gradient(135deg, #3b82f6, #60a5fa);
  border-radius: 10px;
  display: flex;
  align-items: center;
  justify-content: center;
  color: #fff;
  font-size: 1.5rem;
  flex-shrink: 0;
}

.file-content {
  display: flex;
  align-items: center;
  justify-content: space-between;
  flex: 1;
  gap: 1rem;
}

.file-info {
  flex: 1;
  min-width: 0;
}

.file-name {
  font-size: 1.125rem;
  font-weight: 600;
  color: #1e293b;
  margin: 0 0 0.5rem 0;
  word-break: break-word;
}

.file-meta {
  display: flex;
  gap: 1.5rem;
  margin: 0;
  font-size: 0.875rem;
  color: #64748b;
}

.file-size,
.file-date {
  display: flex;
  align-items: center;
  gap: 0.375rem;
}

.file-size i,
.file-date i {
  color: #94a3b8;
  font-size: 0.75rem;
}

.file-actions {
  display: flex;
  gap: 0.5rem;
  flex-shrink: 0;
}

.action-button {
  background: #fff;
  border: 1px solid #e2e8f0;
  border-radius: 8px;
  padding: 0.75rem;
  cursor: pointer;
  font-size: 1rem;
  transition: all 0.2s ease;
  color: #64748b;
  display: flex;
  align-items: center;
  justify-content: center;
  width: 44px;
  height: 44px;
}

.action-button:hover {
  transform: translateY(-1px);
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

.download-button:hover {
  background: #dcfce7;
  border-color: #22c55e;
  color: #16a34a;
}

.delete-button:hover {
  background: #fef2f2;
  border-color: #ef4444;
  color: #dc2626;
}

@media (max-width: 768px) {
  .files-header {
    flex-direction: column;
    gap: 1rem;
    align-items: flex-start;
  }
  
  .file-actions {
    width: 100%;
  }
  
  .upload-button {
    width: 100%;
    justify-content: center;
  }

  .file-content {
    flex-direction: column;
    align-items: flex-start;
    gap: 1rem;
  }

  .file-actions {
    align-self: flex-end;
  }

  .file-meta {
    flex-direction: column;
    gap: 0.5rem;
  }
}

@media (max-width: 640px) {
  .file-card {
    flex-direction: column;
    text-align: center;
    gap: 1rem;
  }

  .file-content {
    width: 100%;
    align-items: center;
  }

  .file-actions {
    align-self: center;
  }
}
</style>