<template>
  <div id='login-form-container'>
    <b-alert variant="danger" v-model="alerts.invalidCredentials">
      The email / password are incorrect
    </b-alert>
    <b-form @submit="onLoginSubmit">
      <b-container fluid>
        <b-row class="my-1">
          <b-col cols="12">
            <b-form-group
              id="email-input-group"
              label="Email Address:"
              label-for="email-input">
              <b-form-input
                id="email-input"
                type="email"
                required
                autocomplete="username"
                :state="fieldState.login"
                v-model="form.email">
              </b-form-input>
            </b-form-group>
          </b-col>
        </b-row>
        <b-row class="my-1">
          <b-col cols="12">
            <b-form-group
              id="password-input-group"
              label="Password:"
              label-for="password-input">
              <b-form-input
                id="password-input"
                type="password"
                required
                autocomplete="current-password"
                :state="fieldState.login"
                v-model="form.password">
              </b-form-input>
            </b-form-group>
          </b-col>
        </b-row>
      </b-container>
      <b-button id="login-form-submit-btn" class="login-form-btn" variant="primary" type="submit">Login</b-button>
    </b-form>
  </div>
</template>

<script>
export default {
  name: 'LoginForm',
  data () {
    return {
      api: {
        endpoint: process.env.VUE_APP_API_URL,
        errors: []
      },
      alerts: {
        invalidCredentials: false
      },
      fieldState: {
        login: null
      },
      form: {
        email: '',
        password: ''
      }
    }
  },
  methods: {
    clearFields () {
      this.form.email = '',
      this.form.password = ''
    },
    clearAlerts () {
      this.alerts.invalidCredentials = false
      this.fieldState.login = null
    },
    onLoginSubmit (evt) {
      evt.preventDefault()
      this.clearAlerts()
      let requestData = {
        Username: this.form.email,
        Password: this.form.password
      }
      this.$http.post(this.api.endpoint + '/membership/login', requestData)
      .then(response => {
        this.clearFields()
        this.clearAlerts()
        this.$store.dispatch('storeUserAuth', response.data.token)
        this.$store.dispatch('storeUserId', response.data.userId)
      })
      .catch(e => {
        this.alerts.invalidCredentials = true
        this.fieldState.login = false
        this.api.errors.push(e)
      })
    }
  }
}
</script>

<style scoped>
.form-group {
  text-align: left !important;
}
</style>

