<template>
  <div id='login-form-container'>
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
                v-model="form.password">
              </b-form-input>
            </b-form-group>
          </b-col>
        </b-row>
      </b-container>
      <b-button id="login-form-submit-btn" class="login-form-btn" variant="primary" type="submit">Login</b-button>
      <b-button id="login-form-return-btn" class="login-form-btn" variant="outline-primary" @click="$emit('cancel-login')">Back</b-button>
    </b-form>
  </div>
</template>

<script>
export default {
  name: 'LoginForm',
  data () {
    return {
      form: {
        email: '',
        password: ''
      }
    }
  },
  methods: {
    onLoginSubmit (evt) {
      evt.preventDefault()
      console.log('Logging in as ' + this.form.email)
    }
  }
}
</script>

<style scoped>
.form-group {
  text-align: left !important;
}
.login-form-btn {
  margin-top: 5px;
  margin-right: 20px;
}
</style>

