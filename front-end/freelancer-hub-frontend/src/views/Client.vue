<script setup lang="ts">
import { ref, onMounted, computed } from 'vue';
import { useRouter } from 'vue-router';
import AuthenticatedLayout from '../layouts/AuthenticatedLayout.vue';
import { fetchClientById, deleteClient, type ClientReadDto } from '../services/clients';

// Props recebidas da rota
const props = defineProps<{
    id: string
}>();

const router = useRouter();
const clientData = ref<ClientReadDto | null>(null);
const isLoading = ref(false);
const error = ref('');
const showDeleteModal = ref(false);
const isDeleting = ref(false);

// Computed properties
const avatarInitials = computed(() => {
    if (!clientData.value?.name) return '';
    const names = clientData.value.name.split(' ');
    if (names.length === 1) {
        return names[0].substring(0, 2).toUpperCase();
    }
    return (names[0][0] + names[names.length - 1][0]).toUpperCase();
});

const formattedCreatedDate = computed(() => {
    if (!clientData.value?.createdAt) return '';
    return new Date(clientData.value.createdAt).toLocaleDateString('pt-BR', {
        day: '2-digit',
        month: 'long',
        year: 'numeric',
        hour: '2-digit',
        minute: '2-digit'
    });
});

const formattedUpdatedDate = computed(() => {
    if (!clientData.value?.updatedAt) return '';
    return new Date(clientData.value.updatedAt).toLocaleDateString('pt-BR', {
        day: '2-digit',
        month: 'long',
        year: 'numeric',
        hour: '2-digit',
        minute: '2-digit'
    });
});

// Methods
const loadClient = async () => {
    try {
        isLoading.value = true;
        error.value = '';
        clientData.value = await fetchClientById(props.id);
    } catch (err: any) {
        console.error('Erro ao carregar cliente:', err);
        if (err.status === 404) {
            error.value = 'Cliente não encontrado';
        } else {
            error.value = 'Erro ao carregar dados do cliente';
        }
    } finally {
        isLoading.value = false;
    }
};

const goBackToClients = () => {
    router.push('/clients');
};

const editClient = () => {
    router.push(`/clients/${props.id}/edit`);
};

const confirmDeleteClient = () => {
    showDeleteModal.value = true;
};

const cancelDelete = () => {
    showDeleteModal.value = false;
};

const handleDeleteClient = async () => {
    if (!clientData.value) return;

    try {
        isDeleting.value = true;
        await deleteClient(clientData.value.id);

        // Redirect to clients list after successful deletion
        router.push('/clients');
    } catch (err) {
        console.error('Erro ao excluir cliente:', err);
        error.value = 'Erro ao excluir cliente';
        showDeleteModal.value = false;
    } finally {
        isDeleting.value = false;
    }
};

const sendEmail = () => {
    if (clientData.value?.email) {
        window.open(`mailto:${clientData.value.email}`, '_blank');
    }
};

const makeCall = () => {
    if (clientData.value?.phone) {
        window.open(`tel:${clientData.value.phone}`, '_blank');
    }
};

onMounted(() => {
    loadClient();
});
</script>

