<template>
  <div id="checkin-history-container">
    <b-card 
      v-for="loc in this.mockData" 
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
            {{ Date.parse(loc.CheckIn) }}
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
      zoomLevel: 12,
      mockData: [
        {"VisitId":1,"Longitude":-79.769994,"Latitude":43.563917,"LatitudeRounded":43.564,"LongitudeRounded":-79.77,"CheckIn":"2012-03-19T07:22:00","CheckOut":"2012-03-19T09:22:00","UserId":1,"Address":"Mississauga, ON L5N 7V9, Canada","PlaceId":"ChIJ4ySOJydqK4gRqIMWbEw8fEY","AtRisk":false},
        {"VisitId":2,"Longitude":-79.769994,"Latitude":43.563917,"LatitudeRounded":43.564,"LongitudeRounded":-79.77,"CheckIn":"2012-03-20T07:23:00","CheckOut":"2012-03-19T09:22:00","UserId":1,"Address":"Mississauga, ON L5N 7V9, Canada","PlaceId":"ChIJ4ySOJydqK4gRqIMWbEw8fEY","AtRisk":false}
      ]
    }
  },
  methods: {
    findCenter (location) {
      return {
        lat: location.Latitude,
        lng: location.Longitude
      }
    }
  }
}
</script>

<style scoped>
.location-card {
  margin-bottom: 10px;
}
</style>