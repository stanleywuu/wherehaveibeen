<template>
  <div id='register-form-container'>
    <b-form @submit="onRegisterSubmit">
      <b-container fluid>
        <b-row class="my-1">
          <b-col cols="6">
            <b-form-group
              id="fname-input-group"
              label="First Name:"
              label-for="fname-input">
              <b-form-input
                id="fname-input"
                type="text"
                required
                autocomplete="given-name"
                v-model="form.fname">
              </b-form-input>
            </b-form-group>
          </b-col>
          <b-col cols="6">
            <b-form-group
              id="lname-input-group"
              label="Last Name:"
              label-for="lname-input">
              <b-form-input
                id="lname-input"
                type="text"
                required
                autocomplete="family-name"
                v-model="form.lname">
              </b-form-input>
            </b-form-group>
          </b-col>
        </b-row>
        <b-row class="my-1">
          <b-col cols="6">
            <b-form-group
              id="new-email-input-group"
              label="Email Address:"
              label-for="new-email-input">
              <b-form-input
                id="new-email-input"
                type="email"
                required
                autocomplete="username"
                v-model="form.email">
              </b-form-input>
            </b-form-group>
          </b-col>
          <b-col cols="6">
            <b-form-group
              id="email-validate-input-group"
              label="Verify Email Address:"
              label-for="email-validate-input">
              <b-form-input
                id="email-validate-input"
                type="email"
                required
                v-model="form.emailVerify">
              </b-form-input>
            </b-form-group>
          </b-col>
        </b-row>
        <b-row class="my-1">
          <b-col cols="6">
            <b-form-group
              id="new-password-input-group"
              label="Password:"
              label-for="new-password-input">
              <b-form-input
                id="new-password-input"
                type="password"
                required
                autocomplete="new-password"
                v-model="form.password">
              </b-form-input>
            </b-form-group>
          </b-col>
          <b-col cols="6">
            <b-form-group
              id="password-validate-input-group"
              label="Verify Password:"
              label-for="password-validate-input">
              <b-form-input
                id="password-validate-input"
                type="password"
                required
                v-model="form.passwordVerify">
              </b-form-input>
            </b-form-group>
          </b-col>
        </b-row>
      </b-container>
      <b-button id="register-form-submit-btn" class="register-form-btn" variant="primary" type="submit">Register</b-button>
    </b-form>
  </div>
</template>

<script>
export default {
  name: 'RegisterForm',
  data () {
    return {
      apiEndpoint: 'https://wherehaveibeen.azurewebsites.net',
      form: {
        fname: '',
        lname: '',
        email: '',
        emailVerify: '',
        password: '',
        passwordVerify: ''
      }
    }
  },
  methods: {
    onRegisterSubmit (evt) {
      evt.preventDefault()
      let registerError = false
      if (this.form.email !== this.form.emailVerify) {
        registerError = true
        alert('Email addresses do not match!')
      }
      if (this.form.password !== this.form.passwordVerify) {
        registerError = true
        alert('Passwords do not match!')
      }
      if (!registerError) {
        this.$http.post(this.apiEndpoint + '/membership', {
          Username: [this.form.fname, this.form.lname].join(' '),
          Email: this.form.email,
          Password: this.form.password
        })
        .then(response => {
          console.log('Successfully Registered')
          console.log(response)
          this.$emit('registrationComplete')
        })
        .catch(e => {
          console.log('Registration Failed!')
          console.log(e)
        })
      }
    }
  }
}
</script>

<style scoped>
.form-group {
  text-align: left !important;
}
</style>
