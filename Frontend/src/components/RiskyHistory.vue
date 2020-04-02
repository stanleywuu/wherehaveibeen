<template>
  <div id="danger-map-container">
    <h2>Places carriers have been to</h2>
    <gmap-autocomplete
      style="width:75%"
      placeholder="Search for a place"
      @place_changed="setPlace">
    </gmap-autocomplete>
    <span id="disabled-btn-wrapper">
      <b-button v-show="!this.locatingUser" :disabled="!this.hideGeolocateToolTip" size="sm" id="user-location-btn" class="ml-3" @click="geolocate()">Find Me</b-button>
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
              ref ="map"
              @click="clickedOnMap"
              style="width:100%; height: 500px;">
              <gmap-marker
                :key="index"
                v-for="(m, index) in coords"
                :clickable="true"
                :position="{lat: m.latitude, lng: m.longitude}"
                @click="selectMarker(m)"
              >
              <gmap-info-window :opened = "true">
                <li>CheckedIn:{{m.checkIn}}</li>
                <li>CheckedOut:{{m.checkOut}}</li>
                <li>{{m.address}}</li>
                <p>{{m.placeName}}</p>
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
      // Example defaults to Montreal
      center: { lat: 45.508, lng: -73.587 },
      zoomLevel: 12,
      currentPlace: null,
      locatingUser: false,
      coords: null,
      hideGeolocateToolTip: true
    }
  },
  mounted () {
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
      }, 10000);
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
    clickedOnMap (e)
    {
      this.currentPlace = { placeId: e.placeId, name: '', formatted_address: '' }
      this.center.lat = e.latLng.lat()
      this.center.lng = e.latLng.lng()
      this.onGetCoords(this.center.lat, this.center.lng)
    },
    selectMarker (m)
    {
     m.open = !m.open 
    },
    onGetCoords(lat, lng) {
      this.$http.get(
        this.api.endpoint + '/risk?lat={lat}&lng={lng}'.replace('{lat}', lat).replace('{lng}', lng)
      )
      .then(response => {
        this.api.responses.push(response)
        if (response && response.data)
        {
          this.coords = []
          for (var i = 0; i < response.data.length;i++)
          {
            this.coords.push(response.data[i]);
          }
        }
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
