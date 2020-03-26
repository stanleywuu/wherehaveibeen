<template>
  <div id='user-ops-container'>
    <b-button variant="danger" class="ml-2 mr-2">I Have COVID-19</b-button>
    <b-button variant="outline-primary" class="ml-2 mr-2" @click="onUserLogout">Log Out</b-button>
  </div>
</template>

<script>
export default {
  name: 'UserOps',
  methods: {
    onUserLogout (evt) {
      evt.preventDefault()
      this.$store.dispatch('storeUserAuth', false)
    }
  }
}
</script>