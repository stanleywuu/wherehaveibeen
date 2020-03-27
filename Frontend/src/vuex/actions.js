export default {
  storeUserAuth (context, payload) {
    context.commit('setUserAuth', payload)
  },
  storeUserId (context, payload) {
    context.commit('setUserId', payload)
  }
}