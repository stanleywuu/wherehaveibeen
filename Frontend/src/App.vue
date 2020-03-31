<template>
  <div id="app">
    <NavigationBar
      :userLoggedIn="this.userLoggedIn"
      @displayCheckIn="displayCheckinForm()"
      @displayHistory="displayHistory()"
      @displaySettings="displaySettings()">
    </NavigationBar>
    <Welcome v-show="!this.userLoggedIn" />
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
    <Confidentiality v-show="!this.userLoggedIn" />
    <CheckIn v-show="this.userLoggedIn && this.showCheckin" class='authed-user' />
    <CheckInHistory v-show="this.userLoggedIn && this.showHistory" class='authed-user' />
    <Settings v-show="this.userLoggedIn && this.showSettings" class='authed-user' />
  </div>
</template>

<script>
import NavigationBar from '@/components/NavigationBar.vue'
import LoginForm from '@/components/LoginForm.vue'
import RegisterForm from '@/components/RegisterForm.vue'
import CheckIn from '@/components/CheckIn.vue'
import CheckInHistory from '@/components/CheckInHistory.vue'
import Welcome from '@/components/Welcome.vue'
import Confidentiality from '@/components/Confidentiality.vue'
import Settings from '@/components/Settings.vue'

export default {
  name: 'App',
  components: {
    NavigationBar,
    LoginForm,
    RegisterForm,
    CheckIn,
    CheckInHistory,
    Welcome,
    Confidentiality,
    Settings
  },
  data () {
    return {
      showCheckin: true,
      showHistory: false,
      showSettings: false
    }
  },
  computed: {
    userLoggedIn () {
      return this.$store.getters.getUserAuth
    }
  },
  methods: {
    registrationComplete () {
      this.$refs.loginTab.activate()
    },
    clearComponents () {
      this.showCheckin = false
      this.showHistory = false
      this.showSettings = false
    },
    displayCheckinForm () {
      this.clearComponents()
      this.showCheckin = true
    },
    displayHistory () {
      this.clearComponents()
      this.showHistory = true
    },
    displaySettings () {
      this.clearComponents()
      this.showSettings = true
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
.authed-user {
  width: 75%;
  margin: auto;
}
</style>
