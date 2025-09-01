    <script>
    import Header from '@/components/LPHeader.vue'
    import Footer from '@/components/LPFooter.vue'

    export default {
        name: 'Layout',
        components: { Header, Footer }
    }
</script>

<template>
    <div class="min-h-screen flex flex-col bg-gradient-to-br from-purple-900 via-indigo-900 to-purple-800">
        <Header />
            <main class="flex-1 pt-16">
                <slot />
            </main>
        <Footer />
    </div>
</template>