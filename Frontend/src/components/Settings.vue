<template>
  <div id='settings-container'>
    <h2>Account Settings</h2>
    <b-card class="account-setting-card" title="Clear your history">
      <b-card-body>
        This section will remove all the check ins from your account.<br />
        We don't like to see this used as we rely on check in data to power Where Have I Been?
        <p />
        But if you still want to, click away
        <p />
        <b-button variant="outline-dark" @click="onHistoryDelete()">Clear My History</b-button>
      </b-card-body>
    </b-card>
    <b-card class="account-setting-card" title="Delete your account">
      <b-card-body>
        This section will clear all the information about you and your check ins and remove your account.<br />
        We'd love to keep you on board and help keep you safe, which will require an account.
        <p />
        But if you still want to, click away - there's nothing stopping you making a new one!
        <p />
        <b-button disabled variant="outline-danger" @click="onAccountDelete()">Delete My Account</b-button>
      </b-card-body>
    </b-card>
  </div>
</template>

<script>
export default {
  name: 'Settings',
  data () {
    return {
      api: {
        endpoint: process.env.VUE_APP_API_URL,
        responses: [],
        errors: []
      }
    }
  },
  methods: {
    onHistoryDelete () {
      this.$bvModal.msgBoxConfirm('Are you sure you want to delete your history?', {
          title: 'Clear Your History',
          okVariant: 'dark',
          okTitle: 'Yes',
          cancelVariant: 'outline-dark',
          cancelTitle: 'No',
          hideHeaderClose: false,
          centered: true
      })
      .then(confirmed => {
        if (confirmed) {
          let requestData = {}
          let userId = this.$store.getters.getUserId
          let userToken = this.$store.getters.getUserToken
          this.$http.post(
            this.api.endpoint + '/visit/delete/' + userId,
            requestData,
            {
              headers: { 
                Authorization: "Bearer " + userToken
              }
            }
          )
          .then(response => {
            this.api.reponses.push(response)
          })
          .catch(e => {
            this.api.errors.push(e)
          })
        }
      })
      .catch(err => {
        this.api.errors.push(err)
      })
    },
    onAccountDelete () {
      this.$bvModal.msgBoxConfirm('Are you sure you want to delete your account?', {
          title: 'Delete Your Account',
          okVariant: 'danger',
          okTitle: 'Yes',
          cancelVariant: 'outline-danger',
          cancelTitle: 'No',
          hideHeaderClose: false,
          centered: true
      })
      .then(confirmed => {
        if (confirmed) {
          let requestData = {}
          let userId = this.$store.getters.getUserId
          let userToken = this.$store.getters.getUserToken
          this.$http.post(
            this.api.endpoint + '/membership/delete/' + userId,
            requestData,
            {
              headers: { 
                Authorization: "Bearer " + userToken
              }
            }
          )
          .then(response => {
            this.api.reponses.push(response)
          })
          .catch(e => {
            this.api.errors.push(e)
          })
        }
      })
      .catch(err => {
        this.api.errors.push(err)
      })
    }
  }
}
</script>

<style scoped>
.account-setting-card {
  margin-bottom: 10px;
}
</style>