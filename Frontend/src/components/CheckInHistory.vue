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
          <b-card-body :title="loc.Address">
            {{ dateFormatter(loc.CheckIn) }}
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
    }
  }
}
</script>
