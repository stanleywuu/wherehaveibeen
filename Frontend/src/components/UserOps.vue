<template>
  <div id='user-ops-container'>
    <b-button variant="danger" class="ml-2 mr-2" @click="$bvModal.show('covid-modal')">
      I Have COVID-19
    </b-button>
    <b-button variant="outline-primary" class="ml-2 mr-2" @click="onUserLogout">
      Log Out
    </b-button>

    <b-modal id="covid-modal">
      <CovidModal></CovidModal>
    </b-modal>
  </div>
</template>

<script>
import CovidModal from '@/components/CovidModal.vue'

export default {
  name: 'UserOps',
  components :{
    CovidModal
  },
  methods: {
    onUserLogout (evt) {
      evt.preventDefault()
      this.$store.dispatch('storeUserAuth', false)
    }
  }
}
</script>