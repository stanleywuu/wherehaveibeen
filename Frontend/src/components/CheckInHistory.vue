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
                <h4>
                  {{loc.PlaceName}}
                </h4>
              </b-col>
              <b-col sm="3" class="risk-label">
                <span v-if="loc.AtRisk" class="at-risk">
                  <font-awesome-icon icon="biohazard" />
                  At Risk
                </span>
                <span class="risky" v-if="!loc.AtRisk && loc.RiskyInteractions > 0">
                  <font-awesome-icon icon="hand-sparkles" />
                  Potential Risk 
                  <b-button id='covid-interaction-btn' class="ml-1 mr-1"
                    @click="getDetails(loc)">
                    {{loc.RiskyInteractions}} risky Encounters
                  </b-button>
                  <span v-for="detail in loc.details"
                  :key="detail.VisitId"> 
                  <li>Checked in: {{detail.checkin}}</li>
                  <li>Checked out: {{detail.checkOut}}
                  <li>{{distanceInKm}} km</li>
                  </span>
                </span>
                <span v-if="!loc.AtRisk && loc.RiskyInteractions == 0">
                  <font-awesome-icon icon="hand-sparkles" />
                  Low Risk
                </span>
              </b-col>
            </b-row>
            <div class="history-text">
              {{ loc.Address }}
            </div>
            <div class="history-text">
              {{ dateFormatter(loc.CheckIn) }}
            </div>
          </b-card-body>
        </b-col>
      </b-row>
    </b-card>
  </div>
</template>

<style>
  .history-text {
    text-align: left;
  }
  .risk-label {
    display: flex;
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

<script>
export default {
  name: 'CheckInHistory',
  data () {
    return {
      api: {
        endpoint: process.env.VUE_APP_API_URL,
        errors: []
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
      let epoch = new Date(Date.parse(datetime))
      return epoch
    },
    getDetails(loc)
    {
      this.$http.get(this.api.endpoint + '/visit/risk?visitId=' + loc.VisitId)
        .then(response => {
          if (response.data && response.data.length > 0)
          {
            loc.hasDetail = true
            loc.details = []
            loc.details = response.data
          }
        })
        .catch(e => {
          this.api.errors.push(e)
        })
    }
  }
}
</script>
