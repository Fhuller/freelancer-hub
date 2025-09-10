<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue';
import { useAuthStore } from '../stores/auth';
import { useI18n } from 'vue-i18n';
import { useRouter } from 'vue-router';

const authStore = useAuthStore();
const { locale } = useI18n();
const router = useRouter();

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

onMounted(() => {
    document.addEventListener('click', closeUserMenu);
});

onUnmounted(() => {
    document.removeEventListener('click', closeUserMenu);
});
</script>

<template>
<div class="user-menu-container" ref="userMenuRef">
    <button @click="toggleUserMenu" class="user-avatar-btn" :class="{ 'active': userMenuOpen }" aria-label="User menu">
        <i class="fa-solid fa-user"></i>
        <i class="fa-solid fa-caret-down dropdown-icon"></i>
    </button>

    <div v-if="userMenuOpen" class="user-dropdown">
        <div class="user-info">
            <div class="user-email">{{ authStore.session?.user.email }}</div>
        </div>

        <div class="dropdown-language">
            <button @click="changeLanguage('pt')" :class="{ 'active': locale === 'pt' }" class="dropdown-item">
                <span class="fi fi-br"></span> Português
            </button>
            <button @click="changeLanguage('en')" :class="{ 'active': locale === 'en' }" class="dropdown-item">
                <span class="fi fi-us"></span> English
            </button>
        </div>

        <div class="dropdown-divider"></div>

        <button @click="handleLogout" class="dropdown-item logout-item">
            <i class="fa-solid fa-right-from-bracket"></i> Sair
        </button>
    </div>
</div>
</template>

<style scoped>
.user-menu-container { position: relative; }
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
.dropdown-icon { transition: transform 0.15s ease; }
.user-avatar-btn.active .dropdown-icon { transform: rotate(180deg); }
.user-dropdown {
    position: absolute;
    top: 100%;
    right: 0;
    margin-top: 8px;
    background: white;
    border: 1px solid #d1d9e0;
    border-radius: 12px;
    box-shadow: var(--shadow-default);
    min-width: 200px;
    z-index: 100;
    animation: dropdown-appear 0.15s ease-out;
}
@keyframes dropdown-appear {
    from { opacity: 0; transform: translateY(-4px); }
    to { opacity: 1; transform: translateY(0); }
}
.user-info { padding: 8px 16px; }
.user-email { font-size: 14px; color: #24292f; font-weight: 600; word-break: break-word; }
.dropdown-divider { height: 1px; background: #d1d9e0; margin: 8px 0; }
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
.dropdown-item:hover { background-color: #f6f8fa; }
.dropdown-item:first-of-type { border-radius: 12px 12px 0 0; }
.dropdown-item:last-of-type { border-radius: 0 0 12px 12px; }
.logout-item { color: #d1242f; }
.logout-item:hover { background-color: #ffebee; color: #d1242f; }
.dropdown-language { display: flex; flex-direction: column; margin-top: 8px; }
.dropdown-language .dropdown-item.active { font-weight: bold; }
</style>