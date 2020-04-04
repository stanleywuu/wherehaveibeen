<template>
  <div id="app">
    <NavigationBar
      :userLoggedIn="this.userLoggedIn"
      :dangerMapDisplayed="this.showDangerMap"
      @displayCheckIn="displayCheckinForm()"
      @displayHistory="displayHistory()"
      @toggleDangerMap="toggleDangerMap()"
      @displaySettings="displaySettings()">
    </NavigationBar>
    <Welcome v-show="!this.userLoggedIn && !this.showDangerMap" />
    <b-card no-body v-show="!this.userLoggedIn && !this.showDangerMap" id='user-auth-actons'>
      <b-tabs card>
        <b-tab title='Login' id='login-form-card' ref='loginTab'>
          <LoginForm />
        </b-tab>
        <b-tab title='Register' id='register-form-card'>
          <RegisterForm @registrationComplete="registrationComplete()" />
        </b-tab>
      </b-tabs>
    </b-card>
    <Confidentiality v-show="!this.userLoggedIn && !this.showDangerMap" />
    <CheckIn v-show="this.userLoggedIn && this.showCheckin" class='authed-user' />
    <CheckInHistory v-show="this.userLoggedIn && this.showHistory" class='authed-user' />
    <RiskyHistory v-show="this.showDangerMap" class='authed-user' />
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
import RiskyHistory from '@/components/RiskyHistory.vue'

export default {
  name: 'App',
  components: {
    NavigationBar,
    LoginForm,
    RegisterForm,
    CheckIn,
    CheckInHistory,
    RiskyHistory,
    Welcome,
    Confidentiality,
    Settings
  },
  data () {
    return {
      showCheckin: true,
      showHistory: false,
      showSettings: false,
      showDangerMap: false
    }
  },
  computed: {
    userLoggedIn () {
      return this.$store.getters.getUserAuth
    }
  },
  watch: {
    userLoggedIn() {
      this.hideDangerMapOnAuth()
    }
  },
  methods: {
    hideDangerMapOnAuth () {
      this.showDangerMap = false
      this.displayCheckinForm()
    },
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
    toggleDangerMap () {
      this.clearComponents()
      this.showDangerMap = !this.showDangerMap
      this.showCheckin = !this.showDangerMap
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
/* small */
@media (max-width: 768px) {
  #user-auth-actons {
    width: 90%;
    margin: auto;
  }
  .authed-user {
    width: 90%;
    margin: auto;
  }
}
/* medium */
@media (min-width: 768px) {
  #user-auth-actons {
    width: 50%;
    margin: auto;
  }
  .authed-user {
    width: 75%;
    margin: auto;
  }
}
</style>
