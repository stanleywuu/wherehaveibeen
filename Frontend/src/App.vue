<template>
  <div id="app">
    <NavigationBar
      :userLoggedIn="this.userLoggedIn"
      @displayCheckIn="displayCheckinForm()"
      @displayHistory="displayHistory()"
      @displayRiskHistory="displayRiskHistory()"
      @displaySettings="displaySettings()">
    </NavigationBar>
    <Welcome v-show="!this.userLoggedIn && !this.anonymous" />
    <b-card no-body v-show="!this.userLoggedIn && !this.anonymous" id='user-auth-actons'>
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
    <RiskyHistory v-show="this.showRiskyHistory" />
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
  /* I'm going to comment this out so linter doesn't complain
  created()
  {
      var token = this.$cookie.get('token');
      var userId = this.$cookie.get("userId");
      
      if (token && userId)
      {
        // DOn't really know how this works
        const tokenState = () => token;
        const userIdState = () => userId;
        
        // I can't figure out how to set values in store
        // this.$store.dispatch('token', tokenState)
        // this.$store.dispatch('userId', userIdState)
      }
  },*/
  data () {
    return {
      showCheckin: true,
      showHistory: false,
      showSettings: false,
      anonymous: false
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
      this.showRiskyHistory = false
    },
    displayCheckinForm () {
      this.clearComponents()
      this.showCheckin = true
    },
    displayHistory () {
      this.clearComponents()
      this.showHistory = true
    },
    displayRiskHistory()
    {
        this.clearComponents()
        this.showRiskyHistory = true
        this.anonymous = !this.anonymous
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