<template>
    <AuthenticatedLayout>
        <div class="content-section">
            <div class="section-header">
                <div class="header-left">
                    <button @click="goBackToClients" class="back-btn">
                        ← Voltar para Clientes
                    </button>
                    <h2 v-if="clientData">Cliente: {{ clientData.name }}</h2>
                    <h2 v-else-if="isLoading">Carregando cliente...</h2>
                    <h2 v-else-if="error">Erro ao carregar cliente</h2>
                </div>
                <div v-if="clientData" class="header-actions">
                    <button @click="editClient" class="edit-btn">
                        ✏️ Editar
                    </button>
                    <button @click="confirmDeleteClient" class="delete-btn">
                        🗑️ Excluir
                    </button>
                </div>
            </div>

            <!-- Loading State -->
            <div v-if="isLoading" class="loading-state">
                <div class="spinner"></div>
                <p>Carregando dados do cliente...</p>
            </div>

            <!-- Error State -->
            <div v-else-if="error" class="error-state">
                <div class="error-icon">⚠️</div>
                <h3>Erro ao carregar cliente</h3>
                <p class="error-message">{{ error }}</p>
                <button @click="loadClient" class="retry-btn">Tentar novamente</button>
            </div>

            <!-- Client Details -->
            <div v-else-if="clientData" class="client-details">
                <!-- Basic Information Card -->
                <div class="detail-card">
                    <div class="card-header">
                        <div class="client-avatar">
                            <span>{{ avatarInitials }}</span>
                        </div>
                        <div class="client-basic-info">
                            <h3>{{ clientData.name }}</h3>
                            <p class="client-email">{{ clientData.email }}</p>
                            <span class="status-badge status-active">Ativo</span>
                        </div>
                    </div>
                </div>

                <!-- Contact Information -->
                <div class="detail-card">
                    <h3 class="card-title">📞 Informações de Contato</h3>
                    <div class="detail-grid">
                        <div class="detail-item">
                            <span class="label">Email:</span>
                            <span class="value">
                                <a :href="`mailto:${clientData.email}`" class="email-link">
                                    {{ clientData.email }}
                                </a>
                            </span>
                        </div>
                        <div v-if="clientData.phone" class="detail-item">
                            <span class="label">Telefone:</span>
                            <span class="value">
                                <a :href="`tel:${clientData.phone}`" class="phone-link">
                                    {{ clientData.phone }}
                                </a>
                            </span>
                        </div>
                        <div v-if="clientData.company" class="detail-item">
                            <span class="label">Empresa:</span>
                            <span class="value">{{ clientData.company }}</span>
                        </div>
                    </div>
                </div>

                <!-- System Information -->
                <div class="detail-card">
                    <h3 class="card-title">ℹ️ Informações do Sistema</h3>
                    <div class="detail-grid">
                        <div class="detail-item">
                            <span class="label">ID do Cliente:</span>
                            <span class="value code">{{ clientData.id }}</span>
                        </div>
                        <div class="detail-item">
                            <span class="label">Data de Criação:</span>
                            <span class="value">{{ formattedCreatedDate }}</span>
                        </div>
                        <div class="detail-item">
                            <span class="label">Última Atualização:</span>
                            <span class="value">{{ formattedUpdatedDate }}</span>
                        </div>
                    </div>
                </div>

                <!-- Quick Actions -->
                <div class="quick-actions">
                    <button @click="sendEmail" class="action-btn primary">
                        📧 Enviar Email
                    </button>
                    <button v-if="clientData.phone" @click="makeCall" class="action-btn secondary">
                        📞 Ligar
                    </button>
                    <button @click="editClient" class="action-btn secondary">
                        ✏️ Editar Informações
                    </button>
                </div>
            </div>
        </div>

        <!-- Delete Confirmation Modal -->
        <div v-if="showDeleteModal" class="modal-overlay" @click="cancelDelete">
            <div class="modal-content" @click.stop>
                <h3>Confirmar Exclusão</h3>
                <p>Tem certeza que deseja excluir o cliente <strong>{{ clientData?.name }}</strong>?</p>
                <p class="warning-text">Esta ação não pode ser desfeita e todos os dados relacionados serão perdidos.
                </p>
                <div class="modal-actions">
                    <button @click="cancelDelete" class="cancel-btn">Cancelar</button>
                    <button @click="handleDeleteClient" class="danger-btn" :disabled="isDeleting">
                        {{ isDeleting ? 'Excluindo...' : 'Excluir Cliente' }}
                    </button>
                </div>
            </div>
        </div>
    </AuthenticatedLayout>
</template>

