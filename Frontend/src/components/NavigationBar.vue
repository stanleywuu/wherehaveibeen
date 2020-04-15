<template>
  <div id="nav-bar-container" class="mb-5">
    <b-navbar toggleable="md" type="dark" variant="dark">
      <b-navbar-brand id="nav-branding" href="#" @click="$emit('displayCheckIn')">
        <img src="@/assets/logo.png" id="site-logo" />
        Where Have I Been?
      </b-navbar-brand>
      <b-navbar-toggle target="nav-app-menu">
        <font-awesome-icon icon="bars" />
      </b-navbar-toggle>
      <b-collapse is-nav id="nav-app-menu">
        <b-navbar-nav class="ml-auto">
          <b-button id='covid-history-btn' class="ml-2 mr-2"
            @click="$emit('toggleDangerMap')">
            {{ dangerMapDisplayed ? 'Hide' : 'Show' }} Report Map
          </b-button>
          <b-button v-if="userLoggedIn && !userReportStatus"
            id='covid-report-btn' variant="danger" class="ml-2 mr-2"
            @click="$bvModal.show('covid-modal')">
            I Have Symptoms
          </b-button>
          <b-dropdown v-if="userLoggedIn && userReportStatus" split
            text="I Have Recovered" @click="onReportRecovered()"
            variant="info" class="ml-2 mr-2">
            <b-dropdown-item @click="onReportNotCorona()">I didn't have COVID</b-dropdown-item>
          </b-dropdown>
          <b-nav-item-dropdown text="Account" right v-show="userLoggedIn">
            <b-dropdown-item @click="$emit('displayHistory')">History</b-dropdown-item>
            <b-dropdown-item @click="$emit('displaySettings')">Settings</b-dropdown-item>
            <b-dropdown-item @click="onUserLogout">Log Out</b-dropdown-item>
          </b-nav-item-dropdown>
        </b-navbar-nav>
      </b-collapse>
    </b-navbar>
    <b-modal id="covid-modal" ref="covid-modal" hide-header hide-footer>
      <CovidModal
        @confirmReport="onReportCorona()"
        @cancelReport="onReportCancel()">
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
  props: [
    'userLoggedIn',
    'userReportStatus',
    'dangerMapDisplayed'
  ],
  data () {
    return {
      expanded: false,
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
    onReportCancel () {
      this.$refs['covid-modal'].hide()
    },
    onReportCorona () {
      this.$refs['covid-modal'].hide()
      this.$store.dispatch('storeUserReportStatus', true)
      this.onReportSubmit('corona', true)
    },
    onReportRecovered () {
      this.$store.dispatch('storeUserReportStatus', false)
      this.onReportSubmit('recovered', false)
    },
    onReportNotCorona () {
      this.$store.dispatch('storeUserReportStatus', false)
      let apiPath = 'notcorona?userId=' + this.$store.getters.getUserId
      this.onReportSubmit(apiPath, false)
    },
    onReportSubmit (endpoint, userAtRisk) {
      let requestData = {
        userId: this.$store.getters.getUserId,
        IsAtRisk: userAtRisk
      }
      this.$http.post(this.api.endpoint + '/membership/' + endpoint,
        requestData,
        {headers: { Authorization: "Bearer " + this.$store.getters.getUserToken }}
      )
      .then(response => {
        this.api.responses.push(response)
      })
      .catch(e => {
        this.api.errors.push(e)
      })
    }
  }
}
</script>

<style scoped>
#site-logo {
  width: 32px;
  height: 32px;
}
#nav-branding {
  vertical-align: center;
}
#covid-report-btn {
  color: #343a40 !important;
}
</style>
