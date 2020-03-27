<template>
  <div id="nav-bar-container" v-show="userLoggedIn" class="mb-5">
    <b-navbar toggleable="lg" type="dark" variant="info">
      <b-navbar-nav class="ml-auto">
        <UserOps></UserOps>
      </b-navbar-nav>
    </b-navbar>
  </div>
</template>

<script>
import UserOps from '@/components/UserOps.vue'

export default {
  name: 'NavigationBar',
  components: {
    UserOps
  },
  props: ['userLoggedIn']
}
</script>