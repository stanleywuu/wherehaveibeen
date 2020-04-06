<template>
  <div id="check-in-container">
    <h2>Where did you go today?</h2>
    <gmap-autocomplete
      ref="autocomplete"
      style="width:75%"
      placeholder="Search for a place"
      :select-first-on-enter="true"
      @place_changed="setPlace" />
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
          <b-col md="6">
            <gmap-map
              :center="center"
              :zoom="zoomLevel"
              :options="mapOptions"
              ref ="map"
              @click="clickedOnMap"
              style="width:100%; height: 500px;">
              <gmap-marker :position="center" />
            </gmap-map>
          </b-col>
          <b-col md="6">
            <b-card-body
              class="d-flex align-items-start flex-column bd-highlight"
              style="height: 100%;">
              <div class="place-container">
                <div class="title" v-if="this.currentPlace.name">
                  {{ this.currentPlace.name }}
                </div>
                <div v-if="!this.currentPlace.name">
                  <div class="title">
                    You chose an unknown place
                  </div>
                  <label for="newPlaceName">Name this place:</label>
                  <input id="newPlaceName" type="text" v-model="unknownPlaceName" />
                </div>
                <div class="bd-highlight">
                  {{ this.currentPlace.formatted_address }}
                </div>
              </div>
              <div class="mb-auto bd-highlight">
                <b-row id="datetime-pickers">
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
              </div>
              <div class="bd-highlight">
                <b-row id="submit-alert" class="align-self-end">
                  <b-alert dismissible variant="success" v-model="showSuccessAlert">
                    This location has been stored
                  </b-alert>
                  <b-alert dismissible variant="danger" v-model="showFailureAlert">
                    Failed to store this Check In.<br/>
                    Please refresh or try again later
                  </b-alert>
                </b-row>
                <b-row id="submit-btn" class="align-self-end">
                  <b-button @click="onCheckInSubmit()">Check In Here</b-button>
                </b-row>
              </div>
            </b-card-body>
          </b-col>
        </b-row>
      </b-card>
    </div>
  </div>
</template>

<script>
import { gmapApi } from 'vue2-google-maps'

export default {
  name: 'CheckIn',
  data () {
    return {
      api: {
        endpoint: process.env.VUE_APP_API_URL,
        responses: [],
        errors: []
      },
      mapOptions: {
        mapTypeControl: false,
        streetViewControl: false,
        fullscreenControl: false,
        zoomControl: false
      },
      center: { lat: 0, lng: 0 },
      zoomLevel: 12,
      autoCompleteQuery: '',
      currentPlace: null,
      unknownPlaceName: null,
      locatingUser: false,
      checkInDate: this.formattedDate(),
      checkInTime: this.formattedTime(),
      showSuccessAlert: false,
      showFailureAlert: false,
      hideGeolocateToolTip: true
    }
  },
  mounted () {
    this.geolocate()
    this.timeoutGeoLocate()
  },
  computed: {
    google: gmapApi,
    userAuth () {
      return this.$store.getters.getUserAuth
    }
  },
  watch: {
    userAuth() {
      this.clearMapQuery()
    }
  },
  methods: {
    clearMapQuery () {
      this.currentPlace = null
      this.$refs['autocomplete'].$el.value = ''
    },
    formattedDate () {
      let date = new Date();
      return date.getFullYear() + '-' +
      (date.getMonth() + 1).toString().padStart(2, '0') + '-' +
      date.getDate().toString().padStart(2, '0')
    },
    formattedTime () {
      let date = new Date();
      return date.getHours().toString().padStart(2, '0') + ':' +
        date.getMinutes().toString().padStart(2, '0') + ':' +
        date.getSeconds().toString().padStart(2, '0')
    },
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

    },
    timeoutGeoLocate () {
      this.geolocate()
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
      this.unknownPlaceName = null
      this.center.lat = e.latLng.lat()
      this.center.lng = e.latLng.lng()
      if (e.placeId) {
        this.$refs.map.$mapPromise.then(map => {
          const request = {placeId: e.placeId, fields: ['place_id', 'name', 'formatted_address']}
          const service = new this.google.maps.places.PlacesService(map);
          service.getDetails(request, (place) => this.currentPlace = place);
        })
      } else {
        this.currentPlace = {}
      }
    },
    onCheckInSubmit () {
      let userToken = this.$store.getters.getUserToken
      let placeId = this.currentPlace && this.currentPlace.place_id
      let placeName = this.currentPlace && this.currentPlace.name
      let address = this.currentPlace && this.currentPlace.formatted_address
      let requestData = {
        "userId": this.$store.getters.getUserId,
        "latitude": this.center.lat,
        "longitude": this.center.lng,
        "placeId": placeId || '',
        "placeName": placeName || this.unknownPlaceName || '',
        "address": address || '',
        "checkin": [this.checkInDate, this.checkInTime].join('T')
      }
      this.$http.post(
        this.api.endpoint + '/visit',
        requestData,
        {
          headers: {
            Authorization: "Bearer " + userToken
          }
        }
      )
      .then(response => {
        this.showFailureAlert = false
        this.showSuccessAlert = true
        setTimeout(()=> {this.showSuccessAlert = false}, 3000)
        this.api.responses.push(response)
      })
      .catch(e => {
        this.showSuccessAlert = false
        this.showFailureAlert = true
        setTimeout(()=> {this.showFailureAlert = false}, 3000)
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
.place-container {
  width: 100%;
}
.place-container .title {
  font-weight: bold;
  text-align: center;
  margin: 12px 0;
}
.place-container label {
  margin-right: 8px;
}
.bd-highlight, .card-title, #submit-alert, #submit-btn {
  width: 100%;
  text-align: center;
  justify-content: center;
  padding-bottom: 10px;
}
#submit-alert, #submit-btn {
  padding-left: 25px;
}
</style>
