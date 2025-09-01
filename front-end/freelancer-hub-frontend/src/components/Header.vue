<script setup lang="ts">
import { ref, onMounted, onUnmounted, computed } from 'vue';
import { useRouter, useRoute } from 'vue-router';
import { useAuthStore } from '../stores/auth';
import { useI18n } from 'vue-i18n';

const { locale } = useI18n();
const authStore = useAuthStore();
const router = useRouter();
const route = useRoute();
const emit = defineEmits(['toggle-sidebar']);

const userMenuOpen = ref(false);
const userMenuRef = ref<HTMLElement>();

const toggleUserMenu = () => {
    userMenuOpen.value = !userMenuOpen.value;
};

const changeLanguage = async (lang: string) => {
  await authStore.setLanguage(lang);
};
const closeUserMenu = (event: Event) => {
    if (userMenuRef.value && !userMenuRef.value.contains(event.target as Node)) {
        userMenuOpen.value = false;
    }
};

const handleLogout = async () => {
    await authStore.logout();
    userMenuOpen.value = false;
};

const goToHome = () => {
    router.push('/dashboard');
};

const navigateTo = (path: string) => {
    router.push(path);
};

const breadcrumbs = computed(() => {
    const currentRoute = route.name as string;
    const params = route.params;

    const items = [
        { name: 'Dashboard', path: '/app/dashboard', clickable: true }
    ];

    if (currentRoute === 'Clients') {
        items.push({ name: 'Clientes', path: '/app/clients', clickable: true });
    } else if (currentRoute === 'Client') {
        items.push({ name: 'Clientes', path: '/app/clients', clickable: true });
        items.push({ name: `Cliente #${params.id}`, path: `/app/clients/${params.id}`, clickable: true });
    } else if (currentRoute === 'Finance') {
        items.push({ name: 'Financeiro', path: '/app/finance', clickable: true });
    }

    return items;
});

onMounted(() => {
    document.addEventListener('click', closeUserMenu);
});

onUnmounted(() => {
    document.removeEventListener('click', closeUserMenu);
});
</script>

<template>
    <header class="github-header">
        <div class="header-container">
            <div class="header-left">
                <button @click="emit('toggle-sidebar')" class="hamburger-btn" aria-label="Toggle sidebar">
                    <svg width="16" height="16" viewBox="0 0 16 16" fill="currentColor">
                        <path
                            d="M1 2.75A.75.75 0 0 1 1.75 2h12.5a.75.75 0 0 1 0 1.5H1.75A.75.75 0 0 1 1 2.75Zm0 5A.75.75 0 0 1 1.75 7h12.5a.75.75 0 0 1 0 1.5H1.75A.75.75 0 0 1 1 7.75ZM1.75 12h12.5a.75.75 0 0 1 0 1.5H1.75a.75.75 0 0 1 0-1.5Z" />
                    </svg>
                </button>

                <button @click="goToHome" class="logo-btn">
                    <img alt="Freelancer Hub" src="@/assets/logo_grd.png" width="120" height="auto" />
                </button>

                <nav class="breadcrumbs">
                    <template v-for="(item, index) in breadcrumbs" :key="item.path">
                        <div v-if="index > 0" class="breadcrumb-separator">/</div>
                        <button @click="navigateTo(item.path)" class="breadcrumb-item"
                            :class="{ 'current': index === breadcrumbs.length - 1 }">
                            {{ item.name }}
                        </button>
                    </template>
                </nav>
            </div>

            <div class="header-right">
                <div class="user-menu-container" ref="userMenuRef">
                    <button @click="toggleUserMenu" class="user-avatar-btn" :class="{ 'active': userMenuOpen }"
                        aria-label="User menu">
                        <svg width="20" height="20" viewBox="0 0 16 16" fill="currentColor">
                            <path
                                d="M10.561 8.073a6.005 6.005 0 0 1 3.432 5.142.75.75 0 1 1-1.498.07 4.5 4.5 0 0 0-8.99 0 .75.75 0 0 1-1.498-.07 6.005 6.005 0 0 1 3.431-5.142 3.999 3.999 0 1 1 5.123 0ZM10.5 5a2.5 2.5 0 1 0-5 0 2.5 2.5 0 0 0 5 0Z" />
                        </svg>
                        <svg class="dropdown-icon" width="16" height="16" viewBox="0 0 16 16" fill="currentColor">
                            <path
                                d="m4.427 9.427 3.396 3.396a.25.25 0 0 0 .354 0l3.396-3.396A.25.25 0 0 0 11.396 9H4.604a.25.25 0 0 0-.177.427Z" />
                        </svg>
                    </button>

                    <!-- User dropdown menu -->
                    <div v-if="userMenuOpen" class="user-dropdown">
                        <div class="user-info">
                            <div class="user-email">{{ authStore.session?.user.email }}</div>
                        </div>
                        <div class="dropdown-language">
                            <button
                                @click="changeLanguage('pt')"
                                :class="{ 'active': locale === 'pt' }"
                                class="dropdown-item">
                                <span class="fi fi-br"></span> Português
                            </button>
                            <button
                                @click="changeLanguage('en')"
                                :class="{ 'active': locale === 'en' }"
                                class="dropdown-item">
                                <span class="fi fi-us"></span> English
                            </button>
                        </div>
                        <div class="dropdown-divider"></div>
                        <button @click="handleLogout" class="dropdown-item logout-item">
                            <svg width="16" height="16" viewBox="0 0 16 16" fill="currentColor">
                                <path
                                    d="M2 2.75C2 1.784 2.784 1 3.75 1h2.5a.75.75 0 0 1 0 1.5h-2.5a.25.25 0 0 0-.25.25v10.5c0 .138.112.25.25.25h2.5a.75.75 0 0 1 0 1.5h-2.5A1.75 1.75 0 0 1 2 13.25Zm6.56 4.5h5.69a.75.75 0 0 1 0 1.5H8.56l1.97 1.97a.749.749 0 1 1-1.06 1.06L6.22 8.53a.75.75 0 0 1 0-1.06l3.25-3.25a.749.749 0 1 1 1.06 1.06L8.56 7.25Z" />
                            </svg>
                            Sair
                        </button>
                    </div>
                </div>
            </div>
        </div>
    </header>
