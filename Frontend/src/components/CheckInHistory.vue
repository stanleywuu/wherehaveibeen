<template>
  <div id="checkin-history-container">
    <h2>Check-In History</h2>
    <b-card
      v-for="loc in this.locations"
      :title="loc.Address"
      :key="loc.VisitId"
      class="location-card overflow-hidden"
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
    <b-button @click="loadHistory()">Refresh</b-button>
  </div>
</template>

<script>
export default {
  name: 'CheckInHistory',
  data () {
    return {
      apiEndpoint: process.env.VUE_APP_API_URL,
      apiErrors: [],
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
        this.$http.get(this.apiEndpoint + '/visit?userId=' + userId)
        .then(response => {
          this.locations = response.data.reverse()
        })
        .catch(e => {
          this.apiErrors.push(e)
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

<style scoped>
.location-card {
  margin-bottom: 10px;
}
</style>
