<template>
  <div id="checkin-history-container">
    <div id="title-bar" class="d-flex mb-4">
      <div id="title-text" class="d-flex justify-content-center">
        <h2>Check-In History</h2>
      </div>
      <div id="title-text" class="ml-auto d-flex justify-content-end">
        <b-button @click="loadHistory()" id="refresh-button">Refresh</b-button>
      </div>
    </div>
    <b-card
      v-for="loc in this.locations"
      :title="loc.Address"
      :key="loc.VisitId"
      class="location-card overflow-hidden mb-2"
      no-body>
      <b-row no-gutters>
        <b-col md="6">
          <gmap-map
            :center="findCenter(loc)"
            :zoom="zoomLevel"
              :options="mapOptions"
            style="width:100%; height: 200px;">
            <gmap-marker
              :position="findCenter(loc)"
            ></gmap-marker>
          </gmap-map>
        </b-col>
        <b-col md="6">
          <b-card-body>
            <b-row no-gutters>
              <b-col class="history-text">
                <h4>{{ loc.PlaceName }}</h4>
              </b-col>
              <b-col sm="3" class="risk-label">
                <span v-if="loc.AtRisk" class="at-risk">
                  <font-awesome-icon icon="biohazard" /> Infectious
                </span>
                <span class="risky" v-if="potentialRisk(loc)">
                  <font-awesome-icon icon="virus" /> Potential Risk
                </span>
                <span v-if="!loc.AtRisk && loc.RiskyInteractions === 0">
                  <font-awesome-icon icon="hand-sparkles" /> Low Risk
                </span>
              </b-col>
            </b-row>
            <div class="history-text">{{ loc.Address }}</div>
            <div class="history-text">{{ dateFormatter(loc.CheckIn) }}</div>
            <div v-if="potentialRisk(loc)" class="d-flex flex-column align-content-end">
              <div class="d-flex flex-row-reverse">
                <b-button variant="link" id='covid-interaction-btn' class="ml-1 mr-1"
                          @click="toggleShowRiskyEncounters(loc)">
                  {{ loc.showRiskyEncounters ? 'Hide' : 'Show' }} {{ loc.RiskyInteractions }} risky encounter{{ loc.RiskyInteractions > 1 ? 's' : ''}}
                </b-button>
              </div>
            </div>
            <div class="risky-text" v-if="loc.showRiskyEncounters">
              <span v-for="detail in loc.details"
                    :key="detail.VisitId">
                <div class="label">Someone with symptoms was here at:</div>
                <div class="text"><div class="label">Checked in:</div>{{ dateFormatter(detail.checkIn) }}</div>
                <div class="text"><div class="label">Checked out:</div>{{ dateFormatter(detail.checkOut) }}</div>
                <div class="text">{{ formatDistance(detail) }}</div>
              </span>
            </div>
          </b-card-body>
        </b-col>
      </b-row>
    </b-card>
  </div>
</template>

<script>
export default {
  name: 'CheckInHistory',
  data () {
    return {
      api: {
        endpoint: process.env.VUE_APP_API_URL,
        errors: []
      },
      mapOptions: {
        mapTypeControl: false,
        streetViewControl: false,
        fullscreenControl: false,
        zoomControl: false
      },
      locations: [],
      zoomLevel: 14
    }
  },
  computed: {
    currentUserId () {
      return this.$store.getters.getUserId
    }
  },
  watch: {
    currentUserId () {
      this.loadHistory()
    }
  },
  mounted () {
    this.loadHistory()
  },
  methods: {
    loadHistory () {
      let userId = this.currentUserId
      if (userId !== undefined) {
        this.$http.get(this.api.endpoint + '/visit?userId=' + userId)
        .then(response => {
          this.locations = response.data.reverse()
        })
        .catch(e => {
          this.api.errors.push(e)
        })
      }
    },
    findCenter (location) {
      return {
        lat: location.Latitude,
        lng: location.Longitude
      }
    },
    dateFormatter (datetime) {
      return new Date(Date.parse(datetime))
    },
    getDetails (loc) {
      if (loc.data && loc.data.length > 0) {
        return //already have data
      }
      this.$http.get(this.api.endpoint + '/visit/risk?visitId=' + loc.VisitId)
        .then(response => {
          if (response.data && response.data.length > 0) {
            this.$set(loc, 'details', response.data)
          }
        })
        .catch(e => {
          this.api.errors.push(e)
        })
    },
    toggleShowRiskyEncounters (loc) {
      const isShown = !loc.showRiskyEncounters
      this.$set(loc, 'showRiskyEncounters', isShown)
      if (!isShown) return
      this.getDetails(loc)
    },
    potentialRisk (location) {
      return !location.AtRisk && location.RiskyInteractions > 0
    },
    formatDistance (detail) {
      return detail.distanceInKm > 0 ? `Approximately ${detail.distanceInKm} metres away` : '';
    }
  }
}
</script>

<style scoped>
  .history-text {
    text-align: left;
  }
  .risk-label {
    display: flex;
  }
  .risky-text .text
  {
    text-align:left;
    font-size: smaller;
    font-weight: lighter;
    margin-top:2pt;
    margin-bottom:2px;
  }
  .risky-text .label {
    text-align: left;
    font-size: smaller;
  }
  .risky-text .text .label {
    display: inline-block;
    text-align: right;
    min-width: 80px;
    margin-right: 8px;
    font-weight: bold;
  }
  .at-risk {
    color: red;
    font-weight: bold;
  }
  .risky {
    color:teal;
    font-weight: bold;
  }
</style>