<style scoped>
.content-section {
    background: white;
    border-radius: 8px;
    padding: 30px;
    box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.section-header {
    display: flex;
    justify-content: space-between;
    align-items: flex-start;
    margin-bottom: 30px;
    gap: 20px;
}

.header-left {
    display: flex;
    flex-direction: column;
    gap: 15px;
    flex: 1;
}

.header-actions {
    display: flex;
    gap: 10px;
}

.back-btn {
    background: transparent;
    border: 1px solid #d1d5db;
    color: #6b7280;
    padding: 8px 16px;
    border-radius: 6px;
    cursor: pointer;
    font-size: 14px;
    align-self: flex-start;
    transition: all 0.2s ease;
}

.back-btn:hover {
    background-color: #f9fafb;
    color: #374151;
}

.edit-btn {
    background-color: #7c3aed;
    color: white;
    border: none;
    padding: 10px 16px;
    border-radius: 6px;
    cursor: pointer;
    font-size: 14px;
    font-weight: 500;
    transition: background-color 0.2s ease;
}

.edit-btn:hover {
    background-color: #6d28d9;
}

.delete-btn {
    background-color: #dc2626;
    color: white;
    border: none;
    padding: 10px 16px;
    border-radius: 6px;
    cursor: pointer;
    font-size: 14px;
    font-weight: 500;
    transition: background-color 0.2s ease;
}

.delete-btn:hover {
    background-color: #b91c1c;
}

.section-header h2 {
    margin: 0;
    color: #333;
    font-size: 24px;
    font-weight: 600;
}

.loading-state {
    display: flex;
    flex-direction: column;
    align-items: center;
    gap: 15px;
    padding: 60px 40px;
    color: #666;
}

.error-state {
    display: flex;
    flex-direction: column;
    align-items: center;
    gap: 15px;
    padding: 60px 40px;
    color: #666;
    text-align: center;
}

.error-icon {
    font-size: 48px;
    margin-bottom: 10px;
}

.error-state h3 {
    margin: 0;
    color: #dc2626;
    font-size: 20px;
}

.error-message {
    color: #6b7280;
    margin: 0;
}

.retry-btn {
    background-color: #f3f4f6;
    color: #374151;
    border: none;
    padding: 10px 20px;
    border-radius: 6px;
    cursor: pointer;
    font-weight: 500;
    transition: background-color 0.2s ease;
    margin-top: 10px;
}

.retry-btn:hover {
    background-color: #e5e7eb;
}

.spinner {
    width: 40px;
    height: 40px;
    border: 4px solid #f3f3f3;
    border-top: 4px solid #7c3aed;
    border-radius: 50%;
    animation: spin 1s linear infinite;
}

@keyframes spin {
    0% {
        transform: rotate(0deg);
    }

    100% {
        transform: rotate(360deg);
    }
}

.client-details {
    display: flex;
    flex-direction: column;
    gap: 24px;
}

.detail-card {
    background-color: #ffffff;
    border: 1px solid #e5e7eb;
    border-radius: 12px;
    padding: 24px;
    box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
}

.card-header {
    display: flex;
    align-items: center;
    gap: 20px;
}

.client-avatar {
    width: 80px;
    height: 80px;
    border-radius: 50%;
    background: linear-gradient(135deg, #7c3aed, #a855f7);
    display: flex;
    align-items: center;
    justify-content: center;
    color: white;
    font-weight: bold;
    font-size: 32px;
    flex-shrink: 0;
}

.client-basic-info h3 {
    margin: 0 0 8px 0;
    color: #111827;
    font-size: 28px;
    font-weight: 700;
}

.client-email {
    margin: 0 0 12px 0;
    color: #6b7280;
    font-size: 16px;
}

.status-badge {
    display: inline-flex;
    align-items: center;
    padding: 6px 12px;
    border-radius: 20px;
    font-size: 14px;
    font-weight: 500;
}

.status-active {
    background-color: #d1fae5;
    color: #065f46;
}

.card-title {
    margin: 0 0 20px 0;
    color: #374151;
    font-size: 18px;
    font-weight: 600;
    display: flex;
    align-items: center;
    gap: 8px;
}

.detail-grid {
    display: grid;
    gap: 16px;
}

.detail-item {
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding: 12px 0;
    border-bottom: 1px solid #f3f4f6;
}

.detail-item:last-child {
    border-bottom: none;
}

.label {
    font-weight: 500;
    color: #6b7280;
    min-width: 140px;
}

.value {
    color: #111827;
    text-align: right;
    flex: 1;
}

.code {
    font-family: 'Courier New', monospace;
    background-color: #f3f4f6;
    padding: 4px 8px;
    border-radius: 4px;
    font-size: 14px;
}

.email-link,
.phone-link {
    color: #7c3aed;
    text-decoration: none;
    transition: color 0.2s ease;
}

.email-link:hover,
.phone-link:hover {
    color: #6d28d9;
    text-decoration: underline;
}

.quick-actions {
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
    gap: 16px;
}

.action-btn {
    padding: 14px 20px;
    border-radius: 8px;
    font-weight: 500;
    font-size: 14px;
    cursor: pointer;
    transition: all 0.2s ease;
    border: none;
    display: flex;
    align-items: center;
    justify-content: center;
    gap: 8px;
}

.action-btn.primary {
    background-color: #7c3aed;
    color: white;
}

.action-btn.primary:hover {
    background-color: #6d28d9;
}

.action-btn.secondary {
    background-color: #f3f4f6;
    color: #374151;
}

.action-btn.secondary:hover {
    background-color: #e5e7eb;
}

/* Modal Styles */
.modal-overlay {
    position: fixed;
    top: 0;
    left: 0;
    right: 0;
    bottom: 0;
    background-color: rgba(0, 0, 0, 0.5);
    display: flex;
    align-items: center;
    justify-content: center;
    z-index: 1000;
    padding: 20px;
}

.modal-content {
    background: white;
    border-radius: 12px;
    padding: 24px;
    max-width: 400px;
    width: 100%;
    box-shadow: 0 10px 25px rgba(0, 0, 0, 0.2);
}

.modal-content h3 {
    margin: 0 0 16px 0;
    color: #111827;
    font-size: 18px;
    font-weight: 600;
}

.modal-content p {
    margin: 0 0 8px 0;
    color: #374151;
    line-height: 1.5;
}

.warning-text {
    color: #dc2626 !important;
    font-size: 14px;
    margin-bottom: 24px !important;
}

.modal-actions {
    display: flex;
    gap: 12px;
    justify-content: flex-end;
}

.cancel-btn {
    background-color: #f3f4f6;
    color: #374151;
    border: none;
    padding: 10px 20px;
    border-radius: 6px;
    cursor: pointer;
    font-weight: 500;
    transition: background-color 0.2s ease;
}

.cancel-btn:hover {
    background-color: #e5e7eb;
}

.danger-btn {
    background-color: #dc2626;
    color: white;
    border: none;
    padding: 10px 20px;
    border-radius: 6px;
    cursor: pointer;
    font-weight: 500;
    transition: background-color 0.2s ease;
}

.danger-btn:hover:not(:disabled) {
    background-color: #b91c1c;
}

.danger-btn:disabled {
    opacity: 0.6;
    cursor: not-allowed;
}

@media (max-width: 768px) {
    .content-section {
        padding: 20px;
    }

    .section-header {
        flex-direction: column;
        align-items: stretch;
        gap: 15px;
    }

    .header-actions {
        justify-content: center;
    }

    .card-header {
        flex-direction: column;
        text-align: center;
        gap: 16px;
    }

    .client-avatar {
        width: 64px;
        height: 64px;
        font-size: 24px;
    }

    .detail-item {
        flex-direction: column;
        align-items: flex-start;
        gap: 8px;
    }

    .value {
        text-align: left;
    }

    .quick-actions {
        grid-template-columns: 1fr;
    }

    .modal-actions {
        flex-direction: column;
    }

    .cancel-btn,
    .danger-btn {
        width: 100%;
    }
}
</style>