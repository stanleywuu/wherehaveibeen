<template>
  <div id="covid-modal-container">
    <h2>COVID-19 Reporting</h2>
    <b-alert show variant="danger">
      Please do not use this panel if you have self-diagnosed COVID-19 due to symptoms.
    </b-alert>
    <h3>What you need to do now:</h3>
    <ul>
      <li>Confirm or cancel your report with the buttons at the bottom</li>
      <li>Self-Isolate immediately</li>
      <li>Check local government advice and guidelines</li>
      <li>Get well soon</li>
    </ul>

    <h3>What we will do:</h3>
    <ul>
      <li>Anonymously alert other users who may have crossed paths with you in the last 14 days</li>
    </ul>
    <div id="modal-btn">
      <b-button variant="danger" @click="confirmReport()">Confirm</b-button>
      <b-button variant="outline-danger" @click="cancelReport()">Cancel</b-button>
    </div>
  </div>
</template>

<script>
export default {
  name: 'CovidModal',
  methods: {
    confirmReport () {
      this.$emit('confirmReport')
    },
    cancelReport () {
      this.$emit('cancelReport')
    }
  }
}
</script>

<style scoped>
#modal-btn {
  text-align: right;
}
#modal-btn button {
  margin-left: 10px;
}
</style>