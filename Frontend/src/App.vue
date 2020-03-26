<template>
  <div id="app">
    <b-card v-show="!this.userLoggedIn" id='login-form-card' title="Login">
      <LoginForm></LoginForm>
    </b-card>
    <div v-show="this.userLoggedIn" id='authed-user'>
      <UserOps></UserOps>
      <CheckIn></CheckIn>
    </div>
  </div>
</template>

<script>
import LoginForm from '@/components/LoginForm.vue'
import UserOps from '@/components/UserOps.vue'
import CheckIn from '@/components/CheckIn.vue'

export default {
  name: 'App',
  components: {
    LoginForm,
    UserOps,
    CheckIn
  },
  computed: {
    userLoggedIn () {
      return this.$store.getters.getUserAuth
    }
  }

}
</script>

<style>
#app {
  font-family: Avenir, Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-align: center;
  color: #2c3e50;
  margin-top: 60px;
}

#login-form-card {
  width: 50%;
  margin: auto;
}
#authed-user {
  width: 75%;
  margin: auto;
}
</style>
