<template>
  <div id="check-in-container">
    <h2>Where did you go today?</h2>
    <gmap-autocomplete
      style="width:75%"
      placeholder="Search for a place"
      @place_changed="setPlace">
    </gmap-autocomplete>
    <b-button v-show="!this.locatingUser" size="sm" class="ml-3" @click="geolocate()">Find Me</b-button>
    <b-button disabled v-show="this.locatingUser" size="sm" class="ml-3"><b-spinner small></b-spinner></b-button>
    <p/>
    <div id="search-result" v-if="this.currentPlace">
      <b-card no-body class="overflow-hidden">
        <b-row no-gutters>
          <b-col md="6">
            <gmap-map
              :center="center"
              :zoom="zoomLevel"
              style="width:100%; height: 100%;">
              <gmap-marker
                :position="center"
              ></gmap-marker>
            </gmap-map>
          </b-col>
          <b-col md="6">
            <b-card-body :title="this.currentPlace['name']">
              <b-card-text>
                {{ this.currentPlace['formatted_address'] }}
                <p/>
                <b-button>Check In Here</b-button>
              </b-card-text>
            </b-card-body>
          </b-col>
        </b-row>
      </b-card>
    </div>
  </div>
</template>

<script>
export default {
  name: 'CheckIn',
  data () {
    return {
      // Example defaults to Montreal
      center: { lat: 45.508, lng: -73.587 },
      zoomLevel: 12,
      currentPlace: null,
      locatingUser: false,
      form: {
        locationSearch: ''
      }
    }
  },
  mounted() {
    this.geolocate();
  },
  methods: {
    setPlace (place) {
      this.currentPlace = place
      this.zoomLevel = 16
      this.center.lat = this.currentPlace.geometry.location.lat()
      this.center.lng = this.currentPlace.geometry.location.lng()
    },
    geolocate () {
      this.locatingUser = true
      navigator.geolocation.getCurrentPosition(position => {
        this.zoomLevel = 12
        this.center = {
          lat: position.coords.latitude,
          lng: position.coords.longitude
        }
      this.locatingUser = false
      })
    }
  }
}
</script>

<style scoped>
.form-group {
  text-align: left !important;
}
</style>