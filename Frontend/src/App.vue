<template>
  <div id="app">
    <b-card no-body v-show="!this.userLoggedIn" id='user-auth-actons'>
      <b-tabs card>
        <b-tab title='Login' id='login-form-card'>
          <LoginForm></LoginForm>
        </b-tab>
        <b-tab title='Register' id='register-form-card'>
          <h2>This is the registration form</h2>
        </b-tab>
      </b-tabs>
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

#user-auth-actons {
  width: 50%;
  margin: auto;
}
#authed-user {
  width: 75%;
  margin: auto;
}
</style>
