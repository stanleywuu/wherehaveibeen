<template>
  <div id='register-form-container'>
    <b-alert variant="danger" v-model="validationAlerts.emailNotMatch">
      The emails do not match
    </b-alert>
    <b-alert variant="danger" v-model="validationAlerts.emailNotUnique">
      This email is already registered
    </b-alert>
    <b-alert variant="danger" v-model="validationAlerts.passwordNotMatch">
      The passwords do not match
    </b-alert>
    <b-alert variant="danger" v-model="validationAlerts.policyNotChecked">
      You must agree to the Privacy Policy
    </b-alert>
    <b-form @submit="onRegisterSubmit">
      <b-container fluid>
        <b-row>
          <b-col col cols="12" sm="12" md="12" lg="6" xl="6">
            <b-form-group
              id="new-email-input-group"
              label="Email Address:"
              label-for="new-email-input">
              <b-form-input
                id="new-email-input"
                type="email"
                required
                autocomplete="username"
                :state="fieldStatus.email"
                v-model="form.email">
              </b-form-input>
            </b-form-group>
          </b-col>
          <b-col col cols="12" sm="12" md="12" lg="6" xl="6">
            <b-form-group
              id="email-validate-input-group"
              label="Verify Email Address:"
              label-for="email-validate-input">
              <b-form-input
                id="email-validate-input"
                type="email"
                required
                :state="fieldStatus.email"
                v-model="form.emailVerify">
              </b-form-input>
            </b-form-group>
          </b-col>
        </b-row>
        <b-row>
          <b-col col cols="12" sm="12" md="12" lg="6" xl="6">
            <b-form-group
              id="new-password-input-group"
              label="Password:"
              label-for="new-password-input">
              <b-form-input
                id="new-password-input"
                type="password"
                required
                autocomplete="new-password"
                :state="fieldStatus.password"
                v-model="form.password">
              </b-form-input>
            </b-form-group>
          </b-col>
          <b-col col cols="12" sm="12" md="12" lg="6" xl="6">
            <b-form-group
              id="password-validate-input-group"
              label="Verify Password:"
              label-for="password-validate-input">
              <b-form-input
                id="password-validate-input"
                type="password"
                required
                :state="fieldStatus.password"
                v-model="form.passwordVerify">
              </b-form-input>
            </b-form-group>
          </b-col>
        </b-row>
        <b-row>
          <b-col cols="12">
            <b-form-checkbox
              id="priv-pol-check"
              v-model="form.privacyPolicy"
              name="priv-pol-check"
              value=true
              unchecked-value=false
              :state="fieldStatus.policy">
              I accept the <a href="/privacyPolicy.html" target="_blank">Privacy Policy</a>
            </b-form-checkbox>
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
      api: {
        endpoint: process.env.VUE_APP_API_URL,
        responses: [],
        errors: []
      },
      validationAlerts: {
        emailNotMatch: false,
        emailNotUnique: false,
        passwordNotMatch: false,
        policyNotChecked: false,
      },
      fieldStatus:{
        email: null,
        password: null,
        policy: null
      },
      form: {
        email: '',
        emailVerify: '',
        password: '',
        passwordVerify: '',
        privacyPolicy: false
      }
    }
  },
  methods: {
    /* I am NOT a fan of these */
    clearFields () {
      this.form.email = ''
      this.form.emailVerify = ''
      this.form.password = ''
      this.form.passwordVerify = ''
      this.form.privacyPolicy = false
    },
    clearAlerts () {
      this.validationAlerts.emailNotMatch = false
      this.validationAlerts.emailNotUnique = false
      this.validationAlerts.passwordNotMatch = false
      this.validationAlerts.policyNotChecked = false
      this.fieldStatus.email = null
      this.fieldStatus.password = null
      this.fieldStatus.policy = null
    },
    onRegisterSubmit (evt) {
      evt.preventDefault()
      this.clearAlerts()
      if (this.form.email !== this.form.emailVerify) {
        this.clearAlerts()
        this.fieldStatus.email = false
        this.validationAlerts.emailNotMatch = true
      }
      else if (this.form.password !== this.form.passwordVerify) {
        this.fieldStatus.password = false
        this.validationAlerts.passwordNotMatch = true
      }
      else if (! this.form.privacyPolicy || this.form.privacyPolicy === 'false') {
        this.fieldStatus.policy = false
        this.validationAlerts.policyNotChecked = true
      }
      else {
        let requestData = {
          Username: this.form.email,
          Email: this.form.email,
          Password: this.form.password
        }
        this.$http.post(this.api.endpoint + '/membership', requestData)
        .then(response => {
          this.api.responses.push(response)
          this.clearFields()
          this.clearAlerts()
          this.$emit('registrationComplete')
        })
        .catch(e => {
          this.fieldStatus.email = false
          this.validationAlerts.emailNotUnique = true
          this.api.errors.push(e)
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
