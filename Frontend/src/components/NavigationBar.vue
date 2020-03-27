<template>
  <div id="nav-bar-container" v-show="userLoggedIn" class="mb-5">
    <b-navbar toggleable type="dark" variant="dark">
      <b-navbar-brand>Where Have I Been?</b-navbar-brand>
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