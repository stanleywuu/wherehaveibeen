<template>
  <div id="nav-bar-container" class="mb-5">
    <b-navbar toggleable="lg" type="dark" variant="dark">
      <b-navbar-brand href='#' @click="$emit('displayCheckIn')">Where Have I Been?</b-navbar-brand>
      <b-navbar-nav class="ml-auto">
        <b-button id='covid-history-btn' class="ml-2 mr-2"
          @click="$emit('toggleDangerMap')">
          {{ dangerMapDisplayed ? 'Hide' : 'Show' }} Report Map
        </b-button>
        <b-button id='covid-report-btn' variant="danger" class="ml-2 mr-2"
          @click="$bvModal.show('covid-modal')"
          v-show="userLoggedIn">
          I Have Symptoms
        </b-button>
        <b-nav-item-dropdown text="Account" right v-show="userLoggedIn">
          <b-dropdown-item @click="$emit('displayHistory')">History</b-dropdown-item>
          <b-dropdown-item @click="$emit('displaySettings')">Settings</b-dropdown-item>
          <b-dropdown-item @click="onUserLogout">Log Out</b-dropdown-item>
        </b-nav-item-dropdown>
      </b-navbar-nav>
    </b-navbar>
    <b-modal id="covid-modal" ref="covid-modal" hide-header hide-footer>
      <CovidModal
        @confirmReport="confirmReport()"
        @cancelReport="cancelReport()">
      </CovidModal>
    </b-modal>
  </div>
</template>

<script>
import CovidModal from '@/components/CovidModal.vue'

export default {
  name: 'NavigationBar',
  components :{
    CovidModal
  },
  props: ['userLoggedIn', 'dangerMapDisplayed'],
  data () {
    return {
      api: {
        endpoint: process.env.VUE_APP_API_URL,
        responses: [],
        errors: []
      },
    }
  },
  methods: {
    onUserLogout (evt) {
      evt.preventDefault()
      this.$store.dispatch('storeUserAuth', undefined)
      this.$store.dispatch('storeUserId', undefined)
    },
    confirmReport() {
      this.$refs['covid-modal'].hide()
      let requestData = {
        userId: this.$store.getters.getUserId,
        IsAtRisk: true
      }
      this.$http.post(this.api.endpoint + '/membership/corona', requestData)
      .then(response => {
        this.api.responses.push(response)
      })
      .catch(e => {
        this.api.errors.push(e)
      })
    },
    cancelReport() {
      this.$refs['covid-modal'].hide()
    }
  }
}
</script>

<style scoped>
#covid-report-btn {
  color: #343a40 !important;
}
</style>
