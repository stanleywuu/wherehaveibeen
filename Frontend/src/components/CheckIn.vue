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
              style="width:100%; height: 500px;">
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
                <b-row>
                  <b-col md="6">
                    <b-form-datepicker
                      id="checkin-datepicker"
                      v-model="checkInDate"
                      :placeholder="formattedDate()"
                      :date-format-options="{ year: 'numeric', month: 'numeric', day: 'numeric' }"
                      today-button
                      class="mb-3">
                    </b-form-datepicker>
                  </b-col>
                  <b-col md="6">
                    <b-form-timepicker
                      id="checkin-timepicker"
                      v-model="checkInTime"
                      :placeholder="formattedTime()"
                      now-button
                      class="mb-3">
                    </b-form-timepicker>
                  </b-col>
                </b-row>
                <b-button @click="onCheckInSubmit()">Check In Here</b-button>
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
      apiEndpoint: 'https://wherehaveibeen.azurewebsites.net',
      // Example defaults to Montreal
      center: { lat: 45.508, lng: -73.587 },
      zoomLevel: 12,
      currentPlace: null,
      locatingUser: false,
      checkInDate: '',
      checkInTime: '',
      form: {
        locationSearch: ''
      }
    }
  },
  mounted() {
    this.geolocate();
  },
  methods: {
    formattedDate () {
      return '01/01/1970'
    },
    formattedTime () {
      return '00:00:00'
    },
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
    },
    onCheckInSubmit () {
      let requestData = {
        "userId": this.$store.getters.getUserId,
        "latitude": this.center.lat,
        "longitude": this.center.lng,
        "checkin": [this.checkInDate, this.checkInTime].join('T')
      }
      console.log('POSTing to ' + this.apiEndpoint + '/visit')
      console.log(requestData)
    }
  }
}
</script>

<style scoped>
.form-group {
  text-align: left !important;
}
</style>