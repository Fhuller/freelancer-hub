<script setup lang="ts">
import { computed } from 'vue'
import type { ClientReadDto } from '../services/clients'

interface Props {
    client: ClientReadDto
}

interface Emits {
    (e: 'view', clientId: string): void
    (e: 'edit', clientId: string): void
    (e: 'delete', clientId: string): void
}

const props = defineProps<Props>()
const emit = defineEmits<Emits>()

const avatarInitials = computed(() => {
    const names = props.client.name.split(' ')
    if (names.length === 1) {
        return names[0].substring(0, 2).toUpperCase()
    }
    return (names[0][0] + names[names.length - 1][0]).toUpperCase()
})

const formattedDate = computed(() => {
    const date = new Date(props.client.createdAt)
    return date.toLocaleDateString('pt-BR', {
        month: 'short',
        year: 'numeric'
    })
})

const viewClient = () => {
    emit('view', props.client.id)
}

const editClient = () => {
    emit('edit', props.client.id)
}

const deleteClient = () => {
    emit('delete', props.client.id)
}
</script>

<template>
    <div class="client-card">
        <div class="client-card-header">
            <div class="client-avatar">
                <span>{{ avatarInitials }}</span>
            </div>
            <div class="client-info">
                <h3 class="client-name">{{ client.name }}</h3>
                <p class="client-email">{{ client.email }}</p>
                <p v-if="client.company" class="client-company">{{ client.company }}</p>
            </div>
        </div>

        <div class="client-details">
            <div class="detail-item">
                <span class="detail-icon">📞</span>
                <span class="detail-text">{{ client.phone || 'Não adicionado' }}</span>
            </div>
            <div class="detail-item">
                <span class="detail-icon">📅</span>
                <span class="detail-text">Cliente desde {{ formattedDate }}</span>
            </div>
        </div>

        <div class="client-actions">
            <button @click="viewClient" class="action-btn view-btn">
                Ver Detalhes
            </button>
            <button @click="editClient" class="action-btn edit-btn">
                Editar
            </button>
            <button @click="deleteClient" class="action-btn delete-btn" title="Excluir cliente">
                🗑️
            </button>
        </div>
    </div>
</template>

<style scoped>
.client-card {
    background: white;
    border-radius: 12px;
    padding: 20px;
    box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
    border: 1px solid #e5e7eb;
    transition: all 0.2s ease;
    cursor: pointer;
}

.client-card:hover {
    box-shadow: 0 4px 16px rgba(0, 0, 0, 0.15);
    transform: translateY(-2px);
}

.client-card-header {
    display: flex;
    align-items: center;
    gap: 16px;
    margin-bottom: 16px;
}

.client-avatar {
    width: 50px;
    height: 50px;
    border-radius: 50%;
    background: linear-gradient(135deg, #7c3aed, #a855f7);
    display: flex;
    align-items: center;
    justify-content: center;
    color: white;
    font-weight: bold;
    font-size: 18px;
    flex-shrink: 0;
}

.client-info {
    flex: 1;
    min-width: 0;
}

.client-name {
    margin: 0 0 4px 0;
    font-size: 18px;
    font-weight: 600;
    color: #111827;
    white-space: nowrap;
    overflow: hidden;
    text-overflow: ellipsis;
}

.client-email {
    margin: 0 0 4px 0;
    font-size: 14px;
    color: #6b7280;
    white-space: nowrap;
    overflow: hidden;
    text-overflow: ellipsis;
}

.client-company {
    margin: 0;
    font-size: 13px;
    color: #9ca3af;
    white-space: nowrap;
    overflow: hidden;
    text-overflow: ellipsis;
}

.client-details {
    display: flex;
    flex-direction: column;
    gap: 8px;
    margin-bottom: 20px;
    padding: 12px 0;
    border-top: 1px solid #f3f4f6;
}

.detail-item {
    display: flex;
    align-items: center;
    gap: 8px;
}

.detail-icon {
    font-size: 14px;
    width: 16px;
    flex-shrink: 0;
}

.detail-text {
    font-size: 14px;
    color: #6b7280;
}

.client-actions {
    display: flex;
    gap: 8px;
    align-items: center;
}

.action-btn {
    border: none;
    padding: 8px 16px;
    border-radius: 6px;
    cursor: pointer;
    font-size: 13px;
    font-weight: 500;
    transition: all 0.2s ease;
}

.view-btn {
    background-color: #7c3aed;
    color: white;
    flex: 1;
}

.view-btn:hover {
    background-color: #6d28d9;
}

.edit-btn {
    background-color: #f3f4f6;
    color: #374151;
    flex: 1;
}

.edit-btn:hover {
    background-color: #e5e7eb;
}

.delete-btn {
    background-color: #fef2f2;
    color: #dc2626;
    padding: 8px 12px;
    width: 40px;
    display: flex;
    align-items: center;
    justify-content: center;
}

.delete-btn:hover {
    background-color: #fee2e2;
}

@media (max-width: 768px) {
    .client-card {
        padding: 16px;
    }

    .client-card-header {
        gap: 12px;
        margin-bottom: 12px;
    }

    .client-avatar {
        width: 40px;
        height: 40px;
        font-size: 16px;
    }

    .client-name {
        font-size: 16px;
    }

    .client-actions {
        flex-direction: column;
        gap: 8px;
    }

    .action-btn {
        width: 100%;
    }

    .delete-btn {
        width: 100%;
    }
}
</style>