</template>

<style scoped>
.dropdown-language {
    display: flex;
    flex-direction: column;
    margin-top: 8px;
}

.dropdown-language .dropdown-item.active {
    font-weight: bold;
}

.github-header {
    background: white;
    border-bottom: 1px solid #d1d9e0;
    position: sticky;
    top: 0;
    z-index: 1000;
}

.header-container {
    max-width: 100%;
    margin: 0 auto;
    padding: 0 16px;
    display: flex;
    align-items: center;
    justify-content: space-between;
    min-height: 64px;
}

.header-left {
    display: flex;
    align-items: center;
    gap: 16px;
    flex: 1;
}

.hamburger-btn {
    background: transparent;
    border: none;
    padding: 8px;
    border-radius: 6px;
    cursor: pointer;
    color: #656d76;
    display: flex;
    align-items: center;
    justify-content: center;
    transition: background-color 0.15s ease;
}

.hamburger-btn:hover {
    background-color: #f3f4f6;
    color: #24292f;
}

.logo-btn {
    background: none;
    border: none;
    cursor: pointer;
    padding: 4px;
    border-radius: 6px;
    transition: opacity 0.15s ease;
}

.logo-btn:hover {
    opacity: 0.8;
}

.logo-btn img {
    display: block;
    height: 32px;
    width: auto;
}

.breadcrumbs {
    display: flex;
    align-items: center;
    gap: 8px;
    font-size: 14px;
    color: #656d76;
}

.breadcrumb-separator {
    color: #8c959f;
    font-weight: 400;
}

.breadcrumb-item {
    background: none;
    border: none;
    color: #656d76;
    font-weight: 500;
    cursor: pointer;
    padding: 4px 8px;
    border-radius: 6px;
    font-size: 14px;
    transition: all 0.15s ease;
}

.breadcrumb-item:hover {
    background-color: #f3f4f6;
    color: #24292f;
}

.breadcrumb-item.current {
    color: #7c3aed;
    font-weight: 600;
}

.header-right {
    display: flex;
    align-items: center;
}

.user-menu-container {
    position: relative;
}

.user-avatar-btn {
    background: transparent;
    border: none;
    padding: 4px 8px;
    border-radius: 50px;
    cursor: pointer;
    color: #656d76;
    display: flex;
    align-items: center;
    gap: 4px;
    transition: background-color 0.15s ease;
}

.user-avatar-btn:hover,
.user-avatar-btn.active {
    background-color: #f3f4f6;
    color: #24292f;
}

.dropdown-icon {
    transition: transform 0.15s ease;
}

.user-avatar-btn.active .dropdown-icon {
    transform: rotate(180deg);
}

.user-dropdown {
    position: absolute;
    top: 100%;
    right: 0;
    margin-top: 8px;
    background: white;
    border: 1px solid #d1d9e0;
    border-radius: 12px;
    box-shadow: 0 16px 32px rgba(1, 4, 9, 0.85);
    min-width: 200px;
    z-index: 100;
    animation: dropdown-appear 0.15s ease-out;
}

@keyframes dropdown-appear {
    from {
        opacity: 0;
        transform: translateY(-4px);
    }

    to {
        opacity: 1;
        transform: translateY(0);
    }
}

.user-info {
    padding: 8px 16px;
}

.user-email {
    font-size: 14px;
    color: #24292f;
    font-weight: 600;
    word-break: break-word;
}

.dropdown-divider {
    height: 1px;
    background: #d1d9e0;
    margin: 8px 0;
}

.dropdown-item {
    width: 100%;
    background: none;
    border: none;
    padding: 8px 16px;
    text-align: left;
    cursor: pointer;
    font-size: 14px;
    color: #24292f;
    display: flex;
    align-items: center;
    gap: 8px;
    transition: background-color 0.15s ease;
}

.dropdown-item:hover {
    background-color: #f6f8fa;
}

.dropdown-item:first-of-type {
    border-radius: 12px 12px 0 0;
}

.dropdown-item:last-of-type {
    border-radius: 0 0 12px 12px;
}

.logout-item {
    color: #d1242f;
}

.logout-item:hover {
    background-color: #ffebee;
    color: #d1242f;
}

@media (max-width: 768px) {
    .header-container {
        padding: 0 12px;
        min-height: 56px;
    }

    .header-left {
        gap: 12px;
    }

    .logo-btn img {
        height: 28px;
    }

    .breadcrumbs {
        font-size: 13px;
    }

    .user-dropdown {
        min-width: 180px;
        right: -12px;
    }
}

@media (max-width: 480px) {
    .breadcrumbs {
        display: none;
    }
}
</style>