<script setup lang="ts">
import { useAuthStore } from '../stores/auth';

const authStore = useAuthStore();
const emit = defineEmits(['toggle-sidebar']);

const handleLogout = async () => {
    await authStore.logout();
};
</script>

<template>
    <header class="dashboard-header">
        <div class="header-content">
            <div class="header-icons">
                <button @click="emit('toggle-sidebar')" class="hamburger-btn">
                    ☰
                </button>

                <div class="logo-section">
                    <img alt="Freelancer Hub" src="@/assets/logo_grd.png" width="140" height="50" />
                </div>
            </div>
            <div class="user-section">
                <span class="welcome-text">
                    Olá, {{ authStore.session?.user.email }}!
                </span>
                <button @click="handleLogout" class="logout-btn">
                    Sair
                </button>
            </div>
        </div>
    </header>
</template>

<style scoped>
.header-icons {
    display: flex;
    gap: 2rem;
}

.dashboard-header {
    background: white;
    border-bottom: 1px solid #dee2e6;
    padding: 0 20px;
}

.header-content {
    margin: 0 auto;
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding: 20px 0;
}

.logo-section {
    display: flex;
    align-items: center;
    gap: 15px;
}

.user-section {
    display: flex;
    align-items: center;
    gap: 15px;
}

.welcome-text {
    color: #666;
    font-weight: 500;
}

.logout-btn {
    background-color: #dc3545;
    color: white;
    border: none;
    padding: 8px 16px;
    border-radius: 6px;
    cursor: pointer;
    font-weight: 500;
    transition: background-color 0.2s;
}

.logout-btn:hover {
    background-color: #c82333;
}

.hamburger-btn {
    background: none;
    border: none;
    font-size: 30px;
    cursor: pointer;
    color: #333;
}

@media (max-width: 768px) {
    .header-content {
        flex-direction: column;
        gap: 15px;
    }
}
</style>