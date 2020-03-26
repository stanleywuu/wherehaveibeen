export default {
  storeUserAuth (context, payload) {
    context.commit('setUserAuth', payload)
  }
}