<script setup lang="ts">
// não precisa de lógica, é puramente visual
</script>

<template>
  <div class="under-construction">
    <div class="icon">⚠</div>
    <h2>Módulo em Desenvolvimento</h2>
    <p>Esta funcionalidade ainda está sendo implementada.</p>
  </div>
</template>

<style scoped>
.under-construction {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  background-color: #f1f3f5;
  border: 1px solid #dee2e6;
  border-radius: 8px;
  padding: 40px 20px;
  text-align: center;
  color: #495057;
  grid-column: 1/-1;
  width: 100%;
}

.under-construction .icon {
  font-size: 48px;
  margin-bottom: 12px;
}

.under-construction h2 {
  margin: 0 0 8px;
  font-weight: 600;
  color: #343a40;
}

.under-construction p {
  margin: 0;
  font-size: 14px;
  color: #6c757d;
}
</style>
