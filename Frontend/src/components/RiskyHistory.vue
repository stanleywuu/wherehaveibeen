<template>
  <div id="danger-map-container">
    <h2>Locations carriers have been</h2>
    <gmap-autocomplete
      style="width:75%"
      placeholder="Search for a place"
      @place_changed="setPlace">
    </gmap-autocomplete>
    <span id="disabled-btn-wrapper">
      <b-button v-show="!this.locatingUser" :disabled="!this.hideGeolocateToolTip" size="sm" id="user-location-btn" class="ml-3" @click="timeoutGeoLocate()">Find Me</b-button>
    </span>
    <b-tooltip :disabled="this.hideGeolocateToolTip" target="disabled-btn-wrapper" placement="top">
      Failed to find your location<br/>
      Please check your Browser's permissions
    </b-tooltip>
    <b-button disabled v-show="this.locatingUser" size="sm" class="ml-3"><b-spinner small></b-spinner></b-button>
    <p/>
    <div id="search-result" v-if="this.currentPlace">
      <b-card no-body class="overflow-hidden">
        <b-row no-gutters>
          <b-col style="width:90%">
            <gmap-map
              :center="center"
              :zoom="zoomLevel"
              @click="clickedOnMap"
              style="width:100%; height: 500px;">
              <gmap-marker
                v-for="(marker, index) in coords"
                :key="index"
                :clickable="true"
                :position="{lat: marker.latitude, lng: marker.longitude}"
                @click="marker.infoDisplayed=true"
              >
                <gmap-info-window
                  :opened="marker.infoDisplayed"
                  @closeclick="marker.infoDisplayed=false">
                  <p v-if="marker.placeName"><b>{{ marker.placeName }}</b></p>
                  <li>{{ marker.address }}</li>
                  <li>Checked In: {{ marker.checkIn }}</li>
                  <li>Checked Out: {{ marker.checkOut }}</li>
                </gmap-info-window>
              </gmap-marker>
            </gmap-map>
          </b-col>
        </b-row>
      </b-card>
    </div>
  </div>
</template>

<script>
export default {
  name: 'RiskyHistory',
  data () {
    return {
      api: {
        endpoint: process.env.VUE_APP_API_URL,
        responses: [],
        errors: []
      },
      center: { lat: 0, lng: 0 },
      zoomLevel: 12,
      currentPlace: null,
      locatingUser: false,
      coords: [],
      hideGeolocateToolTip: true
    }
  },
  methods: {
    getPosition: function(marker) {
      return {
        lat: parseFloat(marker.lat),
        lng: parseFloat(marker.lng)
      }
    },
    setPlace (place) {
      this.currentPlace = place
      this.zoomLevel = 16
      this.center.lat = this.currentPlace.geometry.location.lat()
      this.center.lng = this.currentPlace.geometry.location.lng()
      this.onGetCoords(this.center.lat, this.center.lng)
    },
    timeoutGeoLocate () {
      setTimeout(()=>{
        if (this.locatingUser) {
          this.locatingUser = false
          this.hideGeolocateToolTip = false
        }
      }, 10000)
    },
    geolocate () {
      this.locatingUser = true
      navigator.geolocation.getCurrentPosition(position => {
        this.zoomLevel = 12
        this.center = {
          lat: position.coords.latitude,
          lng: position.coords.longitude
        }
        this.currentPlace = {
          name: "Your location",
          lat: position.coords.latitude,
          lng: position.coords.longitude
        }
      this.locatingUser = false
      })
    },
    clickedOnMap (e) {
      this.currentPlace = { placeId: e.placeId, name: '', formatted_address: '' }
      this.center.lat = e.latLng.lat()
      this.center.lng = e.latLng.lng()
      this.onGetCoords(this.center.lat, this.center.lng)
    },
    openInfoWindowTemplate (item) {
      this.setInfoWindowTemplate(item)
      this.infoWindow.position = this.getCoordinates(item)
      this.infoWindow.open = true
    },
    onGetCoords(lat, lng) {
      this.$http.get(
        this.api.endpoint + '/risk?lat=' + lat + '&lng=' + lng
      )
      .then(response => {
        if (response && response.data) {
          this.coords.length = 0
          response.data.forEach(checkin => {
            checkin.infoDisplayed = true
            this.coords.push(checkin)
          })
        }
        console.log('coords')
        console.log(this.coords)
      })
      .catch(e => {
        this.showSuccessAlert = false
        this.showFailureAlert = true
        this.api.errors.push(e)
      })
    }
  }
}
</script>

<style scoped>
.form-group {
  text-align: left !important;
}
.bd-highlight, .card-title, #submit-alert {
  width: 100%;
  text-align: center;
  justify-content: center;
  padding-bottom: 10px;
}
#submit-alert {
  padding-left:25px;
}
</style>
