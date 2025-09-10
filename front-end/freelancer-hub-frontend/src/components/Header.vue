<script setup lang="ts">
import { useRouter } from 'vue-router';
import Breadcrumbs from '@/components/BreadCrumbs.vue';
import UserDropdown from '@/components/UserDropdown.vue';

const router = useRouter();
const emit = defineEmits(['toggle-sidebar']);

const goToHome = () => {
    router.push({ name: 'Dashboard' });
};
</script>

<template>
<header class="github-header">
    <div class="header-container">
        <div class="header-left">
            <button @click="emit('toggle-sidebar')" class="hamburger-btn" aria-label="Toggle sidebar">
                <i class="fa-solid fa-bars"></i>
            </button>

            <button @click="goToHome" class="logo-btn">
                <img alt="Freelancer Hub" src="@/assets/logo_grd.png" width="120" height="auto" />
            </button>

            <Breadcrumbs />
        </div>

        <div class="header-right">
            <UserDropdown />
        </div>
    </div>
</header>
</template>

<style scoped>
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

.header-right {
    display: flex;
    align-items: center;
}

@media (max-width: 768px) {
    .header-container { padding: 0 12px; min-height: 56px; }
    .header-left { gap: 12px; }
    .logo-btn img { height: 28px; }
}

@media (max-width: 480px) {
    .breadcrumbs { display: none; }
}
</style>
