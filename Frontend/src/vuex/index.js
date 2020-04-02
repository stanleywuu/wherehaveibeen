import Vue from 'vue'
import Vuex from 'vuex'
import VuexPersist from 'vuex-persist'

import state from './state.js'
import actions from './actions.js'
import mutations from './mutations.js'
import getters from './getters.js'

Vue.use(Vuex)

const vuexLocalStorage = new VuexPersist({
  key: 'vuex',
  storage: window.localStorage
})


export default new Vuex.Store({
  state,
  actions,
  mutations,
  getters,
  plugins: [vuexLocalStorage.plugin]
})
