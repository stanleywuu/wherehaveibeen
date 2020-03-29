export default {
  getUserAuth (state) {
    return state.userAuth !== undefined
  },
  getUserId (state) {
    return state.userId
  }
}