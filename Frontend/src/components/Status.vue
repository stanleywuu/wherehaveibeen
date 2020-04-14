<template>
  <div class="status-container" >
    <h2>What is your current status?</h2>
    <b-row no-gutters>
      <b-col md="4">
        <b-button block variant="info" @click="onNegativeClicked()">I am healthy</b-button>
      </b-col>
      <b-col md="4">
        <b-button block variant="danger" @click="onPositiveClicked()">I have symptoms</b-button>
      </b-col>
      <b-col md="4">
        <b-button block variant="success" @click="onRecoveredClicked()">I have recovered</b-button>
      </b-col>
    </b-row>
    <b-row no-gutters class="instruction" v-bind:class="{'warning': reportCovid}">
        {{ instruction }}
    </b-row>
    <b-row no-gutters>
      <b-button variant="danger" v-show="reportCovid" @click="onReportPositive()">
        Continue
      </b-button>
      <b-button variant="success" v-show="!reportCovid && !reportRecovery" @click="onReportNegative()">
        Continue
      </b-button>
      <b-button variant="success" v-show="reportRecovery" @click="onReportRecovery()">
        Continue
      </b-button>
    </b-row>
  </div>
</template>

<script>
const negInstruction = "Please use our system to track your travels in our system, we will inform you if any place you have visited became contaminated"
const posInstruction = "Sorry to hear that. Please take care of yourself. To help flatten the curve, please let us know of any public places you have been in the past two weeks so we can inform anyone who was in the area. "
const recoverInstruction = "We are so glad to hear that, please click the Confirm button to register the change in your condition"

export default {
  name: 'Status',
  data () {
    return {
      api: {
        endpoint: process.env.VUE_APP_API_URL,
        responses: [],
        errors: []
      },
      reportCovid: false,
      reportRecovery: false,
      instruction: null
    }
  },
  methods: {
    onPositiveClicked () {
      this.reportCovid = true
      this.reportRecover = false
      this.instruction = posInstruction
    },
    onRecoveredClicked () {
      this.instruction = recoverInstruction
      this.reportCovid = false
      this.reportRecovery = true
    },
    onNegativeClicked () {
      this.reportCovid = false;
      this.reportRecovery = false;
      this.instruction = negInstruction
    },
    onReportPositive () {
      this.$store.dispatch('storeUserStatus', 'corona')
      this.reportStatusChange("corona")
      this.onReportComplete()
    },
    onReportRecovery () {
      this.$store.dispatch('storeUserStatus', 'nocorona')
      this.reportStatusChange("recovered")
      this.onReportNegative()
    },
    onReportNegative () {
      this.$store.dispatch('storeUserStatus', 'nocorona')
      this.onReportComplete()
    },
    onReportComplete () {
        this.$emit('statusComplete')
    },
    reportStatusChange (endpoint) {
      let userToken = this.$store.getters.getUserToken
      let requestData = {
        userId: this.$store.getters.getUserId,
        IsAtRisk: true
      }
      this.$http.post(this.api.endpoint + '/membership/' + endpoint,
        requestData,
        {headers: { Authorization: "Bearer " + userToken }}
      )
      .then(response => {
        this.api.responses.push(response)
      })
      .catch(e => {
        this.api.errors.push(e)
      })
    }
  }
}
</script>
<style scoped>
.status-container {
  width:75%;
  margin:auto;
}
.status-container span {
  text-align: center;
  width:90%;
}
.status-container .instruction {
  text-align: left;
  background-color: #ffffcc;
}
.warning {
  background-color: #ffdddd!important;
}
</style>