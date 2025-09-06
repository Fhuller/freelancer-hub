<script setup lang="ts">
import AuthenticatedLayout from '../layouts/AuthenticatedLayout.vue';
import UnderConstruction from '../components/UnderConstruction.vue';
</script>

<template>
  <AuthenticatedLayout> 
    <div class="content-section">
      <UnderConstruction />
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
</style>