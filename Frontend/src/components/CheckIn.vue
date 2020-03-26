<template>
  <div id="check-in-container">
    <b-form-group
      id="location-input-group"
      label="Search for a location:"
      label-for="location-input">
      <b-form-input
        id="location-input"
        type="text"
        v-on:input="searchGoogleMapsByText"
        required
        v-model="form.locationSearch">
      </b-form-input>
    </b-form-group>
  </div>
</template>

<script>
export default {
  name: 'CheckIn',
  data () {
    return {
      form: {
        locationSearch: ''
      }
    }
  },
  methods: {
    searchGoogleMapsByText () {
      console.log('Searching Google for: ' + this.form.locationSearch)
    }
  }
}
</script>

<style scoped>
.form-group {
  text-align: left !important;
}
</style>