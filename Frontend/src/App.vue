<template>
  <div id="app">
    <NavigationBar
      :userLoggedIn="this.userLoggedIn"
      @displayCheckIn="displayCheckinForm()"
      @displayHistory="displayHistory()">
    </NavigationBar>
    <b-card no-body v-show="!this.userLoggedIn" id='user-auth-actons'>
      <b-tabs card>
        <b-tab title='Login' id='login-form-card' ref='loginTab'>
          <LoginForm></LoginForm>
        </b-tab>
        <b-tab title='Register' id='register-form-card'>
          <RegisterForm
           @registrationComplete="registrationComplete()">
          </RegisterForm>
        </b-tab>
      </b-tabs>
    </b-card>
    <div v-show="this.userLoggedIn && this.showCheckin" id='authed-user'>
      <CheckIn></CheckIn>
    </div>
    <div v-show="this.userLoggedIn && this.showHistory" id='authed-user'>
      <CheckInHistory></CheckInHistory>
    </div>
  </div>
</template>

<script>
import NavigationBar from '@/components/NavigationBar.vue'
import LoginForm from '@/components/LoginForm.vue'
import RegisterForm from '@/components/RegisterForm.vue'
import CheckIn from '@/components/CheckIn.vue'
import CheckInHistory from '@/components/CheckInHistory.vue'

export default {
  name: 'App',
  components: {
    NavigationBar,
    LoginForm,
    RegisterForm,
    CheckIn,
    CheckInHistory
  },
  data () {
    return {
      showCheckin: true,
      showHistory: false
    }
  },
  computed: {
    userLoggedIn () {
      return this.$store.getters.getUserAuth
    }
  },
  methods: {
    registrationComplete () {
      console.log('activating logintab')
      this.$refs.loginTab.activate()
    },
    displayCheckinForm () {
      this.showHistory = false
      this.showCheckin = true
    },
    displayHistory () {
      this.showCheckin = false
      this.showHistory = true
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
