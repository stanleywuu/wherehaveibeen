<template>
  <div id="check-in-container">
    <gmap-autocomplete
      style="width:75%"
      @place_changed="setPlace">
    </gmap-autocomplete>
    <b-button @click="geolocate()">Find Me</b-button>
    <p/>
    <gmap-map
      :center="center"
      :zoom="zoomLevel"
      style="width:100%; height: 400px;">
      <gmap-marker
        :position="center"
      ></gmap-marker>
    </gmap-map>
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
      navigator.geolocation.getCurrentPosition(position => {
        this.zoomLevel = 12
        this.center = {
          lat: position.coords.latitude,
          lng: position.coords.longitude
        };
      });
    }
  }
}
</script>

<style scoped>
.form-group {
  text-align: left !important;
}
</style>